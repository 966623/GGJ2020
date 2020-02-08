using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadTransition : StateTransition
{

    Player player;
    public PlayerDeadTransition(Player owner): base(owner.gameObject)
    {
        player = owner;
    }

    public override bool ToTransition()
    {
        return player.Health <= 0;
    }

}
