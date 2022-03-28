using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Heuristic {Euclidea, Chebychev, Manhattan} 

public class Pathfinding : MonoBehaviour {

    public Grid grid;
    public Heuristic heuristicaInspector;

    private Heuristica heuristica;
    private List<Nodo> listaNodos;

    public Transform StartPosition;
    public Transform TargetPosition;


    void Start() {
        switch (heuristicaInspector) {
            case Heuristic.Euclidea:
                heuristica = new DistanciaEuclidea();
                break;
            case Heuristic.Chebychev:
                heuristica = new DistanciaChebyshev();
                break;
            case Heuristic.Manhattan:
                heuristica = new DistanciaManhattan();
                break;
            default:
                heuristica = new DistanciaManhattan();
                break;
        }
    }


    // Algoritmo LRTA* con subespacio de búsqueda minimal optimizado para generar todo el camino
    public List<Nodo> LRTAStar(Nodo nodoInicial, Nodo nodoFinal) {
        
        Nodo nodoAct = nodoInicial;
        nodoAct.H = nodoAct.G + heuristica.CalcularHeuristica(nodoAct, nodoFinal);

        listaNodos = new List<Nodo>();
        List<Nodo> nodosAdyacentes = new List<Nodo>();
        while (nodoAct != nodoFinal) {
            nodosAdyacentes = grid.GetNodosAdyacentes(nodoAct);
            
            ActualizarCostes(nodoAct, nodoFinal, nodosAdyacentes);
            
            nodoAct = CalcularSiguienteNodo(nodosAdyacentes);

            // Si no existe un camino, se devuelve null
            if (nodoAct == null) {
                listaNodos = new List<Nodo>();
                break;
            }

            listaNodos.Add(nodoAct);
        }

        return listaNodos;
    }


    // Función auxiliar que calcula el mejor siguiente nodo adyacente para seguir el camino
    public Nodo CalcularSiguienteNodo(List<Nodo> nodosAdyacentes) {
        float costeMinimo = Mathf.Infinity;
        Nodo nodoSiguiente = null;
        foreach (Nodo nodoAdy in nodosAdyacentes) {
            if (nodoAdy.isObstaculo == false && !listaNodos.Contains(nodoAdy)) {
                if ((nodoSiguiente == null || ((nodoSiguiente != null) && ((nodoAdy.G + nodoAdy.H) < (nodoSiguiente.G + nodoSiguiente.H))))) {
                    costeMinimo = nodoAdy.G + nodoAdy.H;
                    nodoSiguiente = nodoAdy;
                }
            }
        }

        if (esPathPosible(nodosAdyacentes)) return nodoSiguiente;
        else return null;
    }


    //
    private bool esPathPosible(List<Nodo> nodosAdyacentes) {
        int acc = 0;
        foreach (Nodo nodoAdy in nodosAdyacentes) {
            if (listaNodos.Contains(nodoAdy)) acc++;
        }
        return acc != nodosAdyacentes.Count;
    }


    // Método que actualiza el coste de los nodos adyacentes en función del coste del nodo actual y de la distancia del adyacente al objetivo
    public void ActualizarCostes(Nodo nodoAct, Nodo nodoFinal, List<Nodo> nodosAdyacentes) {
        foreach (Nodo nodoAdy in nodosAdyacentes) 
            nodoAdy.G = nodoAct.G + heuristica.CalcularHeuristica(nodoAdy, nodoFinal); 
    }


    /* ----------------------------------------------------------------------------------
    ----------------------------------- Métodos Bloque 2 --------------------------------
    ---------------------------------------------------------------------------------- */

    // Ref: https://github.com/danielmccluskey/A-Star-Pathfinding-Tutorial/tree/master/Assets

