using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BossSceneManager : MonoBehaviour
{
    public GameObject[] BossPrefabs;
    public Vector2 BossPosition = new Vector2(0.0f, 3.15f);
    public Player Player;
    public Slider BossHPBar;
    public TMP_Text BigText;

    static private int bossIdx = 0;
    private Boss boss = null;
    private bool roundIsOver = false;

    public void BossDied()
    {
        if (roundIsOver) return;

        roundIsOver = true;
        BigText.text = "YOU WON\n(yippee!)";
        NextBoss();
    }

    public void PlayerDied()
    {
        roundIsOver = true;
        BigText.text = "YOU DIED\n(womp womp)";
        EndBossScene();
    }

    public void DisplayAmmo(int count)
    {
        AmmoCounter.SetCount(count);
    }

    public void DisplayBossHealth(float normalizedHealth)
    {
        BossHPBar.normalizedValue = normalizedHealth;
    }

    private void Start()
    {
        bossIdx = Mathf.Clamp(bossIdx, 0, BossPrefabs.Length - 1);
        
        GameObject bossObject = Instantiate(BossPrefabs[bossIdx], BossPosition, Quaternion.identity);
        boss = bossObject.GetComponent<Boss>();
        boss.manager = this;

        Player.manager = this;
    }

    private void Update()
    {
        
    }

    private void NextBoss()
    {
        if (bossIdx < BossPrefabs.Length - 1)
        {
            bossIdx++;
            StartCoroutine(ReloadScene(4));
        }
        else
        {
            EndBossScene();
        }
    }

    private void EndBossScene()
    {
        bossIdx = 0;
        StartCoroutine(LoadMenu(4));
    }

    private IEnumerator ReloadScene(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadMenu(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        SceneManager.LoadScene("MenuScene");
    }
}
