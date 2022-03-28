using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq; 
using UnityEngine.UI;

public class Controlador : MonoBehaviour {

    [HideInInspector]
    public List<AgentNPC> npcSeleccionados = new List<AgentNPC>();
    
    // Lista de agentes (para debug)
    public GameObject agents;
    private List<Agent> agentsShow;
    
    // Rango de seleccion, con el ratón, de NPCs
    private const float rango = Mathf.Infinity;

    private bool modoDepuracion = false; 

    private AgentNPC npc_sel;
    private AgentNPC liderFormacion;

    /* ----------------------------------------------------------------------------------
    ----------------------------------- Atributos Bloque 2 ------------------------------
    ---------------------------------------------------------------------------------- */
    public GameObject respawnRojo;
    public GameObject respawnAzul;
    public GameObject recursosGO;
    public GameObject caminoPatrullaRojo;
    public GameObject caminoPatrullaAzul;
    public GameObject waypointsRojos;
    public GameObject waypointsAzules;
    public GameObject defensas;
    public GameObject edificios;
    public Grid grid;
    private List<Recurso> recursos;
    private List<Estructura> estructuras;
    private List<Torreta> torretas;
    private List<Soldado> soldados;
    public List<List<float>> mapaInfluencias;
    public MiniMapa miniMapa;
    private bool finalJuego = false;
    public GameObject mensajeFinJuego;


    void Start() {
        this.agentsShow = new List<Agent>(agents.transform.GetComponentsInChildren<Agent>());
        
        if (recursosGO != null) this.recursos = new List<Recurso>(recursosGO.transform.GetComponentsInChildren<Recurso>());
        if (defensas != null) this.torretas = new List<Torreta>(defensas.transform.GetComponentsInChildren<Torreta>());
        soldados = agents.transform.GetComponentsInChildren<Soldado>().ToList();
        if (defensas != null && edificios != null) {
            this.estructuras = new List<Estructura>(edificios.transform.GetComponentsInChildren<Estructura>());
            foreach(Torreta t in torretas) soldados.Add(t);
            foreach(Estructura e in estructuras) soldados.Add(e); 
        }
        if (miniMapa != null) StartCoroutine("CalcularInfluencias");
    }


    void Update() {

        // Click izquierdo sin control solo selecciona un NPC
        if (Input.GetButtonDown("Fire1") && !Input.GetKey(KeyCode.LeftControl)) {
            vaciarListaNPC();
            seleccionar();
        } // Click izquierdo con control añade un NPC
        else if (Input.GetButtonDown("Fire1") && Input.GetKey(KeyCode.LeftControl))
            seleccionar();

        // Click derecho mueve NPC's
        if (Input.GetButtonDown("Fire2") && !vacia()) 
            moverNPCS();

        // Formaciones
        if (!vacia()) {
            if (Input.GetKeyDown(KeyCode.Z)) 
                hacerFormacion(0);

            if (Input.GetKeyDown(KeyCode.X)) 
                hacerFormacion(1);

            if (Input.GetKeyDown(KeyCode.C)) 
                hacerFormacion(2);
            
            if (Input.GetKeyDown(KeyCode.V)) 
                hacerFormacion(3);
            
            if (Input.GetKeyDown(KeyCode.B)) 
                hacerFormacion(4);

            if (Input.GetKeyDown(KeyCode.N))
                hacerFormacionRolesDuros();
            
            if (Input.GetKeyDown(KeyCode.M))
                hacerFormacionRolesSuaves();
        }

        // Modo depuración
        if (Input.GetKeyDown(KeyCode.Tab)) {
            this.modoDepuracion = !this.modoDepuracion;
            foreach(Agent ag in agentsShow) {
                ag.SendMessage("actualizarShow", this.modoDepuracion);
            }
        }
    }


