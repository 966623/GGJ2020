using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerTapingState : State
{
    Player player;

    public PlayerTapingState(Player owner) : base(owner.gameObject)
    {
        player = owner;
    }
    public override void OnEnter()
    {
        player.movement.Velocity = new Vector2(0, 0);
        TryApplyUpgrade(player.tapeQueue);
    }

    public override void OnExit()
    {
        player.StopAllCoroutines();
    }

    void TryApplyUpgrade(Platform.Effect effect)
    {
        if (player.tapeInventory[(int)effect] <= 0)
        {
            player.SetState(player.moveState);
            return;
        }

        Platform hitPlatform;
        if (effect != Platform.Effect.NONE)
        {
            hitPlatform = player.GetInteractablePlatform((Platform p) =>
            {
                return p.isFixed && p.currentEffect == Platform.Effect.NONE;
            });
        }
        else
        {
            hitPlatform = player.GetInteractablePlatform((Platform p) =>
            {
                return !p.isFixed;
            });
        }

        if (hitPlatform == null)
        {
            player.SetState(player.moveState);
            return;
        }

        player.StartCoroutine(UpgradePlatform(hitPlatform, effect));
    }

    IEnumerator UpgradePlatform(Platform platform, Platform.Effect effect)
    {
        player.animator?.SetTrigger("Upgrade");
        yield return new WaitForSeconds(0.5f);

        player.RemoveTape(effect);

        platform.isFixed = true;
        platform.currentEffect = effect;
        player.SetState(player.moveState);    
    }

    public override void OnUpdate(float deltaTime)
    {

    }

}
