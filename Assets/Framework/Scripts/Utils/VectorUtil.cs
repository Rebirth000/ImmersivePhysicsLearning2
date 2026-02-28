using UnityEngine;

public static class VectorUtil
{
    public static Vector3 Set(this Vector3 vector3, float x = float.NaN, float y = float.NaN, float z = float.NaN) {
        if (float.IsNaN(x)) x = vector3.x;
        if (float.IsNaN(y)) y = vector3.y;
        if (float.IsNaN(z)) z = vector3.z;

        return new Vector3(x, y, z);
    }
    
    public static Vector3 Add(this Vector3 vector3, float x = float.NaN, float y = float.NaN, float z = float.NaN) {
        if (float.IsNaN(x)) x = 0;
        if (float.IsNaN(y)) y = 0;
        if (float.IsNaN(z)) z = 0;

        return new Vector3(x, y, z) + vector3;
    }
    
    public static Vector3 Plus(this Vector3 vector3, float x = float.NaN, float y = float.NaN, float z = float.NaN) {
        if (float.IsNaN(x)) x = 1;
        if (float.IsNaN(y)) y = 1;
        if (float.IsNaN(z)) z = 1;

        return new Vector3(x * vector3.x, y * vector3.y, z * vector3.z);
    }
    
    public static Vector2 Set(this Vector2 vector2, float x = float.NaN, float y = float.NaN) {
        if (float.IsNaN(x)) x = vector2.x;
        if (float.IsNaN(y)) y = vector2.y;

        return new Vector2(x, y);
    }
    
    public static Vector2 Add(this Vector2 vector2, float x = float.NaN, float y = float.NaN) {
        if (float.IsNaN(x)) x = 0;
        if (float.IsNaN(y)) y = 0;

        return new Vector2(x, y) + vector2;
    }
    
    public static Vector2 Plus(this Vector2 vector2, float x = float.NaN, float y = float.NaN) {
        if (float.IsNaN(x)) x = 1;
        if (float.IsNaN(y)) y = 1;

        return new Vector2(x * vector2.x, y * vector2.y);
    }
}