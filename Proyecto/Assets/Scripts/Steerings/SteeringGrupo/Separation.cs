using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBehaviour {

    GameObject[] agentsNpc;
    float threshold = 5f;
    float decayCoefficient = 15f;

    void Start() {
        agentsNpc = GameObject.FindGameObjectsWithTag("NPC");
    }

    public override Steering GetSteering(AgentNPC agent) {
        Steering steering = new Steering();

        foreach (GameObject target in agentsNpc) {
            Vector3 direccion = agent.transform.position - target.transform.position;
            float distancia = direccion.magnitude;

            if (target.Equals(this.gameObject))
                 continue;
                 
            if (distancia < threshold) {
                float fuerza = Mathf.Min(decayCoefficient/(distancia*distancia), agent.getAceleracionMax());
                direccion = direccion.normalized;
                steering.linear += fuerza * direccion;
            }
        }
        
        return steering;
    }
}