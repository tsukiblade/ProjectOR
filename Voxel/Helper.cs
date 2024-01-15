using System.Numerics;

namespace Voxel;

public static class Helper
{
    public static bool Rti(Vector3 rayOrigin, Vector3 rayDirection, Vector3 a, Vector3 b, Vector3 c, out Vector3 intersect)
    {
        const float epsilon = 0.00001f;
        intersect = Vector3.Zero;
        
        var e1 = b - a;
        var e2 = c - a;
        
        var h = Vector3.Cross(rayDirection, e2);
        var det = Vector3.Dot(e1, h);

        if (det is > -epsilon and < epsilon)
        {
            return false;
        }
            
        
        var invDet = 1 / det;
        var tvec = rayOrigin - a;
        
        var u = Vector3.Dot(tvec, h) * invDet;
        if (u < 0 || u > 1)
            return false;
        
        var qvec = Vector3.Cross(tvec, e1);
        var v = Vector3.Dot(rayDirection, qvec) * invDet;
        if (v < 0 || u + v > 1)
            return false;
        
        var t = Vector3.Dot(e2, qvec) * invDet;

        if (!(t > epsilon))
        {
            return false;
        }
        
        intersect = rayOrigin + rayDirection * t;
        return true;
    }
}