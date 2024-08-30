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
        DisplayName.text = data.DisplayName;
        bool set = data.Value > 0;
        if (Toggle.isOn != set)
            Toggle.isOn = set;
    }
    private void Start()
    {
        LinkToSetting();
    }
    public void SetData(bool IsOn)
    {
        data.WriteValue(IsOn ? 1 : 0);
        LinkToSetting();
    }
    //public void OnChangeValueInput(string input)
    //{
    //    //if (DisplayNumber.characterValidation == InputField.CharacterValidation.Decimal && input.Contains("%"))
    //    //{
    //    //    input = input.Replace("%", "");
    //    //}
    //    //DisplayNumber.text = input;
    //}
    //public void ManualValueInput(string input)
    //{
    //    //if (DisplayNumber.characterValidation == InputField.CharacterValidation.Decimal && input.Contains("%"))
    //    //{
    //    //    input = input.Replace("%", "");
    //    //}
    //    //float num = float.Parse(input);
    //    //if (PercentFormat)
    //    //{
    //    //    num /= 100f;
    //    //}
    //    SetData(num);
    //    LinkToSetting();
    //}
}
