using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanciaEuclidea : Heuristica {
    public override float CalcularHeuristica(Nodo nodo, Nodo nodoFinal) {
        float dx = Mathf.Abs(nodo.posicionReal.x - nodoFinal.posicionReal.x);
        float dz = Mathf.Abs(nodo.posicionReal.z - nodoFinal.posicionReal.z);
        return Mathf.Sqrt(dx * dx + dz * dz);
    }
}