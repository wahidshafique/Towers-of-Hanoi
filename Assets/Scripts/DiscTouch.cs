using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscTouch : MonoBehaviour {
    #region variables
    //placement
    [HideInInspector]
    public bool isInteractable = false;
    [HideInInspector]
    public int valueID = 0;
    private float origZ;//just to keep it in place
    private Renderer myRenderer;
    private Rigidbody rig;
    [SerializeField]
    Transform[] destHanoiTops;
    Vector3[] refposes;

    //physics
    private float travelTime;
    private float time = 1.0f;
    private float acceleration;
    float forceForHeight;
    private Vector3 force = Vector3.zero;
    private Vector3 desiredDisplacement = Vector3.zero;
    Collider[] attachedColliders;

    [Tooltip("Layer to ignore for the beam")]
    private LayerMask beamMask;
    #endregion variables

    void Awake() {
        print("tops len" + destHanoiTops.Length);
        refposes = new Vector3[destHanoiTops.Length];
        LoadRefs();
        if (transform.CompareTag("TorusSmall")) {
            valueID = 0;
        } else if (transform.CompareTag("TorusMedium")) {
            valueID = 1;
        } else if (transform.CompareTag("TorusLarge")) {
            valueID = 2;
        } else if (transform.CompareTag("TorusXL")) {
            valueID = 3;
        } else if (transform.CompareTag("TorusMega")) {
            valueID = 4;
        } else if (transform.CompareTag("TorusGod")) {
            valueID = 4;
        }

        myRenderer = GetComponent<Renderer>();
        rig = GetComponent<Rigidbody>();
        attachedColliders = GetComponents<Collider>();
    }

    void Start() {
        isInteractable = true;
        origZ = transform.position.z;
    }

    void LoadRefs() {
        for (int i = 0; i < destHanoiTops.Length; i++) {
            refposes[i] = GetOffsetPosition(destHanoiTops[i], false);
        }
    }

    public void MoveDisc(int destinationNum, float initdelay) {
        switch (destinationNum) {
            case 1:
                StartCoroutine(WaitAndTravel(1.0f, refposes[0], initdelay));
                break;
            case 2:
                StartCoroutine(WaitAndTravel(1.0f, refposes[1], initdelay));
                break;
            case 3:
                StartCoroutine(WaitAndTravel(1.0f, refposes[2], initdelay));
                break;
            case 4:
                StartCoroutine(WaitAndTravel(1.0f, refposes[3], initdelay));
                break;
            default:
                print("You suppied bad argument");
                break;
        }
    }

    private IEnumerator WaitAndTravel(float waitTime, Vector3 refPos, float initdelay) {
        yield return new WaitForSeconds(initdelay);
        LaunchProjectile();
        yield return new WaitForSeconds(waitTime);
        rig.velocity = Vector3.zero;
        LoadRefs();
        Vector3 ourPos = GetOffsetPosition(this.transform, true);
        desiredDisplacement = refPos - ourPos;
        LaunchProjectileToDest();
        SetAllCollidersStatus(true);
        yield return new WaitForSeconds(time);
    }

    #region physics
    float CalculateYImpulse(float displacement, float time) {
        float velocity = (displacement - (0.5f * Physics.gravity.y * (time * time))) / (time);
        //since we are starting at rest, the difference in velocity is the velocity we calculated
        return velocity * rig.mass;
    }

    void LaunchProjectile() {
        float g = Physics.gravity.magnitude;
        SetAllCollidersStatus(false);
        forceForHeight = Mathf.Sqrt(2 * g * 50);
        travelTime = 2 * forceForHeight / g;
        //time = travelTime;
        //forceForHeight = Mathf.Sqrt(2 * destHanoi1.transform.position.y * 4 * 9.8f);
        //rig.velocity = new Vector3(0, vertSpeed, 0);
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

    public void SetAllCollidersStatus(bool active) {
        foreach (Collider c in attachedColliders) {
            c.enabled = active;
        }
    }
    #endregion physics
}
