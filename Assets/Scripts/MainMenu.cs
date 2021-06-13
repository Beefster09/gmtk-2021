using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string scene;
    public Animator sceneChangeCircle;

    public void StartGame()
    {
        StartCoroutine(SceneChange());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator SceneChange()
    {
        sceneChangeCircle.SetTrigger("Change");
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(scene);
    }
}
