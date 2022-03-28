using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum PlanI {all, sharkFleeing, leaderFollowing} 

public class AgentNPC : Agent {

    [Header("[Control]")]
    public Pathfinding pathFinding;
    public Controlador controlador;

    // Lista de referencias a todas las componentes SteeringBehavior que tiene el personaje
    public Agent agenteAuxPrefab;
    public Steering steer = new Steering();
    public List<Steering> aplicables = new List<Steering>();
    public PlanI planInspector;

    private Plan plan;
    private const float offsetFormacion = 3f;
    protected bool enCamino = false;
    private bool formando = false;
    private bool soyLider = false;
    private int nformacion;
    protected Agent agenteAux;
    private Agent targetFinal = null;
    private PathFollowing steeringPf = null;
    private OffsetArrive arrive = null;
    private OffsetAlign align = null;
    protected List<Nodo> path = new List<Nodo>();
    protected List<SteeringBehaviour> steerings;


    protected virtual void Start() {
        switch (planInspector) {
            case PlanI.all:
                plan = new Plan(Esquemas.all());
                break;
            case PlanI.sharkFleeing:
                plan = new Plan(Esquemas.sharkFleeing());
                break;
            case PlanI.leaderFollowing:
                plan = new Plan(Esquemas.leaderFollowing());
                break;
            default:
                plan = new Plan(Esquemas.all());
                break;
        }
    }

    void Awake() {
        steerings = new List<SteeringBehaviour>(GetComponents<SteeringBehaviour>());
        if (agenteAuxPrefab != null) {
            agenteAux = Instantiate(agenteAuxPrefab);
            foreach (SteeringBehaviour steer in steerings) {
                steer.SendMessage("SetTarget", agenteAux);
            }
        }
    }


    void Update() {
        ApplySteering();
        CompruebaPath();
    }

    void LateUpdate() {
        this.steer = Arbitro.dinamico(this, plan.esquema, steerings);
        plan.planificar();
    }

    
    // Método que aplica el steering calculado
    protected void ApplySteering() {
        // Basado en Kinematic::Update del libro.
        float time = Time.deltaTime;

        // Actualizar posición y orientación
        transform.position += velocidad_vec * time;
        orientacion += rotacion * time;

        // Actualizar velocidad y rotación
        velocidad_vec += steer.linear * time;
        rotacion += steer.angular * time;

        // Limitar velocidad
        if (velocidad_vec.magnitude > velocidad_max*(100/masa)) {
            velocidad_vec.Normalize();
            velocidad_vec *= velocidad_max*(100/masa) * coef_frenado;
        }

        // Limitar rotacion
        if (Mathf.Abs(rotacion) > rotacion_max) {
            rotacion = Mathf.Sign(rotacion) * rotacion_max;
        }

        // Pasar los valores Position y Orientation a Unity. Por ejemplo
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, orientacion);
    }


    // Método que pone a un agente en formación en función de un líder
    public void formar(Vector3 pos_sold, float orientacion_sold, Agent lider) {
        this.formando = true;
        
        if (this.GetComponent<OffsetAlign>() == null)
            align = (OffsetAlign) addBehaviour("OffsetAlign", 5f, lider);

        if (this.GetComponent<OffsetArrive>() == null) 
            arrive = (OffsetArrive) addBehaviour("OffsetArrive", 5f, lider);
        
        align.offset = orientacion_sold;
        arrive.offset = pos_sold;
        align.peso = 5f;
        arrive.peso = 5f;
    }


    // Si llaman a formar y eres el primer seleccionado eres el lider de la formación
    // Además, guardas el número de formación (será necesario si te mueves)
    public void eresLider(int nformacion) {
        this.soyLider = true;
        this.nformacion = nformacion;
        
        if (arrive != null) {
            arrive.peso = 0f;
            this.velocidad_vec = Vector3.zero;
            this.rotacion = 0f;
        }
        
        if (align != null) {
            align.peso = 0f;
        }
    }


    public bool estoyFormando() {
        return this.soyLider || this.formando;
    }


    // Método que añade un steering a partir de un Strings
    public SteeringBehaviour addBehaviour(string behaviour, float peso, Agent target) {
        Type tipo = Type.GetType(behaviour);

        // Obtener si existe
        var steering = steerings.FirstOrDefault(steer => tipo == steer.GetType());

        // Crear si no existe
        if (steering == null) {
            steering = (SteeringBehaviour) gameObject.AddComponent(tipo);
            steerings.Add(steering);
        }

        // Inicializar
        steering.peso = peso;
        steering.target = target;
        return steering;
    }


    // Método que establece un target donde se haya hecho click y se ejecuta un pathfinding para llegar hasta él
    public void NewTarget(Agent target, bool formando) {
        if (!formando) deseleccionar();

        targetFinal = target;
        Vector3 posFinal = targetFinal.transform.position; 
        Nodo nodoFinal = pathFinding.grid.posicionRealToNodo(posFinal);
        Nodo nodoInicial = pathFinding.grid.posicionRealToNodo(transform.position);

        if (nodoFinal == null || nodoFinal.isObstaculo) return; 

        path = this.pathFinding.LRTAStar(nodoInicial, nodoFinal);
        
        // Si el agente ya estaba realizando una busqueda, la cancela y va a la nueva
        if (this.enCamino) { 
            steeringPf = (PathFollowing) gameObject.GetComponent(Type.GetType("PathFollowing"));
            steerings.Remove(steeringPf);
            Destroy(steeringPf);
        } else this.enCamino = true;

        // Ahora iremos a cada uno de los nodos calculados con LRTA* mediante el steering PathFollowing
        if (show) pathFinding.grid.DibujarCamino(path);
    }


    // Si te han deseleccionado y tenías algún steering relacionado con la formación se eliminan
    public void deseleccionar() {
        // Si estaba formando y ahora tengo que parar, anulos los steerings de la formación
        if (this.formando) {
            align.peso = 0f;
            arrive.peso = 0f;
        }
        this.soyLider = false;
        this.formando = false;
    }


    // Método que comprueba si un agente estaba en camino si se vuelve a hacer click derecho para recalcular un nuevo camino
    void CompruebaPath() {
        
        // Si se está en camino cuando se vuelve a hacer click derecho, se calcula de nuevo el pathfinding
        if (this.enCamino && this.GetComponent<PathFollowing>() == null) {
            steeringPf = (PathFollowing) gameObject.AddComponent(Type.GetType("PathFollowing"));
            steerings.Add(steeringPf);
            steeringPf.peso = 3;
            steeringPf.target = targetFinal; 
            steeringPf.path = path;
        } 
    }


    // Método que activa el modo depuración en aquellos steerings del agente que lo necesiten
    public override void actualizarShow(bool show) {
        base.actualizarShow(show);
        if (show)
            foreach(SteeringBehaviour steering in steerings)
                steering.SendMessage("actualizarShowB", show);
    }


    // La velocidad cambia según terreno
    private void OnTriggerEnter(Collider col) {
        switch (col.tag) {
            case "Hierba":
                coef_frenado = 0.5f;
                break;
            case "Camino":
                coef_frenado = 1f;
                break;
            case "Bosque":
                coef_frenado = 0.5f;
                break;
            default:
                break;
        }
    }


    // Gizmo para visualizar la posición de los agentes auxiliares que actúan como targets
    protected override void OnDrawGizmos() {
        base.OnDrawGizmos();
        if (agenteAux !=null)
            if (!show)
                agenteAux.gameObject.GetComponent<MeshRenderer>().forceRenderingOff = true;
            else
                agenteAux.gameObject.GetComponent<MeshRenderer>().forceRenderingOff = false;
    }
}