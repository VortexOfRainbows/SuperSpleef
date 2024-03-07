using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance { get; private set; }
    public static Camera Camera => Instance.MainCamera;
    public static ScreenBlocker Blocker => Instance.ScreenBlocker;
    public static GameObject Outline => Instance.BlockOutline;
    [SerializeField] private Camera MainCamera;
    [SerializeField] private ScreenBlocker ScreenBlocker;
    [SerializeField] private GameObject BlockOutline;
    [SerializeField] private string ApocalypseText;
    [SerializeField] private string CreativeText;
    [SerializeField] private string MultiplayerText;
    [SerializeField] private Text BelowTimerText;
    public void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        //Instance = this;
        if (GameStateManager.LocalMultiplayer)
            BelowTimerText.text = MultiplayerText;
        else if (GameStateManager.Mode == GameModeID.Apocalypse)
            BelowTimerText.text = ApocalypseText;
        else if(GameStateManager.Mode == GameModeID.Creative)
            BelowTimerText.text = CreativeText;
    }
    public void Update()
    {
        //Instance = this;
    }
}
