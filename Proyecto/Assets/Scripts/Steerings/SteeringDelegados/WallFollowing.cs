using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallFollowing : Seek {
    
    public float avoidDistance = 3.0f;
    public int lookahead = 10;
    public int numeroRayos = 10;
    public int angulosEntreRayos = 20;
    
    private float predictTime = 0.25f;
     
    Dictionary<int, RaycastHit> rayos = new Dictionary<int, RaycastHit>();
     
    public override Steering GetSteering(AgentNPC agent) {

        Vector3 futurePos = agent.transform.position + agent.getVelocidadVec() * predictTime;

        ConstruirRayos(agent, futurePos);
 
        foreach (KeyValuePair<int, RaycastHit> rayo in rayos) {
            RaycastHit hit = rayo.Value; 

            if (hit.collider != null && hit.collider.CompareTag("Obstaculo")) {
                
                base.target.transform.position = hit.point + hit.normal * avoidDistance;
                base.target.transform.position = new Vector3(base.target.transform.position.x, 1, base.target.transform.position.z);
                if (show)
                    Debug.DrawLine(hit.point, hit.point + hit.normal * avoidDistance);
                return base.GetSteering(agent);
            }
        }

        base.target.transform.position = futurePos;
        return base.GetSteering(agent);
        
    }

    private void ConstruirRayos(AgentNPC agent, Vector3 futurePos) {
        
        rayos.Clear();
        float ang = agent.getOrientacion() - angulosEntreRayos;

        Vector3 rayVectorAux;
        Vector3 rayVector = agent.getVelocidadVec().normalized; 

        for (int i = 0; i < numeroRayos; i++) {
            RaycastHit hit = new RaycastHit();
            rayVectorAux = new Vector3(rayVector.x * Mathf.Cos(ang * Mathf.Deg2Rad) - rayVector.z * Mathf.Sin(ang * Mathf.Deg2Rad), rayVector.y, rayVector.x * Mathf.Sin(ang * Mathf.Deg2Rad) + rayVector.z * Mathf.Cos(ang * Mathf.Deg2Rad)).normalized;
            rayVectorAux *= (lookahead);

            if (show)
                Debug.DrawLine(agent.transform.position, agent.transform.localPosition + rayVectorAux);

            bool choque = Physics.Raycast(futurePos, rayVectorAux, out hit, lookahead);
            rayos.Add(i, hit);
            ang += angulosEntreRayos;
        }
    }
}
