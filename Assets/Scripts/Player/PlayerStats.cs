using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

public class PlayerStats : MonoBehaviour ///Team members that contributed to this script: Samuel Gines
{
    [SerializeField] private int maxPlayerHP; // Assigns the Max HP of the player
    [SerializeField] private int currentPlayerHP; // Assigns the current HP of the character

    [SerializeField] private GameObject GameOverUI; // Gameobject which encompasses all objects related to the Game Over UI
    [SerializeField] private GameObject GameplayUI; // Gameobject which encompasses all objects related to the Gameplay UI
    [SerializeField] private GameObject ClientPackage;

    private float timeCount = 0.0f; // Artifical Stopwatch

    public Rigidbody rb; // The rigidbody of the player gameobject

    private void Start()
    {
        currentPlayerHP = maxPlayerHP; // sets current HP to max HP upon loading the scene
        GameOverUI.SetActive(false); // Disambes Game Over UI upon loading scene
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && Time.timeScale == 1) // If the "P" key is pressed, and the game is not forzen (aka paused)...
        {
            currentPlayerHP -= maxPlayerHP; // Deal damage equal to the player's max HP to the player
            rb.AddForce(transform.up * 150f); // Add a force to the player in the upwards direction
            //Debug.Log(currentPlayerHP);
        }

        if (currentPlayerHP <= 0f || transform.position.y < World.OutOfBounds) // If the player's current HP reaches zero, or if the player falls too far down the world...
        {
            OnDeath(); // Trigger the Death Behavior of the character
            timeCount = timeCount + Time.deltaTime; // Start the stopwatch
        }
    }

    private void OnDeath()
    {
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(0,0,0), Quaternion.Euler(0, 0, 90), timeCount * timeCount * 9); // Interpolate the player and Rotate the character 90 degrees
        GameOverUI.SetActive(true); // Make the Game Over UI visible
        GameplayUI.SetActive(false); // Make the Gameplay UI invisible

       
        gameObject.GetComponent<Player>().enabled = false; // Dsable Player Controls
        
        Cursor.lockState = CursorLockMode.None; // Unlock the mouse
        Cursor.visible = true; // Make the mouse visible
    }

}
