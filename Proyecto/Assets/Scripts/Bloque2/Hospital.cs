using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hospital : Estructura {

    private const float SANACION_COOLDOWN = 3f;
    private const float CANTIDAD_SANACION = 30f;

    private bool sanando = false;
    
    protected override void Start() {
        this.ataque = 0f;
        this.rango = 0f;
        this.distanciaVision = 0f;
        this.defensa = 5f;
        this.vidaMax = 80f;
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
    }

    void OnTriggerStay(Collider other) {
        if (this.vida > 0) {
            Soldado s = other.transform.gameObject.GetComponent<Soldado>();
            if (s != null && !sanando && s.bando == this.bando) StartCoroutine(realizarSanacion(s));
        }
    }

    IEnumerator realizarSanacion(Soldado s) {
        sanando = true;
        yield return new WaitForSeconds (SANACION_COOLDOWN);
        s.Sanar(CANTIDAD_SANACION, bando);
        sanando = false;
    }
    
    public override void Atacar(float dano) {
        this.vida -= dano;
        this.hpbar.SetVida(this.vida);

        if (this.vida <= 0) Destruirse();
    }

    public override void Destruirse() {
        this.transform.position -= new Vector3(0, 20, 0);
    }

    public override float GetInfluencia() {
        if (this.vida <= 0) return 0;
        if (bando == Bando.Rojo) return 8.4f;
        else return -8.4f;
    }

}
