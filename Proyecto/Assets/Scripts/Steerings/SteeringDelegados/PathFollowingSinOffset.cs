using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowingSinOffset : Seek {
    public GameObject targetsCamino;
    private int pathDir = 0;

    public override Steering GetSteering(AgentNPC agent) {
        Vector3 direccion;

        // Obtenemos el target actual, que será el punto del camino que indique pathDir
        target = targetsCamino.transform.GetChild(pathDir).gameObject.GetComponent<Agent>();

        // Se obtiene la dirección hacia el target
        direccion = target.transform.position - agent.transform.position;

        // Si se ha "llegado" al target, se pasa al siguiente target. Desde el último va al primero, directamente.
        if (direccion.magnitude <= target.getRadioExt()) { 
            pathDir = (pathDir + 1) % targetsCamino.transform.childCount; 
        } 
    
        return base.GetSteering(agent);
    }
}
