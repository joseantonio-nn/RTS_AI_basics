using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Soldado {
    
    protected override void Start() {
        base.Start();
        this.vidaMax = 70f;
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
        this.ataque = 15f;
        this.defensa = 10f;
        this.rango = 25f;
        terreno_factor = new Dictionary<string, float>{
            {"Camino", 2f},
            {"Hierba", 0.1f},
            {"Bosque", 2f}
        };
    }
}