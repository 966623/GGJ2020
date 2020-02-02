using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformA : MonoBehaviour
{
    public enum Status
    {
        BROKEN,
        OK,
        FIXED
    }

    public enum Effect
    {
        NONE,
        BOUNCE,
        SPEED
    }

    public BoxCollider2D boxCollider2D;
    public Sprite brokenSprite;
    public Sprite okSprite;
    public Sprite fixedSprite;
    public SpriteRenderer spriteRenderer;
    public Status status = Status.BROKEN;
    public Effect initialEffect = Effect.NONE;
    Effect privEffect = Effect.NONE;
    public Effect effect
    {
        get { return privEffect; }

        set
        {
            switch (value)
            {
                case Effect.NONE:
                    spriteRenderer.color = Color.white;
                    break;
                case Effect.BOUNCE:
                    spriteRenderer.color = new Color(0, 200, 255);
                    break;
                case Effect.SPEED:
                    spriteRenderer.color = new Color(255, 69, 0);
                    break;
                default:
                    break;
            }
            privEffect = value;
        }
    }
    public void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetStatus(status, initialEffect);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetStatus(Status newStatus, Effect newEffect)
    {
        status = newStatus;

        switch (status)
        {
            case Status.BROKEN:
                boxCollider2D.isTrigger = true;
                spriteRenderer.sprite = brokenSprite;
                effect = Effect.NONE;
                break;
            case Status.OK:
                spriteRenderer.sprite = okSprite;
                effect = Effect.NONE;
                break;
            case Status.FIXED:
                boxCollider2D.isTrigger = false;
                spriteRenderer.sprite = fixedSprite;
                effect = newEffect;
                break;
            default:
                break;
        }
    }

}
