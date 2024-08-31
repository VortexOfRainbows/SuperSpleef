using UnityEngine;
using UnityEngine.UI;

public class SettingToggle : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private Text DisplayName;
    [SerializeField] private Toggle Toggle;
    private SaveData<int> data => (SaveData<int>)ClientData.Dict[Key];
    private void LinkToSetting()
    {
        bool valid = true;
        if (Key == ClientData.WorldGenChaos.Key)
        {
            valid = false;
            if (FallingCube.BestBlocksBroken >= 50)
            {
                valid = true;
            }
        }
        if (!valid)
            data.WriteValue(0);

        if (valid)
            DisplayName.text = data.DisplayName;
        else
        {
            DisplayName.text = "LOCKED";
        }

        bool set = data.Value > 0 && valid;
        if (Toggle.isOn != set)
            Toggle.isOn = set;
    }
    private void Start()
    {
        LinkToSetting();
    }
    public void SetData(bool IsOn)
    {
        bool valid = true;
        if(Key == ClientData.WorldGenChaos.Key)
        {
            valid = false;
            if (FallingCube.BestBlocksBroken >= 50)
            {
                valid = true;
            }
        }
        if (valid)
            data.WriteValue(IsOn ? 1 : 0);
        else
            data.WriteValue(0);
        LinkToSetting();
    }
}
