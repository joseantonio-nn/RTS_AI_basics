using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapaInfluencia {
    public static List<List<float>> Neutral((int x, int z) dimension) {
        if (dimension.x < 0) return new List<List<float>>();
        if (dimension.z < 0) return new List<List<float>>();

        var matrix = Enumerable.Repeat(0, dimension.x)
            .Select(x => Enumerable.Repeat(0f, dimension.z).ToList())
            .ToList();
        return matrix;
    }

    public static List<List<float>> Influencias(List<(int x, int z, float i)> influencias, (int x, int z) dimension) {
        if (influencias.Count == 0) return Neutral(dimension);

        var NEUTRAL = Neutral(dimension);

        // Para cada influencia de cada personaje
        foreach (var influencia in influencias) {

            // Obtenemos las influencias que hace ese personaje
            var individuales = Influenciar(influencia, dimension);

            // Aplicamos cada influencia de ese personaje sobre el NEUTRAL
            foreach (var individual in individuales) {
                // Posicion
                var x = individual.x;
                var z = individual.z;
                // Valor
                var i = individual.i;
                // Aplicar sobre NEUTRAL
                NEUTRAL[x][z] = NEUTRAL[x][z] + i;
            }
        }
        return NEUTRAL;
    }

    private static List<(int x, int z, float i)> Influenciar((int x, int z, float i) influencia, (int x, int z) dimension) {
        // Generar subespacio
        var capas = (int) Radio(influencia.i);
        var subespacio = Subespacio.Posiciones(capas);

        // Absoluto al origen
        var origen = (influencia.x, influencia.z);
        subespacio = Subespacio.Absoluto(subespacio, origen);

        // Limitar a las dimensiones
        subespacio = Subespacio.Limitar(subespacio, dimension);

        // Calculamos las distancias al origen
        // Calculamos las influencias al origen
        // Lo pegamos con las posiciones
        var I0 = influencia.i;
        return subespacio
            .Select(posicion => Distancia(origen, posicion))
            .Select(distancia => Influencia(I0, distancia))
            .Zip(subespacio, (i, p) => (p.x, p.z, i))
            .ToList();
    }

    private static float Distancia((int x, int z) p1, (int x, int z) p2) {
        // Euclidea
        var dx = p2.x - p1.x;
        var dz = p2.z - p1.z;
        return Mathf.Sqrt(dx*dx + dz*dz);
    }

    // Influencias positivas (bando positivo) -> se reducen
    // Influencias negativas (bando negativo) -> se aumentan
    private static float Influencia(float I0, float distancia) {
        if (distancia < 0) return 0;
        if (I0 == 0) return 0;
        if (I0 > 0) {
            var i = I0 - distancia;
            if (i < 0) return 0;
            else return i;
        }
        if (I0 < 0) {
            var i = I0 + distancia;
            if (i > 0) return 0;
            else return i;
        }
        return 0;
    }

    private static float Radio(float I0) {
        return Mathf.Abs(I0);
    }
}