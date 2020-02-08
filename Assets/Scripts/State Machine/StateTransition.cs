using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateTransition
{
    GameObject owner;
    public StateTransition(GameObject initOwner)
    {
        owner = initOwner;
    }

    public abstract bool ToTransition();
}
