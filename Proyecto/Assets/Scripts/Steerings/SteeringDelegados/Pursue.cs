using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : Seek {

    public Agent explicitTarget;
    // Se establece un tiempo de predicción máximo
    public float maxPrediccion = 0.5f;

    public override Steering GetSteering(AgentNPC agent) {

        // Se obtienen la dirección hacia el target y la distancia a la que está
        Vector3 direccion = target.transform.position - agent.transform.position;
        float distancia = direccion.magnitude;

        // Se obtiene la velocidad actual del agente
        float speed = agent.getVelocidadVec().magnitude;

        float prediccion;
        // Si la velocidad es demasiado baja se le da un tiempo de predicción por defecto
        if (speed <= (distancia / maxPrediccion))
            prediccion = maxPrediccion;
        // En otro caso se calcula ese tiempo de predicción
        else 
            prediccion = distancia / speed;
        
        
        target.transform.position = explicitTarget.transform.position;
        target.transform.position += explicitTarget.getVelocidadVec() * prediccion;

        if (show)
            Debug.DrawLine(agent.transform.position, agent.transform.position + agent.getVelocidadVec(), Color.cyan);
        
        base.target = target;

        // Se delega al Seek el movimiento
        return base.GetSteering(agent);
    }
}
