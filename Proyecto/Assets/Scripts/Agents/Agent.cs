using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Bodi {

    // Declaración de variables
    [Header("[Comportamiento Steerings]")]
    [SerializeField]
    protected float radio_int = 5.0f;
    [SerializeField]
    protected float radio_ext = 10.0f;
    [SerializeField]
    protected float angulo_int = 10.0f;
    [SerializeField]
    protected float angulo_ext = 45.0f;
    [Header("[Depuración]")]
    [SerializeField]
    protected bool show = false;

    // Métodos get
    public float getRadioInt() {return this.radio_int;}
    public float getRadioExt() {return this.radio_ext;}
    public float getAnguloInt() {return this.angulo_int;}
    public float getAnguloExt() {return this.angulo_ext;}


    void Update() {
        if (radio_int > radio_ext) radio_int = radio_ext;
        if (angulo_int > angulo_ext) angulo_int = angulo_ext;
    }


    // Método para activar o desactivar el modo depuración
    public virtual void actualizarShow(bool show) {
        this.show = show;
    }


    // Método para mostrar figuras que ayudan en la depuración de los movimientos desarrollados
    protected virtual void OnDrawGizmos() {
        
        // Para que el Gizmo no toque el suelo 
        Vector3 from = transform.position + (new Vector3(0,1,0));

        // Si se ha seleccionado el modo depuracion
        if (show) {
            #if UNITY_EDITOR
                // Radio interior
                UnityEditor.Handles.DrawWireDisc(from, new Vector3(0,1,0), radio_int);
                // Radio exterior
                UnityEditor.Handles.DrawWireDisc(from, new Vector3(0,1,0), radio_ext);
            #endif
            // Bigotes ángulo interior
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + orientacionToVector(angulo_int / 2f + this.orientacion) * radio_ext);
            Gizmos.DrawLine(transform.position, transform.position + orientacionToVector(-angulo_int / 2f + this.orientacion) * radio_ext);
            // Bigotes ángulo exterior
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + orientacionToVector(angulo_ext / 2f + this.orientacion) * radio_ext);
            Gizmos.DrawLine(transform.position, transform.position + orientacionToVector(-angulo_ext / 2f + this.orientacion) * radio_ext);
            // Vector velocidad
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + orientacionToVector() * velocidad_vec.magnitude);
        }
    }
}
