using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int maxPlayerHP;
    [SerializeField] private int currentPlayerHP;

    public Text GameOver;

    public GameObject GameOverUI;
    public GameObject GameplayUI;
    public GameObject ClientPackage;
    public Transform deathPlane;

    private float timeCount = 0.0f;

    public Rigidbody rb;

    private void Start()
    {
        currentPlayerHP = maxPlayerHP;
        GameOverUI.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            currentPlayerHP -= maxPlayerHP;
            rb.AddForce(transform.up * 150f);
            //Debug.Log(currentPlayerHP);
        }

        if (currentPlayerHP <= 0f || transform.position.y < deathPlane.position.y) 
        {
            OnDeath();
            timeCount = timeCount + Time.deltaTime;
        }
    }

    private void OnDeath()
    {
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(0,0,0), Quaternion.Euler(0, 0, 90), timeCount * timeCount * 9);
        GameOverUI.SetActive(true);
        GameplayUI.SetActive(false);

        // This would also be the play to disable controls
        gameObject.GetComponent<Player>().enabled = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
