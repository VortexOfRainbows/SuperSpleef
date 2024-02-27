using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderPercent : MonoBehaviour
{

    Text percentText;

    // Start is called before the first frame update
    void Start()
    {
        percentText = GetComponent<Text>();
    }

    public void textUpdate(float value)
    {
        percentText.text = Mathf.RoundToInt(value * 100) + "%";
    }
}
