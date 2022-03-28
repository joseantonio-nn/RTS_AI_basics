using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Rol {Melee, ApoyoLateral, Retaguardia, Tanque}

public class Formaciones : MonoBehaviour {
    private static float separacion = 2.5f;
    private static float separacion_escalable = 100f;
    private static Vector3 vectorAltura = new Vector3(0,1,0); 


    // Formación estática 1
    public static List<(Vector3, float)> defensiva() {
        return new List<(Vector3, float)> {
            (new Vector3(0,0,0) * separacion, 0),
            (new Vector3(-1,0,-1) * separacion, -90),
            (new Vector3(1,0,-1) * separacion, 90),
            (new Vector3(0,0,-2) * separacion, 180),
        };
    }


    // Formación estática 2
    public static List<(Vector3, float)> inspectionPatrol() {
        return new List<(Vector3, float)> {
            (new Vector3(0,0,0) * separacion, 0),
            (new Vector3(-1,0,0) * separacion, 0),
            (new Vector3(-2,0,-1) * separacion, 315),
            (new Vector3(-1,0,-1) * separacion, 180),
            (new Vector3(0,0,-1) * separacion, 180),
            (new Vector3(1,0,-1) * separacion, 45)
        };
    }


    // Formación escalable 1 - Círculo
    public static List<(Vector3, float)> escalableCirculo(int tamano) {
        // Casos especiales
        if (tamano <= 0) return new List<(Vector3, float)>();

        // Parametros extra
        var delta = 360.0f/tamano;
        var radio = (tamano*separacion_escalable + (tamano-1)*separacion_escalable)/360.0f;

        // Generar formacion
        var angulos = Enumerable
            .Range(0, tamano)
            .Select(x => x*delta);
        var formacion = angulos
            .Select(angulo => posicion(angulo, radio))
            .ToList();

        return formacion;
    }


    // Formación escalable 2 - Triángulo
    public static List<(Vector3, float)> escalableTriangulo(int tamano) {
        List<(Vector3, float)> formacion = new List<(Vector3, float)>();

        // Casos especiales
        if (tamano <= 0) return formacion;
        
        // Número de soldados que caben en la fila actual, incialmente 2
        int soldFila = 2;
        // Número de soldados restantes en la fila
        int sold = soldFila;
        // Posición en Z, disminuirá cada vez que se cambie de fila, inicialmente -2 para estar detrás del líder
        int posZ = -2;
        // Posición en X, irá aumentando pero al cambiar de fila tendrá que volver un poco antes que la primera de la fila anteior,
        // inicialmente -1 para estar a la izquierda del líder
        int posX = -1;
        // Primera posición en X de la fila actual
        int primX = posX;

        while (tamano != 0) {
            // Cambia de fila (una más)
            if (sold == 0) {
                soldFila++;
                sold = soldFila;
                posZ -= 2;
                // Cada vez que se añada una fila, la siguiente empezará una casilla más a la izquierda que la anterior
                posX = primX - 1; 
                primX = posX;
            }
            formacion.Add((new Vector3(posX, 0, posZ), 0));
            posX += 2;
            sold--;
            tamano--;
        }
        return formacion;
    }


    // Formación escalable 3 - Cuadrados consecutivos
    public static List<(Vector3, float)> escalableCuadrado(int tamano) {
        List<(Vector3, float)> formacion = new List<(Vector3, float)>();

        // Casos especiales
        if (tamano <= 0) return formacion;

        // Número de soldados restantes en en el cuadrado (siempre habrán 9)
        int sold = 9;
        // Posición en Z, queremos la Z del lider (0) pero como entra primera al if se restará 2, por tanto inicializamos a 2
        int posZ = 2;
        // Posición en X, pasa lo mismo que arriba
        int posX = 6;
        
        while (tamano != 0) {
            // Cambio cuadrado (uno más)
            if (sold == 0) {
                posZ = 0;
                sold = 9;
                posX += 2;
            } // Si no es cero pero si modulo 3, entonces es una nueva fila dentro del mismo cuadrado
            else if (sold % 3 == 0) {
                // Bajamos una fila
                posZ -= 2;
                // 2 posiciones a la izquierda + el 2 de debajo
                posX -= 6; 
            }
            formacion.Add((new Vector3(posX, 0, posZ), 0));
            posX += 2;
            sold--;
            tamano--;
        }
        //La 0 es la posicion del lider que no la queremos, pero la usamos para que los cuadrados salgan bien
        formacion.RemoveAt(0); 
        return formacion;
    }


