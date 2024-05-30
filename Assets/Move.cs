using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Move : MonoBehaviour
{
    //�����܂łɂ����鎞��
    public float duration = 0.2f;
    //�o�ߎ���
    float elapsedTime;
    //�ړI�n
    Vector3 destination;
    //�o���n�_
    Vector3 origin;

    public void MoveTo(Vector3 destination)
    {
        elapsedTime = 0;
        origin = this.destination;
        // �ړ����������ꍇ�̓L�����Z�����ĖړI�Ƀ��[�v����
        transform.position = origin;
        this.destination = destination;
    }

    void Start()
    {
        destination = transform.position;
        origin = destination;
    }

    void Update()
    {
        if (origin == destination) 
        {
            return;
        }
        elapsedTime += Time.deltaTime;
        float timeRate = elapsedTime / duration;
        
        if (timeRate > 1) { timeRate = 1; }
        Vector3 currentPositon = Vector3.Lerp(origin, destination, timeRate);
        transform.position = currentPositon;
    }
}
