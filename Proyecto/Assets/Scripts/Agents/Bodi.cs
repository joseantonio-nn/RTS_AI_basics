using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script Cuerpo Físico
public class Bodi : MonoBehaviour {

    // Declaración de variables
    [Header("[Características físicas]")]

    [SerializeField]
    protected float masa = 70.0f; 
    [SerializeField]
    protected float velocidad_max = 6.0f;
    [SerializeField]
    protected float orientacion = 0.0f;
    [SerializeField]
    protected float rotacion = 0.0f;
    [SerializeField]
    protected float rotacion_max = 100.0f;
    [SerializeField]
    protected float aceleracion_max = 12f;
    [SerializeField]
    protected float aceleracion_angular_max = 100.0f;
    [SerializeField]
    protected Vector3 velocidad_vec = Vector3.zero, aceleracion = Vector3.zero;
    protected float coef_frenado = 1f;


    // Métodos get y set
    public float getVelocidadMax() {return this.velocidad_max;}
    public float getOrientacion() {return this.orientacion;}
    public float getRotacion() {return this.rotacion;}
    public float getRotacionMax() {return this.rotacion_max;}
    public float getAceleracionMax() {return this.aceleracion_max;}
    public float getAceleracionAngularMax() {return this.aceleracion_angular_max;}
    public Vector3 getVelocidadVec() {return this.velocidad_vec;}
    public void setOrientacion(float orientacion) {this.orientacion = orientacion;}
    public void setRotacion(float rotacion) {this.rotacion = rotacion;}
    public void setVelocidadVec(Vector3 velocidad_vec) {this.velocidad_vec = velocidad_vec;}


    // Convertir la posición del personaje en un ángulo. TO-DO: Quizas meter "* Mathf.Rad2Deg";
    public float posicionToAngulo() { 
        return Mathf.Atan2(this.transform.position.x, this.transform.position.z); 
    }


    // Convertir la orientación del personaje en un vector
    public Vector3 orientacionToVector() {
        return new Vector3(Mathf.Sin(this.orientacion * Mathf.Deg2Rad), 0, Mathf.Cos(this.orientacion * Mathf.Deg2Rad));
    }


    // Función sobrecargada de la anterior
    public Vector3 orientacionToVector(float angulo) {
        return new Vector3(Mathf.Sin(angulo * Mathf.Deg2Rad), 0, Mathf.Cos(angulo * Mathf.Deg2Rad));
    }


    // Dada la posición de otro personaje (un Vector3) determinar cuál es el ángulo más pequeño para que el personaje se rote hacia él.
    public float posicionPersonajeToAngulo(Vector3 personaje) {
        // Producto escalar vector del player con el del personaje
        float prodEscalar = this.transform.position.x*personaje.x + this.transform.position.z*personaje.z;
        // Módulo del vector del player
        float moduloV1 = Mathf.Sqrt(Mathf.Pow(this.transform.position.x, 2) + Mathf.Pow(this.transform.position.z, 2));
        // Módulo del vector del personaje
        float moduloV2 = Mathf.Sqrt(Mathf.Pow(personaje.x, 2) + Mathf.Pow(personaje.z, 2));
        if (moduloV1 != 0 && moduloV2 != 0)
            return Mathf.Acos(prodEscalar/(moduloV1 * moduloV2));
        else
            return 0;
    }
}
