using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPercent : MonoBehaviour //Team members that contributed to this script: David Bu, Ian Bunnel
{
    [SerializeField] private bool Percent = false;
    private Text percentText;
    void Start()
    {
        percentText = GetComponent<Text>();
    }
    public void TextUpdate(float value)
    {
        if (Percent)
            percentText.text = Mathf.RoundToInt(value * 100) + "%";
        else
            percentText.text = (Mathf.RoundToInt(value * 100) / 100f).ToString();
    }
}
