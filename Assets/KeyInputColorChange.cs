using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyInputColorChange : MonoBehaviour
{
    //�t�H���g�̐F���Ǘ�����X�N���v�g
    public UiController uiController;

    public TextMeshProUGUI texObject;

    //�F�̔z��
    public Color colors;

    void Start()
    {
        colors = Color.white; // �F�����ϐ����쐬
    }


    void Update()
    {
        ChangeFontColor();
        // �L�[���͂����o���ĐF��ύX
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
    // �t�H���g�̐F��ύX����֐�
    private void ChangeFontColor()
    {
          Color nextColor = colors;

          // �t�H���g�̐F��ύX
          uiController.ChangedFontColor(nextColor);
    }
}
