using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour {
    public void Play() {
        SceneManager.LoadScene("Main");
    }

    public void Return() {
        SceneManager.LoadScene("Menu");
    }

    public void Quit() {
        Application.Quit();
    }
}
