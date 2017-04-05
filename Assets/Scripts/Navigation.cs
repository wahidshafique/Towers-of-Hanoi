using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour {
    public void Play() {
        SceneManager.LoadScene(Application.loadedLevel);
    }
    public void Easy() {
        SceneManager.LoadScene("Easy");
    }
    public void Medium() {
        SceneManager.LoadScene("Medium");
    }
    public void Hard() {
        SceneManager.LoadScene("Hard");
    }

    public void Return() {
        SceneManager.LoadScene("Menu");
    }

    public void Quit() {
        Application.Quit();
    }
}
