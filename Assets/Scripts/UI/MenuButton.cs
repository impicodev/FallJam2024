using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(LoadBossScene());
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator LoadBossScene()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("BossScene");

        while (!load.isDone) yield return null;
    }
}
