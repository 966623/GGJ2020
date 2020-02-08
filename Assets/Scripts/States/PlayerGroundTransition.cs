﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundTransition : StateTransition
{

    Player player;
    public PlayerGroundTransition(Player owner): base(owner.gameObject)
    {
        player = owner;
    }

    public override bool ToTransition()
    {
        return player.movement.grounded && player.damageState.damageTimer > 0.1f;
    }

}