    static (Vector3, float) posicion(float angulo, float radio) {
        float radian = angulo * Mathf.Deg2Rad;
        float X = radio * Mathf.Sin(radian);
        float Z = radio * Mathf.Cos(radian) - radio;
        return (new Vector3(X, 0, Z), angulo);
    }


    // Rol duro siendo un Melee Pesado el líder (Se le asigna el tanque de arriba a la izquierda)
    public static List<(Vector3, float, string)> rolDuroLiderMeleeP() {
        return new List<(Vector3, float, string)> {
            (new Vector3(0,0,0) * separacion, 0, "Tanque"),
            (new Vector3(1,0,0) * separacion, 0, "Tanque"),
            (new Vector3(-1,0,-1) * separacion, 0, "Melee"),
            (new Vector3(0,0,-1) * separacion, 0, "Melee"),
            (new Vector3(1,0,-1) * separacion, 0, "Melee"),
            (new Vector3(2,0,-1) * separacion, 0, "Melee"),
            (new Vector3(-2,0,-2) * separacion, -90, "ApoyoLateral"),
            (new Vector3(-2,0,-3) * separacion, -90, "ApoyoLateral"),
            (new Vector3(3,0,-2) * separacion, 90, "ApoyoLateral"),
            (new Vector3(3,0,-3) * separacion, 90, "ApoyoLateral"),
            (new Vector3(-1,0,-4) * separacion, 180, "Retaguardia"),
            (new Vector3(0,0,-4) * separacion, 180, "Retaguardia"),
            (new Vector3(1,0,-4) * separacion, 180, "Retaguardia"),
            (new Vector3(2,0,-4) * separacion, 180, "Retaguardia")
        };
    }

    // Rol duro siendo un Melee Ligero el líder (Se le asigna el melee de arriba a la izquierda)
    public static List<(Vector3, float, string)> rolDuroLiderMeleeL() {
        return new List<(Vector3, float, string)> {
            (new Vector3(0,0,0) * separacion, 0, "Melee"),
            (new Vector3(1,0,1) * separacion, 0, "Tanque"),
            (new Vector3(2,0,1) * separacion, 0, "Tanque"),
            (new Vector3(1,0,0) * separacion, 0, "Melee"),
            (new Vector3(2,0,0) * separacion, 0, "Melee"),
            (new Vector3(3,0,0) * separacion, 0, "Melee"),
            (new Vector3(-1,0,-1) * separacion, -90, "ApoyoLateral"),
            (new Vector3(-1,0,-2) * separacion, -90, "ApoyoLateral"),
            (new Vector3(4,0,-1) * separacion, 90, "ApoyoLateral"),
            (new Vector3(4,0,-2) * separacion, 90, "ApoyoLateral"),
            (new Vector3(0,0,-3) * separacion, 180, "Retaguardia"),
            (new Vector3(1,0,-3) * separacion, 180, "Retaguardia"),
            (new Vector3(2,0,-3) * separacion, 180, "Retaguardia"),
            (new Vector3(3,0,-3) * separacion, 180, "Retaguardia")
        };
    }

