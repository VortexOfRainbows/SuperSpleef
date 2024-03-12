using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;
using System;

[Obsolete] 
///functionality is now part of the player class
public class PlayerStats : MonoBehaviour ///Team members that contributed to this script: Samuel Gines, Ian Bunnell
{
    public const float DamageFromVoid = 200f;
    private const float maxPlayerHP = 100; // Assigns the Max HP of the player
    private float currentPlayerHP = maxPlayerHP; // Assigns the current HP of the character
    //[SerializeField] private GameObject ClientPackage;
    [SerializeField] private Player player;
    [SerializeField] private Rigidbody rb; // The rigidbody of the player gameobject
    private void Start()
    {
        currentPlayerHP = maxPlayerHP; // sets current HP to max HP upon loading the scene
    }
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.P) && Time.timeScale == 1) // If the "P" key is pressed, and the game is not forzen (aka paused)...
        {
            currentPlayerHP -= maxPlayerHP; // Deal damage equal to the player's max HP to the player
            rb.AddForce(transform.up * 150f); // Add a force to the player in the upwards direction
            //Debug.Log(currentPlayerHP);
        }*/
        if (currentPlayerHP <= 0f) // If the player's current HP reaches zero, or if the player falls too far down the world...
        {
            OnDeath(); // Trigger the Death Behavior of the character
        }
        else
        {
            if (transform.position.y < World.OutOfBounds)
            {
                currentPlayerHP -= DamageFromVoid * Time.deltaTime;
            }
        }
    }
    private float deathAnimTimer = 0.0f; // Artifical Stopwatch
    private void OnDeath()
    {
        deathAnimTimer += Time.deltaTime; // Start the stopwatch
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(0,0,0), Quaternion.Euler(0, 0, 90), deathAnimTimer * deathAnimTimer * 9); // Interpolate the player and Rotate the character 90 degrees
        if(Time.timeScale >= 1)
        {
            //Debug.Log(currentPlayerHP);
            string DeathText = GameStateManager.DefaultGameOverText;
            Color deathColor = Color.white;
            if(GameStateManager.LocalMultiplayer)
            {
                if(player.ControlManager.UsingGamepad)
                {
                    //blue player uses gamepad
                    DeathText = "Yellow Wins";
                    deathColor = Color.yellow;
                }
                else
                {
                    //yellow player uses gamepad
                    DeathText = "Blue Wins";
                    deathColor = Color.blue;
                }
            }
            GameStateManager.EndGame(DeathText, deathColor);
        }
    }
}
