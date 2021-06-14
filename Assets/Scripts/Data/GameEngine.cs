using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;


public class GameEngine : MonoBehaviour
{

    public CoreData coreDataObject;
    public static CoreData coreData;

    public static float hitStop;

    public static GameEngine gameEngine;

    public float deadZone = 0.2f;

    

    public CharacterObject mainCharacter;

    public int globalMovelistIndex, maxIndex=2;
    public UnityEvent OnFormChange;

    public MoveList CurrentMoveList()
    {
        return coreData.moveLists[globalMovelistIndex];
    }
    public void ChangeMovelist(int index)
    {
        globalMovelistIndex=index;
        if (globalMovelistIndex>coreData.moveLists.Count-1)
        {
            globalMovelistIndex = 0;
        }
        OnFormChange.Invoke();
    }
    public void ToggleMovelist()
    {
        globalMovelistIndex++;
        if (globalMovelistIndex>maxIndex)
        {
            globalMovelistIndex = 0;
        }
        OnFormChange.Invoke();
    }
    
    // Use this for initialization
    void Start ()
    {
        coreData = coreDataObject;
        gameEngine = this;
    }
    public static void SetHitPause(float _pow)
    {
        if (_pow > hitStop)
        {
            hitStop = _pow;
        }
    }
	// Update is called once per frame
	void Update ()
    {
        if (hitStop>0)
        {
            hitStop--;
        }
	}
    public static void GlobalPrefab(int _index, GameObject _parentObj, int _state, int _ev)
    {
        GameObject nextPrefab = Instantiate(coreData.globalPrefabs[_index], _parentObj.transform.position, _parentObj.transform.rotation, _parentObj.transform);
        nextPrefab.transform.localScale = _parentObj.transform.localScale;
        if (_state!=-1)
        {
            StateEvent thisEvent = coreData.characterStates[_state].events[_ev];

            nextPrefab.transform.localPosition += new Vector3(thisEvent.parameters[0].val, thisEvent.parameters[1].val, thisEvent.parameters[2].val);
            nextPrefab.transform.localRotation = Quaternion.Euler(new Vector3(thisEvent.parameters[3].val, thisEvent.parameters[4].val, thisEvent.parameters[5].val));
            nextPrefab.transform.localScale =Vector3.Scale(nextPrefab.transform.localScale, new Vector3(thisEvent.parameters[6].val, thisEvent.parameters[7].val, thisEvent.parameters[8].val));
        }

        //foreach (Animator myAnimator in nextPrefab.transform.GetComponentsInChildren<Animator>())
        //{
        //    VFXControl[] behaviors = myAnimator.GetBehaviours<VFXCOntrol>();
        //    for (int i = 0; i < behaviors.Length; i++)
        //    {
        //        behaviors[i].vfxRoot = nextPrefab.transform;
        //    }
        //    if (_state != -1)
        //    {
        //        StateEvent thisEvent = coreData.characterStates[_state].events[_ev];
        //        myAnimator.speed *= thisEvent.parameters[9].val;
        //    }
        //}
    }


    public InputBuffer playerInputBuffer;
    //void OnGUI()
    //{
    //    int xSpace = 25;
    //    int ySpace = 15;
    //    //GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
    //    for (int i = 0; i < playerInputBuffer.buttonCommandCheck.Count; i++)
    //    {
    //    }
    //    for (int b = 0; b < playerInputBuffer.buffer.Count; b++)
    //    {
    //        //GUI.Label(new Rect(xSpace - 10f, b * ySpace, 100, 20), b.ToString() + ":");
    //        for (int i = 0; i < playerInputBuffer.buffer[b].rawInputs.Count; i++)
    //        {
    //            if (playerInputBuffer.buffer[b].rawInputs[i].used)
    //            { GUI.Label(new Rect(10f + (i * xSpace), 35f + (b * ySpace), 100, 20), playerInputBuffer.buffer[b].rawInputs[i].hold.ToString("0") + ">"); }
    //            else { GUI.Label(new Rect(10f + (i * xSpace), 35f + (b * ySpace), 100, 20), playerInputBuffer.buffer[b].rawInputs[i].hold.ToString("0")); }
    //        }
    //    }

    //    for (int m = 0; m < playerInputBuffer.motionCommandCheck.Count; m++)
    //    {
    //        GUI.Label(new Rect(500f - 25f, m * ySpace, 100, 20), playerInputBuffer.motionCommandCheck[m].ToString());
    //        GUI.Label(new Rect(500f, m * ySpace, 100, 20), coreData.motionCommands[m].name);

    //    }
    //    GUI.Label(new Rect(600f, 10f, 100, 20), CurrentMoveList().name.ToString());

    //}
}
