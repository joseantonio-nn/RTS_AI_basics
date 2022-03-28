using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grupos {
    public static readonly List<string> flocking = new List<string>
        {"Separation", "Cohesion", "Alignment", "Wander"};

    public static readonly List<string> following = new List<string>
        {"Seek", "Arrive", "VelocityMatching", "OffsetArrive", "Wander", "WallFollowing"};

    public static readonly List<string> fleeing = new List<string>
        {"Flee", "Leave"};

    public static readonly List<string> colision = new List<string>
        {"WallAvoidance", "Separation", "CollisionAvoidance"};

    public static readonly List<string> all = new List<string>
        {"Seek", "Arrive", "VelocityMatching", "Flee", "Leave", "Align", "AntiAlign",
         "Face", "Interpose", "LookWhereYouGoing", "OffsetAlign", "OffsetArrive",
         "OffsetPursuit", "PathFollowingSinOffset", "PathFollowing", "PredictivePathFollowing", "Pursue",
         "WallAvoidance", "WallFollowing", "Wander", "Separation", "Cohesion", "Alignment", "CollisionAvoidance"
        };
}
