using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private int _state = -1;
    public int State { get; set; }
    public int PreviousState { get; private set; }

    bool StartCallNeeded;

    Func<int>[] updateCallbacks;
    Action[] endCallbacks, startCallbacks;
    Func<IEnumerator>[] coroutineCallbacks;
    Coroutine activeCoroutine;
    private int maxState;

    public StateMachine(int nStates)
    {
        maxState = nStates;
        updateCallbacks = new Func<int>[nStates];
        endCallbacks = new Action[nStates];
        startCallbacks = new Action[nStates];
        coroutineCallbacks = new Func<IEnumerator>[nStates];
    }

    public void SetCallbacks(int state, Func<int> updateCallback, Func<IEnumerator> coroutineCallback, Action startCallback, Action endCallback)
    {
        updateCallbacks[state] = updateCallback;
        endCallbacks[state] = endCallback;
        startCallbacks[state] = startCallback;
        coroutineCallbacks[state] = coroutineCallback;
    }

    public void Update()
    {
        if (State != _state)
        {
            //end  callback
            if (!StateOutOfBounds()) endCallbacks[_state]?.Invoke();
            
            PreviousState = _state;

            _state = State;

            //start callback
            if (!StateOutOfBounds()) startCallbacks[_state]?.Invoke();

        }
        //update callback
        if (!StateOutOfBounds() && updateCallbacks[_state] != null)
        {
            State = updateCallbacks[_state]();
        }
    }

    public IEnumerator CoroutineHandler()
    {
        while (true)
        {
            if (StateOutOfBounds() || coroutineCallbacks[_state] == null)
            {
                yield return null;
            }
            else
            {
                yield return coroutineCallbacks[_state]();
            }
        }
    }

    private bool StateOutOfBounds()
    {
        return _state < 0 || _state >= maxState;
    }
}
