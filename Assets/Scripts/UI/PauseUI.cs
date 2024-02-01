using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    [SerializeField] private GameObject PauseUI_gobj;
    [SerializeField] private GameObject Player;

    public static bool GameIsPaused { get; private set; }

    private void Start()
    {
        PauseUI_gobj.SetActive(false); // Disables Pause UI upon loading the scene
        Time.timeScale = 1.0f; // Resets the time scale back to normal, in case the player leaves in the pause menu
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // If the player presses the ESCAPE key...
        {
            if (!GameIsPaused) //If the game is not pauseed...
            {
                Pause(); // Pauses the game.
            }
            else
            {
                Resume(); // Unpauses the game.
            }
        }
    }

    public void Pause()
    {
        GameIsPaused = true; // Sets the boolean statement GameIsPaused to true.
        PauseUI_gobj.SetActive(true); // Enables the Pause UI so that it is visible
        Cursor.lockState = CursorLockMode.None; // Unlocks the Mouse so buttons can be selected
        Cursor.visible = true; // Makes the cursor visible
        
        //Player.GetComponent<Player>().enabled = false;
        
        Time.timeScale = 0f; // Freezes the state of the game
    }
    
    public void Resume()
    {
        GameIsPaused = false; // Sets the boolean statement GameIsPaused to false.
        PauseUI_gobj.SetActive(false); // Disables the Pause UI so that it is no longer visible
        Cursor.lockState = CursorLockMode.Locked; //Locks the cursor to the center of the screen
        Cursor.visible = false; // Makes the cursor invisible

        //Player.GetComponent<Player>().enabled = true;
        
        Time.timeScale = 1f; // Unfreezes the state of the game
    }
}

