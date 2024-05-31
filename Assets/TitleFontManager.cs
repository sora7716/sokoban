using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleFontManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleFont;

    IEnumerator FadeIn()
    {
        titleFont.text = "sokoban";
        while (true)
        {
            for (int i = 0; i < 255; i++)
            {
                titleFont.color = titleFont.color + new Color32(0, 0, 0, 1);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    void Start()
    {
        titleFont.text = "";
        titleFont.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        StartCoroutine("FadeIn");
    }

   
    void Update()
    {
        
    }
}
