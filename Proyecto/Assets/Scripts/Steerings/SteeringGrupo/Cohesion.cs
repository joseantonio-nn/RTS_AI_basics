using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : Seek {
    GameObject[] agentsNpc;
    float threshold = 100f;
    int count;
    Vector3 centroDeMasa = Vector3.zero;

    void Start() {
        agentsNpc = GameObject.FindGameObjectsWithTag("NPC");
    }

    public override Steering GetSteering(AgentNPC agent) {
        count = 0;
        centroDeMasa = Vector3.zero;

        foreach (GameObject target in agentsNpc) {
            if (target.Equals(this.gameObject))
                 continue;
            Vector3 direccion = target.transform.position - agent.transform.position;
            float distancia = direccion.magnitude;

            if (distancia <= threshold) {
                centroDeMasa += target.transform.position;
                count++;  
            }   
        }

        if (count == 0)
            return new Steering();
        
        centroDeMasa /= count; 
        base.target.transform.position = centroDeMasa;
        return base.GetSteering(agent);
    }
}