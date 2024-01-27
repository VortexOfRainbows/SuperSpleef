using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public GameObject PauseUI_gobj;

    public bool GameIsPaused = false;

    private void Start()
    {
        PauseUI_gobj.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameIsPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        if (GameIsPaused == true) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    public void Pause()
    {
        GameIsPaused = true;
        Time.timeScale = 0f;
        PauseUI_gobj.SetActive(true);
    }

    public void Resume()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        PauseUI_gobj.SetActive(false);
    }
}

