using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    [SerializeField] private GameObject PauseUI_gobj;
    [SerializeField] private GameObject Player;
    private void Start()
    {
        PauseUI_gobj.SetActive(false); // Disables Pause UI upon loading the scene
    }
    void Update()
    {
        PauseUI_gobj.SetActive(GameStateManager.GameIsPaused); // Enables the Pause UI so that it is visible
        if (Input.GetKeyDown(KeyCode.Escape)) // If the player presses the ESCAPE key...
        {
            if(!GameStateManager.GameIsOver) //Cannot pause if the game is over
            {
                if (!GameStateManager.GameIsPaused) //If the game is not pauseed...
                {
                    Pause(); // Pauses the game.
                }
                else
                {
                    Resume(); // Unpauses the game.
                }
            }
        }
    }
    public void Pause()
    {
        GameStateManager.Pause();
    }
    public void Resume()
    {
        GameStateManager.Unpause();
    }
}

