using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private Text DisplayName;
    [SerializeField] private Text DisplayNumber;
    [SerializeField] private Slider Slider;
    [SerializeField] private bool PercentFormat = false;
    private void LinkToSetting()
    {
        SaveData<float> data = (SaveData<float>)ClientData.Dict[Key];
        DisplayName.text = data.DisplayName;
        if (!PercentFormat)
        {
            DisplayNumber.text = data.Value.ToString(format: ".#");
        }
        else
        {
            DisplayNumber.text = (data.Value * 100).ToString(format: "###") + "%";
        }
        Slider.value = data.Value;
    }
    private void Start()
    {
        LinkToSetting();
    }
}
