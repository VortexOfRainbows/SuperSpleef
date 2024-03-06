using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerText : MonoBehaviour
{
    [SerializeField] private Text LoggedDisplay;
    private void Update()
    {
        if (LoggedDisplay != null)
            LoggedDisplay.text = NetHandler.TotalClients.ToString();
    }
}
