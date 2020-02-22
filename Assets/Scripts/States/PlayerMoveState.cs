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
        player.OnCollision += Player_OnCollision;
        player.runAudioSource.Play();
        player.runAudioSource.Pause();
    }


    public override void OnExit()
    {
        player.gameplay.Jump.performed -= player.Jump;
        player.gameplay.ApplyTape.performed -= StartApplyTape;
        player.gameplay.BounceTape.performed -= StartApplyTape;
        player.gameplay.DashTape.performed -= StartApplyTape;
        player.gameplay.Downgrade.performed -= StartDowngrade;

        player.SpeedModifier = 1;

        player.runAudioSource.Pause();

    }

    private void Player_OnCollision(Collision2D collision)
    {
        PlatformPhysics pp = collision.gameObject.GetComponent<PlatformPhysics>();
        if (pp != null)
        {
            //if (pp.mainPlatform.currentEffect != Platform.Effect.DASH)
            //{
            //    player.SpeedModifier = 1;
            //}
            //if (pp.mainPlatform.currentEffect != Platform.Effect.BOUNCE)
            //{
            //    player.jumpModifier = 1;
            //}
        }
        else
        {

            RaycastHit2D hit = Physics2D.BoxCast(player.movement.Position - new Vector2(0, player.movement.colliderSize.y * 0.25f), new Vector2(player.movement.colliderSize.x * 0.5f, player.movement.colliderSize.y * 0.5f), 0, Vector2.down, .02f, player.movement.groundLayers);
            if (hit.collider != null && hit.collider.gameObject.GetComponent<PlatformPhysics>() == null)
            {
                player.SpeedModifier = 1;
            }
            else
            {
            }
            player.movement.jumpModifier = 1;
        }


    }

    private void StartDowngrade(InputAction.CallbackContext obj)
    {
        if (!player.movement.OnGround)
        {
            return;
        }

        player.SetState(player.untapingState);
    }


    void StartApplyTape(InputAction.CallbackContext obj)
    {
        if (!player.movement.OnGround)
        {
            return;
        }

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

    bool isRunning = false;

    public override void OnUpdate(float deltaTime)
    {
        float scale = player.gameplay.Move.ReadValue<Vector2>().x;
        player.Move(scale);
        if (!player.Grounded)
        {
            player.runAudioSource.Pause();
            return;
        }
        if (scale != 0)
        {
           
            player.runAudioSource.UnPause();
        }
        else
        {

            player.runAudioSource.Pause();
        }


    }
}
