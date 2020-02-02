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

    public Sprite brokenSprite;
    public Sprite okSprite;
    public Sprite fixedSprite;
    public SpriteRenderer spriteRenderer;
    public Status status = Status.BROKEN;

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
    }
    // Start is called before the first frame update
    void Start()
    {
        SetStatus(status, effect);
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
                spriteRenderer.sprite = brokenSprite;
                effect = Effect.NONE;
                break;
            case Status.OK:
                spriteRenderer.sprite = okSprite;
                effect = Effect.NONE;
                break;
            case Status.FIXED:
                spriteRenderer.sprite = fixedSprite;
                effect = newEffect;
                break;
            default:
                break;
        }
    }

}
