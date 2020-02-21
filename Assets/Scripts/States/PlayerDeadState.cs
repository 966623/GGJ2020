using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//[CreateAssetMenu(fileName = "PlayerMoveState", menuName = "States/Player/Move", order = 1)]
public class PlayerDeadState : State
{
    Player player;
    public PlayerDeadState(Player owner) : base(owner.gameObject)
    {
        player = owner;
    }
    public override void OnEnter()
    {
        player.renderer.color = new Color(1, 1, 1, 1f);
        player.movement.StopMovement();
        player.StartCoroutine(DeathCoroutine());
    }

    public override void OnExit()
    {
    }

  
    IEnumerator DeathCoroutine()
    {
        player.dieAudio.PlayRandomClip(player.audioSource);
        player.animator?.SetTrigger("Dead");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public override void OnUpdate(float deltaTime)
    {
    }

}
