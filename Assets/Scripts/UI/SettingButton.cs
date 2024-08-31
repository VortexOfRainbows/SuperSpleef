using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private Button button;
    private SaveData<int> data => (SaveData<int>)ClientData.Dict[Key];
    //private void LinkToSetting()
    //{
    //    //bool set = data.Value > 0;
    //    //if (Toggle.isOn != set)
    //    //    Toggle.isOn = set;
    //}
    private void Start()
    {
        //LinkToSetting();
    }
    public void SetData(int value)
    {
        data.WriteValue(value);
        //LinkToSetting();
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