    // Método para añadir un agente a la lista de seleccionados y dibujar un aro cuando se le hace click
    void seleccionar() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rango)) {
            npc_sel = hit.transform.GetComponent<AgentNPC>();
            if (npc_sel != null && !npcYaClickado(npc_sel)) {
                hit.transform.GetChild(1).gameObject.SetActive(true);
                npcSeleccionados.Add(npc_sel);
            } else if (npc_sel != null && npcYaClickado(npc_sel)) {
                hit.transform.GetChild(1).gameObject.SetActive(false);
                npc_sel.deseleccionar();
                npcSeleccionados.Remove(npc_sel);
            }
        }      
    }


    // Método para deseleccionar todos los agentes que estaban seleccionados, quitándoles el anillo también
    void vaciarListaNPC() {
        foreach(AgentNPC npc in npcSeleccionados) {
            npc.transform.GetChild(1).gameObject.SetActive(false);
            npc.deseleccionar();
        }
        npcSeleccionados = new List<AgentNPC>();
    }


    // Función que devuelve 'true' si un NPC se encuentra ya seleccionado y 'false' en caso contrario
    bool npcYaClickado(AgentNPC npcB) {
        foreach(AgentNPC npc in npcSeleccionados) {
            if (npc == npcB) return true;
        }
        return false;
    }


    // Función que devuelve 'true' si la lista de NPCs seleccionados está vacía y 'false' en caso contrario
    bool vacia() {
        return npcSeleccionados.Count == 0;
    }


    // Método que hace mover a todos los agente seleccionados cuando se hace click derecho
    void moverNPCS() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rango)) {
            
            if (hit.collider != null && !hit.collider.CompareTag("Agua")) {
                // Punto en el que se hace click
                Vector3 newTarget = hit.point;
                // AgenteClickD será el destino hacia el que iremos
                GameObject go = new GameObject("Click derecho");
                Agent agentClickD = go.AddComponent<Agent>();
                agentClickD.transform.position = newTarget;
                
                // Si todos están en formación, se mueve al líder y el resto lo seguirán
                if (seleccionadosEnFormacion()) {
                    npcSeleccionados[0].NewTarget(agentClickD, true);

                // Si no, cada uno de los seleccionados irá al punto (rompiendo la formación)
                } else {
                    foreach (AgentNPC npc in npcSeleccionados) {
                        // Si no están formando, para que no todos vayan al mismo punto exactamente, randomizamos un poco
                        Vector3 punto = newTarget;
                        float r1 = UnityEngine.Random.Range(-2f, 2f);
                        float r2 = UnityEngine.Random.Range(-2f, 2f);
                        float flip = UnityEngine.Random.Range(0f, 1f);
                        if (flip < 0.25f) {
                            r1 = r1 * -1;
                            r2 = r2 * -1;
                        } else if (flip < 0.5f) {
                            r2 = r2 * -1;
                        } else if (flip < 0.75f) {
                            r1 = r1 * -1;
                        }
                        punto.x += r1;
                        punto.z += r2;
                        agentClickD.transform.position = punto;
                        // Mandamos al agente al punto y hacemos que deje de formar  
                        npc.NewTarget(agentClickD, false);
                    }
                }
            }
        }
    }


    // Función que comprueba si todos los NPCs seleccionados están en formación
    private bool seleccionadosEnFormacion() {
        foreach(AgentNPC npc in npcSeleccionados) {
            if (!npc.estoyFormando()) return false;
        }
        return true;
    }


    // Método que pone a formar a los NPCs seleccionados según un número entero
    // procedencia es para diferenciar desde donde se invoca el método
    public void hacerFormacion(int nformacion) {
        
        List<(Vector3, float)> gridFormacion;
        switch (nformacion) {
            case 0:
                gridFormacion = Formaciones.defensiva();
                break;
            case 1:
                gridFormacion = Formaciones.inspectionPatrol();
                break;
            case 2:
                gridFormacion = Formaciones.escalableCirculo(npcSeleccionados.Count);
                break;
            case 3:
                // Se resta 1 para no contar al lider
                gridFormacion = Formaciones.escalableTriangulo(npcSeleccionados.Count-1);
                break;
            case 4:
                // Contamos al lider para que los cuadrados salgan bien, pero no devolveremos su posición
                gridFormacion = Formaciones.escalableCuadrado(npcSeleccionados.Count);
                break;
            default:
                gridFormacion = new List<(Vector3, float)>();
                break;
        }

        // El líder es el primero de gridFormacion
        AgentNPC lider = npcSeleccionados[0];
        lider.eresLider(nformacion);
        
        // La formacion escalable de 2 niveles es un caso especial 
        if (nformacion == 3 || nformacion == 4) {
            for (int i = 0; i < gridFormacion.Count; i++) 
                npcSeleccionados[i+1].formar(gridFormacion[i].Item1, gridFormacion[i].Item2, lider);
            
        } else {
            for (int i = 1; i < gridFormacion.Count; i++) {
                if (i < npcSeleccionados.Count) {
                    npcSeleccionados[i].formar(gridFormacion[i].Item1, gridFormacion[i].Item2, lider); 
                } 
            }
        }
    }


    public void hacerFormacionRolesDuros() {
        // Booleano para ver si se ha asignado a una unidad una posición
        bool pasar = false;
        
        List<(Vector3, float, String)> gridFormacionRolDuro = null;

        // El líder es el primero de gridFormacion
        AgentNPC lider = npcSeleccionados[0];
        String tagLider = npcSeleccionados[0].tag;

        // La formación irá en base al líder, por tanto, obtenemos la formación en función del tag del primer npc seleccionado
        switch (tagLider) {
            case "MeleePesado":
                gridFormacionRolDuro = Formaciones.rolDuroLiderMeleeP();
                break;
            case "MeleeLigero":
                gridFormacionRolDuro = Formaciones.rolDuroLiderMeleeL();
                break;
            case "Espia":
                gridFormacionRolDuro = Formaciones.rolDuroLiderEspia();
                break;
            case "Ranged":
                gridFormacionRolDuro = Formaciones.rolDuroLiderRanged();
                break;
            default:
                break;
        }

        lider.eresLider(5);

        // Lista que contiene NPCs sin posición asignada
        List<AgentNPC> copiaNpcSeleccionados = new List<AgentNPC>(npcSeleccionados);
        copiaNpcSeleccionados.Remove((AgentNPC)lider);

        for (int i = 1; i < gridFormacionRolDuro.Count; i++) {
            String rol = gridFormacionRolDuro[i].Item3;
            foreach (AgentNPC agent in npcSeleccionados) {
                if (copiaNpcSeleccionados.Contains(agent) && RolCorrecto(rol, agent.tag)) {
                    agent.formar(gridFormacionRolDuro[i].Item1, gridFormacionRolDuro[i].Item2, lider);
                    copiaNpcSeleccionados.Remove(agent);
                    pasar = true;
                    break;
                } 
                if (pasar) { pasar = false; continue;}
            }
        } 
    }


    // Función que comprueba si el tipo de unidad es compatible con el rol que se le quiere asignar
    private bool RolCorrecto(string rol, String unidad) {
        // Tanque: melee ligero y melee pesado
        // Melee: melee ligero, melee pesado y espía
        // ApoyoLateral: espía y ranged
        // Retaguardia: melee ligero y ranged
        switch (rol) {
            case "Tanque":
                return unidad == "MeleeLigero" || unidad == "MeleePesado";
            case "Melee":
                return unidad == "Espia" || unidad == "MeleeLigero" || unidad == "MeleePesado";
            case "ApoyoLateral":
                return unidad == "Espia" || unidad == "Ranged";
            case "Retaguardia":
                return unidad == "MeleeLigero" || unidad == "Ranged";
            default:
                return false;
        }
    }


    public void hacerFormacionRolesSuaves() {
        List<(Vector3, float, List<int>)> gridFormacionRolSuave = Formaciones.rolSuave();

        AgentNPC lider = obtenerMejorNPC(gridFormacionRolSuave[0].Item3, npcSeleccionados);
        lider.eresLider(6);

        // Lista que contiene NPCs sin posición asignada
        List<AgentNPC> copiaNpcSeleccionados = new List<AgentNPC>(npcSeleccionados);
        copiaNpcSeleccionados.Remove(lider);

        for (int i = 1; i < gridFormacionRolSuave.Count; i++) {
            if (copiaNpcSeleccionados.Count != 0) {
                // Busco el mejor candidato para la posición actual y lo mando a formar
                AgentNPC candidato = obtenerMejorNPC(gridFormacionRolSuave[i].Item3, copiaNpcSeleccionados);     
                candidato.formar(gridFormacionRolSuave[i].Item1, gridFormacionRolSuave[i].Item2, lider);
                copiaNpcSeleccionados.Remove(candidato);
            }
        }
    }


    // El mejor NPC para una posición será un NPC cuyo coste sea el más bajo posible para dicha posición del grid
    private AgentNPC obtenerMejorNPC(List<int> costes, List<AgentNPC> npcsDisponibles) { 
        AgentNPC candidato = null;
        int menorCoste = 500; //Sabemos que ningún coste es mayor que 500, por tanto, es como poner un Math.Infinity
        int costeAct = 0;

        foreach (AgentNPC a in npcsDisponibles) {
            String tipoUnidad = a.tag;
            switch (tipoUnidad) {
                case "MeleeLigero":
                    costeAct = costes[0];
                    break;
                case "MeleePesado":
                    costeAct = costes[1];
                    break;
                case "Espia":
                    costeAct = costes[2];
                    break;
                case "Ranged":
                    costeAct = costes[3];
                    break;
                default:
                    costeAct = 500;
                    break;
            }
            if (costeAct < menorCoste) {
                candidato = a;
                menorCoste = costeAct;
            }

        }
        return candidato;
    }

    /* ----------------------------------------------------------------------------------
    ----------------------------------- Métodos Bloque 2 --------------------------------
    ---------------------------------------------------------------------------------- */

    public List<Recurso> GetRecursosLibres() {
        List<Recurso> libres = new List<Recurso>();
        foreach (Recurso r in this.recursos) {
            if (!r.IsRecolectado())
                libres.Add(r);
        }
        return libres;
    }

    public List<Torreta> GetTorretasSinConstruir(Bando bando) {
        List<Torreta> libres = new List<Torreta>();
        foreach (Torreta t in this.torretas) {
            if (t.bando == bando && t.Construible())
                libres.Add(t);
        }
        return libres;
    }

    public Vector3 GetPosicionAparicion(Bando bando) {
        if (bando == Bando.Rojo)
            return respawnRojo.transform.position;
        else 
            return respawnAzul.transform.position;
    }

    public void Ganar(Bando bandoGanador) {
        this.mensajeFinJuego.transform.GetChild(0).gameObject.GetComponent<Text>().text = "¡El bando " + bandoGanador + " ha ganado!";
        this.mensajeFinJuego.SetActive(true);
        finalJuego = true;
        Time.timeScale = 0;
        StartCoroutine("CerrarJuego");
    }

    IEnumerator CerrarJuego() {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }

    // Método que devuelve el camino completo a realizar a una unidad que se encuentre patrullando
    public Vector3[] obtenerRutaPatrulla(Bando b) {
        int tamCamino;

        if (b == Bando.Rojo) 
            tamCamino = caminoPatrullaRojo.transform.childCount;
        else
            tamCamino = caminoPatrullaAzul.transform.childCount;

        Vector3[] rutaPatrulla = new Vector3[tamCamino];

        for(int i = 0; i < tamCamino; i++)
        {
            if (b == Bando.Rojo) 
                rutaPatrulla[i] = caminoPatrullaRojo.transform.GetChild(i).transform.position;
            else
                rutaPatrulla[i] = caminoPatrullaAzul.transform.GetChild(i).transform.position;
        }

        return rutaPatrulla;
    }   


    public void ActivarGuerraTotal() {
        foreach (Soldado soldado in soldados) {
            if (soldado.bando == Bando.Rojo) soldado.ActivarGuerraTotal(Actitud.OFENSIVO, respawnAzul.transform.position);
            else soldado.ActivarGuerraTotal(Actitud.OFENSIVO, respawnRojo.transform.position);  
        }
        Debug.Log("Modo Guerra Total activado en ambos bandos.");
    }


    // Método llamado por una unidad con baja vida que necesite buscar un punto seguro
    public List<Nodo> ModoAntisuicida(Soldado soldado) {

        List<Hospital> hospitales = HayHospitales(soldado.bando);
        
        float minDist = Mathf.Infinity;
        Vector3 destino = Vector3.zero;
        
        for (int i = 0; i < hospitales.Count; i++) {
            float dist = (this.transform.position - hospitales[i].transform.position).magnitude;
            if (dist < minDist) {
                destino = hospitales[i].transform.position;
                minDist = dist;
            }
        }

        List<Nodo> caminoEscogido = soldado.pathFinding.AStar(soldado.transform.position, destino, mapaInfluencias, soldado);
        return caminoEscogido;
    }


    public List<Hospital> HayHospitales(Bando bando) {
        List<Hospital> hospitales = new List<Hospital>();
        foreach (Estructura e in estructuras) {
            if (e.GetComponent<Hospital>() != null && e.bando == bando && e.vida > 0) {
                hospitales.Add(e.GetComponent<Hospital>());
            }
        }
        return hospitales;
    }


    public void ActivarModoEnBando(Actitud act, Bando bando) {
        foreach (Soldado soldado in soldados) {
            if (soldado.bando == bando) {
                soldado.CambiarActitud(act);
            }
        }
        Debug.Log("Modo " + act + " para equipo " + bando + " activado.");
    }


    // Método que devuelve los waypoints en función de la influencia que se ejerce sobre ellos
    public List<Transform> GetWaypoints(Bando bandoWaypoint, Bando bandoSoldado) { 
        List<Transform> wp = new List<Transform>();

        Transform[] waypoints;
        
        if (bandoWaypoint == Bando.Azul) waypoints = waypointsAzules.GetComponentsInChildren<Transform>();
        else waypoints = waypointsRojos.GetComponentsInChildren<Transform>();

        for (int i = 1; i < waypoints.Length; i++) {
            Nodo n = grid.posicionRealToNodo(waypoints[i].position);
            float valor = ValorInfluenciaNodo(n);

            // Si se pide un bando y se es de ese bando se cogen los wps que no tengan influencia de su bando
            // Si se pide un bando y no se es de ese bando se cogen los wps con influencia del otro bando
            if ((bandoWaypoint == Bando.Rojo && bandoSoldado == Bando.Rojo && valor <= 0) || 
                (bandoWaypoint == Bando.Rojo && bandoSoldado == Bando.Azul && valor > 0) ||
                (bandoWaypoint == Bando.Azul && bandoSoldado == Bando.Rojo && valor < 0) ||
                (bandoWaypoint == Bando.Azul && bandoSoldado == Bando.Azul && valor >= 0)) {
                    wp.Add(waypoints[i]); //Si el valor del waypoint no es azul, añadir a posible zona
                }
        }
        
        // Si no hay wps disponibles (por la influencia) se devuelven todos, en caso contrario, se devuelven los disponibles
        if (wp.Count == 0) {
            if (bandoWaypoint == Bando.Azul) return waypointsAzules.GetComponentsInChildren<Transform>().ToList();
            else return waypointsRojos.GetComponentsInChildren<Transform>().ToList();
        } else return wp;
    }


    private float ValorInfluenciaNodo(Nodo n) {
        if (n == null) return 0;
        return mapaInfluencias[(int)n.posicionNodo.y][(int)n.posicionNodo.x];
    }

    
    // Método que actualiza el mapa de influencias
    IEnumerator CalcularInfluencias() {
        
        while (true) {
            
            List<(int x, int z, float i)> influencias = new List<(int x, int z, float i)>();
            foreach (Soldado soldado in soldados) {
                influencias.Add(((int)grid.posicionRealToNodo(soldado.transform.position).posicionNodo.y, 
                                (int)grid.posicionRealToNodo(soldado.transform.position).posicionNodo.x, 
                                soldado.GetInfluencia()));
            }
        
            mapaInfluencias = MapaInfluencia.Influencias(influencias, (grid.tamColumnas,grid.tamFilas));
            miniMapa.Actualizar(mapaInfluencias, 20f);

            yield return new WaitForSeconds(3);
        }
    }


    public bool FinPartida() {
        return finalJuego;
    }
}