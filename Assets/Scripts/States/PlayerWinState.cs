using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerWinState : State
{
    Player player;
    public PlayerWinState(Player owner) : base(owner.gameObject)
    {
        player = owner;
    }
    public override void OnEnter()
    {
        player.StopAllCoroutines();
        player.invincible = true;
        player.movement.Velocity = new Vector2(0, 0);
        player.StartCoroutine(WinCoroutine());
    }

    public override void OnExit()
    {
    }

  
    IEnumerator WinCoroutine()
    {
        player.animator?.SetTrigger("Win");
        yield return new WaitForSeconds(1f);
    }

    public override void OnUpdate(float deltaTime)
    {
    }

}
