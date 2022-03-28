using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetArrive : Arrive {
    public Vector3 offset = new Vector3(0, 0, -10);
    
    public override Steering GetSteering(AgentNPC agent) {
        // Angulo del offset
        float angulo = target.getOrientacion();

        // Rotar offset
        Vector3 objetivo = rotar(offset, angulo);

        // Relativo al target
        objetivo = objetivo + target.transform.position;

        // Guardar estado
        Vector3 old = target.transform.position;

        // Modificar target
        target.transform.position = objetivo;

        // Obtener steering
        var steering = base.GetSteering(agent);

        // Restaurar target
        target.transform.position = old;
        return steering;
    }

    Vector3 rotar(Vector3 offset, float angulo) {
        angulo = angulo * Mathf.Deg2Rad;
        float Xcoord = Mathf.Cos(angulo) * offset.x + Mathf.Sin(angulo) * offset.z;
        float Zcoord = - Mathf.Sin(angulo) * offset.x + Mathf.Cos(angulo) * offset.z;
        return new Vector3(Xcoord, offset.y, Zcoord);
    }

    void OnDrawGizmos() {
        if (show) {
            #if UNITY_EDITOR
                UnityEditor.Handles.DrawWireDisc(target.transform.position, new Vector3(0,1,0), 1f);
            #endif
        }
    }
}

