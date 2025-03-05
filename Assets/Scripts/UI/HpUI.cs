using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class HpUI : MonoBehaviour
{
    public Player player;
    private Slider slider;
    public RawImage _background;
    public RawImage _fillImage;
    public float lerpSpeed = 5f;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = player.GetMaxHp();

        RectTransform fillrt = _fillImage.GetComponent<RectTransform>();
        slider.fillRect = null;
        fillrt.anchorMin = new Vector2(0, 0.5f);
        fillrt.anchorMax = new Vector2(0, 0.5f);
        fillrt.pivot = new Vector2(0, 0.5f);
        slider.fillRect = fillrt;

        slider.onValueChanged.AddListener(OnHpChanged);
    }

    private void Update()
    {
        UpdateHpBar();
    }


    void UpdateHpBar()
    {
        float currentHp = player.hp;
        float maxHp = player.GetMaxHp();

        slider.value = Mathf.Clamp(currentHp, 0, maxHp);// 체력 게이지 0 ~ maxHp 사이의 값으로 제한

        float targetWidth = (currentHp / maxHp) * _background.GetComponent<RectTransform>().sizeDelta.x;
        float currentWidth = _fillImage.GetComponent<RectTransform>().sizeDelta.x;
        float newWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * lerpSpeed);// 게이지 부드럽게 이동
    }
    void OnHpChanged(float n)
    {
        slider.gameObject.GetComponent<RectTransform>().DOShakeAnchorPos(0.1f, 10, 90, 90, false, true);// 체력 변경시 흔들리는 애니메이션
    }
}
