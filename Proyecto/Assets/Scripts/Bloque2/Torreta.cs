using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torreta : Estructura {

    public const float TIEMPO_CONSTRUCCION = 5f;
    private float constActual = 0f;
    private bool enConstruccion = false;
    private bool construida = false;
    private Trabajador trabajador = null;

    public Barras construccionbar;

    protected override void Start() {
        this.ataque = 20f;
        this.rango = 25f;
        this.defensa = 5f;
        this.vidaMax = 150f;
        this.vida = this.vidaMax;
        // Barra de vida desactivada (inicialmente no hay torretas)
        this.hpbar.SetVidaMax(vidaMax);
        this.hpbar.gameObject.SetActive(false);
        // Barra de construcción desactivada (inicialmente no se construye nada)
        this.construccionbar.SetTiempoMaxConst(TIEMPO_CONSTRUCCION);
        this.construccionbar.gameObject.SetActive(false);
        // No se muestra la torreta (la ponemos debajo del mapa para ocultarla)
        this.gameObject.transform.position -= new Vector3(0, 20, 0);
    }

    void Update() {
        if (enConstruccion) {
            construccion();
            if (trabajador.vida <= 0 || trabajador.detectado != null) 
                cancelarConstruccion();
        }

        if (construida) {
            Soldado detectado = buscarEnemigo();

            if (detectado != null && (this.transform.position - detectado.transform.position).magnitude <= rango) {
                if (!atacando) StartCoroutine(atacarEnemigo(detectado));
                atacando = true;
            }
        }
    }


    // Método llamado mientras una torreta esté en construcción
    private void construccion() {
        constActual += Time.deltaTime;
        construccionbar.SetTiempoConstruccion(constActual);
        // Si ha terminado de construirse
        if (constActual >= TIEMPO_CONSTRUCCION) {
            vida = vidaMax;
            enConstruccion = false;
            constActual = 0f;
            hpbar.enabled = true;
            construida = true;
            this.construccionbar.gameObject.SetActive(false);
            this.hpbar.gameObject.SetActive(true); 
            this.hpbar.SetVidaMax(vidaMax);
            this.gameObject.transform.position += new Vector3(0, 20, 0); 
        }
    }

    // Método llamado desde otra clase para empezar a construir una torreta
    public void ConstruirTorreta(Trabajador trabajador) {
        this.trabajador = trabajador;
        this.construccionbar.gameObject.SetActive(true);
        enConstruccion = true;
    }

    // Método llamado desde otra clase para dañar una torreta
    public override void Atacar(float dano) {
        if (construida) {
            this.vida -= dano;
            this.hpbar.SetVida(this.vida);

            if (this.vida <= 0) Destruirse();  
        }      
    }

    // Si pierde toda la vida, se destruye
    public override void Destruirse() {
        constActual = 0f;
        construida = false;
        enConstruccion = false;
        this.hpbar.gameObject.SetActive(false);
        this.gameObject.transform.position -= new Vector3(0, 20, 0);
    }

    // Una torreta es construible si no está construida ni en construcción
    public bool Construible() {
        return !enConstruccion && !construida;
    }

    public override float GetInfluencia() {
        if (!construida) return 0;
        else if (bando == Bando.Rojo) return 3;
        else return -3;
    }

    void cancelarConstruccion() {
        enConstruccion = false;
        construida = false;
        constActual = 0f;
        construccionbar.SetTiempoConstruccion(constActual);
        this.construccionbar.gameObject.SetActive(false);
    }
}
