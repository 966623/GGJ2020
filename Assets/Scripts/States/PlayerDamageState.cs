using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerDamageState : State
{
    Player player;
    public PlayerDamageState(Player owner) : base(owner.gameObject)
    {
        player = owner;
    }
    public override void OnEnter()
    {
        player.movement.StopMovement();
        player.StartCoroutine(DamageCoroutine(player.lastHazardHit));
    }

    public override void OnExit()
    {
        player.lastHazardHit = null;
    }

    IEnumerator MakeInvincible()
    {
        player.invincible = true;
        player.renderer.color = new Color(1, 0.5f, 0.5f, 0.5f);
        yield return new WaitForSeconds(1f);
        player.invincible = false;
        player.renderer.color = new Color(1, 1, 1, 1f);
    }

    IEnumerator DamageCoroutine(Transform damageSourceTransform)
    {
        player.hurtAudio.PlayRandomClip(player.audioSource);
        player.Health--;
        player.animator?.SetTrigger("Flinch");
        player.movement.ForceMove(new Vector2(4f * Mathf.Sign(player.movement.Position.x - damageSourceTransform.position.x), 4f));
        player.StartCoroutine(MakeInvincible());
        yield return new WaitForSeconds(0.5f);
        player.SetState(player.moveState);
    }

    public override void OnUpdate(float deltaTime)
    {
    }

}
