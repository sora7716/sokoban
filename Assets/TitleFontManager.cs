using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleFontManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleFont;

    public IEnumerator FadeIn()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Title")  // シーン1の名前を確認します
        {
            titleFont.text = "Sokoban";
        }
        else
        {
            titleFont.text = "GameClear";
        }
        while (true)
        {
            for (int i = 0; i < 255; i++)
            {
                titleFont.color = titleFont.color + new Color(0, 0, 0, 1.0f/255);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        titleFont.text = "";
        if (sceneName == "Title")  // シーン1の名前を確認します
        {
            titleFont.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
        }
        else
        {
            titleFont.color = new Color(0.0f, 1.0f, 0.0f, 0.0f);
        }
       
       StartCoroutine("FadeIn");
    }

    

    void Update()
    {
        
    }
}
