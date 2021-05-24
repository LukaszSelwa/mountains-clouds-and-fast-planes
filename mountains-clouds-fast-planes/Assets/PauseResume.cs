using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseResume : MonoBehaviour
{
    bool isRunning = false;

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (isRunning)
            {
                isRunning = false;
                PauseGame();
            }
            else
            {
                ResumeGame();
                isRunning = true;
            }
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}
