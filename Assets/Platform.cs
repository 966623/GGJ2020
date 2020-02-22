using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public enum Effect
    {
        NONE,
        DASH,
        BOUNCE
    }


    public new SpriteRenderer renderer;
    public SpriteRenderer rippedRenderer;
    PlatformPhysics platformPhysics;
    public BoxCollider2D physicsCollider;
    BoxCollider2D interactionCollider;
    public bool IsFixedInitial = false;
    public Effect InitialEffect = Effect.NONE;

    public SpriteRenderer tapeRenderer;
    public SpriteRenderer upgradeRenderer;
    public List<Sprite> upgradeSprites = new List<Sprite>();

    public Player collidingPlayer;

    Effect _currentEffect = Effect.NONE;
    public Effect currentEffect
    {
        get => _currentEffect;
        set
        {
            _currentEffect = value;
            upgradeRenderer.sprite = upgradeSprites[(int)value];
            if (collidingPlayer != null)
            {
                ApplyPlayerEffect(collidingPlayer);
            }
        }
    }

    bool _isFixed = false;
    public bool isFixed
    {
        get => _isFixed;
        set
        {
            _isFixed = value;

            if (_isFixed)
            {
                tapeRenderer.enabled = true;
                renderer.enabled = true;
                rippedRenderer.enabled = false;
                physicsCollider.enabled = true;
            }
            else
            {
                tapeRenderer.enabled = false;
                renderer.enabled = false;
                rippedRenderer.enabled = true;
                physicsCollider.enabled = false;
            }
        }
    }

    private void Awake()
    {
        interactionCollider = GetComponent<BoxCollider2D>();

        physicsCollider.size = interactionCollider.size;
        physicsCollider.offset = interactionCollider.offset;
        platformPhysics = physicsCollider.GetComponent<PlatformPhysics>();
        platformPhysics.OnCollisionEnter += PlatformPhysics_OnCollisionEnter;
        platformPhysics.OnCollisionStay += PlatformPhysics_OnCollisionStay;
        isFixed = IsFixedInitial;
        currentEffect = InitialEffect;
    }

    private void PlatformPhysics_OnCollisionStay(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collidingPlayer = collision.gameObject.GetComponent<Player>();
        }
        else
        {
            collidingPlayer = null;
        }
    }

    private void PlatformPhysics_OnCollisionEnter(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            ApplyPlayerEffect(collision.gameObject.GetComponent<Player>());
        }
    }

    void ApplyPlayerEffect(Player player)
    {

        if (player.movement.Position.y >= transform.position.y + 1f)
        {
            switch (currentEffect)
            {
                case Platform.Effect.NONE:
                    player.movement.jumpModifier = 1;
                    player.SpeedModifier = 1;
                    break;
                case Platform.Effect.DASH:
                    player.SpeedModifier = 3;
                    break;
                case Platform.Effect.BOUNCE:
                    player.movement.jumpModifier = 3;
                    if (player.SpeedModifier > 1 || !player.Grounded)
                    {
                        player.bounceAudio.PlayRandomClip(player.audioSource);
                        player.Grounded = true;
                        player.Jump();
                        //player.movement.Velocity = new Vector2(player.movement.Velocity.x, 0);
                    }
                    break;
                default:
                    break;
            }
        }
        
        else
        {
            player.SpeedModifier = 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}

