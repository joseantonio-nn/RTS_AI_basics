using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Estructura : Soldado {

    // No hace override para que no se herede de Soldado las cosas que no nos interesan
    protected override void Start() {}
    void Update() {}
    void LateUpdate() {}
    
    public abstract void Destruirse();
}
