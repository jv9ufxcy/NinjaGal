using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthVisualManager : MonoBehaviour
{
    public static HealthSystem healthSystemStatic;

    public int maxHealth=40;
    public float heartOffset = 4f, heartAnchor = -60f;
    [SerializeField] private Vector2 sizeDelta = new Vector2(64,64);
    [SerializeField] private Sprite healthSprite0;
    [SerializeField] private Sprite healthSprite1;
    [SerializeField] private Sprite healthSprite2;
    private List<HealthImage> healthImageList;
    private HealthSystem healthSystem;
    private RectTransform healthTransform;
    private void Awake()
    {
        healthImageList = new List<HealthImage>();
        healthTransform = transform.parent.GetComponent<RectTransform>();
    }
    private void Start()
    {
        HealthSystem healthSys = new HealthSystem(maxHealth/2);//num of hearts
        SetHealthSystem(healthSys);
    }
    public void SetHealthSystem(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
        healthSystemStatic = healthSystem;

        List<HealthSystem.Heart> heartList = healthSystem.GetHeartList();
        Vector2 heartAnchorPos = new Vector2(heartAnchor, 0);
        for (int i = 0; i < heartList.Count; i++)
        {
            HealthSystem.Heart heart = heartList[i];
            CreateHeartImage(heartAnchorPos).SetHealthFragments(heart.GetFragmentAmount());
            heartAnchorPos += new Vector2(heartOffset, 0);//4px
        }
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;
        healthSystem.OnDead += HealthSystem_OnDead;
    }
    private void HealthSystem_OnDamaged(object sender, System.EventArgs e)
    {
        RefereshAllHearts();
        healthTransform.DOShakeAnchorPos(0.2f, 5, 2, 0, true, false);
    }
    private void HealthSystem_OnHealed(object sender, System.EventArgs e)
    {
        RefereshAllHearts();
    }
    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        RefereshAllHearts();
    }
    private void RefereshAllHearts()
    {
        List<HealthSystem.Heart> heartList = healthSystem.GetHeartList();
        for (int i = 0; i < healthImageList.Count; i++)
        {
            HealthImage heartImage = healthImageList[i];
            HealthSystem.Heart heart = heartList[i];
            heartImage.SetHealthFragments(heart.GetFragmentAmount());
        }
    }
    private HealthImage CreateHeartImage(Vector2 anchoredPos)
    {
        GameObject heartGO = new GameObject("Health", typeof(Image));
        //child
        heartGO.transform.parent = transform;
        heartGO.transform.localPosition = Vector3.zero;

        //pos and size
        heartGO.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        heartGO.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        heartGO.GetComponent<RectTransform>().localScale = new Vector3(16,16,1);

        //image sprite
        Image healthImageUI = heartGO.GetComponent<Image>();
        healthImageUI.sprite = healthSprite2;

        HealthImage healthImage = new HealthImage(this, healthImageUI);
        healthImageList.Add(healthImage);
        return healthImage;//???
    }
    public class HealthImage
    {
        private int fragments;
        private Image heartImage;
        private HealthVisualManager healthVisual;
        public HealthImage(HealthVisualManager healthVis,Image healthImage)
        {
            this.healthVisual = healthVis;
            this.heartImage = healthImage;
        }
        public void SetHealthFragments(int fragments)
        {
            this.fragments = fragments;
            switch (fragments)
            {
                case 0:
                    heartImage.sprite = healthVisual.healthSprite0;
                    break;
                case 1:
                    heartImage.sprite = healthVisual.healthSprite1;
                    break;
                case 2:
                    heartImage.sprite = healthVisual.healthSprite2;
                    break;
                default:
                    break;
            }
        }
        public int GetFragmentAmount()
        {
            return fragments;
        }
    }
            
}

