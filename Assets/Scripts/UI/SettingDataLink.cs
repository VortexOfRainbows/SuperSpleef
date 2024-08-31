using TMPro;
using UnityEngine;

public class SettingDataLink : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private TextMeshProUGUI text;
    private SaveData<int> data => (SaveData<int>)ClientData.Dict[Key];
    private int currentData = 0;
    private void LinkToSetting()
    {
        if(data.Value != currentData)
        {
            currentData = data.Value;
            if(Key == "WorldType")
            {
                if (currentData == -1)
                {
                    text.text = "RANDOM";
                }
                if (currentData == 0)
                {
                    text.text = "FOREST";
                }
                if(currentData == 1)
                {
                    text.text = "DESERT";
                }
            }
        }
    }
    private void Update()
    {
        LinkToSetting();   
    }
}
