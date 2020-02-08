using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    GameObject owner;
    public State(GameObject initOwner)
    {
        owner = initOwner;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnUpdate(float deltaTime) { }
}
