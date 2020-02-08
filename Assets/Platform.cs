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
    public BoxCollider2D physicsCollider;
    BoxCollider2D interactionCollider;
    public bool IsFixedInitial = false;
    public Effect InitialEffect = Effect.NONE;

    public SpriteRenderer tapeRenderer;
    public SpriteRenderer upgradeRenderer;
    public List<Sprite> upgradeSprites = new List<Sprite>();

    Effect _currentEffect = Effect.NONE;
    public Effect currentEffect
    {
        get => _currentEffect;
        set
        {
            _currentEffect = value;
            upgradeRenderer.sprite = upgradeSprites[(int)value];
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
        physicsCollider.GetComponent<PlatformPhysics>().OnCollision += Platform_OnCollision;
        isFixed = IsFixedInitial;
        currentEffect = InitialEffect;
    }

    private void Platform_OnCollision(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && player.movement.Position.y >= transform.position.y + 1f)
        {
            switch (currentEffect)
            {
                case Effect.NONE:
                    break;
                case Effect.DASH:
                    player.SpeedModifier = 2;
                    break;
                case Effect.BOUNCE:
                    player.movement.ImpulseMove(new Vector2(0, player.jumpVelocity * 2f));

                    break;
                default:
                    break;
            }
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

