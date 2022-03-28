using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Botones : MonoBehaviour {
    public Controlador controlador;

    public void botonModoGuerraTotal() { controlador.ActivarGuerraTotal(); }

    public void botonOfensivoAzul() { controlador.ActivarModoEnBando(Actitud.OFENSIVO, Bando.Azul); }

    public void botonDefensivoAzul() { controlador.ActivarModoEnBando(Actitud.DEFENSIVO, Bando.Azul); }

    public void botonOfensivoRojo() { controlador.ActivarModoEnBando(Actitud.OFENSIVO, Bando.Rojo); }

    public void botonDefensivoRojo() { controlador.ActivarModoEnBando(Actitud.DEFENSIVO, Bando.Rojo); }

    public void botonSalir() { StartCoroutine("salir"); }

    IEnumerator salir() {
        yield return new WaitForSeconds(5);
        Application.Quit();
    }
}
