using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Move : MonoBehaviour
{
    //完了までにかかる時間
    public float duration = 0.2f;
    //経過時間
    float elapsedTime;
    //目的地
    Vector3 destination;
    //出発地点
    Vector3 origin;

    public void MoveTo(Vector3 destination)
    {
        elapsedTime = 0;
        origin = this.destination;
        // 移動中だった場合はキャンセルして目的にワープする
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
