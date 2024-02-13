using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    [SerializeField] private string ApocalypseText;
    [SerializeField] private string CreativeText;
    [SerializeField] private string MultiplayerText;
    [SerializeField] private Text BelowTimerText;
    public void Start()
    {
        if(GameStateManager.Mode == GameModeID.Apocalypse)
            BelowTimerText.text = ApocalypseText;
        if (GameStateManager.Mode == GameModeID.Creative)
            BelowTimerText.text = CreativeText;
        else
            BelowTimerText.text = MultiplayerText;
    }
}
