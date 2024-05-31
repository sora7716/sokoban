using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearFontManager : MonoBehaviour
{
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeTime;

    private float fadeAlpha;
    private bool isFadeIn  = false;
    private bool isFadeOut = false;

    private void FadeIn()
    {
        fadeAlpha -= Time.deltaTime / fadeTime;
        if (fadeAlpha <= 0.0f)
        {
            fadeAlpha= 0.0f;
            isFadeIn = false;
        }
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, fadeAlpha);
    }

    private void FadeOut()
    {
        fadeAlpha += Time.deltaTime / fadeTime;
        if (fadeAlpha >= 1.0f)
        {
            fadeAlpha = 1.0f;
            isFadeOut = false;
        }
        fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, fadeAlpha);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            fadeAlpha = 1.0f;
            isFadeIn = true;
            isFadeOut = false;
        }
        else if (Input.GetMouseButton(1))
        {
            fadeAlpha = 0.0f;
            isFadeOut = true;
            isFadeIn = false;
        }
       
        if (isFadeIn)
        {
            FadeIn();
        }
        else if (isFadeOut)
        {
            FadeOut();
        }
    }
}
