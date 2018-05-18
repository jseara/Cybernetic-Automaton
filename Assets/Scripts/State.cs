using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State
{
    List<Transition> inTransitions = new List<Transition>();
    List<Transition> outTransitions = new List<Transition>();
    bool rewardState = false;
    bool punishmentState = false;

    public State()
    {

    }
    public State(Transition inT)
    {
        inTransitions.Add(inT);
    }

    public void AddInTransition(Transition inT)
    {
        inTransitions.Add(inT);
    }

    public List<Transition> GetInTransitions()
    {
        return inTransitions;
    }

    public void AddOutTransition(Transition ouT)
    {
        outTransitions.Add(ouT);
    }

    public List<Transition> GetOutTransitions()
    {
        return outTransitions;
    }

    public Transition GetOutTransition(string inputSymbol)
    {
        foreach (Transition t in outTransitions)
        {
            if (t.GetInputSymbol() == inputSymbol)
            {
                return t;
            }
        }
        return null;
    }
    // public 
}

