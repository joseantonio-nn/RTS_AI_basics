using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Actitud {OFENSIVO, DEFENSIVO}

public class Soldado : AgentNPC {
    // Estadísticas
    protected float vidaMax;
    public float vida;
    protected float ataque;
    protected float defensa;
    protected float rango;
    protected float distanciaVision = 15f;

    // Otros
    private const float cadencia = 0.3f;
    protected const float tiempoRespawn = 12f;
    private Vector3[] caminoPatrulla;
    public bool patrullando = false;
    public Barras hpbar;
    public Bando bando;
    public Actitud actitud;
    private Vector3 objetivo;
    
    protected bool yendoAltarget = false;
    private bool enCaminoPatrulla = false;
    protected bool enCaminoWaypoint = false;
    protected bool curando = false;
    protected bool atacando = false;
    private List<Transform> waypointsMarcados = new List<Transform>();

    private int numNodosPatrulla;
    private int nodoActualPatrulla;
    protected Canvas canvas;

    protected PathFollowing pathFollowing;
    protected Dictionary<string, float> terreno_factor = new Dictionary<string, float>();

    protected override void Start() {
        base.Start();

        canvas = this.transform.GetChild(0).GetComponent<Canvas>();
        if (patrullando) {
            
            caminoPatrulla = this.controlador.obtenerRutaPatrulla(this.bando);
            
            numNodosPatrulla = caminoPatrulla.Length;
            nodoActualPatrulla = 0; 

            
            pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);
            path = pathFinding.AStar(this.transform.position, caminoPatrulla[nodoActualPatrulla], controlador.mapaInfluencias, this);
            pathFollowing.path = path;
            if (path.Count != 0) enCaminoPatrulla = true;
        }
    }

    void Update() {
        ApplySteering();
        
        canvas.transform.position = transform.position + new Vector3(-2,0,0);
        canvas.transform.rotation = Quaternion.Euler(90,0,90);

        if (!controlador.FinPartida()) {

            if (vida > 0) {

                if (patrullando)  {

                    Soldado detectado = buscarEnemigo();
                    
                    if (detectado != null && (this.transform.position - detectado.transform.position).magnitude <= rango) {
                        
                        steerings.Remove(pathFollowing);
                        Destroy(pathFollowing);
                        velocidad_vec = Vector3.zero;

                        enCaminoPatrulla = false;
                        yendoAltarget = false;

                        if (!atacando) StartCoroutine(atacarEnemigo(detectado));

                    } else if (detectado != null) {
                        
                        atacando = false;
                        yendoAltarget = true;
                        reasignarCamino(false, detectado);

                    } else if (!yendoAltarget) {
                        
                        atacando = false;
                        if (!enCaminoPatrulla) {
                            pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);
                            path = pathFinding.AStar(this.transform.position, caminoPatrulla[nodoActualPatrulla], controlador.mapaInfluencias, this);
                            pathFollowing.path = path;
                            if (path.Count != 0) enCaminoPatrulla = true;
                        } else {
                
                            if (NodoFinalAlcanzado()) {
                                steerings.Remove(pathFollowing);
                                Destroy(pathFollowing);

                                // Lo pongo a false para que suba por el if de arriba que comprueba que enCamino está a false 
                                enCaminoPatrulla = false; 
                                nodoActualPatrulla = (nodoActualPatrulla + 1) % numNodosPatrulla;   
                            }
                        } 

                    }

                } else {
    
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
                        reasignarCamino(false, detectado);
                    
                    } else if (!yendoAltarget) {
                        
                        atacando = false;
                        if (!enCaminoWaypoint) {
                            
                             if (actitud == Actitud.OFENSIVO) {
                            
                                reasignarCamino(false, null);

                                if (path.Count != 0) enCaminoWaypoint = true;

                            } else if (actitud == Actitud.DEFENSIVO) {
  
                                if (vida > vidaMax * 0.3f || controlador.HayHospitales(this.bando).Count == 0) {

                                    reasignarCamino(false, null);

                                    if (path.Count != 0) enCaminoWaypoint = true;
                                
                                } else if (vida <= vidaMax * 0.3f) {

                                    reasignarCamino(true, null);
                                    curando = true;

                                    if (path.Count != 0) enCaminoWaypoint = true;

                                }
                            
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
                yendoAltarget = false; 
            }
        }   
    }

    void reasignarCamino(bool antisuicida, Soldado detectado) {
        steerings.Remove(pathFollowing);
        Destroy(pathFollowing);
        pathFollowing = (PathFollowing) addBehaviour("PathFollowing", 1, agenteAux);

        if (antisuicida)
            path = controlador.ModoAntisuicida(this);
        else {
            if (detectado != null)
                path = pathFinding.AStar(this.transform.position, detectado.transform.position, controlador.mapaInfluencias, this);
            else
                path = AsignarObjetivo();
        }
        pathFollowing.path = path;
    }
    
    void DebugTodo() {
        Debug.Log("yendoAlTarget: " + yendoAltarget + "; curando: " + curando + "; atacando: " + atacando + "; enCaminoWaypoint: " + enCaminoWaypoint);
    }

    protected bool NodoFinalAlcanzado() {
        return path.Count != 0 && (this.transform.position - this.path[path.Count-1].posicionReal).magnitude < 2f;
    }

    public virtual float GetInfluencia() {
        if (this.bando == Bando.Azul) return -1 * (this.vida + this.ataque) * 0.07f;
        else return (this.vida + this.ataque) * 0.07f;
    }

    // Acciones
    public virtual void Atacar(float ataque) {
        float real;

        // Randomizamos el daño de ataque
        ataque += Random.Range(-5,10);

        real = ataque - defensa;
        if (real < 0f) real = 0f;

        vida -= real;
        this.hpbar.SetVida(this.vida);
        if (vida <= 0) Matar();
    }

    public void Matar() {
        vida = 0;
        Vector3 puntoAparicion = controlador.GetPosicionAparicion(this.bando);
        this.transform.position = puntoAparicion;
        this.velocidad_vec = Vector3.zero;

        steerings.Remove(pathFollowing);
        Destroy(pathFollowing);
        path = new List<Nodo>();

        StartCoroutine("Reaparecer");
    }

    public void Sanar(float sanacion, Bando bando) {
        if (this.bando == bando && vida < vidaMax) {
            this.vida += sanacion;
            if (this.vida > this.vidaMax)
                this.vida = this.vidaMax;
        }
        this.hpbar.SetVida(this.vida);
    }

    protected virtual IEnumerator Reaparecer() {
        yield return new WaitForSeconds(tiempoRespawn);
        this.vida = this.vidaMax;
        this.hpbar.SetVidaMax(this.vidaMax);
        yendoAltarget = false;
        enCaminoPatrulla = false;
        enCaminoWaypoint = false;
        curando = false;
        atacando = false;
        nodoActualPatrulla = 0;
        waypointsMarcados = new List<Transform>();
        
    }

    // Método utilizado para localizar enemigos
    protected Soldado buscarEnemigo() {
        int i = 0;
        float distMin = Mathf.Infinity;
        float distAct;
        Soldado alcanzado;
        Soldado masProximo = null;

        Collider[] soldadosAlcanzados = Physics.OverlapSphere(transform.position, distanciaVision, LayerMask.GetMask("NPC"));
    
        while (i < soldadosAlcanzados.Length) {
    
            alcanzado = soldadosAlcanzados[i].gameObject.GetComponent<Soldado>();
            if (alcanzado.vida <= 0) {
                i++;
                continue;
            }

            if ((this.bando == Bando.Rojo && alcanzado.bando == Bando.Azul) || 
                (this.bando == Bando.Azul && alcanzado.bando == Bando.Rojo)) {
                    // Calculamos el enemigo más próximo
                    distAct = (transform.position - alcanzado.transform.position).magnitude;
                    if (distAct < distMin && distAct < distanciaVision) {
                        distMin = distAct;
                        masProximo = alcanzado;
                    }
            } 
            
            i++;
        }
    
        return masProximo;
    }

    protected IEnumerator atacarEnemigo(Soldado soldado) {
        atacando = true;
        while (soldado.vida > 0 && this.vida > 0) {
            yield return new WaitForSeconds(cadencia);
            if ((this.bando == Bando.Rojo && soldado.bando == Bando.Azul) || (this.bando == Bando.Azul && soldado.bando == Bando.Rojo))
                    soldado.Atacar(this.ataque);
        }
        atacando = false;
    }

    public void ActivarGuerraTotal(Actitud act, Vector3 objetivo) {
        this.actitud = act;
        this.objetivo = objetivo;
    }

    public void CambiarActitud(Actitud act) {
        this.actitud = act;
    }

    // Método utilizado para establecer un waypoint objetivo (que será calculado en función de la influencia)
    public List<Nodo> AsignarObjetivo() {
        
        List<Transform> wayPoints = new List<Transform>();

        if (this.bando == Bando.Rojo && this.actitud == Actitud.OFENSIVO) 
            wayPoints = controlador.GetWaypoints(Bando.Azul, this.bando);
        else if (this.bando == Bando.Rojo && this.actitud == Actitud.DEFENSIVO) 
            wayPoints = controlador.GetWaypoints(Bando.Rojo, this.bando);  
        else if (this.bando == Bando.Azul && this.actitud == Actitud.OFENSIVO) 
            wayPoints = controlador.GetWaypoints(Bando.Rojo, this.bando); 
        else if (this.bando == Bando.Azul && this.actitud == Actitud.DEFENSIVO) 
            wayPoints = controlador.GetWaypoints(Bando.Azul, this.bando);
            
        float minDist = Mathf.Infinity;
        Transform waypointEscogido = null;
    
        for (int i = 0; i < wayPoints.Count; i++) {
            if (minDist > CalcularDistanciaWaypoint(wayPoints[i].position)) {
                minDist = CalcularDistanciaWaypoint(wayPoints[i].position);
                waypointEscogido = wayPoints[i];
            }
        }
        List<Nodo> camino = pathFinding.AStar(this.transform.position, waypointEscogido.position, controlador.mapaInfluencias, this);
        return camino;
    }

    public float CalcularDistanciaWaypoint(Vector3 posicionWaypoint) {
        return (this.transform.position - posicionWaypoint).magnitude;
    }


    void Desplazar(Vector3 destino) {

        Nodo nodoFinal = pathFinding.grid.posicionRealToNodo(destino);
        Nodo nodoInicial = pathFinding.grid.posicionRealToNodo(transform.position);

        if (nodoFinal == null) return; 

        path = this.pathFinding.AStar(transform.position, destino, controlador.mapaInfluencias, this);
        
        // Ahora iremos a cada uno de los nodos calculados con A* mediante el steering PathFollowing
        if (show) pathFinding.grid.DibujarCamino(path);
    }

    // Terrenos
    public float Factor(string terreno) {
        // Si no hay terreno
        if (terreno == null) return 1f;

        // Si hay terreno
        // Si no tiene factor de terreno
        if (!terreno_factor.ContainsKey(terreno)) return 1f;

        // Si tiene factor de terreno
        return terreno_factor[terreno];
    }


    public float CosteTerreno(string terreno) {
        // Si no hay terreno
        if (terreno == null) return 0f;

        // Si hay terreno
        // Si desconocido
        if (!Terrenos.coste.ContainsKey(terreno)) return 0f;

        // Si conocido
        return Factor(terreno) * Terrenos.coste[terreno];
    }

    public void OnTriggerEnter(Collider col) {
        var terreno = col.tag;
        var coste = CosteTerreno(terreno);
        coef_frenado = 1f/(1+Mathf.Abs(coste));
    }
}