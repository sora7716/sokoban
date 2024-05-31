using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyInputColorChange : MonoBehaviour
{
    //フォントの色を管理するスクリプト
    public UiController uiController;

    public TextMeshProUGUI texObject;

    //色の配列
    public Color colors;

    void Start()
    {
        colors = Color.white; // 色を持つ変数を作成
    }


    void Update()
    {
        ChangeFontColor();
        // キー入力を検出して色を変更
        if (Input.GetKey(KeyCode.LeftArrow)&&texObject.tag=="LeftUI")
        {
            colors = Color.blue;
        }
        else if (Input.GetKey(KeyCode.RightArrow) && texObject.tag == "RightUI")
        {
            colors = Color.blue;
        
        }
        else if (Input.GetKey(KeyCode.UpArrow) && texObject.tag == "UpUI")
        {
            colors = Color.blue;
        }
        else if (Input.GetKey(KeyCode.DownArrow) && texObject.tag == "DownUI")
        {
            colors = Color.blue;
        }
        else
        {
            colors=Color.white;
        }
    }
    // フォントの色を変更する関数
    private void ChangeFontColor()
    {
          Color nextColor = colors;

          // フォントの色を変更
          uiController.ChangedFontColor(nextColor);
    }
}
