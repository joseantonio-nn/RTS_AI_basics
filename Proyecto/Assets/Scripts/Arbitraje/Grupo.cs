using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grupo {
    // Atributos
    public List<string> grupo;

    // Quantums
    public int original;
    public int actual;

    public Grupo(List<string> grupo, int original) {
        this.grupo = grupo;
        this.original = original;
        this.actual = original;
    }

    public void reducir() {
        if (actual == 0) return ;
        actual = actual - 1;
    }

    public void reset() {
        actual = original;
    }
}
