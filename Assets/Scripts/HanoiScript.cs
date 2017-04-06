using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HanoiScript : MonoBehaviour {
    Text winner;
    Button autoSolveBtn;
    [SerializeField]
    DiscTouch[] discs;

    //this is basically a temp for when you have selected something
    private GameObject selectedRealPole;
    private GameObject selectedOtherPole;
    private int selectedOtherPoleval;

    private Stack<DiscTouch>[] poles;
    public GameObject[] hanoiTowers;

    private bool autoRunReset = false;
    private bool canInteract = true;
    private bool isNotMakingMove = true;
    private bool gameOver = false;
    private bool auto = false;
    void Start() {
        print("d len" + discs.Length);
        selectedOtherPoleval = 999;
        selectedRealPole = null;
        selectedOtherPole = null;
        winner = GameObject.Find("Winner").GetComponent<Text>();

        autoSolveBtn = GameObject.Find("AutoSolve").GetComponent<Button>();
        //create the poles for reference
        poles = new Stack<DiscTouch>[hanoiTowers.Length];

        //init stack inside of each pole
        for (int i = 0; i < poles.Length; i++) {
            poles[i] = new Stack<DiscTouch>();
        }
        clearAndResetAllPoles();
    }

    void Update() {
        if (!gameOver) {
            winner.text = "";
            if (!auto) {
                if (Input.GetButtonDown("Fire1")) {
                    RaycastHit hitinfo;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hitinfo, 100, 1 << 8)) {
                        print(hitinfo.transform.gameObject.name);
                        if (isNotMakingMove) {
                            if (hitinfo.transform.CompareTag("Hanoi1") && poles[0].Count > 0) {
                                hitinfo.transform.GetComponentInChildren<GlowObject>().TriggerGlow();
                                selectedRealPole = hitinfo.transform.gameObject;
                                StartCoroutine(makeSubsequentMove(selectedRealPole, poles[0]));
                            } else if (hitinfo.transform.CompareTag("Hanoi2") && poles[1].Count > 0) {
                                hitinfo.transform.GetComponentInChildren<GlowObject>().TriggerGlow();
                                selectedRealPole = hitinfo.transform.gameObject;
                                StartCoroutine(makeSubsequentMove(selectedRealPole, poles[1]));
                            } else if (hitinfo.transform.CompareTag("Hanoi3") && poles[2].Count > 0) {
                                hitinfo.transform.GetComponentInChildren<GlowObject>().TriggerGlow();
                                selectedRealPole = hitinfo.transform.gameObject;
                                StartCoroutine(makeSubsequentMove(selectedRealPole, poles[2]));
                            } else if (hitinfo.transform.CompareTag("Hanoi4") && poles[3].Count > 0) {
                                hitinfo.transform.GetComponentInChildren<GlowObject>().TriggerGlow();
                                selectedRealPole = hitinfo.transform.gameObject;
                                StartCoroutine(makeSubsequentMove(selectedRealPole, poles[3]));
                            } else {
                                print("Invalid Move");
                            }
                        } else {
                            if (hitinfo.transform.CompareTag("Hanoi1")) {
                                selectedOtherPoleval = 0;
                                selectedOtherPole = hitinfo.transform.gameObject;
                                //StartCoroutine(makeSubsequentMove(selectedRealPole, poles[0]));
                            } else if (hitinfo.transform.CompareTag("Hanoi2")) {
                                selectedOtherPoleval = 1;
                                selectedOtherPole = hitinfo.transform.gameObject;
                            } else if (hitinfo.transform.CompareTag("Hanoi3")) {
                                selectedOtherPoleval = 2;
                                selectedOtherPole = hitinfo.transform.gameObject;
                            } else if (hitinfo.transform.CompareTag("Hanoi4")) {
                                selectedOtherPoleval = 3;
                                selectedOtherPole = hitinfo.transform.gameObject;
                            } else {
                                isNotMakingMove = true;
                                //selectedRealPole.GetComponent<GlowObject>().ExitGlow();
                            }
                            //first check to see if the stack even has something on top
                            //if it does then wait for player to make next move
                            //else return 
                            //now wait to select next move
                        }
                    }
                }
            }
        } else {
            winner.text = "Winner!";
        }
    }
    void checkState() {
        //for (int i = 0; i < discs.Length; i++) {
        //    if (poles[1].Contains(discs[i])) {
        //        tempChState = true;
        //    } else {
        //        break;
        //    }
        //}
        for (int i = 0; i < poles.Length; i++) {
            if (poles[i].Count >= discs.Length) {
                gameOver = true;
            }
        }
    }

    private IEnumerator makeSubsequentMove(GameObject tempSelection, Stack<DiscTouch> tempPole) {
        isNotMakingMove = false;
        StartCoroutine(chooseTheNextPole(tempSelection, tempPole));
        yield return new WaitForSeconds(3.0f);
        //end execution of function 
        selectedOtherPole = null;
        selectedOtherPoleval = 999;
        StopCoroutine(chooseTheNextPole(tempSelection, tempPole));
        tempSelection.GetComponentInChildren<GlowObject>().ExitGlow();
        isNotMakingMove = true;
    }

    private IEnumerator chooseTheNextPole(GameObject sel, Stack<DiscTouch> fromPole) {
        yield return new WaitUntil(() => selectedOtherPole != null && selectedOtherPoleval < 50);
        int tempToID = 100;
        DiscTouch tempFrom = fromPole.Peek();
        Stack<DiscTouch> tempToPole = poles[selectedOtherPoleval];
        if (tempToPole.Count > 0) {
            DiscTouch tempToDisc = tempToPole.Peek();
            tempToID = tempToDisc.valueID;
        }
        if (tempFrom.valueID < tempToID) {
            fromPole.Pop();
            print(selectedOtherPoleval);
            tempFrom.MoveDisc(selectedOtherPoleval + 1, 0.0f);
            poles[selectedOtherPoleval].Push(tempFrom);

            //cant do auto anymore
            if (autoSolveBtn.interactable) {
                autoSolveBtn.interactable = false;
            }
            //done move
            checkState();
        } else {
            sel.transform.GetComponentInChildren<GlowObject>().TriggerGlow(Color.red);
            print("You cannot put larger piece to the top of a smaller one");
        }
        //both cases are assumed to be true, launch the from -> to 
    }

    public void callSolve() {
        if (!autoRunReset) {
            auto = true;
            autoSolveBtn.interactable = false;
            StartCoroutine(SolveAllTasker());
        } else {
            clearAndResetAllPoles();
        }
    }

    private IEnumerator SolveAll(int n, Stack<DiscTouch> fr, int frInt, Stack<DiscTouch> to, int toInt, Stack<DiscTouch> spare, int spInt) {
        if (n == 1) {
            yield return new WaitForSeconds(1.0f);
            DiscTouch tempfrom = fr.Peek();
            fr.Pop();
            tempfrom.MoveDisc(toInt, 1.0f);
            to.Push(tempfrom);
            yield return new WaitForSeconds(1.0f);
        } else {
            yield return StartCoroutine(SolveAll(n - 1, fr, frInt, spare, spInt, to, toInt));
            yield return StartCoroutine(SolveAll(1, fr, frInt, to, toInt, spare, spInt));
            yield return StartCoroutine(SolveAll(n - 1, spare, spInt, to, toInt, fr, frInt));
        }
    }

    private IEnumerator SolveAllTasker() {
        yield return StartCoroutine(SolveAll(discs.Length, poles[0], 1, poles[1], 2, poles[2], 3));
        autoSolveBtn.interactable = true;
        gameOver = true;
        autoRunReset = true;
    }

    void clearAndResetAllPoles() {
        foreach (Stack<DiscTouch> pole in poles) {
            pole.Clear();
        }
        float increma = 0.0f;
        for (int i = discs.Length - 1; i >= 0; i--) {
            discs[i].MoveDisc(1, increma);
            increma += 0.5f;
        }
        autoRunReset = false;
        for (int i = discs.Length - 1; i >= 0; i--) {
            poles[0].Push(discs[i]);
        }
    }
}
