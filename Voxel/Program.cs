using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using Assimp;
using CommandLine;
using Voxel;

var fileName = string.Empty;
var xDim = 64;
var yDim = 64;
var zDim = 64;
var parallelTasks = 0;
var shouldSaveOutput = false;

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(o =>
    {
        Console.WriteLine($"File path: {o.FileName}");
        Console.WriteLine($"X dim: {o.XDim}");
        Console.WriteLine($"Y dim: {o.YDim}");
        Console.WriteLine($"Z dim: {o.ZDim}");
        
        fileName = o.FileName;
        xDim = o.XDim;
        yDim = o.YDim;
        zDim = o.ZDim;
        parallelTasks = o.Parallel;
        shouldSaveOutput = o.ShouldSaveOutput;
    });

var importer = new AssimpContext();
var filePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Resources", fileName);
var scene = importer.ImportFile(filePath);

var faces = scene.Meshes.SelectMany<Mesh, Face>(mesh => mesh.Faces).ToList();
var vertices = scene.Meshes.SelectMany<Mesh, Vector3D>(mesh => mesh.Vertices).ToList();

var minPointX = vertices.Min(vector => vector.X);
var minPointY = vertices.Min(vector => vector.Y);
var minPointZ = vertices.Min(vector => vector.Z);
var maxPointX = vertices.Max(vector => vector.X);
var maxPointY = vertices.Max(vector => vector.Y);
var maxPointZ = vertices.Max(vector => vector.Z);

var origin = new Vector3(minPointX, minPointY, minPointZ);
var maxPoint = new Vector3(maxPointX, maxPointY, maxPointZ);

var widthX = maxPoint.X - origin.X;
var heightY = maxPoint.Y - origin.Y;


for (var i = 0; i < scene.Meshes.Count; ++i)
{
    var facesWithIdx = scene.Meshes[i].Faces.Select(face => new FaceWithMeshIdx(face, i)).ToList();
    Console.WriteLine($"Adding {facesWithIdx.Count} faces with idx for mesh {i}");
}


var dims = new[] {xDim, yDim, zDim};
var deltaX = widthX / dims[0];
var deltaY = heightY / dims[1];
var deltaZ = (maxPointZ - minPointZ) / dims[2];

Console.WriteLine($"Dimensions: {dims[0]} {dims[1]} {dims[2]}");
Console.WriteLine($"Number of faces: {faces.Count}");
Console.WriteLine($"Number of vertices: {vertices.Count}");
Console.WriteLine($"Delta X: {deltaX}");
Console.WriteLine($"Delta Y: {deltaY}");
Console.WriteLine($"Delta Z: {deltaZ}");

var parallelInfoText = parallelTasks switch
{
    0 or 1 => "Sequential",
    -1 => "Parallel (all cores). Available threads: " + Environment.ProcessorCount,
    _ => $"Parallel ({parallelTasks} tasks)"
};
Console.WriteLine($"Processing in parallel: {parallelInfoText}");
Console.WriteLine($"Processing {xDim*yDim*zDim} points in grid");

List<Vector3> resultCenterPoints;

if (parallelTasks is 0 or 1)
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    
    resultCenterPoints = Voxelizer.Voxelize(origin, new Deltas(deltaX, deltaY, deltaZ), dims, faces, vertices);
    
    var ticks = stopWatch.ElapsedTicks;
    var ms = stopWatch.ElapsedMilliseconds;
    stopWatch.Stop();
    Console.WriteLine($"Finished in {ticks} ticks");
    Console.WriteLine($"Finished in {ms} ms");
}
else
{
    var xyPairs = new List<(int, int)>();
    for (var x = 0; x < dims[0]; x++)
    {
        for (var y = 0; y < dims[1]; y++)
        {
            xyPairs.Add((x, y));
        }
    }

    var resultCenterPointsParallel = new ConcurrentBag<Vector3>();

    var stopWatch = new Stopwatch();
    stopWatch.Start();
    
    await Parallel.ForEachAsync(xyPairs, new ParallelOptions
    {
        MaxDegreeOfParallelism = parallelTasks
    }, async (tuple, _) =>
    {
        
        var singleResult = Voxelizer.VoxelizeSingle(tuple.Item1, tuple.Item2, origin,
            new Deltas(deltaX, deltaY, deltaZ), dims, faces, vertices);

        singleResult.ForEach(x => resultCenterPointsParallel.Add(x));
    });
    
    var ticks = stopWatch.ElapsedTicks;
    var ms = stopWatch.ElapsedMilliseconds;
    stopWatch.Stop();
    Console.WriteLine($"Finished in {ticks} ticks");
    Console.WriteLine($"Finished in {ms} ms");
    
    resultCenterPoints = resultCenterPointsParallel.ToList();
}

Console.WriteLine($"Produced {resultCenterPoints.Count} voxels");

if (!shouldSaveOutput)
{
    Console.WriteLine("Not saving output.");
    return;
}

Console.WriteLine("Exporting to obj");

var voxels = resultCenterPoints.Select(vector => new Voxel.Voxel(vector, deltaX, deltaY, deltaZ)).ToList();
ExportHelper.ExportToObj(voxels, Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Resources", "result.obj"));

Console.WriteLine("Exporting to point cloud");
var pcexport = new PCExporter();
pcexport.ExportToPC(resultCenterPoints, Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Resources", "result_pc.xyz"));