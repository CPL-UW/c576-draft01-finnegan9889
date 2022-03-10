using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    public Slider slider;
    public Gradient grad;
    public Image fill;

    public void SetMaxValue(int value){
        slider.maxValue = value;
        slider.value = value;

        fill.color = grad.Evaluate(1f);
    }

    public void SetValue(int value){
        slider.value = value;
        fill.color = grad.Evaluate(slider.normalizedValue);
    }

}