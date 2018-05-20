using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class Program : MonoBehaviour
{
    List<State> states;
    List<Transition> transitions;
    State originalState;
    List<string> inputAlphabet;
    List<string> outputAlphabet = new List<string> { "z", "y", "x" };
    List<Dictionary<string, float>> data;

    //List<MarkedElementProbability> markedElementProbabilities;
    float expectationLearnFactor = .05f;       //alpha
    float confidenceLearnFactor = .5f;        //beta
    float speedOfLearnFactor = .1f;           //zeta
    float speedOfConditioning = .001f;          //gamma
    float probabilityPropagationFactor = .5f; //nu
    float nullProbability = .5f;              //eta
    float relevanceOverTime = .9f;            //kappa

    //Updating Variables
    State lastState;
    State anchorState;
    State currentState;

    string lastInputSymbol;
    Dictionary<string, float> dominantInput;
    string dominantInputSymbol;
    string currentInputSymbol;

    string lastOutputSymbol;
    string currentOutputSymbol;
    float currentOutputSymbolStrength;

    Dictionary<string, float> lastInputBatch;
    Dictionary<string, float> currentInputBatch;

    int iteration;

    List<State> RewardStates;
    List<State> PunishmentStates;

    private void initialize()
    {
        states = new List<State>();
        transitions = new List<Transition>();

        data = new List<Dictionary<string, float>>();
        iteration = 0;
        dominantInput = new Dictionary<string, float>();
        originalState = new State();

        CreateTransitionAndLinkWithStates(originalState, "epsilon", originalState);

        currentState = originalState;
        lastState = originalState;
        lastInputSymbol = "epsilon";
        lastOutputSymbol = "epsilon";


    }



    /*
        Sigma = input [ a b c e ]
        Delta = Output [ delta beta gamma ]
        Q(state) = [q0]
        delta   = []
        lambda  = randomly choosing something in your probability output matrix (dictionary )
        q0 = original state
        list of reward states
        list of punishment states
        list of confidences
        list of expectations
        epsilon = NULL
        beta    = Confidence Learning Factor
        alpha   = Expectation Learning Factor
        zeta    = Speed of Learning Factor
        kappa   = Time Relevance  
        */
    void Start()
    {
        //      Step 1 - Initial State

        initialize();

        //This is just testing information.  Checks to make sure all initialization works as intented.
        /* 
        if ((p.data != null) && (p.iteration == 0) && (p.dominantInput != null))
            if ((p.originalState != null) && (p.currentState.Equals(p.originalState)) && (p.lastState.Equals(p.originalState)))
                if (p.lastInputSymbol.Equals("epsilon") && (p.lastOutputSymbol.Equals("epsilon")))
                    Console.WriteLine("Initialization complete");
        */
        //Console.WriteLine(p.currentState.GetOutTransition("epsilon").GetInputSymbol());


        if (data.Count == 0)                                                            //      Step 3 - Gather input
        {
            DataReader reader = new DataReader();
            data = reader.gatherInput();
            currentInputBatch = data[0];
        }
        else
        {
            iteration++;
            lastInputBatch = currentInputBatch;
            currentInputBatch = data[iteration];
        }

        if (currentInputBatch.ContainsKey("epsilon"))                                     //      Step 2 - If epsilon symbol, reset
            reset();

        findDominantInput();                                                              //      Step 4 - find strongest input

        CreateNewTransitions();                                                           //      Step 5 - Create New Transitions

        lastOutputSymbol = currentOutputSymbol;                                         //      Step 6 - last output = output

        Transition dominantTransition = currentState.GetOutTransition(dominantInputSymbol);

        rollDie(dominantTransition);                                                      //      Step 7 - Strength of output symbol is equal to (input symbol strength * confidence)/(1+confidence)

        Mark(dominantInput);                                                            //      Step 8 - Mark the output symbol and its strength for later modification

        Output();
        /*


        Try doing: 
            - init statement
                - q0 is initial state which can be set

            - Step 9.5 Set last state to be current state (ql = c)
            - c = delta(c, ad) Set c to be where transition delta(c,ad) takes you
            - Step 10.5 Set current input symbol to last input symbol al = ad
            - Step 11 If current state is listed in Reward State list, Apply Reward.  Else if current state is listed in Punishment State List, apply punishment.  Else if does not exist in either list, apply conditioning
                - How to set reward state?
                Apply Reward
                    set t = 1
                    for each distribution that is marked, starting with most recent and moving back in time
                        for each marked element (x) in distribution
                            the percentage that gets us x = previous percentge + (zeta, previously ) * t * strength of input *(1/Confidence)/(1 + (zeta, previously set value) * t * strength of input * (1/Confidence))
                            for all percentages that do not get us x (y), decrease by math
                            deltaConfidence = zeta * t * strength of input
                            Confidence += deltaConfidence
                            Unmark (x)
                            for each state q in q
                                update values based on their percentage values

                            Unmark percentage
                            t = (kappa, previously  * t

                Apply Punishment
                    set t = 1
                        for each distribution that is marked, starting with most recent and moving back in time
                            for each marked element (x) in distribution
                                increase probability of all y by equal amount
                                decrease probability of x by sum total of all deltaY probabilities
                                change confidence
                                for each state q in q
                                    update values based on their percentage matrices
                                umark percentage
                                t = kappa *t

                Apply Conditioning
                    when you have a chain of outputs that are not the same (last output != epsilon) & current output != last output
                        for each input symbol in input library
                            if expectation exists between our last transition and ANY transition that was on our last state THAT WAS IN OUR LAST INPUT
                                then increase probability of input symbol we are checking producing last output symbol
                                decrease probability of everything else
                            for every state AND every symbol if there is a transition (using that state,symbol pairing) to our last state
                                update the probability of the transition leading to the last state


    ///     - Step 11 
    ///     - Step 12 go to step 2
    */
        Console.ReadLine();
        //        NOTE: (q0,A) = transition 
    }

    private void reset()
    {
        // Step 2
        Transition nullTransition = new Transition(currentState, "a");                      //  If transition (current state, epsilon) is defined
        if (transitions.Contains(new Transition(currentState, "a")))
            if (nullTransition.temporary == true)                                           //      If transition(current state, epsilon) is temporary
                nullTransition.temporary = false;                                           //          Mark as permanent
        lastState = currentState;                                                           //      Let old state = current state
                                                                                            //currentState = Transition.transitionTo(currentState, "a");                        //      Let current state = transition(current, epsilon);
                                                                                            //anchorState = currentState;                                                       //      Let anchor state = current state
        lastInputSymbol = "e";                                                              //      Let last symbol and last output symbol equal epsilon
        lastOutputSymbol = "e";
        //MarkedElementProbabilities.clear()                                                //      Unmark all symbols and distributions
        //Loop
        // Go to step 2
    }

    private void findDominantInput()
    {

        float maximumValue = 0;
        string maximumKey = "";
        foreach (KeyValuePair<string, float> kvp in currentInputBatch)
        {
            if (kvp.Value > maximumValue)
            {
                maximumValue = kvp.Value;
                maximumKey = kvp.Key;
            }
        }
        Console.WriteLine("Dominant Input is: {0}, {1}", maximumKey, maximumValue);
        dominantInput.Add(maximumKey, maximumValue);
        dominantInputSymbol = maximumKey;
    }

    private void CreateNewTransitions()
    {
        // - Step 5 Perform create new transitions
        //FIGURE OUT WAHT THE FUCK TO DO FOR EPSILON
        //Transition nullTransition = new Transition(currentState, "epsilon");

        for (int i = 0; i < transitions.Count; i++)                                         //      If transition (c,epsilon) is defined and temporary
        {
            if (transitions[i].GetPreviousState().Equals(currentState))
                if (transitions[i].GetInputSymbol().Equals("epsilon"))
                    if (transitions[i].IsTemporary())
                        transitions.Remove(transitions[i]);                                 //      Remove

        }
        foreach (KeyValuePair<string, float> kvp in currentInputBatch)                      //      For each input pair in I
        {
            List<Transition> outTrans = currentState.GetOutTransitions();
            for (int i = 0; i < outTrans.Count; i++)
            {
                if (outTrans[i].GetInputSymbol() != kvp.Key)                                //      if transition (c, input) DNE
                {
                    State newState = new State();                                           //      create new state QN
                    Transition newTransition = new Transition(currentState, kvp.Key, newState);//      let the transition go to QN
                                                                                               //      Add state to state list
                    states.Add(newState);                                                   //      Add transition to TransitionList
                    Transition nullTransition = new Transition(currentState, "epsilon", anchorState);//    create temporary transition back to anchor state

                    foreach (Transition t in transitions)
                    {
                        if (t.GetInputSymbol() == kvp.Key)                                  //    if there exists a transition for which transition exists
                        {
                            newTransition.SetOutputProbability(t.GetOutputProbability());   //        copy confidence, probability
                            newTransition.SetConfidence(t.GetConfidence());
                            break;
                        }
                    }

                    if (newTransition.GetConfidence()[0, 0] == 0)                              //    otherwise
                    {
                        newTransition.ChangeProbabilityValue("epsilon", nullProbability);   //        define probability of null as (eta), which is pre-set
                        for (int j = 0; j < outputAlphabet.Count; j++)
                        {
                            float math = (1 - nullProbability) / (Mathf.Abs(outputAlphabet.Count));
                            newTransition.ChangeProbabilityValue(outputAlphabet[j], math);  //        define probability of marked states (beta) to be equal percentage of delta minus (eta).
                        }
                        newTransition.AddValueToConfidence(0, i, .1f);

                    }
                    newTransition.SetPreviousState(currentState);
                    newTransition.SetToState(newState);
                    newState.AddInTransition(newTransition);
                    currentState.AddOutTransition(newTransition);
                    transitions.Add(newTransition);

                }

            }

        }

    }

    private void rollDie(Transition transition)
    {
        Console.WriteLine(transition.GetInputSymbol());
        int iterator = 0;
        float previousValue = 0;
        System.Random rnd = new System.Random();
        int roll = rnd.Next(1, 100);

        int percentSize = transition.GetOutputProbability().Count * 2;
        float[] percentages = new float[percentSize];
        foreach (KeyValuePair<string, float> kvp in transition.GetOutputProbability())
        {
            percentages[iterator] = previousValue;
            percentages[iterator + 1] = previousValue + ((kvp.Value * 100));
            previousValue = percentages[iterator + 1];
            Console.WriteLine("Percentage: " + percentages[iterator + 1]);
            if (roll <= percentages[iterator + 1])
            {
                currentOutputSymbol = kvp.Key;
                break;
            }
            iterator += 2;
        }
    }

    private void Mark(Dictionary<string, float> input)
    {

    }


    private void CreateTransitionAndLinkWithStates(State startState, string input, State endState)
    {
        if (!states.Contains(startState))
            states.Add(startState);

        if (!states.Contains(endState))
            states.Add(endState);

        Transition newTransition = new Transition(startState, input, endState);
        startState.AddOutTransition(newTransition);
        Console.WriteLine(newTransition.GetInputSymbol());
        endState.AddInTransition(newTransition);
        transitions.Add(newTransition);
    }

    private void UpdateExpectations()
    {
        //     - Step 9 Update Expectations
        Transition currentTrans = currentState.GetOutTransition(dominantInputSymbol);
        Transition lastTrans = new Transition(lastInputSymbol);
        int x = -1, y = -1;

        foreach (Transition t in currentState.GetInTransitions())
        {
            if (t.GetPreviousState().Equals(lastState))
            {
                lastTrans = t;
                break;
            }
        }
        for (int i = 0; i < transitions.Count; i++)
        {
            if (lastTrans.Equals(transitions[i]))
                x = i;
            if (currentTrans.Equals(transitions[i]))
                y = i;
        }
        float[,] expectation = currentTrans.GetExpectationMatrix();
        if (x != -1 && y != -1)
        {
            if (expectation[x, y] != 0)                                          // If transition exists
            {
                //     deltaExpectation = (alpha)*(1-previous expectation)
                //     expectation += deltaExpectation
                //     confidence = confidence * (1-((beta)confidenceLearningStrength * abs(deltaExpectation))
            }
            else                                                                // else if expectation did not exist previously
            {
                //     create new link in expectation storage unit (symmetrically) and set the expectation to be equal to (alpha) predetermined value
            }
        }
        foreach (String a in inputAlphabet) // for each symbol in the input language 
        {
            if (/*checkForExpectationExistence*/)
            {
                if (!currentInputBatch.ContainsKey(a)) //     if an expectation exists from the previous state to the current state using input symbol a and a was not in the current input batch
                {
                    //         lower expectation using math

                    //         lower confidence using math
                }
            }

        }

        foreach (State q in states)
        {
            foreach (String a in inputAlphabet)
            {
                if (currentState.Equals(lastState) && a != lastInputSymbol) //     if (there's another way we could have gotten here) a state exists within state machine, 
                {
                    if (/*checkForExpectationExistence*/)
                    {
                        /*DO MATH*/
                        //         lower
                    }
                }
            }
        }

        foreach (String a in inputAlphabet)
        {
            foreach (String b in inputAlphabet)         //     if two or more input symbols exists in input language and they have a previous expectation relationship
            {
                if (b == a)
                {
                    break;
                }
                if (inputAlphabet.Contains(a) && inputAlphabet.Contains(b))         //         if both symbols exist in input batch 
                {
                    if (/*checkForExpectationExistence*/)
                    {
                        //             increase expectation
                        //             increase confidence
                    }
                    else            
                    {
                        expectation[x,y] = expectationLearnFactor;
                    }
                }
                else(inputAlphabet.Contains(a) || inputAlphabet.Contains(b)) //         if both symbols did not exist in input batch
                {
                    if (/*checkForExpectationExistence*/)
                    {

                    }
                    //             decrease expectation
                        //             decrease confidence
                }
            }
        }

        // ///Expectations are how transitions relate to each other
        // Symbols are how transitions relate to states












    }
    private void Output()
    {
        Debug.LogWarning("Current Number of States in the System: " + states.Count);
        Debug.LogWarning("Current Number of Transitions in the System: " + transitions.Count);
        Debug.LogWarning("Current State has {0} transitions. " + currentState.GetInTransitions().Count);
        Debug.LogWarning("Current Output Symbol is: " + currentOutputSymbol);
    }







}

