using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictivePathFollowing : Seek {

    public GameObject targetsCamino; // path
    int pathOffset = 1;              // A qué nodo del path se encuentra haciendo Seek
    int currentParam = 0;            
    float predictTime = 0.1f;

    public override Steering GetSteering(AgentNPC agent) {

        Vector3 futurePos = agent.transform.position + agent.getVelocidadVec() * predictTime;
    
        currentParam = getParam(futurePos, agent.transform.position);

        int targetParam = (currentParam + pathOffset) % targetsCamino.transform.childCount;

        target.transform.position = getPosition(targetParam);

        return base.GetSteering(agent);
    }

    // Función que devuelve el índice del camino con el punto más cercano a la futurePos
    int getParam(Vector3 futurePos, Vector3 currentPos) {
        // i_min será el índice del nodo del que más cerca nos encontremos, por tanto, será al que nos interese ir (inicialmente el primer nodo)
        int i_min = 0;
        // dist_min contendrá la distancia más pequeña al nodo i_min (inicialmente la distancia de futurePos con el primer nodo)
        float dist_min = (futurePos - targetsCamino.transform.GetChild(0).gameObject.transform.position).magnitude;

        // Recorremos el resto de nodos para ver si alguno mejora la dist_min
        for (int i=1; i<targetsCamino.transform.childCount; i++) {
            Vector3 posicionNodo = targetsCamino.transform.GetChild(i).gameObject.transform.position;
            float dist = (futurePos - posicionNodo).magnitude;
            if (dist < dist_min) {
                i_min = i;
                dist_min = dist;
            }
        }
        return i_min;
    }

    // Función que devolverá la posición en la que se encuentra el siguiente punto al que tenemos que ir
    Vector3 getPosition(int targetParam) {
        return targetsCamino.transform.GetChild(targetParam).gameObject.transform.position;
    }

}