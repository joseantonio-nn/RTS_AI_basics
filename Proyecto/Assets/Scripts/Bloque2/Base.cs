using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Estructura {

    protected override void Start() {
        this.ataque = 0f;
        this.rango = 0f;
        this.distanciaVision = 0f;
        this.defensa = 5f;
        this.vidaMax = 500f;
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
    }
    
    public override void Atacar(float dano) {
        this.vida -= dano;
        this.hpbar.SetVida(this.vida);

        if (this.vida <= 0) Destruirse();
    }

    public override void Destruirse() {
        if (this.bando == Bando.Rojo)
            controlador.Ganar(Bando.Azul);
        else 
            controlador.Ganar(Bando.Rojo);
    }

    public override float GetInfluencia() {
        if (this.vida <= 0) return 0;
        if (bando == Bando.Rojo) return 8.4f;
        else return -8.4f;
    }
}
