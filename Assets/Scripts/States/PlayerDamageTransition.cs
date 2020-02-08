using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageTransition : StateTransition
{

    Player player;
    public PlayerDamageTransition(Player owner): base(owner.gameObject)
    {
        player = owner;
    }

    public override bool ToTransition()
    {
        return player.lastHazardHit != null;
    }

}
