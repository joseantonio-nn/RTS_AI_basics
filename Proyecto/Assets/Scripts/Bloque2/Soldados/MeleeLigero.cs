using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeLigero : Soldado {
    
    protected override void Start() {
        base.Start();
        this.vidaMax = 100f;
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
        this.ataque = 20f;
        this.defensa = 10f;
        this.rango = 5f;
        this.distanciaVision = 10f;
        this.terreno_factor = new Dictionary<string, float>{
            {"Camino", 0.5f},
            {"Hierba", 1f},
            {"Bosque", 1f}
        };
    }

}