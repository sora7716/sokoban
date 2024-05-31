using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiController : MonoBehaviour
{
    //テキストオブジェクト
    public TextMeshProUGUI textObject;
   
    public void ChangedFontColor(Color color)
    {
        textObject.color = color;
    }
    void Start()
    {
        
    }

  
    void Update()
    {
        
    }
}
