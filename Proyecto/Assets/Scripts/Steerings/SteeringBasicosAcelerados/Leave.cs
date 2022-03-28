using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Es como ARRIVE acelerado pero al revés
public class Leave : SteeringBehaviour {
    public float timeToTarget = 0.25f;

    public override Steering GetSteering(AgentNPC agent) {

        // Se crea la estructura para guardar la salida
        Steering steering = new Steering();

        // Se obtiene la dirección hacia el target
        Vector3 direccion = agent.transform.position - target.transform.position;

        // Se comprueba si se está dentro del radio del target
        float distancia = direccion.magnitude;

        if (distancia > target.getRadioExt()) {
            return steering; 
        }

        float targetSpeed;
        if (distancia < target.getRadioInt()) 
            targetSpeed = agent.getVelocidadMax();
        else
            targetSpeed = agent.getVelocidadMax() * (1f - (distancia / target.getRadioExt()));

        Vector3 targetVelocity = direccion.normalized;
        targetVelocity *= targetSpeed;

        steering.linear = targetVelocity - agent.getVelocidadVec();

        // El agente se mueve hacia el target en timeToTarget segundos
        steering.linear /= timeToTarget;

        // Se comprueba que no se pase de la velocidad máxima
        if (steering.linear.magnitude > agent.getAceleracionMax()) {
            steering.linear = steering.linear.normalized;
            steering.linear *= agent.getAceleracionMax();
        }

        // Se devuelve el steering
        steering.angular = 0;
        return steering;
    }
}