    public List<Nodo> AStar(Vector3 posInicial, Vector3 posFinal, List<List<float>> mapaInfluencias, Soldado soldado) {
        
        if (mapaInfluencias == null) mapaInfluencias = MapaInfluencia.Neutral((grid.tamColumnas,grid.tamFilas));

        Nodo nodoInicial = grid.posicionRealToNodo(posInicial);
        Nodo nodoFinal = grid.posicionRealToNodo(posFinal);

        if (posInicial == posFinal) return new List<Nodo>{nodoInicial};

        if (nodoFinal == null || nodoFinal.isObstaculo) return new List<Nodo>();

        List<Nodo> listaAbiertos = new List<Nodo>(); //List of nodes for the open list
        HashSet<Nodo> listaCerrados = new HashSet<Nodo>(); //Hashset of nodes for the closed list

        listaAbiertos.Add(nodoInicial); //Add the starting node to the open list to begin the program

        //Whilst there is something in the open list
        while(listaAbiertos.Count > 0) {
            
            Nodo nodoActual = listaAbiertos[0]; //Create a node and set it to the first item in the open list
            float HnodoActual = nodoActual.H;
            HnodoActual += soldado.CosteTerreno(nodoActual.tipo);
            HnodoActual += ConversionInfluenciaCoste(mapaInfluencias[(int)nodoActual.posicionNodo.y][(int)nodoActual.posicionNodo.x], HnodoActual, soldado);

            //Loop through the open list starting from the second object
            for(int i = 1; i < listaAbiertos.Count; i++) {
                float HnodoAbierto = listaAbiertos[i].H;
                HnodoAbierto += soldado.CosteTerreno(listaAbiertos[i].tipo);
                HnodoAbierto += ConversionInfluenciaCoste(mapaInfluencias[(int)listaAbiertos[i].posicionNodo.y][(int)listaAbiertos[i].posicionNodo.x], HnodoAbierto, soldado);

                //If the f cost of that object is less than or equal to the f cost of the current node
                if ((listaAbiertos[i].G + HnodoAbierto) < (nodoActual.G + HnodoActual) || 
                    (listaAbiertos[i].G + HnodoAbierto) == (nodoActual.G + HnodoActual) && 
                    (HnodoAbierto < HnodoActual)) {
                    nodoActual = listaAbiertos[i]; //Set the current node to that object
                    HnodoActual = nodoActual.H;
                }
            }

            listaAbiertos.Remove(nodoActual); //Remove that from the open list
            listaCerrados.Add(nodoActual); //And add it to the closed list

            //If the current node is the same as the target node
            if (nodoActual == nodoFinal) {
                return GetCaminoFinal(nodoInicial, nodoFinal); //Calculate the final path
            }

            //Loop through each neighbor of the current node
            foreach (Nodo nodoAdyacente in grid.GetNodosAdyacentes(nodoActual)) {
                
                //If the neighbor is a wall or has already been checked
                if (nodoAdyacente.isObstaculo || listaCerrados.Contains(nodoAdyacente)) {
                    continue; //Skip it
                }

                float costeMov = nodoActual.G + heuristica.CalcularHeuristica(nodoActual, nodoAdyacente);//Get the F cost of that neighbor

                //If the f cost is greater than the g cost or it is not in the open list
                if (costeMov < nodoAdyacente.G || !listaAbiertos.Contains(nodoAdyacente)) {
                    nodoAdyacente.G = costeMov; //Set the g cost to the f cost
                    nodoAdyacente.H = heuristica.CalcularHeuristica(nodoAdyacente, nodoFinal); //Set the h cost
                    nodoAdyacente.nodoPadre = nodoActual; //Set the parent of the node for retracing steps

                    //If the neighbor is not in the openlist
                    if (!listaAbiertos.Contains(nodoAdyacente)) {
                        listaAbiertos.Add(nodoAdyacente); //Add it to the list
                    }
                }
            }
        }
        return new List<Nodo>();
    }


    List<Nodo> GetCaminoFinal(Nodo nodoInicial, Nodo nodoFinal) {
        List<Nodo> caminoFinal = new List<Nodo>(); //List to hold the path sequentially 
        Nodo nodoActual = nodoFinal; //Node to store the current node being checked

        //While loop to work through each node going through the parents to the beginning of the path
        while (nodoActual != nodoInicial) {
            caminoFinal.Add(nodoActual); //Add that node to the final path
            nodoActual = nodoActual.nodoPadre; //Move onto its parent node
        }

        caminoFinal.Reverse(); //Reverse the path to get the correct order
     
        return caminoFinal; //Return the final path
    }


    public float ConversionInfluenciaCoste(float influencia, float coste, Soldado soldado) {
        if (soldado.GetComponent<Espia>() != null) {
            Bando bando = soldado.bando;
            if (bando == Bando.Rojo) {
                if (influencia <= 0) {
                    return Mathf.Abs(coste * (influencia + 1));
                } else {
                    return Mathf.Abs(coste / (influencia + 1));
                }
                
            } else {
                if (influencia < 0) {
                    return Mathf.Abs(coste / (influencia + 1));
                } else {
                    return Mathf.Abs(coste * (influencia + 1));
                }
            }
        }
        return Mathf.Abs(coste / (influencia + 1));
    }
}