using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAlign : Align {
    public float offset = 0;

    public override Steering GetSteering(AgentNPC agent) {
        // Rotar offset
        float angulo = target.getOrientacion() + offset;

        // Guardar estado
        float old = target.getOrientacion();

        // Modificar target
        target.setOrientacion(angulo);

        // Obtener steering
        var steering = base.GetSteering(agent);

        // Restaurar target
        target.setOrientacion(old);
        return steering;
    }
}
