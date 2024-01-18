using Assimp;
using Auios.QuadTree;

namespace Voxel;

public class FaceWithMeshIdx
{
    public Face Face { get; set; }
    public int MeshIdx { get; set; }

    public FaceWithMeshIdx(Face face, int meshIdx)
    {
        Face = face;
        MeshIdx = meshIdx;
    }
}

public class ObjectBound : IQuadTreeObjectBounds<FaceWithMeshIdx>
{
    private readonly List<Mesh> _meshes;

    public ObjectBound(List<Mesh> meshes)
    {
        _meshes = meshes;
    }
    
    public float GetLeft(FaceWithMeshIdx obj)
    {
        var x0 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[0]].X;
        var x1 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[1]].X;
        var x2 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[2]].X;
        
        return Math.Min(x0, Math.Min(x1, x2));
    }

    public float GetRight(FaceWithMeshIdx obj)
    {
        var x0 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[0]].X;
        var x1 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[1]].X;
        var x2 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[2]].X;
        
        return Math.Max(x0, Math.Max(x1, x2));
    }

    public float GetTop(FaceWithMeshIdx obj)
    {
        var y0 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[0]].Y;
        var y1 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[1]].Y;
        var y2 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[2]].Y;
        
        return Math.Min(y0, Math.Min(y1, y2));
    }

    public float GetBottom(FaceWithMeshIdx obj)
    {
        var y0 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[0]].Y;
        var y1 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[1]].Y;
        var y2 = _meshes[obj.MeshIdx].Vertices[obj.Face.Indices[2]].Y;
        
        return Math.Max(y0, Math.Max(y1, y2));
    }
}
