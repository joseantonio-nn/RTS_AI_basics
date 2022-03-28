using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Clase que guarda los quantums correspondiente a cada steering
public class Plan {
    public LinkedList<Grupo> esquema;

    public Plan(LinkedList<Grupo> esquema) {
        this.esquema = esquema;
    }

    // Round robin
    public void planificar() {
        if (esquema.Count == 0) return ;

        // Reencolar todos los grupos al principio cuyo quantum = 0
        while (esquema.First().actual == 0) {
            // Reencolar
            var grupo = esquema.First();
            esquema.RemoveFirst();
            esquema.AddLast(grupo);

            // Reiniciar quantum
            grupo.reset();
        }

        // Consumir primero
        esquema.First().reducir();
    }
}
