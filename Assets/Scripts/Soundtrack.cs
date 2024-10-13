using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Soundtrack : MonoBehaviour
{
    public static Soundtrack Instance;
    public AudioClip finalBossClip;
    public AudioClip regularBossClip;

    [System.NonSerialized] public AudioSource src;

    void Start() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            src = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DOTween.To(() => Soundtrack.Instance.src.volume, x => Soundtrack.Instance.src.volume = x, 0.5f, 0.2f);
        if (BossSceneManager.bossIdx == 2) {
            src.clip = finalBossClip;
            src.Play();
        }
        else if (src.clip != regularBossClip) {
            src.clip = regularBossClip;
            src.Play();
        }
    }
}
