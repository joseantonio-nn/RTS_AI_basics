using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour {

    public Agent target;
    public float peso = 1.0f; // [1, in)
    protected bool show = false;

    // Se encargará de calcular el Steering para el agente dado en función del comportamiento deseado.
    public abstract Steering GetSteering(AgentNPC agent);

    public void SetTarget(Agent agenteAux) { 
        string tipo = this.GetType().ToString();
        // Solo se asigna el auxiliar a los scripts que necesitan un agente auxiliar para que funcionen
        if (tipo == "Pursue" || tipo == "Face" || tipo == "Wander" || tipo == "OffsetPursuit" ||
            tipo == "WallAvoidance" || tipo == "WallFollowing" || tipo == "Interpose" || 
            tipo == "PredictivePathFollowing" || tipo == "Cohesion" || tipo == "Alignment" || tipo == "Separation" ||
            tipo == "PathFollowing" || tipo == "LookWhereYouGoing")
            this.target = agenteAux; 
    }
        
    public void actualizarShowB(bool show) {this.show = show;}
}
