using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobTitle : MonoBehaviour
{
    //where I got the code to bob the title art
    //https://gamedev.stackexchange.com/questions/96878/how-to-animate-objects-with-bobbing-up-and-down-motion-in-unity/96880

    [SerializeField] private BobDirection _bobDirection;
    private enum BobDirection { vertical, horizontal, diagonal };
    [SerializeField]
    private float bobStrength;

    [SerializeField]
    private float bobSpeed;

    private float originalPositionX;
    private float originalPositionY;

	void Start ()
    {
        originalPositionX = transform.position.x;
        originalPositionY = transform.position.y;
	}
	
	void Update ()
    {
        
        switch (_bobDirection)
        {
            case BobDirection.diagonal:
                BobTheTitle();
                break;
            case BobDirection.vertical:
                VerticalBob();
                break;
            case BobDirection.horizontal:
                HorizontalBob();
                break;
            default:
                BobTheTitle();
                break;
        }
    }

    private void BobTheTitle()
    {
        transform.position = new Vector2(originalPositionX + ((float)Mathf.Sin(Time.time * bobSpeed) * bobStrength), 
            originalPositionY + ((float)Mathf.Sin(Time.time * bobSpeed) * bobStrength));
    }
    private void VerticalBob()
    {
        transform.position = new Vector2(originalPositionX, originalPositionY + ((float)Mathf.Sin(Time.time * bobSpeed) * bobStrength));
    }
    private void HorizontalBob()
    {
        transform.position = new Vector2(originalPositionX + ((float)Mathf.Sin(Time.time * bobSpeed) * bobStrength),
            originalPositionY);
    }
}
