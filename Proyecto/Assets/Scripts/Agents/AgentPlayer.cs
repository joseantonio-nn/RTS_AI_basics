using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentPlayer : Agent {

    void Update() {
        velocidad_vec = (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * velocidad_max * (100/masa);
        transform.Translate(velocidad_vec * Time.deltaTime, Space.World);
        transform.LookAt(transform.position + velocidad_vec);
        base.orientacion = transform.rotation.eulerAngles.y;
    }

}