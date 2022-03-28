using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpose : Arrive {

    // Al igual que en face, explicitTarget contendrá el player, es decir, el primer target entre el que nos situaremos
    public Agent explicitTarget;

    // SecondTarget será el segundo NPC entre el que nos situaremos
    public Agent secondTarget;

    public override Steering GetSteering(AgentNPC agent) {

        //Target solo será el target auxiliar que utilizaremos para hacer Arrive y por tanto podremos manipular su position sin problema

        // Vemos el punto medio de los agentes, que será donde interceptaremos
        Vector3 puntoMedio = (explicitTarget.transform.position + secondTarget.transform.position) / 2f;

        // El tiempo que tarden los agentes en llegar al punto medio vendrá dado por su velocidad máxima
        float tiempoPuntoM = ((secondTarget.transform.position - puntoMedio).magnitude) / secondTarget.getVelocidadMax();

        // Calculamos donde se encontrarán los agentes pasado el tiempo T
        Vector3 APos = explicitTarget.transform.position + explicitTarget.getVelocidadVec() * tiempoPuntoM;
        Vector3 BPos = secondTarget.transform.position + secondTarget.getVelocidadVec() * tiempoPuntoM;

        // Vemos ese punto medio (futuro)
        puntoMedio = (APos + BPos) / 2f;

        // Y se lo asignamos al target auxiliar que nos servirá para hacerle arrive
        target.transform.position = puntoMedio;

        return base.GetSteering(agent);;
    }
}
