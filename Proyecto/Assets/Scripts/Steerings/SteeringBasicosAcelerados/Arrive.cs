using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehaviour {

    public float timeToTarget = 0.25f;
    float velocidadMin = 0.01f;

    public override Steering GetSteering(AgentNPC agent) {

        // Se crea la estructura para guardar la salida
        Steering steering = new Steering();

        // Se obtiene la dirección hacia el target
        Vector3 direccion = target.transform.position - agent.transform.position;

        // Se comprueba si se está dentro del radio del target
        float distancia = direccion.magnitude;

        if (distancia < target.getRadioInt()) {
            steering.linear = -agent.getVelocidadVec();
            if (agent.getVelocidadVec().magnitude < velocidadMin)
                agent.setVelocidadVec(Vector3.zero);
            return steering;
        }

        float targetSpeed;
        if (distancia > target.getRadioExt()) 
            targetSpeed = agent.getVelocidadMax();
        else
            targetSpeed = agent.getVelocidadMax() * distancia / target.getRadioExt();

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

