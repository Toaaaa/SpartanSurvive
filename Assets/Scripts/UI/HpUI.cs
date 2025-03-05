using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


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
        slider.gameObject.GetComponent<RectTransform>().DOShakeAnchorPos(0.1f, 10, 90, 90, false, true);// ü�� ����� ��鸮�� �ִϸ��̼�
    }
}
