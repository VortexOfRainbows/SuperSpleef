using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private Text DisplayName;
    [SerializeField] private Text DisplayNumber;
    private void LinkToSetting()
    {
        SaveData<float> data = (SaveData<float>)ClientData.Dict[Key];
        DisplayName.text = data.DisplayName;
        DisplayNumber.text = data.Value.ToString(format: "##.#");
    }
    private void Start()
    {
        LinkToSetting();
    }
}
