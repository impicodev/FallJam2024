using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AmmoCounter : MonoBehaviour
{
    private static int count = 0;

    public int index = 0;
    private Image image = null;

    public static void SetCount(int value)
    {
        count = value;
    }

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (index >= count)
            image.color = Color.grey;
        else
            image.color = Color.white;
    }
}
