using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StateMachine : MonoBehaviour
{
    public struct TransitionStatePair
    {
        public TransitionStatePair(StateTransition newTransition, State newState)
        {
            transition = newTransition;
            state = newState;
        }

        public StateTransition transition;
        public State state;
    }

    protected Dictionary<State, List<TransitionStatePair>> stateMap = new Dictionary<State, List<TransitionStatePair>>();
    protected List<TransitionStatePair> globalTransitions = new List<TransitionStatePair>();


    State currentState;

    private void Start()
    {
        //SetState(initialState);
    }

    public void Update()
    {
        // find the set of transitions for the current state
        OnUpdate(Time.deltaTime);

        List<TransitionStatePair> transitions;
        if (stateMap.ContainsKey(currentState) && (transitions = stateMap[currentState]) != null)
        {
            foreach (var pair in transitions)
            {
                if (pair.transition.ToTransition())
                {
                    SetState(pair.state);
                    break;
                }
            }
        }

        foreach (var pair in globalTransitions)
        {
            if (pair.transition.ToTransition())
            {
                SetState(pair.state);
                break;
            }
        }

        // update the current state
        if (currentState != null)
        {
            currentState.OnUpdate(Time.deltaTime);
        }
    }

    public virtual void OnUpdate(float deltaTime)
    {

    }

    public void SetState(State newState)
    {
        currentState?.OnExit();
        currentState = newState;
        newState?.OnEnter();
    }

}
