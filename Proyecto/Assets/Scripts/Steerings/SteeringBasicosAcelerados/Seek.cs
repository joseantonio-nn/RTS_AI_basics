using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviour {

    public override Steering GetSteering(AgentNPC agent) {

        // Se crea la estructura para guardar la salida
        Steering steering = new Steering();

        // Se obtiene la dirección hacia el target
        steering.linear = target.transform.position - agent.transform.position;
       
        // La velocidad es máxima en esa dirección
        steering.linear = steering.linear.normalized;
        steering.linear *= agent.getAceleracionMax();

        // Se devuelve el steering
        steering.angular = 0;
        return steering; 
    } 
}
