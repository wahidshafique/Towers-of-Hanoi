using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerp : MonoBehaviour {
    Animator anim;
    private bool lerpSwitch = false;
    // Use this for initialization
    void Start() {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            lerpSwitch = !lerpSwitch;
            if (lerpSwitch) {
                anim.SetBool("ChangeCam", true);
            } else {
                anim.SetBool("ChangeCam", false);
            }
        }
    }
}
