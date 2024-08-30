using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private Text DisplayName;
    [SerializeField] private InputField DisplayNumber;
    [SerializeField] private Slider Slider;
    [SerializeField] private bool PercentFormat = false;
    [SerializeField] private bool WholeNumbers = false;
    private SaveData<float> data => (SaveData<float>)ClientData.Dict[Key];
    private void LinkToSetting()
    {
        DisplayName.text = data.DisplayName;
        if (!PercentFormat)
        {
            if(WholeNumbers)
            {
                string txt = data.Value.ToString(format: "0");
                DisplayNumber.text = txt;
            }
            else
            {
                string txt = data.Value.ToString(format: "0.00");
                DisplayNumber.text = txt;
            }
        }
        else
        {
            string txt = (data.Value * 100).ToString(format: "0") + "%";
            DisplayNumber.characterValidation = InputField.CharacterValidation.None;
            DisplayNumber.text = txt;
            DisplayNumber.characterValidation = InputField.CharacterValidation.Decimal;
        }
        Slider.value = data.Value;
    }
    private void Start()
    {
        LinkToSetting();
    }
    private float minValue => Slider.minValue;
    private float maxValue => Slider.maxValue;
    public void SetData(float sliderValue)
    {
        sliderValue = Mathf.Round(sliderValue * 100f) / 100f;
        data.WriteValue(Mathf.Clamp(sliderValue, minValue, maxValue));
        LinkToSetting();
    }
    public void OnChangeValueInput(string input)
    {
        if (DisplayNumber.characterValidation == InputField.CharacterValidation.Decimal && input.Contains("%"))
        {
            input = input.Replace("%", "");
        }
        DisplayNumber.text = input;
    }
    public void ManualValueInput(string input)
    {
        if (DisplayNumber.characterValidation == InputField.CharacterValidation.Decimal && input.Contains("%"))
        {
            input = input.Replace("%", "");
        }
        float num = float.Parse(input);
        if(PercentFormat)
        {
            num /= 100f;
        }
        SetData(num);
        LinkToSetting();
    }
}
