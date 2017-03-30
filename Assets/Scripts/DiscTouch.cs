﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscTouch : MonoBehaviour {
    #region variables
    //placement
    [HideInInspector]
    public bool isInteractable;
    private bool isLegal = true;
    private bool isMouseDown = false;
    private float origZ;//just to keep it in place
    private Renderer myRenderer;
    private Rigidbody rig;
    [SerializeField]
    Transform destHanoi1;
    [SerializeField]
    Transform destHanoi2;
    [SerializeField]
    Transform destHanoi3;

    //physics
    private float time = 1f;
    private float acceleration;
    float forceForHeight;
    private Vector3 force = Vector3.zero;
    private Vector3 desiredDisplacement = Vector3.zero;

    //references for posts
    Vector3 refPos1;
    Vector3 refPos2;
    Vector3 refPos3;

    [Tooltip("Layer to ignore for the beam")]
    private LayerMask beamMask;
    #endregion variables

    void Start() {
        myRenderer = GetComponent<Renderer>();
        isInteractable = true;
        origZ = transform.position.z;
        rig = GetComponent<Rigidbody>();
        refPos1 = GetOffsetPosition(destHanoi1, false);
        refPos2 = GetOffsetPosition(destHanoi2, false);
        refPos3 = GetOffsetPosition(destHanoi3, false);
        //MoveDisc("F", "f", "f");
    }

    void Update() {
        if (isMouseDown) {
            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
        }
    }

    IEnumerator onCoroutine() {
        while (true) {
            StartCoroutine(WaitAndTravel(1.0f, refPos1));
            yield return new WaitForSeconds(3f);
            StartCoroutine(WaitAndTravel(1.0f, refPos2));
            yield return new WaitForSeconds(3f);
            StartCoroutine(WaitAndTravel(1.0f, refPos3));
            yield return new WaitForSeconds(3f);
        }
    }

    bool isValidMove() {
        isLegal = true;
        RaycastHit hit;
        float thickness = 1f; //<-- Desired thickness here.
        Vector3 origin = transform.position - new Vector3(0, 1, 0);
        Vector3 direction = transform.TransformDirection(Vector3.up);
        if (Physics.SphereCast(origin, thickness, direction, out hit)) {
            if (hit.transform.tag == "Torus") {
                isLegal = false;
                print(hit.transform.gameObject.name);
            }
        }
        return true;
    }

    public void MoveDisc(int destinationNum) {
        switch (destinationNum) {
            case 1:
                StartCoroutine(WaitAndTravel(1.0f, refPos1));
                break;
            case 2:
                StartCoroutine(WaitAndTravel(1.0f, refPos2));
                break;
            case 3:
                StartCoroutine(WaitAndTravel(1.0f, refPos3));
                break;
            default:
                break;
        }
    }

    void OnMouseDown() {
        isValidMove();
        if (isInteractable && isLegal) {
            myRenderer.material.color = Color.white;
            transform.position = new Vector3(transform.position.x, transform.position.y, origZ);
            transform.rotation = Quaternion.identity;
            isMouseDown = true;
            rig.isKinematic = true;
        } else {
            myRenderer.material.color = Color.red;
        }
    }

    void OnMouseUp() {
        isMouseDown = false;
        rig.isKinematic = false;
    }
    private IEnumerator WaitAndTravel(float waitTime, Vector3 refPos) {
        LaunchProjectile();
        yield return new WaitForSeconds(waitTime);
        rig.velocity = Vector3.zero;
        refPos1 = GetOffsetPosition(destHanoi1, false);
        refPos2 = GetOffsetPosition(destHanoi2, false);
        refPos3 = GetOffsetPosition(destHanoi3, false);
        Vector3 ourPos = GetOffsetPosition(this.transform, true);
        desiredDisplacement = refPos - ourPos;
        LaunchProjectileToDest();
        yield return new WaitForSeconds(time);
    }

    #region physics
    float CalculateYImpulse(float displacement, float time) {
        float velocity = (displacement - (0.5f * Physics.gravity.y * (time * time))) / (time);
        //since we are starting at rest, the difference in velocity is the velocity we calculated
        return velocity * rig.mass;
    }

    void LaunchProjectile() {
        forceForHeight = Mathf.Sqrt(2 * destHanoi1.transform.position.y * 4 * 9.8f);
        rig.AddForce(new Vector3(0, forceForHeight, 0), ForceMode.Impulse);
    }

    void LaunchProjectileToDest() {
        force.y = CalculateYImpulse(desiredDisplacement.y, time);
        force.x = (desiredDisplacement.x / time) * rig.mass;
        rig.AddForce(force / Time.fixedDeltaTime);
        force = Vector3.zero;
    }

    Vector3 GetOffsetPosition(Transform refTransform, bool isBottom) {
        if (isBottom) {
            transform.rotation = Quaternion.identity;
            rig.freezeRotation = true;
            refTransform.position = new Vector3(transform.position.x, forceForHeight, transform.position.z);
        }
        return isBottom ? refTransform.position - (new Vector3(0.0f, refTransform.localScale.y, 0.0f) * 0.5f) : refTransform.position + (new Vector3(0.0f, refTransform.localScale.y, 0.0f) * 0.5f);
    }
    #endregion physics
}
