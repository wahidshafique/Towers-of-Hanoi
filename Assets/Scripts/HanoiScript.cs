using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanoiScript : MonoBehaviour {
    [SerializeField]
    DiscTouch disc1;
    [SerializeField]
    DiscTouch disc2;
    [SerializeField]
    DiscTouch disc3;

    private Stack<DiscTouch> pole1;
    private Stack<DiscTouch> pole2;
    private Stack<DiscTouch> pole3;

    void Start () {

        pole1 = new Stack<DiscTouch>();
        pole2 = new Stack<DiscTouch>();
        pole3 = new Stack<DiscTouch>();

        pole1.Push(disc3);
        pole1.Push(disc2);
        pole1.Push(disc1);

        print("top is : " + pole1.Peek());
        StartCoroutine(MakeMove(3, pole1, 1, pole2, 2, pole3, 3));
	}

    void Update() {

    }

    private IEnumerator MakeMove(int n, Stack<DiscTouch> fr, int frInt, Stack<DiscTouch> to, int toInt, Stack<DiscTouch> spare, int spInt) {
        if (n == 1) {
            yield return new WaitForSeconds(1.0f);
            //PrintMove(fr, to);
            DiscTouch tempfrom = fr.Peek();
            fr.Pop();
            tempfrom.MoveDisc(toInt);
            to.Push(tempfrom);
            yield return new WaitForSeconds(1.0f);
        } else {
            yield return StartCoroutine(MakeMove(n - 1, fr, 1, spare, 3, to, 2));
            yield return StartCoroutine(MakeMove(1, fr, 1, to, 2, spare, 3));
            yield return StartCoroutine(MakeMove(n - 1, spare, 3, to, 2, fr, 1));
        }
    }

    void PrintMove(string fr, string to) {
        print("move from " + fr + " to " + to);
    }
}
