using System.Numerics;
using Assimp;

namespace Voxel;

public record Deltas(float DeltaX, float DeltaY, float DeltaZ);

public static class Voxelizer
{
    public static List<Vector3> Voxelize(Vector3 origin, Deltas deltas, int[] dims,
        List<Face> faces, List<Vector3D> vertices)
    {
        var resultCenterPoints = new List<Vector3>();

        for (var x = 0; x < dims[0]; x++)
        {
            for (var y = 0; y < dims[1]; y++)
            {
                var zList = new List<float>();

                var point = new Vector3(
                    origin.X + (x + 0.5f) * deltas.DeltaX,
                    origin.Y + (y + 0.5f) * deltas.DeltaY,
                    origin.Z);
                var ray = new Vector3(0, 0, 1);
                
                foreach (var face in faces)
                {
                    var i1 = vertices[face.Indices[0]];
                    var i2 = vertices[face.Indices[1]];
                    var i3 = vertices[face.Indices[2]];

                    var a = new Vector3(i1.X, i1.Y, i1.Z);
                    var b = new Vector3(i2.X, i2.Y, i2.Z);
                    var c = new Vector3(i3.X, i3.Y, i3.Z);

                    if (Helper.Rti(point, ray, a, b, c, out var intersectPoint))
                    {
                        zList.Add(intersectPoint.Z);
                    }
                }

                var zc = origin.Z + deltas.DeltaZ / 2;

                for (var z = 0; z < dims[2]; z++)
                {
                    var count = zList.Count(zValue => zValue > zc);

                    if (count % 2 == 1)
                    {
                        resultCenterPoints.Add(new Vector3(point.X, point.Y, zc));
                    }

                    zc += deltas.DeltaZ;
                }
            }
        }
        
        return resultCenterPoints;
    }

    public static List<Vector3> VoxelizeSingle(float x, float y, Vector3 origin, Deltas deltas, int[] dims,
        List<Face> faces, List<Vector3D> vertices)
    {
        var resultCenterPoints = new List<Vector3>();
        var zList = new List<float>();

        var point = new Vector3(
            origin.X + (x + 0.5f) * deltas.DeltaX,
            origin.Y + (y + 0.5f) * deltas.DeltaY,
            origin.Z);
        var ray = new Vector3(0, 0, 1);

        foreach (var face in faces)
        {
            var i1 = vertices[face.Indices[0]];
            var i2 = vertices[face.Indices[1]];
            var i3 = vertices[face.Indices[2]];

            var a = new Vector3(i1.X, i1.Y, i1.Z);
            var b = new Vector3(i2.X, i2.Y, i2.Z);
            var c = new Vector3(i3.X, i3.Y, i3.Z);

            if (Helper.Rti(point, ray, a, b, c, out var intersectPoint))
            {
                zList.Add(intersectPoint.Z);
            }
        }

        var zc = origin.Z + deltas.DeltaZ / 2;

        for (var z = 0; z < dims[2]; z++)
        {
            var count = zList.Count(zValue => zValue > zc);

            if (count % 2 == 1)
            {
                resultCenterPoints.Add(new Vector3(point.X, point.Y, zc));
            }

            zc += deltas.DeltaZ;
        }

        return resultCenterPoints;
    }
}