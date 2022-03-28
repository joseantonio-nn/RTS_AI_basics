using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barras : MonoBehaviour {
    
    public Slider slider;

    public void SetVida(float vida) { slider.value = vida; }
    public void SetVidaMax(float maxVida) { slider.maxValue = maxVida; slider.value = maxVida;}

    public void SetTiempoConstruccion(float tCons) { slider.value = tCons; }
    public void SetTiempoMaxConst(float tMaxCons) { slider.maxValue = tMaxCons; slider.value = 0f; }
}
