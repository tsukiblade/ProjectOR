using Assimp;
using Auios.QuadTree;

namespace Voxel;

public class ObjectBound : IQuadTreeObjectBounds<Face>
{
    private readonly List<Vector3D> _vertices;

    public ObjectBound(List<Vector3D> vertices)
    {
        _vertices = vertices;
    }
    
    public float GetLeft(Face obj)
    {
        var x0 = _vertices[obj.Indices[0]].X;
        var x1 = _vertices[obj.Indices[1]].X;
        var x2 = _vertices[obj.Indices[2]].X;
        
        return Math.Min(x0, Math.Min(x1, x2));
    }

    public float GetRight(Face obj)
    {
        var x0 = _vertices[obj.Indices[0]].X;
        var x1 = _vertices[obj.Indices[1]].X;
        var x2 = _vertices[obj.Indices[2]].X;
        
        return Math.Max(x0, Math.Max(x1, x2));
    }

    public float GetTop(Face obj)
    {
        var y0 = _vertices[obj.Indices[0]].Y;
        var y1 = _vertices[obj.Indices[1]].Y;
        var y2 = _vertices[obj.Indices[2]].Y;
        
        return Math.Max(y0, Math.Max(y1, y2));
    }

    public float GetBottom(Face obj)
    {
        var y0 = _vertices[obj.Indices[0]].Y;
        var y1 = _vertices[obj.Indices[1]].Y;
        var y2 = _vertices[obj.Indices[2]].Y;
        
        return Math.Min(y0, Math.Min(y1, y2));
    }
}
