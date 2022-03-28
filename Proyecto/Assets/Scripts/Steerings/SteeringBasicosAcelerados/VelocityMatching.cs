using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityMatching : SteeringBehaviour
{
    public float timeToTarget = 0.1f;
    
    public override Steering GetSteering(AgentNPC agent) {

        // Se crea la estructura para guardar la salida
        Steering steering = new Steering();

        // Se obtiene la dirección hacia el target
        steering.linear = target.getVelocidadVec() - agent.getVelocidadVec();
        
        steering.linear /= timeToTarget;

        if (steering.linear.magnitude > agent.getAceleracionMax()) {
            steering.linear = steering.linear.normalized;
            steering.linear *= agent.getAceleracionMax();
        }

        steering.angular = 0;

        return steering;
    }
}
