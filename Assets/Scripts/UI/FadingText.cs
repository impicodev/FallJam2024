using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.U2D;

public class FadingText : MonoBehaviour
{
    private TMP_Text text = null;
    
    private void Start()
    {
        text = GetComponent<TMP_Text>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
    }

    public void SetText(string val)
    {
        text.text = val;
    }

    public void SetColor(Color val)
    {
        text.color = val;
    }

    public void MakeFullyTransparent()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
    }

    public void MakeFullyVisible()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
    }

    public void FadeOut(float duration)
    {
    
    }

    public void Flicker()
    {
        StartCoroutine(FlashText(1.5f));
    }

    public void FadeInOut(float startDelay, float durationIn, float durationOut)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(startDelay);
        seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(text.color.r, text.color.g, text.color.b, 1.0f), durationIn));
        seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(text.color.r, text.color.g, text.color.b, 0.0f), durationOut));
    }

    private IEnumerator FlashText(float seconds)
    {
        int flashTimes = 3;
        for (int i = 0; i < flashTimes; i++)
        {
            MakeFullyTransparent();
            yield return new WaitForSecondsRealtime(seconds / (flashTimes * 2));
            MakeFullyVisible();
            yield return new WaitForSecondsRealtime(seconds / (flashTimes * 2));
        }
    }
}
