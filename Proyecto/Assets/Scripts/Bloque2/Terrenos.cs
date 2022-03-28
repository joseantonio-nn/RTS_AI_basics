using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrenos {
    public static readonly Dictionary<string, float> coste = new Dictionary<string, float>{
        {"Camino", 1f},
        {"Hierba", 5f},
        {"Bosque", 10f}
    };
}
