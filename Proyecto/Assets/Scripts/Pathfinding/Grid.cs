using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {        

    public GameObject cuadricula;
    public LayerMask layerObstaculos;
    public Material materialDebug;

    // Offset de la posición real, porque se toma en el centro del cubo y los NPC se hundían un poco
    private const float offsetCubo = 0.54f;

    public int tamFilas;
    public int tamColumnas;
    private int numNodos;
    private float tamCuadricula;
    public Nodo[,] nodos;
    private List<GameObject> debugPath = new List<GameObject>();


    void Awake() {
        InicializarGrid();
    }


    void Start() {
        GenerarNodos();
    }


    // Método que inicializa el grid con la cuadrícula creada
    private void InicializarGrid() {
        tamColumnas = cuadricula.transform.GetChild(0).transform.childCount;
        tamFilas = cuadricula.transform.childCount;

        // Se guarda el tamaño de cada cuadrícula (longitud del cuadrado)
        tamCuadricula = cuadricula.transform.GetChild(0).transform.GetChild(0).transform.localScale.x;

        // Se guarda el número de nodos por eje 
        nodos = new Nodo[tamFilas,tamColumnas];
    }


    // Método que genera los nodos del grid, asociándoles una posición en el juego y en la lista de nodos
    private void GenerarNodos() {
    
        bool isObstaculo;
        Vector3 posicionReal, posicionRealX, posicionRealZ;
    
        for (int i = 0; i < tamColumnas; i++) {
            for (int j = 0; j < tamFilas; j++) {

                // Se asume que el grid empieza y crece desde el origen (0,0,0)
                posicionRealX = (new Vector3(1,0,0)) * i * 3; 
                posicionRealZ = (new Vector3(0,0,1)) * j * 3;
                posicionReal = posicionRealX + posicionRealZ + new Vector3(0,offsetCubo,0); 

                isObstaculo = false;

                // Si alrededor del nodo existe un obstáculo, ese nodo será obstáculo 
                if (Physics.CheckSphere(posicionReal, tamCuadricula/3, layerObstaculos))
                    isObstaculo = true;

                nodos[j,i] = new Nodo(posicionReal, new Vector2(j,i), isObstaculo, cuadricula.transform.GetChild(j).transform.GetChild(i).transform.tag);
            }
        }
    }


    // Función que devuelve una lista de nodos adyacentes al que se pasa por parámetro
    public List<Nodo> GetNodosAdyacentes(Nodo nodoAct) {
        List<Nodo> nodosAdy = new List<Nodo>();

        // Nodos derecha, arriba, izquierda y abajo
        if (nodoAct.posicionNodo.x + 1 < tamFilas)
            nodosAdy.Add(nodos[(int)nodoAct.posicionNodo.x + 1, (int)nodoAct.posicionNodo.y]);
        if (nodoAct.posicionNodo.y + 1 < tamColumnas)
            nodosAdy.Add(nodos[(int)nodoAct.posicionNodo.x, (int)nodoAct.posicionNodo.y + 1]);
        if (nodoAct.posicionNodo.x - 1 >= 0)
            nodosAdy.Add(nodos[(int)(nodoAct.posicionNodo.x - 1), (int)nodoAct.posicionNodo.y]);
        if (nodoAct.posicionNodo.y - 1 >= 0)
            nodosAdy.Add(nodos[(int)nodoAct.posicionNodo.x, (int)nodoAct.posicionNodo.y - 1]);
        // Diagonales
        if (nodoAct.posicionNodo.x + 1 < tamFilas && nodoAct.posicionNodo.y + 1 < tamColumnas) 
            nodosAdy.Add(nodos[(int)nodoAct.posicionNodo.x + 1, (int)nodoAct.posicionNodo.y + 1]);
        if (nodoAct.posicionNodo.x + 1 < tamFilas && nodoAct.posicionNodo.y - 1 >= 0)
            nodosAdy.Add(nodos[(int)nodoAct.posicionNodo.x + 1, (int)nodoAct.posicionNodo.y - 1]);
        if (nodoAct.posicionNodo.x - 1 >= 0 && nodoAct.posicionNodo.y + 1 < tamColumnas)
            nodosAdy.Add(nodos[(int)nodoAct.posicionNodo.x - 1, (int)nodoAct.posicionNodo.y + 1]);
        if (nodoAct.posicionNodo.x - 1 >= 0 && nodoAct.posicionNodo.y - 1 >= 0)
            nodosAdy.Add(nodos[(int)nodoAct.posicionNodo.x - 1, (int)nodoAct.posicionNodo.y - 1]);

        return nodosAdy;
    }


    // Función que dada una posición devuelve un nodo del grid
    public Nodo posicionRealToNodo(Vector3 posicionReal) {        
        return nodos[(int)(posicionReal.z/tamCuadricula),(int)(posicionReal.x/tamCuadricula)];
    }


    // Método para dibujar el camino seguido
    public void DibujarCamino(List<Nodo> path) {
        
        foreach(GameObject punto in debugPath)
            Destroy(punto);

        foreach(Nodo nodo in path) {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale *= 1.5f;
            sphere.transform.position = new Vector3(nodo.posicionReal.x, -0.5f, nodo.posicionReal.z);
            sphere.GetComponent<MeshRenderer>().material = materialDebug;
            this.debugPath.Add(sphere);
        }
    }
}