using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Collision Avoidance predictivo
public class CollisionAvoidance : SteeringBehaviour {
    
    private List<Agent> targets;
    private Vector3 targetDebug = Vector3.zero;

    public float radius = 20f;
    public GameObject targetsGO;


    void Awake() {
        this.targets = new List<Agent>(targetsGO.transform.GetComponentsInChildren<Agent>());
    }


    public override Steering GetSteering(AgentNPC agent) {
        float shortestTime = Mathf.Infinity;

        Agent firstTarget = null;
        float firstMinSeparation = 0f;
        float firstDistance = 0f;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;

        Vector3 relativePos = Vector3.zero;
        Vector3 relativeVel = Vector3.zero;
        float relativeSpeed = 0f;
        float timeToCollision = 0f;
        float distance = 0f;
        float minSeparation = 0f;

        // Se itera sobre cada target
        foreach (Agent t in targets) {

            // Si el target es el agente nos lo saltamos
            if ((AgentNPC) t == agent) continue;

            // Se calcula el tiempo que falta para la siguiente colisión
            relativePos = t.transform.position - agent.transform.position;
            relativeVel = t.getVelocidadVec() - agent.getVelocidadVec();
            relativeSpeed = relativeVel.magnitude;
            timeToCollision = Mathf.Abs((relativePos.x * relativeVel.x) + (relativePos.z * relativeVel.z)) / (relativeSpeed * relativeSpeed);

            // Comprobar si va a haber colisión
            distance = relativePos.magnitude;
            minSeparation = distance - relativeSpeed * timeToCollision;
    
            if (minSeparation > 2 * radius) continue;
            
            // Se comprueba si es la colisión más próxima en el tiempo
            if (timeToCollision > 0 && timeToCollision < shortestTime) {
                
                // Se almacena el tiempo, el target y otras variables
                shortestTime = timeToCollision;
                firstTarget = t;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        }

        Steering steering = new Steering();

        // Si no se tiene target, se sale
        if (firstTarget == null) return steering;
        
        // Si se va a colisionar y estamos colisionando, se modifica el steering en función
        // de la posición actual
        if (firstMinSeparation <= 0 || firstDistance < 2 * radius) {
            relativePos = firstTarget.transform.position - agent.transform.position;
        } else {
            relativePos = firstRelativePos + firstRelativeVel * shortestTime;
        }

        targetDebug = relativePos;
        
        Debug.DrawLine(agent.transform.position, targetDebug, Color.gray);

        // Se evita al target
        relativePos.Normalize();

        Vector3 new_target = firstTarget.transform.position + (relativePos - firstTarget.transform.position).normalized * (radius + 10f);
        steering.linear = (new_target - agent.transform.position).normalized * agent.getAceleracionMax();
        
        steering.angular = 0f;
        Debug.DrawLine(agent.transform.position, steering.linear, Color.red);

        // Se devuelve el steering
        return steering;
    }

    void OnDrawGizmos() {
        if (show) {
            #if UNITY_EDITOR
                UnityEditor.Handles.DrawWireDisc(targetDebug, new Vector3(0,1,0), radius);
            #endif
        }
    }
}
