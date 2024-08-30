using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour ///Team members that contributed to this script: Samuel Gines, Ian Bunnell
{ 
    public void ModifyGameOverText(string str, Color color)
    {
        gameOverSubscript.text = str;
        gameOverSubscript.color = color;
    }
    [SerializeField] private GameObject PauseUI_gobj;
    [SerializeField] private GameObject GameOverUI; // Gameobject which encompasses all objects related to the Game Over UI
    //[SerializeField] private GameObject GameplayUI; // Gameobject which encompasses all objects related to the Gameplay UI
    [SerializeField] private Text gameOverSubscript;
    private void Start()
    {
        PauseUI_gobj.SetActive(false); // Disables Pause UI upon loading the scene
    }
    void Update()
    {
        if(Main.GameIsOver)
        {
            ModifyGameOverText(Main.GameOverText, Main.GameOverTextColor);
        }
        GameOverUI.SetActive(Main.GameIsOver); // Make the Game Over UI visible
        //GameplayUI.SetActive(); // Make the Gameplay UI invisible
        PauseUI_gobj.SetActive(Main.GameIsPaused); // Enables the Pause UI so that it is visible
        if (Input.GetKeyDown(KeyCode.Escape)) // If the player presses the ESCAPE key...
        {
            if(!Main.GameIsOver) //Cannot pause if the game is over
            {
                if (!Main.GameIsPaused) //If the game is not pauseed...
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
        Main.Pause();
    }
    public void Resume()
    {
        Main.Unpause();
    }
}

