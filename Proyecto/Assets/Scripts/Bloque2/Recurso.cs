using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recurso : MonoBehaviour {
    
    public const float TIEMPO_RECOLECCION = 3f;
    private const float TIEMPO_APARICION = 60f;
    
    private bool recolectado = false;
    private float tiempoRecoleccion;
    private float tiempoAparicion = TIEMPO_APARICION; 

    public bool IsRecolectado() { return this.recolectado; }

    void Update() {
        Aparecer();
    }

    // Método que activa la visualización del recurso si ha sido recolectado y ha pasado un tiempo
    void Aparecer() {
        if (recolectado) {
            if (!ContadorEnCurso()) {
                this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y+10,this.transform.position.z);
                tiempoAparicion = TIEMPO_APARICION;
                recolectado = false;
            }
        }
    }


    // Función que devuelve 'false' si no ha pasado el tiempo suficiente para volver a aparecer y 'true' en caso contrario
    bool ContadorEnCurso() {
        if (tiempoAparicion > 0)
            tiempoAparicion -= Time.deltaTime;
        return tiempoAparicion > 0;
    }

    public void IniciarRecoleccion() {
        StartCoroutine("Recolectar");
    }

    // Método que simula un tiempo de recolección del recurso y lo desactiva pasado su tiempo
    IEnumerator Recolectar() {
        yield return new WaitForSeconds(TIEMPO_RECOLECCION);
        this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y-10,this.transform.position.z);
        recolectado = true;
    }
}
