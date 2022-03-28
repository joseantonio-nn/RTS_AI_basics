using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWhereYouGoing : Align
{
    public override Steering GetSteering(AgentNPC agent) {
        
        if (agent.getVelocidadVec().magnitude == 0)
            return new Steering();
    
        target.setOrientacion(Mathf.Atan2(agent.getVelocidadVec().x, agent.getVelocidadVec().z) * Mathf.Rad2Deg);

        return base.GetSteering(agent);
    }
}