    // Rol duro siendo un Ranged el líder (Se le asigna la retaguardia de abajo a la derecha)
    public static List<(Vector3, float, string)> rolDuroLiderRanged() {
        return new List<(Vector3, float, string)> {
            (new Vector3(0,0,0) * separacion, 0, "Retaguardia"),
            (new Vector3(1,0,-4) * separacion, 180, "Tanque"),
            (new Vector3(2,0,-4) * separacion, 180, "Tanque"),
            (new Vector3(0,0,-3) * separacion, 180, "Melee"),
            (new Vector3(1,0,-3) * separacion, 180, "Melee"),
            (new Vector3(2,0,-3) * separacion, 180, "Melee"),
            (new Vector3(3,0,-3) * separacion, 180, "Melee"),
            (new Vector3(-1,0,-1) * separacion, -90, "ApoyoLateral"),
            (new Vector3(-1,0,-2) * separacion, -90, "ApoyoLateral"),
            (new Vector3(4,0,-1) * separacion, 90, "ApoyoLateral"),
            (new Vector3(4,0,-2) * separacion, 90, "ApoyoLateral"),
            (new Vector3(1,0,0) * separacion, 0, "Retaguardia"),
            (new Vector3(2,0,0) * separacion, 0, "Retaguardia"),
            (new Vector3(3,0,0) * separacion, 0, "Retaguardia")
        };
    }

    // Rol duro siendo un Espia el líder (Se le asigna el apoyo lateral de arriba a la izquierda)
    public static List<(Vector3, float, string)> rolDuroLiderEspia() {
        return new List<(Vector3, float, string)> {
            (new Vector3(0,0,0) * separacion, 0, "ApoyoLateral"),
            (new Vector3(2,0,-2) * separacion, 90, "Tanque"),
            (new Vector3(2,0,-3) * separacion, 90, "Tanque"),
            (new Vector3(1,0,-1) * separacion, 90, "Melee"),
            (new Vector3(1,0,-2) * separacion, 90, "Melee"),
            (new Vector3(1,0,-3) * separacion, 90, "Melee"),
            (new Vector3(1,0,-4) * separacion, 90, "Melee"),
            (new Vector3(-1,0,0) * separacion, 0, "ApoyoLateral"),
            (new Vector3(0,0,-5) * separacion, 180, "ApoyoLateral"),
            (new Vector3(-1,0,-5) * separacion, 180, "ApoyoLateral"),
            (new Vector3(-2,0,-1) * separacion, -90, "Retaguardia"),
            (new Vector3(-2,0,-2) * separacion, -90, "Retaguardia"),
            (new Vector3(-2,0,-3) * separacion, -90, "Retaguardia"),
            (new Vector3(-2,0,-4) * separacion, -90, "Retaguardia")
        };
    }

    // Rol suave
    public static List<(Vector3, float, List<int>)> rolSuave() {
        
        // Costes: melee ligero, melee pesado, espía y ranged 
        List<int> costesTanque = new List<int>(){0,0,120,60};
        List<int> costesMelee = new List<int>(){0,0,0,50};
        List<int> costesApoyoLateral = new List<int>(){50,100,0,0};
        List<int> costesRetaguardia = new List<int>(){0,150,30,0};

        return new List<(Vector3, float, List<int>)> {
            (new Vector3(0,0,0) * separacion, 0, costesTanque),
            (new Vector3(1,0,0) * separacion, 0, costesTanque),
            (new Vector3(-1,0,-1) * separacion, 0, costesMelee),
            (new Vector3(0,0,-1) * separacion, 0, costesMelee),
            (new Vector3(1,0,-1) * separacion, 0, costesMelee),
            (new Vector3(2,0,-1) * separacion, 0, costesMelee),
            (new Vector3(-2,0,-2) * separacion, -90, costesApoyoLateral),
            (new Vector3(-2,0,-3) * separacion, -90, costesApoyoLateral),
            (new Vector3(3,0,-2) * separacion, 90, costesApoyoLateral),
            (new Vector3(3,0,-3) * separacion, 90, costesApoyoLateral),
            (new Vector3(-1,0,-4) * separacion, 180, costesRetaguardia),
            (new Vector3(0,0,-4) * separacion, 180, costesRetaguardia),
            (new Vector3(1,0,-4) * separacion, 180, costesRetaguardia),
            (new Vector3(2,0,-4) * separacion, 180, costesRetaguardia)
        };
    }
}
