using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerMoveState : State
{
    Player player;



    public PlayerMoveState(Player owner) : base(owner.gameObject)
    {
        player = owner;
    }
    public override void OnEnter()
    {
        player.gameplay.Jump.performed += player.Jump;
        player.gameplay.ApplyTape.performed += StartApplyTape;
        player.gameplay.BounceTape.performed += StartApplyTape;
        player.gameplay.DashTape.performed += StartApplyTape;
        player.gameplay.Downgrade.performed += StartDowngrade;

    }

    private void StartDowngrade(InputAction.CallbackContext obj)
    {
        if (!player.movement.grounded) return;
        player.SetState(player.untapingState);
    }

    public override void OnExit()
    {
        player.gameplay.Jump.performed -= player.Jump;
        player.gameplay.ApplyTape.performed -= StartApplyTape;
        player.gameplay.BounceTape.performed -= StartApplyTape;
        player.gameplay.DashTape.performed -= StartApplyTape;
        player.gameplay.Downgrade.performed -= StartDowngrade;
    }

    void StartApplyTape(InputAction.CallbackContext obj)
    {
        if (!player.movement.grounded) return;

        if (obj.action == player.gameplay.ApplyTape)
        {
            player.tapeQueue = Platform.Effect.NONE;
        }
        else if (obj.action == player.gameplay.BounceTape)
        {
            player.tapeQueue = Platform.Effect.BOUNCE;
        }
        else if (obj.action == player.gameplay.DashTape)
        {
            player.tapeQueue = Platform.Effect.DASH;
        }

        player.SetState(player.tapingState);
    }

    public override void OnUpdate(float deltaTime)
    {
        float scale = player.gameplay.Move.ReadValue<Vector2>().x;
        player.Move(scale);
    }

}
