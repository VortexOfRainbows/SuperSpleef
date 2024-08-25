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
}
