using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanciaManhattan : Heuristica {
    public override float CalcularHeuristica(Nodo nodo, Nodo nodoFinal) {
        float dx = Mathf.Abs(nodo.posicionReal.x - nodoFinal.posicionReal.x);
        float dy = Mathf.Abs(nodo.posicionReal.z - nodoFinal.posicionReal.z);
        return dx + dy;
    }
}
