using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseResume : MonoBehaviour
{
    private bool _isRunning = true;
    public bool IsRunning
    {
        get => _isRunning;

        private set
        {
            _isRunning = value;
            if (_isRunning)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IsRunning = !IsRunning;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
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
