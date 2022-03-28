using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : Align {

    GameObject[] agentsNpc;
    float threshold = 30f;
    float heading;
    int count;

    void Start() {
        agentsNpc = GameObject.FindGameObjectsWithTag("NPC");
    }

    public override Steering GetSteering(AgentNPC agent) {
        heading = 0f;
        count = 0;

        foreach (GameObject target in agentsNpc) {
            if (target.Equals(this.gameObject))
                 continue;

            Vector3 direction = target.transform.position - agent.transform.position;
            float distance = direction.magnitude;

            if (distance <= threshold) {
                heading += target.GetComponent<AgentNPC>().getOrientacion();
                count++; 
            }
        }

        if (count > 0) {
            heading /= count;
            target.setOrientacion(heading);
        }

        return base.GetSteering(agent);
    }
}