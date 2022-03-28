using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  
    WallAvoidance utilizando General Bigotes, lo que conlleva el uso de tres bigotes. 
    El principal y más largo mira al frente y los otros dos son más cortos
    que el primero y miran con cierto ángulo a derecha e izquierda. 
*/

public class WallAvoidance : Seek {

    public float avoidDistance = 4.0f;
    public int lookahead = 10;

    public override Steering GetSteering(AgentNPC agent) {
        // Rayo para cada bigote
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        // Los 3 bigotes
        Vector3 rayVector = agent.getVelocidadVec().normalized;        
        rayVector *= lookahead;
        Vector3 rayVector2 = new Vector3(rayVector.x * Mathf.Cos(30 * Mathf.Deg2Rad) - rayVector.z * Mathf.Sin(30 * Mathf.Deg2Rad), rayVector.y, rayVector.x * Mathf.Sin(30 * Mathf.Deg2Rad) + rayVector.z * Mathf.Cos(30 * Mathf.Deg2Rad)).normalized;
        rayVector2 *= (lookahead / 2);
        Vector3 rayVector3 = new Vector3(rayVector.x * Mathf.Cos(-30 * Mathf.Deg2Rad) - rayVector.z * Mathf.Sin(-30 * Mathf.Deg2Rad), rayVector.y, rayVector.x * Mathf.Sin(-30 * Mathf.Deg2Rad) + rayVector.z * Mathf.Cos(-30 * Mathf.Deg2Rad)).normalized;
        rayVector3 *= (lookahead / 2);
        // Dibujando los bigotes
        if (show) {
            Debug.DrawLine(agent.transform.position, agent.transform.localPosition + rayVector);
            Debug.DrawLine(agent.transform.position, agent.transform.localPosition + rayVector2);
            Debug.DrawLine(agent.transform.position, agent.transform.localPosition + rayVector3);
        }
        // Vemos los rayos de los 3 bigotes
        bool rayoCe = Physics.Raycast(agent.transform.position, rayVector, out hit1, lookahead);
        bool rayoDe = Physics.Raycast(agent.transform.position, rayVector2, out hit2, lookahead / 2);
        bool rayoIz = Physics.Raycast(agent.transform.position, rayVector3, out hit3, lookahead / 2);

        /* Si golpean los 3, es que nos encontramos en una esquina (caso un poco especial, 
           pues si la esquina es de 90º los bigotes laterales tienen el problema de que sus 
           normales corresponden con la otra esquina y por tanto acaban atravesando la pared),
           por tanto, hacemos que siempre gire en la misma dirección para que salga de ahí */
        if (rayoCe && rayoDe && rayoIz) {
            if (hit1.collider != null && hit1.collider.CompareTag("Obstaculo") && 
                hit2.collider != null && hit2.collider.CompareTag("Obstaculo") &&
                hit3.collider != null && hit3.collider.CompareTag("Obstaculo")) {
                    base.target.transform.position = hit2.point + hit2.normal * avoidDistance;
                    base.target.transform.position = new Vector3(base.target.transform.position.x, 1, base.target.transform.position.z);
                    if (show)
                        Debug.DrawLine(hit2.point, hit2.point + hit2.normal * avoidDistance);
                    return base.GetSteering(agent);
            }
        }

        if (rayoDe && hit2.collider != null && hit2.collider.CompareTag("Obstaculo")) {
            base.target.transform.position = hit2.point + hit2.normal * avoidDistance;
            base.target.transform.position = new Vector3(base.target.transform.position.x, 1, base.target.transform.position.z);
            if (show)
                Debug.DrawLine(hit2.point, hit2.point + hit2.normal * avoidDistance);
            return base.GetSteering(agent);
        }

        if (rayoIz && hit3.collider != null && hit3.collider.CompareTag("Obstaculo")) {
            base.target.transform.position = hit3.point + hit3.normal * avoidDistance;
            base.target.transform.position = new Vector3(base.target.transform.position.x, 1, base.target.transform.position.z);
            if (show)
                Debug.DrawLine(hit3.point, hit3.point + hit3.normal * avoidDistance);
            return base.GetSteering(agent);
        }

        if (rayoCe && hit1.collider != null && hit1.collider.CompareTag("Obstaculo")) {
            base.target.transform.position = hit1.point + hit1.normal * avoidDistance;
            base.target.transform.position = new Vector3(base.target.transform.position.x, 1, base.target.transform.position.z);
            if (show)
                Debug.DrawLine(hit1.point, hit1.point + hit1.normal * avoidDistance);
            return base.GetSteering(agent);
        }

        return new Steering();    
    }
}