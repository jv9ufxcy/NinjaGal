using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenController : MonoBehaviour
{
    private bool isMainMenuActive=false;
    public GameObject startPanel, menuPanel;
    private int value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMainMenuActive)
        {
            if (Input.GetButtonDown("Ps4Options"))
            {
                value = 0;
                GlobalVars.instance.PassControllerValue(0);
                Debug.Log("PS4 Start");
                ActivateMenu();
            }
            else if (Input.GetButtonDown("XboxMenu"))
            {
                value = 1;
                GlobalVars.instance.PassControllerValue(1);
                Debug.Log("Xbox Start");
                ActivateMenu();
            }
            else if (Input.GetKey(KeyCode.Return))
            {
                value = 2;
                GlobalVars.instance.PassControllerValue(2);
                Debug.Log("PC Start");
                ActivateMenu();
            }
        }
    }
    void ActivateMenu()
    {
        startPanel.SetActive(false);
        menuPanel.SetActive(true);
        menuPanel.GetComponent<Button>().Select(); ;
        isMainMenuActive = true;
        GlobalVars.instance.PassControllerValue(value);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
