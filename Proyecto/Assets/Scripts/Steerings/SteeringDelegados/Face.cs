using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : Align {
    public Agent explicitTarget;

    public override Steering GetSteering(AgentNPC agent) {
        // Direccion a delegar
        Vector3 direction = explicitTarget.transform.position - agent.transform.position;

        // Si la direccion es cero, no mover
        if (direction.magnitude == 0)
            return new Steering();
        
        target.setOrientacion(Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg);

        if (show) 
            Debug.DrawLine(agent.transform.position, agent.transform.localPosition + new Vector3(direction.x, 1, direction.z), Color.cyan);

        return base.GetSteering(agent);
    }
}
