using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BossSceneManager : MonoBehaviour
{
    public GameObject[] BossPrefabs;
    public GameObject defeat1, defeat2, defeat3, defeat4, dark;
    public Vector2 BossPosition = new Vector2(0.0f, 3.15f);
    public Player Player;
    public Slider BossHPBar;
    public Slider BossHPBar2;
    public Slider PlayerHPBar;
    public GameObject AmmoUIObject;
    public FadingText BigText;
    public FadingText BossNameText;
    public TutorialPanel tutorial;

    private AudioSource audioSource;
    public AudioClip victorySound;

    static public int bossIdx = 0;
    static private bool tutorialClosed = false;
    static private bool UIEntered = false;
    private Boss boss = null;
    private bool roundIsOver = false;
    private bool updateBars = false;

    public void BossDied()
    {
        if (roundIsOver) return;

        roundIsOver = true;

        DOTween.To(() => Soundtrack.Instance.src.volume, x => Soundtrack.Instance.src.volume = x, 0.15f, 0.2f);
        audioSource.PlayDelayed(0.5f);

        BigText.SetText("BOUNTY SECURED");
        BigText.SetColor(new Color(0.95f, 0.83f, 0.5f, 1.0f));
        BigText.MakeFullyVisible();
        BigText.Flicker();

        Minion.SetFrozen(true);
        Player.animator.SetBool("Walking", false);
        Player.Freeze();

        NextBoss();
    }

    public void PlayerDied()
    {
        if (roundIsOver) return;

        roundIsOver = true;

        int r = Random.Range(0, 4);
        dark.SetActive(true);
        if (r == 0)
            defeat1.SetActive(true);
        else if (r == 1)
            defeat2.SetActive(true);
        else if (r == 2)
            defeat3.SetActive(true);
        else
            defeat4.SetActive(true);
        /*BigText.SetText("LIFE RUINED");
        BigText.SetColor(new Color(0.83f, 0.63f, 0.63f, 1.0f));
        BigText.MakeFullyVisible();
        BigText.Flicker();*/

        Minion.SetFrozen(true);

        //EndBossScene();
        ResetBoss();
    }

    public void DisplayAmmo(int count)
    {
        AmmoCounter.SetCount(count);
    }

    public void DisplayBossHealth(float normalizedHealth)
    {
        if (!updateBars) return;
        
        if (BossHPBar.normalizedValue <= normalizedHealth) {
            DOTween.To(() => BossHPBar.normalizedValue, x => BossHPBar.normalizedValue = x, normalizedHealth, 0.1f);
        }
        else {
            BossHPBar2.normalizedValue = BossHPBar.normalizedValue;
            BossHPBar.normalizedValue = normalizedHealth;
            DOTween.To(() => BossHPBar2.normalizedValue, x => BossHPBar2.normalizedValue = x, normalizedHealth, 1.2f).SetEase(Ease.OutSine);
            boss.FlashHurt();
        }
    }

    public void DisplayPlayerHealth(float normalizedHealth)
    {
        if (!updateBars) return;
        
        DOTween.To(() => PlayerHPBar.normalizedValue, x => PlayerHPBar.normalizedValue = x, normalizedHealth, 0.05f);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = victorySound;

        Minion.SetFrozen(false);
        
        bossIdx = Mathf.Clamp(bossIdx, 0, BossPrefabs.Length - 1);

        GameObject bossObject = Instantiate(BossPrefabs[bossIdx], BossPosition, Quaternion.identity);
        boss = bossObject.GetComponent<Boss>();
        boss.manager = this;

        Player.manager = this;

        if (!UIEntered)
            HideUI();

        if (tutorialClosed)
        {
            if (!UIEntered)
            {
                ShowUI();
                UIEntered = true;
            }

            tutorial.gameObject.SetActive(false);
            BeginRound();
        }
    }

    private void Update()
    {
        if (!tutorialClosed && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        { 
            tutorial.FadeOut();
            tutorialClosed = true;
            
            ShowUI();
            UIEntered = true;
            
            BeginRound();
        }

        if (BossHPBar.normalizedValue > 0.0f)
            updateBars = true;
    }

    private void HideUI()
    {
        RectTransform ammoRect = AmmoUIObject.GetComponent<RectTransform>();
        RectTransform playerHPRect = PlayerHPBar.gameObject.GetComponent<RectTransform>();
        RectTransform bossHPRect = BossHPBar.gameObject.GetComponent<RectTransform>();

        ammoRect.anchoredPosition = new Vector2(ammoRect.anchoredPosition.x, -90.0f);
        playerHPRect.anchoredPosition = new Vector2(-218.0f, playerHPRect.anchoredPosition.y);
        bossHPRect.anchoredPosition = new Vector2(bossHPRect.anchoredPosition.x, 40.0f);
    }

    private void ShowUI()
    {
        RectTransform ammoRect = AmmoUIObject.GetComponent<RectTransform>();
        RectTransform playerHPRect = PlayerHPBar.gameObject.GetComponent<RectTransform>();
        RectTransform bossHPRect = BossHPBar.gameObject.GetComponent<RectTransform>();

        DOTween.To(() => ammoRect.anchoredPosition, x => ammoRect.anchoredPosition = x, new Vector2(ammoRect.anchoredPosition.x, 90.0f), 0.75f);
        DOTween.To(() => playerHPRect.anchoredPosition, x => playerHPRect.anchoredPosition = x, new Vector2(218.0f, playerHPRect.anchoredPosition.y), 0.75f);
        DOTween.To(() => bossHPRect.anchoredPosition, x => bossHPRect.anchoredPosition = x, new Vector2(bossHPRect.anchoredPosition.x, -40.0f), 0.75f);
    }

    private void FillBars()
    {
        BossHPBar2.normalizedValue = 0.0f;
        BossHPBar.normalizedValue = 0.0f;
        DOTween.To(() => BossHPBar.normalizedValue, x => BossHPBar.normalizedValue = x, 1.0f, 1.0f);

        PlayerHPBar.normalizedValue = 0.0f;
        DOTween.To(() => PlayerHPBar.normalizedValue, x => PlayerHPBar.normalizedValue = x, 1.0f, 1.0f);
    }

    private void BeginRound()
    {
        FillBars();
        
        BossNameText.SetText(boss.Name);
        BossNameText.FadeInOut(1.0f, 1.0f, 2.0f);

        boss.BeginAttacking();
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
            EndGame();
        }
    }

    private void ResetBoss()
    {
        StartCoroutine(ReloadScene(4));
    }

    private void EndBossScene()
    {
        UIEntered = false;
        bossIdx = 0;
        StartCoroutine(LoadMenu(4));
    }

    private void EndGame()
    {
        UIEntered = false;
        bossIdx = 0;
        StartCoroutine(LoadEnd(4));
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

    private IEnumerator LoadEnd(float wait)
    {
        yield return new WaitForSecondsRealtime(wait);
        SceneManager.LoadScene("EndScene");
    }
}
