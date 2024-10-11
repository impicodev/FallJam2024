using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    public static int Count = 0;

    public int index = 0;
    private Image image = null;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (index >= Count)
            image.color = Color.grey;
        else
            image.color = Color.white;
    }
}
