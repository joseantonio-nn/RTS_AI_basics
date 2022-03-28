using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector {
    
    public static float angulo(Vector3 referencia, Vector3 v) {
        var a0 = Mathf.Atan2(referencia.x, referencia.z);
        var a1 = Mathf.Atan2(v.x, v.z);
        return (a1 - a0) * Mathf.Rad2Deg;
    }

    public static float pequeno(float angulo) {
        angulo = angulo % 360;
        if (angulo > 180) return angulo -= 360;
        if (angulo < -180) return angulo += 360;
        
        return angulo;
    }

    public static Vector3 vector(float angulo) {
        float radian = angulo * Mathf.Deg2Rad;
        float X = Mathf.Sin(radian);
        float Y = 0;
        float Z = Mathf.Cos(radian);
        return new Vector3(X, Y, Z);
    }

    public static Vector3 rotar(Vector3 referencia, float angulo) {
        angulo = angulo * Mathf.Deg2Rad;
        float X = Mathf.Cos(angulo)*referencia.x + Mathf.Sin(angulo)*referencia.z;
        float Y = referencia.y;
        float Z = -Mathf.Sin(angulo)*referencia.x + Mathf.Cos(angulo)*referencia.z;
        return new Vector3(X, Y, Z);
    }
}
