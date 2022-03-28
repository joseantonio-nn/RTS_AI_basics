using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MiniMapa : MonoBehaviour {
    private List<List<GameObject>> MAPA;
    public Grid grid;

    void Start() {
        MAPA = Construir((grid.tamColumnas,grid.tamFilas), new Vector3(0, 0, 300), 1/5f);
    }

    public void Actualizar(List<List<float>> influencias, float total) {
        for (int i = 0; i < MAPA.Count; i++) {
            for (int j = 0; j < MAPA[i].Count; j++) {
                // Solo dentro de limites
                if (i >= influencias.Count) continue;
                if (j >= influencias[i].Count) continue;

                // Color
                var renderer = MAPA[i][j].GetComponent<Renderer>();
                renderer.material.color = Color(influencias[i][j], total);
            }
        }
    }

    private static Color Color(float influencia, float total) {
        // Casos especiales
        if (influencia == 0) return new Color(255, 255, 255);
        if (total == 0) return new Color(255, 255, 255);

        // Proporcion
        var proporcion = Mathf.Abs(influencia/total);

        // Limitar a 1
        if (proporcion > 1) proporcion = 1;

        // Nivel de blanco
        var blanco = 1 - proporcion;

        // Intensidad
        Color color = new Color(255, 255, 255);
        if (influencia > 0)
            color = new Color(1f, blanco, blanco);
        if (influencia < 0)
            color = new Color(blanco, blanco, 1f);
        return color;
    }

    private static List<List<GameObject>> Construir((int x, int z) dimension, Vector3 origen, float tamano) {
        if (dimension.x < 0) return new List<List<GameObject>>();
        if (dimension.z < 0) return new List<List<GameObject>>();

        var desplazamiento = tamano*10f;
        var MAPA = new List<List<GameObject>>();
        for (int i = 0; i < dimension.x; i++) {
            var LINEA = new List<GameObject>();
            for (int j = 0; j < dimension.z; j++) {
                // Objeto
                var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
                // Posicion
                go.transform.position = origen + (new Vector3(i*desplazamiento, 0, j*desplazamiento));
                // Escala
                go.transform.localScale = new Vector3(tamano, 1, tamano);
                // Siguiente
                LINEA.Add(go);
            }
            MAPA.Add(LINEA);
        }
        return MAPA;
    }

}
