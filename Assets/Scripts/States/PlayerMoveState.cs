using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerMoveState : State
{
    Player player;

    float timeRunning = 0;
    float timeStopped = 0;

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
    }


    public override void OnExit()
    {
        player.gameplay.Jump.performed -= player.Jump;
        player.gameplay.ApplyTape.performed -= StartApplyTape;
        player.gameplay.BounceTape.performed -= StartApplyTape;
        player.gameplay.DashTape.performed -= StartApplyTape;
        player.gameplay.Downgrade.performed -= StartDowngrade;

        player.SpeedModifier = 1;

        player.runAudioSource.Stop();

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

            player.SpeedModifier = 1;
            player.jumpModifier = 1;
        }


    }

    private void StartDowngrade(InputAction.CallbackContext obj)
    {
        if (!player.movement.grounded)
        {
            return;
        }

        player.SetState(player.untapingState);
    }


    void StartApplyTape(InputAction.CallbackContext obj)
    {
        if (!player.movement.grounded)
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
        if (scale != 0)
        {
            if (!isRunning)
            {
                if (timeRunning + timeStopped > 0.5f)
                {
                    player.runAudioSource.Play();
                }
                timeRunning = 0;
            }
            timeRunning += deltaTime;
            isRunning = true;
        }
        else
        {
            if (isRunning)
            {
                timeStopped = 0;
            }
            if (timeRunning + timeStopped > 0.5f)
            {
                player.runAudioSource.Stop();
            }
            timeStopped += deltaTime;
            isRunning = false;
        }


    }

    IEnumerator StopRunAudio(float remainingTime)
    {
        yield return new WaitForSeconds(remainingTime);
        player.runAudioSource.Stop();
    }
}
