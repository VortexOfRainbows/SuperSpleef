using UnityEngine;

public class SkipToMainScene : MonoBehaviour
{
    private void Start()
    {
        GameStateManager.StartGame(GameModeID.Creative); // Loads the Main Scene (Gameplay Scene)
    }
}
