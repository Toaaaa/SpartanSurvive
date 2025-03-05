using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUI : MonoBehaviour
{
    public Player player;
    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = player.maxHp;
        slider.onValueChanged.AddListener(OnHpChanged);
    }

    void OnHpChanged(float n)
    {
        
    }
}
