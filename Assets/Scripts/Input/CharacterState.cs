using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterState
{
    public string stateName;
    [HideInInspector] public int index;

    public float length;
    public bool loop;
    public float blendRate = 0.1f;

    //public int frame;

    //public float currentTime;
    //public int currentState;

    public List<StateEvent> events;
    public List<AttackEvent> attacks;

    public int jumpReq;
    public int dashReq;
    public int meterReq;
    public float dashCooldownReq;
    public bool groundedReq;
    public bool wallReq;

    public bool ConditionsMet(CharacterObject character)
    {
        if (character.specialMeter < meterReq) { return false; }
        else { character.nextSpecialMeterUse = meterReq; }

        //else { character.jumps--; }
        return true;
    }
}
[System.Serializable]
public class StateEvent
{
    public float start;
    public float end;
    //public float variable;
    public bool active = true;


    [IndexedItem(IndexedItemAttribute.IndexedItemType.SCRIPTS)]
    public int script;

    public List<ScriptParameters> parameters;
    public StateEvent()
    {
        active = true;
        parameters = new List<ScriptParameters>();
    }
}
[System.Serializable]
public class ScriptParameters
{
    public string name;
    public float val;
}
[System.Serializable]
public class CharacterScript
{
    [HideInInspector]
    public int index;

    public string name="<NEW CHARACTER SCRIPT>";

    public List<ScriptParameters> parameters = new List<ScriptParameters>();
    //public float variable;
}
[System.Serializable]
public class InputCommand
{
    [IndexedItem(IndexedItemAttribute.IndexedItemType.MOTION_COMMAND)]
    public int motionCommand;

    [IndexedItem(IndexedItemAttribute.IndexedItemType.RAW_INPUTS)]
    public int input;

    //public string inputString;
    [IndexedItem(IndexedItemAttribute.IndexedItemType.STATES)]
    public int state;

    public List<int> inputs;
}

[System.Serializable]
public class MoveList
{
    public string name = "<NEW MOVE LIST>";
    public List<CommandState> commandStates = new List<CommandState>();
    public MoveList()
    {
        commandStates.Add(new CommandState());
    }
}
    
[System.Serializable]
public class CommandState
{
    public string stateName;
    //flags
    public bool aerial;
    //public bool wall;
    //explicit state
    public bool explicitState;
    [IndexedItem(IndexedItemAttribute.IndexedItemType.STATES)]
    public int state;

    public List<CommandStep> commandSteps;

    [HideInInspector] public List<int> omitList;
    [HideInInspector] public List<int> nextFollowups;
    public CommandState()
    {
        commandSteps = new List<CommandStep>();
        stateName = "<NEW COMMAND STATE>";
    }
    public CommandStep AddCommandStep()
    {
        foreach (CommandStep s in commandSteps)
        {
            if (!s.activated) { s.activated = true;return s; }
        }
        CommandStep nextStep = new CommandStep(commandSteps.Count);
        nextStep.activated = true;
        commandSteps.Add(nextStep);
        return nextStep;
    }
    public void RemoveChainCommands(int _id)
    {
        if (_id == 0) { return; }
        commandSteps[_id].activated = false;
        commandSteps[_id].followUps = new List<int>();
    }

    public void CleanUpBaseState()
    {

        omitList = new List<int>();

        for (int s = 1; s < commandSteps.Count; s++)
        {
            for (int f = 0; f < commandSteps[s].followUps.Count; f++)
            {
                omitList.Add(commandSteps[s].followUps[f]);
            }
        }

        nextFollowups = new List<int>();
        for (int s = 1; s < commandSteps.Count; s++)
        {
            bool skip = false;
            for (int m = 0; m < omitList.Count; m++)
            {
                if (omitList[m] == s) { skip = true; }
                if (omitList[m] >= commandSteps.Count) { skip = true; }
                if (!commandSteps[s].activated) { skip = true; }
            }
            if (!skip) { nextFollowups.Add(s); }

        }

        commandSteps[0].followUps = nextFollowups;

    }
    public void CleanUpFollowups()
    {
        for (int s = 0; s < commandSteps.Count; s++)
        {
            omitList = new List<int>();
        }

    }
}
[System.Serializable]
public class CommandStep
{
    public int idIndex;

    public InputCommand command;

    //[IndexedItem(IndexedItemAttribute.IndexedItemType.CHAIN_COMMAND)]
    public List<int> followUps;

    public bool strict; //Alternatively you could make a whole new Command State

    [HideInInspector]
    public Rect myRect;

    public bool activated;

    public int priority;

    public void AddFollowUp(int _nextID)
    {
        if (_nextID == 0) { return; }
        if (idIndex == _nextID) { return; }
        for (int i = 0; i < followUps.Count; i++)
        {

            if (followUps[i] == _nextID) { return; }
        }
        followUps.Add(_nextID);
    }

    public CommandStep(int _index)
    {
        idIndex = _index;
        followUps = new List<int>();
        command = new InputCommand();
        myRect = new Rect(50, 50, 200, 200);
    }
}
