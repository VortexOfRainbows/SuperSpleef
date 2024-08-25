using System;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    void Start()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
        Application.Quit();
    }
    /*
    void Start()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();

        StartCoroutine(ExitGame());
    }

    IEnumerator ExitGame()
    {
        yield return new WaitForEndOfFrame();
        Application.Quit();
    } 
    */
}
