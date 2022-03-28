using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Face {
    //Distancia al círculo; Radio del círculo; Máximo ratio de cambio de orientacion (giro); Orientación actual del wander target; Aceleracion máxima del personaje
    public float wanderOffset, wanderRadius, wanderRate, wanderOrientation;
    private Vector3 targetDebug;
    
    static float randomBinomial() {
        return Random.value - Random.value;
    }

    Vector3 orientacionToVector(float targetOrientation) {
        targetOrientation = targetOrientation * Mathf.Deg2Rad;
        Vector3 x = (new Vector3(Mathf.Sin(targetOrientation), 0, Mathf.Cos(targetOrientation)));
        return x;
    }

    public override Steering GetSteering(AgentNPC agent) {
        
        // 1. Calculate the target to delegate to face
        // Update the wander orientation
        wanderOrientation += randomBinomial() * wanderRate;
        
        // Calculate the combined target orientation
        float targetOrientation = wanderOrientation + agent.getOrientacion();

        Vector3 target = agent.transform.position + wanderOffset * agent.orientacionToVector();

        targetDebug = target;
        if (show)
            Debug.DrawLine(agent.transform.position, target, Color.gray);

        // Calculate the target location
        target += wanderRadius * orientacionToVector(targetOrientation);
        base.explicitTarget.transform.position = target;

        // 2. Delegate to face
        Steering steering = base.GetSteering(agent);

        // 3. Now set the linear acceleration to be at full acceleration in the direction of the orientation
        steering.linear = agent.getAceleracionMax() * agent.orientacionToVector();

        return steering; 
    }

    void OnDrawGizmos() {
        if (show) {
            #if UNITY_EDITOR
                UnityEditor.Handles.DrawWireDisc(targetDebug, new Vector3(0,1,0), wanderRadius);
            #endif
        }
    }
}