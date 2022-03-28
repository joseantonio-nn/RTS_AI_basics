using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Subespacio {
    public static List<(int x, int z)> Posiciones(int capas) {
        if (capas < 0) return new List<(int, int)>();
        
        // Rango de [-capas, capas]
        var rango = Enumerable.Range(-1*capas, 2*capas + 1);

        // P. Cartesiano de rango x rango
        return rango
            .SelectMany(x => rango, (n, m) => (n, m))
            .ToList();
    }

    public static List<(int x, int z)> Absoluto(List<(int x, int z)> posiciones, (int x, int z) origen) {
        return posiciones.Select(p => (origen.x + p.x, origen.z + p.z)).ToList();
    }

    public static bool Dentro((int x, int z) posicion, (int x, int z) limite) {
        return Dentro(posicion.x, limite.x) && Dentro(posicion.z, limite.z);
    }

    public static bool Dentro(int posicion, int limite) {
        return 0 <= posicion && posicion < limite;
    }

    public static List<(int x, int z)> Limitar(List<(int x, int z)> grid, (int x, int z) limite) {
        return grid.Where(posicion => Dentro(posicion, limite)).ToList();
    }
}
