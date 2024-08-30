using UnityEngine;

public class SkipToMainScene : MonoBehaviour
{
    private void Start()
    {
        Main.StartGame(GameModeID.Creative); // Loads the Main Scene (Gameplay Scene)
    }
}
