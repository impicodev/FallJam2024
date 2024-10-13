using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickSceneTransition : MonoBehaviour
{
    public float AllowClickAfter = 4.0f;

    private float timeElapsed = 0.0f;

    void Update()
    {
        if (timeElapsed < AllowClickAfter)
        {
            timeElapsed += Time.deltaTime;
        }
        else
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                StartCoroutine(LoadMenuScene());
            }
        }
    }

    IEnumerator LoadMenuScene()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("MenuScene");

        while (!load.isDone) yield return null;
    }
}