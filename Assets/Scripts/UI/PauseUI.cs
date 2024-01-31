using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public GameObject PauseUI_gobj;
    public GameObject Player;

    public bool GameIsPaused = false;

    private void Start()
    {
        PauseUI_gobj.SetActive(false);
        Time.timeScale = 1.0f;
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
    }

    public void Pause()
    {
        GameIsPaused = true;
        PauseUI_gobj.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Player.GetComponent<Player>().enabled = false;
        Time.timeScale = 0f;
    }
    
    public void Resume()
    {
        GameIsPaused = false;
        PauseUI_gobj.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Player.GetComponent<Player>().enabled = true;
        Time.timeScale = 1f;
    }
}

