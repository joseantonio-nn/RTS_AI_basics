using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Espia : Soldado {

    //Espía tiene preferencia de ir al bosque

    protected override void Start() {
        base.Start();
        this.vidaMax = 60f;
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
        this.ataque = 50f;
        this.defensa = 5f;
        this.rango = 4f;
        this.distanciaVision = 10f;
        terreno_factor = new Dictionary<string, float>{
            {"Camino", 2f},
            {"Hierba", 0.5f},
            {"Bosque", 0.1f}
        };
    }

    // El espía siempre estará en modo OFENSIVO
    void Update() {
        ApplySteering();
        
        canvas.transform.position = transform.position + new Vector3(-2,0,0);
        canvas.transform.rotation = Quaternion.Euler(90,0,90);

        if (!controlador.FinPartida()) {

            if (vida > 0) {

                Soldado detectado = buscarEnemigo();
                
                if (detectado != null && (this.transform.position - detectado.transform.position).magnitude <= rango) {
                    
                    steerings.Remove(pathFollowing);
                    Destroy(pathFollowing);
                    velocidad_vec = Vector3.zero;

                    enCaminoWaypoint = false;
                    yendoAltarget = false;

                    if (!atacando) StartCoroutine(atacarEnemigo(detectado));

                } else if (detectado != null) {
                    
                    atacando = false;
                    yendoAltarget = true;
                    steerings.Remove(pathFollowing);
                    Destroy(pathFollowing);
                    pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);
                    path = pathFinding.AStar(this.transform.position, detectado.transform.position, controlador.mapaInfluencias, this);
                    pathFollowing.path = path;
                
                } else if (!yendoAltarget) {
                    
                    atacando = false;
                    if (!enCaminoWaypoint) {
                        
                        if (vida > vidaMax * 0.5f || controlador.HayHospitales(this.bando).Count == 0) {

                            steerings.Remove(pathFollowing);
                            Destroy(pathFollowing);
                            pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);
                            path = AsignarObjetivo();
                            pathFollowing.path = path;

                            if (path.Count != 0) enCaminoWaypoint = true;
                        
                        } else if (vida <= vidaMax * 0.5f) {
                            steerings.Remove(pathFollowing);
                            Destroy(pathFollowing);
                            pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);
                            path = controlador.ModoAntisuicida(this);
                            curando = true;
                            pathFollowing.path = path;

                            if (path.Count != 0) enCaminoWaypoint = true;
                        }

                    } else {
                        
                        atacando = false;
                        if (NodoFinalAlcanzado()) {
                            
                            if (!curando) {
                                enCaminoWaypoint = false;
                                yendoAltarget = false;
                            } else if (vida == vidaMax) {
                                curando = false;
                            }

                        }
                    }       
                }  
            }
        }   
    }
}