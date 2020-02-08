using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerDashState : State
{
    Player player;
    public PlayerDashState(Player owner) : base(owner.gameObject)
    {
        player = owner;
    }
    public override void OnEnter()
    {
        player.movement.ImpulseMove(new Vector2(player.maxSpeed * 2 * player.Facing, player.jumpVelocity * 0.5f));
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
    }

}
