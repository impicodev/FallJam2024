using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.U2D;

public class EndScreenUI : MonoBehaviour
{
    public float StartDelay = 0.0f;
    public float FadeInDuration = 1.0f;
    public float ChangeColorDuration = 1.0f;
    public bool ChangeColors = false;

    private TMP_Text text = null;
    private float delayTime = 0.0f;

    private void Appear()
    {
        Sequence seq = DOTween.Sequence();

        if (ChangeColors)
        {
            seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(Color.red.r, Color.red.g, Color.red.b, 1.0f), FadeInDuration));
            seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 1.0f), FadeInDuration));
            seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(Color.green.r, Color.green.g, Color.green.b, 1.0f), FadeInDuration));
            seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 1.0f), FadeInDuration));
            seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 1.0f), FadeInDuration));
            seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(Color.blue.r, Color.blue.g, Color.blue.b, 1.0f), FadeInDuration));
            seq.SetLoops(-1);
        }
        else
        {
            seq.Append(DOTween.To(() => text.color, x => text.color = x, new Color(text.color.r, text.color.g, text.color.b, 1.0f), FadeInDuration));
        }
    }

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0.0f);
    }

    private void Update()
    {
        if (delayTime <= StartDelay)
        {
            delayTime += Time.deltaTime;
            if (delayTime > StartDelay)
                Appear();
        }
    }
}
