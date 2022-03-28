using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodo {

    public bool isObstaculo;
    public float G;
    public float H;
    public string tipo;
    public Vector2 posicionNodo;
    public Vector3 posicionReal;
    public Nodo nodoPadre;
    

    public Nodo (Vector3 posicionReal, Vector2 posicionNodo, bool isObstaculo, string tag) {
        this.posicionReal = posicionReal;
        this.posicionNodo = posicionNodo;
        
        switch (tag) {
            case "Camino":
                this.G = Terrenos.coste[tag];
                this.isObstaculo = isObstaculo;
                break;
            case "Hierba":
                this.G = Terrenos.coste[tag];
                this.isObstaculo = isObstaculo;
                break;
            case "Bosque":
                this.G = Terrenos.coste[tag];
                this.isObstaculo = isObstaculo;
                break;
            case "Agua":
                this.isObstaculo = true;
                break;
            default:
                this.isObstaculo = true;
                break;
        }
        this.tipo = tag;
    }
}