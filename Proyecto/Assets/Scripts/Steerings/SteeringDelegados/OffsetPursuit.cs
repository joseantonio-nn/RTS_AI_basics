using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPursuit : Seek {

    // Vector de comportamiento habitual (se puede cambiar en inspector)
    public Vector3 offset = new Vector3(0, 0, -10);
    public Agent explicitTarget;
    
    public override Steering GetSteering(AgentNPC agent) {
        // Angulo del offset
        float angulo = explicitTarget.getOrientacion();

        // Rotar offset
        Vector3 objetivo = calcularNuevaPosicion(angulo, offset);

        // Relativo a target
        objetivo = objetivo + explicitTarget.transform.position;

        target.transform.position = objetivo;
        return base.GetSteering(agent);
    }

    Vector3 calcularNuevaPosicion(float angulo, Vector3 offset) {
        angulo = angulo * Mathf.Deg2Rad;
        float Xcoord = Mathf.Cos(angulo) * offset.x + Mathf.Sin(angulo) * offset.z;
        float Zcoord = - Mathf.Sin(angulo) * offset.x + Mathf.Cos(angulo) * offset.z;
        return new Vector3(Xcoord, 0, Zcoord);
    }
}

