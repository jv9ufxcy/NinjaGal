using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : MonoBehaviour
{
    public enum ControllerState { ps4, xbox, keyboard}
    public static ControllerState _controllerState;
    public static int controllerNumber;
    public static GlobalVars instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
            Destroy(gameObject);
    }
    public void PassControllerValue(int dropdownValue)
    {
        controllerNumber = dropdownValue;
        switch (controllerNumber)
        {
            case 0:
                GameEngine.coreData.rawInputs = GameEngine.coreData.ps4Inputs;
                break;
            case 1:
                GameEngine.coreData.rawInputs = GameEngine.coreData.xboxInputs;
                break;
            case 2:
                GameEngine.coreData.rawInputs = GameEngine.coreData.keyboardInputs;
                break;
            default:
                break;
        }
        
    }
}
