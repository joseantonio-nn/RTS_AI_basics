using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePesado : Soldado {
    
    protected override void Start() {
        base.Start();
        this.vidaMax = 150f;
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
        this.ataque = 15f;
        this.defensa = 10f;
        this.rango = 5f;
        this.distanciaVision = 10f;
        terreno_factor = new Dictionary<string, float>{
            {"Camino", 1f},
            {"Hierba", 2f},
            {"Bosque", 3f}
        };
    }
}
