using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialPanel : MonoBehaviour
{
    public TMP_Text Text1;
    public TMP_Text Text2;
    public float TextFadeDuration = 2.0f;
    public float PanelFadeDuration = 1.0f;

    public void FadeOut()
    {
        Image img = GetComponent<Image>();

        DOTween.To(() => img.color, x => img.color = x, new Color(img.color.r, img.color.g, img.color.b, 0.0f), PanelFadeDuration);
        DOTween.To(() => Text1.color, x => Text1.color = x, new Color(Text1.color.r, Text1.color.g, Text1.color.b, 0.0f), TextFadeDuration);
        DOTween.To(() => Text2.color, x => Text2.color = x, new Color(Text2.color.r, Text2.color.g, Text2.color.b, 0.0f), TextFadeDuration);
    }
}
