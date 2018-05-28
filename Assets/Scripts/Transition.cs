using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Transition : IEquatable<Transition>
{
    string inputSymbol;
    float[,] expectationMatrix = new float[1000, 1000];
    float[,] confidenceMatrix = new float[1000, 1000];
    Dictionary<string, float> outputProbability = new Dictionary<string, float>();
    State from = new State();
    State to = new State();
    public bool temporary = false;

    public Transition(string input)
    {
        inputSymbol = input;
    }
    public Transition(State previousState, string input)
    {
        from = previousState;
        inputSymbol = input;
    }

    public Transition(State previousState, string input, State toState)
    {
        //State[] transitionEndPoints = new State[2];
        from = previousState;
        inputSymbol = input;
        to = toState;

    }

    public void CreateTransition(State previousState, string input, State toState)
    {
        //State[] transitionEndPoints = new State[2];
        from = previousState;
        inputSymbol = input;
        to = toState;

        //previousState.AddOutTransition()


        //State nextState = new State("epsilon");

        //return nextState;
    }


    public bool Equals(Transition other)
    {
        return true;
    }

    public void SetPreviousState(State prev)
    {
        from = prev;
    }

    public State GetPreviousState()
    {
        return from;
    }

    public void SetToState(State t)
    {
        to = t;
    }

    public string GetInputSymbol()
    {
        return inputSymbol;
    }

    public void SetOutputProbability(Dictionary<string, float> prob)
    {
        outputProbability = prob;
    }

    public Dictionary<string, float> GetOutputProbability()
    {
        return outputProbability;
    }

    public void SetConfidence(float[,] con)
    {
        confidenceMatrix = con;
    }

    public float[,] GetConfidence()
    {
        return confidenceMatrix;
    }

    public void AddValueToConfidence(int x, int y, float value)
    {
        confidenceMatrix[x, y] = value;
    }
    public void ChangeProbabilityValue(string key, float value)
    {
        outputProbability[key] = value;
    }
    public bool IsTemporary()
    {
        return temporary;
    }
    public float[,] GetExpectationMatrix()
    {
        return expectationMatrix;
    }
    public void SetExpectationMatrix(float[,] expect)
    {
        expectationMatrix = expect;
    }
}
