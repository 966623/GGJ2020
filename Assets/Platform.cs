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

    public PlatformSkin skin;
    public new SpriteRenderer renderer;
    public BoxCollider2D physicsCollider;
    public BoxCollider2D interactionCollider;
    public bool IsFixedInitial = false;
    public Effect InitialEffect = Effect.NONE;

    public Effect currentEffect = Effect.NONE;
    bool _isFixed = false;
    public bool isFixed
    {
        get
        {
            return _isFixed;
        }
        set
        {
            _isFixed = value;

            if (_isFixed)
            {
                renderer.sprite = skin.fixedSprite;
                renderer.transform.localPosition = skin.offset;
                physicsCollider.enabled = true;
            }
            else
            {
                renderer.sprite = skin.rippedSprite;
                renderer.transform.localPosition = skin.rippedOffset;
                physicsCollider.enabled = false;
            }
        }
    }
    private void Awake()
    {
        renderer.sprite = skin.fixedSprite;
        renderer.transform.localPosition = skin.offset;
        renderer.transform.localScale = new Vector3(skin.spriteScale, skin.spriteScale, 1);
        physicsCollider.size = new Vector2(2, skin.colliderScale);
        physicsCollider.offset = new Vector2(0, 1 - skin.colliderScale / 2);
        interactionCollider.size = new Vector2(2, skin.colliderScale);
        interactionCollider.offset = new Vector2(0, 1 - skin.colliderScale / 2);

        isFixed = IsFixedInitial;
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

