using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    //”z—ñ‚ÌéŒ¾
    int[] map;
    // Start is called before the first frame update
    void Start()
    {
        map = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, };

        /*Debug.Log("Hello,World");*/

        string debugText = "";
        for (int i = 0; i < map.Length; i++)
        {
            //•ÏXB•¶š—ñ‚ÉŒ‹‡‚µ‚Ä‚¢‚­
            debugText += map[i].ToString() + ",";
        }
        Debug.Log(debugText);
    }

    // Update is called once per frame
    void Update()
    {
        int playerIndex = -1;

        string debugText = "";

        bool isDebugText = false;

        for (int i = 0; i < map.Length; i++)
        {
            if (map[i] == 1)
            {
                playerIndex = i;
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (playerIndex < map.Length - 1)
            {
                map[playerIndex+1] = 1;
                map[playerIndex] = 0;
            }
            isDebugText = true;
        }
       else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (playerIndex < map.Length&&playerIndex != 0)
            {
                map[playerIndex - 1] = 1;
                map[playerIndex] = 0;
            }
            isDebugText = true;
        }

        if (isDebugText)
        {
            DebugText(debugText);
        }
        void DebugText(string debugText)
        {
            for (int i = 0; i < map.Length; i++)
            {
                debugText += map[i].ToString() + ",";
            }
            Debug.Log(debugText);
        }

    }
}
