using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerUntapingState : State
{
    Player player;

    public PlayerUntapingState(Player owner) : base(owner.gameObject)
    {
        player = owner;
    }
    public override void OnEnter()
    {
        player.movement.StopMovement();
        TryApplyDonwgrade(player.tapeQueue);
    }

    public override void OnExit()
    {
        player.StopAllCoroutines();
    }

    private void TryApplyDonwgrade(Platform.Effect effect)
    {
        Platform hitPlatform = player.GetInteractablePlatform((Platform p) =>
        {
            return p.isFixed;
        });

        if (hitPlatform == null)
        {
            player.SetState(player.moveState);
            return;
        }

        player.StartCoroutine(DowngradePlatform(hitPlatform));
    }

    IEnumerator DowngradePlatform(Platform platform)
    {
        player.untapeAudio.PlayRandomClip(player.audioSource);

        player.animator?.SetTrigger("Downgrade");
        yield return new WaitForSeconds(0.5f);

        switch (platform.currentEffect)
        {
            case Platform.Effect.NONE:
                player.AddTape(platform.currentEffect);
                platform.isFixed = false;
                break;
            default:
                player.AddTape(platform.currentEffect);
                platform.currentEffect = Platform.Effect.NONE;
                break;
        }

        player.SetState(player.moveState);

    }

    public override void OnUpdate(float deltaTime)
    {

    }

}
