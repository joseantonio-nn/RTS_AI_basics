using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviour {
    public float timeToTarget = 0.01f;
    float rotacionMin = 0.01f;

    public override Steering GetSteering(AgentNPC agent) {

        // Se crea la estructura para guardar la salida
        Steering steering = new Steering();

        // Se obtiene la dirección hacia el target
        float rotation = target.getOrientacion() - agent.getOrientacion();
        
        //Mapear el resultado al intervalo (-pi, pi)
        rotation = mapRango(rotation);
        
        float rotationSize = Mathf.Abs(rotation);

        if (rotationSize < target.getAnguloInt()) {
            steering.angular = -agent.getRotacion();
            if (agent.getRotacion() < rotacionMin)
                agent.setRotacion(0f);
            return steering;
        }

        float targetRotation;
        if (rotationSize >= target.getAnguloExt())
            targetRotation = agent.getRotacionMax();
        else 
            targetRotation = agent.getRotacionMax() * rotationSize / agent.getAnguloExt();

        targetRotation *= rotation / rotationSize;
        steering.angular = targetRotation - agent.getRotacion();
        steering.angular /= timeToTarget;

        float angularAcceleration = Mathf.Abs(steering.angular);
        if (angularAcceleration > agent.getAceleracionAngularMax()) {
            steering.angular /= angularAcceleration;
            steering.angular *= agent.getAceleracionAngularMax();
        }
        
        // Se devuelve el steering
        steering.linear = Vector3.zero;
        return steering;
    }

    float mapRango(float rotacion) {
        rotacion = rotacion % 360;
        if (rotacion > 180) return rotacion -= 360;
        if (rotacion < -180) return rotacion += 360;
        
        return rotacion;
    }
}

