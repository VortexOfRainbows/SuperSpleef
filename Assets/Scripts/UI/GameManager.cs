using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete] ///Currently unused
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public bool gameIsPlaying;
    public bool gameIsPaused;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        StartCoroutine(RoundPreparing());
        StartCoroutine(RoundPlaying());
        StartCoroutine(RoundEnding());
        
        yield return null;
    }

    private IEnumerator RoundPreparing()
    {
        Debug.Log("Game is Preparing");
        yield return null;
    }

    private IEnumerator RoundPlaying()
    {
        Debug.Log("Game is Playing");
        
        while (true) 
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        Debug.Log("Game has ended");
        yield return null;
    }

    private bool onePlayerLeft() 
    {
        return false;
    }
}
