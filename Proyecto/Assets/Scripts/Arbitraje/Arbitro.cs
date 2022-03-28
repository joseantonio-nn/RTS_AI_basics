using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Arbitro {

    public static Steering ponderado(AgentNPC agent, List<SteeringBehaviour> steerings) {
        Steering acumulado = new Steering();

        // Acumular
        foreach (var behavior in steerings) {
            Steering steering = behavior.GetSteering(agent);
            acumulado.linear += behavior.peso * steering.linear;
            acumulado.angular += behavior.peso * steering.angular;
        }

        // No permitimos que se pase de los valores maximos
        acumulado.linear = acumulado.linear.normalized * Mathf.Min(acumulado.linear.magnitude, agent.getAceleracionMax());
        if (Mathf.Abs(acumulado.angular) > Mathf.Abs(agent.getAceleracionAngularMax()))
            acumulado.angular = Mathf.Sign(acumulado.angular)*agent.getAceleracionAngularMax();

        return acumulado;
    }
    
    public static Steering prioritario(AgentNPC agent, List<List<SteeringBehaviour>> grupos, float epsilon) {
        var prioritario = new Steering();

        // Aplicar el primero más prioritario
        foreach (var grupo in grupos) {
            prioritario = ponderado(agent, grupo);
            float linear  = prioritario.linear.magnitude;
            float angular = Mathf.Abs(prioritario.angular);
            if (linear > epsilon || angular > epsilon)
                return prioritario;
        }

        // Aplicar el último si no aplica ninguno o cero si lista vacia
        return prioritario;
    }

    public static Steering dinamico(AgentNPC agent, LinkedList<Grupo> esquema, List<SteeringBehaviour> steerings) {
        List<List<SteeringBehaviour>> grupos = agrupar(steerings, esquema);
        if (grupos.Count == 0)
            return new Steering();
        else
            return ponderado(agent, grupos[0]);
    }

    public static List<List<SteeringBehaviour>> agrupar(List<SteeringBehaviour> steerings, LinkedList<Grupo> esquema) {
        // Construir indice para mayor eficiencia
        var indice = steerings
            .Select(steer => steer.GetType().ToString())
            .Zip(steerings, (nombre, steering) => (nombre, steering))
            .ToDictionary(x => x.nombre, x => x.steering);

        // Construir grupos segun la estructura de `esquema`
        // Si al NPC le faltase alguno, no se pone y ya.
        var grupos = esquema
            .Select(grupo => grupo.grupo
                .Where(steering => indice.ContainsKey(steering))
                .Select(steering => indice[steering])
                .ToList()
            )
            .Where(grupo => grupo.Count != 0)
            .ToList();

        return grupos;
    }
}
