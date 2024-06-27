using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    [SerializeField] private string Key;
    [SerializeField] private Text DisplayName;
    [SerializeField] private Text DisplayNumber;
    [SerializeField] private Slider Slider;
    [SerializeField] private bool PercentFormat = false;
    private SaveData<float> data => (SaveData<float>)ClientData.Dict[Key];
    private void LinkToSetting()
    {
        DisplayName.text = data.DisplayName;
        if (!PercentFormat)
        {
            string txt = data.Value.ToString(format: "0.00");
            DisplayNumber.text = txt;
        }
        else
        {
            string txt = (data.Value * 100).ToString(format: "0") + "%";
            DisplayNumber.text = txt;
        }
        Slider.value = data.Value;
    }
    private void Start()
    {
        LinkToSetting();
    }
    private void Update()
    {
        if(gameObject.activeSelf)
            LinkToSetting();
    }
    private float minValue => Slider.minValue;
    private float maxValue => Slider.maxValue;
    public void SetData(float sliderValue)
    {
        sliderValue = Mathf.Round(sliderValue * 100f) / 100f;
        data.WriteValue(Mathf.Clamp(sliderValue, minValue, maxValue));
    }
}
