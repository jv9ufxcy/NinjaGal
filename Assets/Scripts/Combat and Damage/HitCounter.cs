using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HitCounter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image timeSlider;
    public static HitCounter instance;
    public int hitCount;
    public GameObject comboContainer;
    private void Awake()
    {
        //text = GetComponent<TextMeshProUGUI>();
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        HideCounter();
    }
    public float curComboTime, maxTime=4f;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (curComboTime>0)
        {
            curComboTime -= Time.fixedDeltaTime;
            timeSlider.fillAmount = curComboTime / maxTime;
        }
        else
        {
            if (timeSlider.enabled == true)
            {
                HideCounter();
            }
        }
    }
    public void OnPlayerDamaged()
    {
        HideCounter();
    }
    public void OnEnemyKilled()
    {
        curComboTime = maxTime;
        hitCount++;
        SetHitCounter(hitCount);
        float baseIntensity = .2f;
        float perHitIntensity = .02f;
        text.transform.DOShakePosition(baseIntensity, Mathf.Clamp(baseIntensity + (perHitIntensity * hitCount),baseIntensity, 1f));
    }
    private void SetHitCounter(int hitCount)
    {
        comboContainer.SetActive(true);
        text.enabled = true;
        timeSlider.enabled = true;

        text.SetText(hitCount.ToString());
    }
    private void HideCounter()
    {
        hitCount = 0;
        curComboTime = 0;
        text.enabled=false;
        timeSlider.enabled=false;
        comboContainer.SetActive(false);
    }
}
