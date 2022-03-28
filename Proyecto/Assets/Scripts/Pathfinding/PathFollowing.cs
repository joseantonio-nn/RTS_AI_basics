using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowing : Seek {

    private int indiceNodoActual = 0;
    private float radio = 0.25f;
    public List<Nodo> path = null;
    private bool primeraVez = true;


    public override Steering GetSteering(AgentNPC agent) {
        if (path == null) return new Steering();
        if (indiceNodoActual != path.Count) {
    
            // Obtenemos el target actual, que será el punto del camino que indique pathDir
            target.transform.position = path[indiceNodoActual].posicionReal;
            
            // Se obtiene la dirección hacia el target
            Vector3 direccion = target.transform.position - agent.transform.position;

            // Si se ha "llegado" al target, se pasa al siguiente target 
            if (direccion.magnitude <= radio)
                indiceNodoActual++;
        
            return base.GetSteering(agent);

        } else if (primeraVez) {
            if (agent.getVelocidadVec() != Vector3.zero) {
                primeraVez = false;
                FrenarAgente(agent);
            }
        }
        
        return new Steering();
    }

    private void FrenarAgente(AgentNPC agent) {
        agent.setVelocidadVec(Vector3.zero);
        peso = 0f;
    }
    
}
