using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Esquemas {

    public static LinkedList<Grupo> sharkFleeing() {
        return new LinkedList<Grupo>(new [] {
            new Grupo(Grupos.fleeing, 3),
            new Grupo(Grupos.flocking, 2),
            new Grupo(Grupos.following, 1)
        });
    }

    public static LinkedList<Grupo> leaderFollowing() {
        return new LinkedList<Grupo>(new [] {
            new Grupo(Grupos.following, 3),
            new Grupo(Grupos.fleeing, 1),
            new Grupo(Grupos.colision, 3),
            new Grupo(Grupos.fleeing, 1)
        });
    }

    public static LinkedList<Grupo> all() {
        return new LinkedList<Grupo>(new [] {
            new Grupo(Grupos.all, 1),
        });
    }
}
