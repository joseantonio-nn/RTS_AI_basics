using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Las tres heurísticas a implementar vienen de: http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html

public abstract class Heuristica {
    public abstract float CalcularHeuristica(Nodo nodo, Nodo nodoFinal);
}
