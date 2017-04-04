using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class HanoiScript : MonoBehaviour {
    Button autoSolveBtn;
    [SerializeField]
    DiscTouch disc1;
    [SerializeField]
    DiscTouch disc2;
    [SerializeField]
    DiscTouch disc3;

    private Stack<DiscTouch> pole1;
    private Stack<DiscTouch> pole2;
    private Stack<DiscTouch> pole3;
    private Stack<DiscTouch>[] poles;

    private bool autoRunReset = false;

    void Start() {
        poles = new Stack<DiscTouch>[3];
        autoSolveBtn = GameObject.Find("AutoSolve").GetComponent<Button>();

        poles[0] = new Stack<DiscTouch>();
        poles[1] = new Stack<DiscTouch>();
        poles[2] = new Stack<DiscTouch>();

        clearAndResetAllPoles(false);
    }

    void Update() {
    }

    public void callSolve() {
        if (!autoRunReset) {
            autoSolveBtn.interactable = false;
            StartCoroutine(SolveAllTasker());
        } else {
            clearAndResetAllPoles(false);
        }
    }

    private IEnumerator SolveAll(int n, Stack<DiscTouch> fr, int frInt, Stack<DiscTouch> to, int toInt, Stack<DiscTouch> spare, int spInt) {
        if (n == 1) {
            yield return new WaitForSeconds(1.0f);
            DiscTouch tempfrom = fr.Peek();
            fr.Pop();
            tempfrom.MoveDisc(toInt, 0.0f);
            to.Push(tempfrom);
            yield return new WaitForSeconds(1.0f);
        } else {
            yield return StartCoroutine(SolveAll(n - 1, fr, frInt, spare, spInt, to, toInt));
            yield return StartCoroutine(SolveAll(1, fr, frInt, to, toInt, spare, spInt));
            yield return StartCoroutine(SolveAll(n - 1, spare, spInt, to, toInt, fr, frInt));
        }
    }

    private IEnumerator SolveAllTasker() {
        yield return StartCoroutine(SolveAll(3, poles[0], 1, poles[1], 2, poles[2], 3));
        autoSolveBtn.interactable = true;
        autoRunReset = true;
    }

    void clearAndResetAllPoles(bool isFirstRun) {
        foreach (Stack<DiscTouch> pole in poles) {
            pole.Clear();
        }
        if (!isFirstRun) {
            disc3.MoveDisc(1, 0.0f);
            disc2.MoveDisc(1, 1.0f);
            disc1.MoveDisc(1, 2.0f);
        }

        autoRunReset = false;
        poles[0].Push(disc3);
        poles[0].Push(disc2);
        poles[0].Push(disc1);
    }

    void PrintMove(string fr, string to) {
        print("move from " + fr + " to " + to);
    }
}
