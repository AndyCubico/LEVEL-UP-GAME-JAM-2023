using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxValue(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetCurrentValue(int health)
    {
        slider.value = health;
    }
}
