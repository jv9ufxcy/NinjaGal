using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class DamagePopup : MonoBehaviour
{
    public static DamagePopup Create(Vector3 pos, int amount, bool isCrit)
    {
        Transform dpTransform = Instantiate(GameEngine.gameEngine.DamagePopup, pos, Quaternion.identity);

        DamagePopup damage = dpTransform.GetComponent<DamagePopup>();
        damage.Setup(amount, isCrit);

        return damage;
    }
    private TextMeshPro textMesh;
    private float disappearTimer = 1f;
    private Color textColor;
    private static int sortOrder;
    void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    public void Setup(int damAmt, bool isCrit)
    {
        textMesh.SetText(damAmt.ToString());
        if (!isCrit)
        {
            textMesh.fontSize = 8;
            textColor = textMesh.color;
        }
        else
        {
            textMesh.fontSize = 12;
            textColor = Color.yellow;
        }
        textMesh.color = textColor;
        disappearTimer = 1f;
        sortOrder++;
        textMesh.sortingOrder = sortOrder;
        transform.DOScale(1.5f, 0.3f);
        transform.DOScale(.25f, 2).SetDelay(0.3f);
        float moveYSpeed = 10f;
        transform.DOLocalMoveX(transform.localPosition.x-2, 1);
        transform.DOLocalMoveY(transform.localPosition.y+moveYSpeed, 1).SetDelay(1);
        textMesh.DOFade(0, 1).SetDelay(1);
        Destroy(this.gameObject, 2f);
    }
    //private void FixedUpdate()
    //{
       
    //    //transform.position += new Vector3(0, moveYSpeed) * Time.fixedDeltaTime;
        
    //    disappearTimer -= Time.fixedDeltaTime;
    //    if (disappearTimer<0)
    //    {
            
    //    }
    //}
}
