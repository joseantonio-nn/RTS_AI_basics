using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trabajador : Soldado {

    // Atributos
    private bool tengoRecurso = false;
    private bool recolectando = false;
    private bool construyendo = false;
    private bool esperando = false;
    private Vector3 puntoAparicion;
    private Torreta torretaEnConstruccion;
    private List<Recurso> recursos = new List<Recurso>();
    private List<Torreta> torretas = new List<Torreta>();

    public Soldado detectado;

    protected override void Start() {
        base.Start();
        this.vidaMax = 75f;
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
        this.ataque = 5f;
        this.defensa = 10f;
        this.rango = 3f;
        this.distanciaVision = 7f;
        terreno_factor = new Dictionary<string, float>{
            {"Camino", 1f},
            {"Hierba", 1f},
            {"Bosque", 1f}
        };
        puntoAparicion = controlador.GetPosicionAparicion(this.bando);
    }

    void Update() {
        ApplySteering();
        
        canvas.transform.position = transform.position + new Vector3(-2,0,0);
        canvas.transform.rotation = Quaternion.Euler(90,0,90);

        if (!controlador.FinPartida()) {

            if (vida > 0) {

                detectado = buscarEnemigo();
                
                if (detectado != null && (this.transform.position - detectado.transform.position).magnitude <= rango) {
                        
                    steerings.Remove(pathFollowing);
                    Destroy(pathFollowing);
                    velocidad_vec = Vector3.zero;

                    enCamino = false;
                    yendoAltarget = false;

                    if (!atacando) StartCoroutine(atacarEnemigo(detectado));
                    atacando = true;

                } else if (detectado != null) {
                    
                    atacando = false;
                    yendoAltarget = true;
                    steerings.Remove(pathFollowing);
                    Destroy(pathFollowing);
                    pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);
                    path = pathFinding.AStar(this.transform.position, detectado.transform.position, controlador.mapaInfluencias, this);
                    pathFollowing.path = path;
                
                } else if (!yendoAltarget) {

                    if (tengoRecurso || esperando) {
                        
                        if (!esperando) this.torretas = controlador.GetTorretasSinConstruir(this.bando);
                        
                        if (this.torretas.Count != 0) {
                            
                            Torreta t = null;
                            if (!esperando) {
                                t = torretaCercana();
                                torretaEnConstruccion = t;
                            }
                            
                            Vector3 distancia = this.transform.position - torretaEnConstruccion.transform.position;
                            distancia.y = 0;
                            if (distancia.magnitude <= rango) {
                                
                                enCamino = false;
                                if (t != null) t.ConstruirTorreta(this);
                                if (!construyendo) StartCoroutine(EsperarConstruccion(Torreta.TIEMPO_CONSTRUCCION));
                                tengoRecurso = false;

                            } else if (!enCamino && t != null) 
                                MoverseAlObjetivo(t.transform.position);

                        } else {
                            MoverseAlObjetivo(puntoAparicion);
                            enCamino = false;
                        }

                    } else {
                        
                        this.recursos = controlador.GetRecursosLibres();
                        
                        if (this.recursos.Count != 0) {
                            
                            Recurso r = recursoCercano(); 
                            float dist = new Vector3(this.transform.position.x - r.transform.position.x, 0, this.transform.position.z - r.transform.position.z).magnitude;  
                            if (dist <= rango) {
                                
                                enCamino = false;
                            
                                if (!recolectando) {
                                    r.IniciarRecoleccion();
                                    StartCoroutine(EsperarRecoleccion(Recurso.TIEMPO_RECOLECCION));
                                }

                            } else if (!enCamino) 
                                MoverseAlObjetivo(r.transform.position);
                            
                            
                        } else if (!enCamino) {
                            MoverseAlObjetivo(puntoAparicion);
                            enCamino = false;
                        }
                    }
                }
                yendoAltarget = false;
                enCamino = false;
            }
        }
    }

    private Recurso recursoCercano() {
        float distMin = Mathf.Infinity;
        Recurso recursoMin = null;
        float distAct;
        foreach (Recurso r in this.recursos) {
            distAct = (transform.position - r.transform.position).magnitude;
            if (distAct < distMin) {
                recursoMin = r;
                distMin = distAct;
            }
        }
        return recursoMin;
    }

    private Torreta torretaCercana() {
        float distMin = Mathf.Infinity;
        Torreta torretaMin = null;
        float distAct;
        foreach (Torreta t in this.torretas) {
            distAct = (transform.position - t.transform.position).magnitude;
            if (distAct < distMin) {
                torretaMin = t;
                distMin = distAct;
            }
        }
        return torretaMin;
    }

    private void MoverseAlObjetivo(Vector3 objetivo) {
        
        steerings.Remove(pathFollowing);
        Destroy(pathFollowing);
        pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);
        path = pathFinding.AStar(this.transform.position, objetivo, controlador.mapaInfluencias, this);
        pathFollowing.path = path;
        if (path.Count != 0) enCamino = true;
    }

    IEnumerator EsperarRecoleccion(float TIEMPO_RECOLECCION) {
        recolectando = true;
        yield return new WaitForSeconds(TIEMPO_RECOLECCION);
        tengoRecurso = true;
        recolectando = false;
    }

    IEnumerator EsperarConstruccion(float TIEMPO_CONSTRUCCION) {
        construyendo = true;
        esperando = true;
        yield return new WaitForSeconds(TIEMPO_CONSTRUCCION);
        construyendo = false;
        esperando = false;
    }

    protected override IEnumerator Reaparecer() {
        yield return new WaitForSeconds(tiempoRespawn);
        tengoRecurso = false;
        recolectando = false;
        construyendo = false;
        esperando = false;
        enCamino = false;
        vida = vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
    }
    
}