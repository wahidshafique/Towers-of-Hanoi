using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanoiScript : MonoBehaviour {

	void Start () {
        //Towers(2, "f", "t", "s");
        StartCoroutine(MakeMove(3, "f", "t", "s"));
	}

    void Update() {

    }
    private IEnumerator MakeMove(int n, string fr, string to, string spare) {
        if (n == 1) {
            PrintMove(fr, to);
            yield return new WaitForSeconds(1.0f);
        } else {
            yield return StartCoroutine(MakeMove(n - 1, fr, spare, to));
            yield return StartCoroutine(MakeMove(1, fr, to, spare));
            yield return StartCoroutine(MakeMove(n - 1, spare, to, fr));
        }
    }

    void PrintMove(string fr, string to) {
        print("move from " + fr + " to " + to);
    }
}
