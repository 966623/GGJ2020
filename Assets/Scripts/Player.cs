using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    public enum State
    {
        MOVING,
        TAPING,
        DAMAGE,
        DEAD,
        WIN
    }

    public State currentState = State.MOVING;
    bool invincible = false;
    Controls controls;
    Controls.GameplayActions gameplay => controls.Gameplay;
    Rigidbody2D body;
    new PolygonCollider2D collider;
    public Vector2 colliderSize = new Vector2(1, 1);
    new SpriteRenderer renderer;
    Animator animator;
    public float maxSpeed = 10f;
    public float jumpVelocity = 10f;
    public LayerMask groundLayers;
    public LayerMask platformLayer;

    public List<int> initialTapeCount = new List<int>();

    public delegate void TapeCountHandler(Platform.Effect effect);
    public event TapeCountHandler TapeCountIncrement;
    public event TapeCountHandler TapeCountDecrement;

    int[] tapeInventory = new int[System.Enum.GetNames(typeof(Platform.Effect)).Length];

    public int GetTapeCount(Platform.Effect effect = Platform.Effect.NONE)
    {
        return tapeInventory[(int)effect];
    }

    public void AddTape(Platform.Effect effect = Platform.Effect.NONE)
    {
        tapeInventory[(int)effect]++;
        TapeCountIncrement(effect);
    }

    public void RemoveTape(Platform.Effect effect = Platform.Effect.NONE)
    {
        tapeInventory[(int)effect]--;
        TapeCountDecrement(effect);
    }



    float _hInput = 0;
    float hInput
    {
        get
        {
            return _hInput;
        }
        set
        {
            _hInput = value;
            if (_hInput < 0)
            {
                Facing = -1;
            }
            else if (_hInput > 0)
            {

                Facing = 1;
            }
            animator?.SetFloat("Speed", Mathf.Abs(_hInput));
        }
    }

    int _facing = -1;
    int Facing
    {
        get
        {
            return _facing;
        }
        set
        {
            _facing = value;
            if (_facing == 1)
            {
                renderer.flipX = true;
            }
            else if (_facing == -1)
            {
                renderer.flipX = false;
            }
        }
    }

    float hVelocity = 0;

    float _vVelocity = 0;
    float vVelocity
    {
        get
        {
            return _vVelocity;
        }
        set
        {
            _vVelocity = value;
            animator?.SetFloat("VerticalSpeed", value);
        }
    }

    bool _grounded = false;
    bool grounded
    {
        get
        {
            return _grounded;
        }
        set
        {
            _grounded = value;
            animator?.SetBool("Grounded", value);
        }
    }

    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gameplay.Jump.performed += Jump;
        gameplay.Downgrade.performed += TryDowngrade;
        gameplay.ApplyTape.performed += TryApplyTape;
        gameplay.BounceTape.performed += ApplyBounce;
        gameplay.DashTape.performed += ApplyDash;

        foreach (Platform.Effect effect in (Platform.Effect[])Platform.Effect.GetValues(typeof(Platform.Effect)))
        {
            tapeInventory[(int)effect] = initialTapeCount[(int)effect];
        }
    }

    private void ApplyDash(InputAction.CallbackContext obj)
    {
        TryApplyUpgrade(Platform.Effect.DASH);
    }

    private void ApplyBounce(InputAction.CallbackContext obj)
    {
        TryApplyUpgrade(Platform.Effect.BOUNCE);
    }

    void TryApplyUpgrade(Platform.Effect effect)
    {
        if (currentState != State.MOVING || !grounded || tapeInventory[(int)effect] <= 0)
        {
            return;
        }

        Platform hitPlatform = GetInteractablePlatform((Platform p) =>
        {
            return p.isFixed && p.currentEffect == Platform.Effect.NONE;
        });

        if (hitPlatform == null)
        {
            return;
        }

        StartCoroutine(UpgradePlatform(hitPlatform, effect));
    }

    IEnumerator UpgradePlatform(Platform platform, Platform.Effect effect)
    {
        currentState = State.TAPING;
        hInput = 0;
        animator?.SetTrigger("Upgrade");
        yield return new WaitForSeconds(0.5f);

        if (currentState == State.TAPING)
        {
            RemoveTape(effect);
            platform.currentEffect = effect;
            currentState = State.MOVING;
        }
    }

    private void TryApplyTape(InputAction.CallbackContext obj)
    {
        if (currentState != State.MOVING || !grounded || tapeInventory[(int)Platform.Effect.NONE] <= 0)
        {
            return;
        }

        Platform hitPlatform = GetInteractablePlatform((Platform p) =>
        {
            return !p.isFixed;
        });

        if (hitPlatform == null)
        {
            return;
        }

        StartCoroutine(FixPlatform(hitPlatform));
    }

    IEnumerator FixPlatform(Platform platform)
    {
        currentState = State.TAPING;
        hInput = 0;
        animator?.SetTrigger("Upgrade");
        yield return new WaitForSeconds(0.5f);
        if (currentState == State.TAPING)
        {
            RemoveTape(Platform.Effect.NONE);
            platform.isFixed = true;
            currentState = State.MOVING;
        }
    }

    private void TryDowngrade(InputAction.CallbackContext obj)
    {
        if (currentState != State.MOVING || !grounded)
        {
            return;
        }

        Platform hitPlatform = GetInteractablePlatform((Platform p) =>
        {
            return p.isFixed;
        });

        if (hitPlatform == null)
        {
            return;
        }

        StartCoroutine(DowngradePlatform(hitPlatform));
    }

    IEnumerator DowngradePlatform(Platform platform)
    {
        currentState = State.TAPING;
        hInput = 0;
        animator?.SetTrigger("Downgrade");
        yield return new WaitForSeconds(0.5f);

        if (currentState == State.TAPING)
        {
            switch (platform.currentEffect)
            {
                case Platform.Effect.NONE:
                    AddTape(platform.currentEffect);
                    platform.isFixed = false;
                    break;
                case Platform.Effect.DASH:
                    AddTape(platform.currentEffect);
                    platform.currentEffect = Platform.Effect.NONE;
                    break;
                case Platform.Effect.BOUNCE:
                    AddTape(platform.currentEffect);
                    platform.currentEffect = Platform.Effect.NONE;
                    break;
                default:
                    break;
            }

            currentState = State.MOVING;
        }
    }

    public delegate bool PlatformConditionFunc(Platform platform);
    private Platform GetInteractablePlatform(PlatformConditionFunc condition)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(body.position + Facing * new Vector2(colliderSize.x, 0), new Vector2(colliderSize.x, colliderSize.y * 0.5f), 0, Vector2.down, colliderSize.y, platformLayer);
        foreach (var hit in hits)
        {
            Platform hitPlatform = hit.collider.gameObject.GetComponent<Platform>();
            if (hitPlatform != null && condition(hitPlatform))
            {
                return hitPlatform;
            }
        }
        return null;
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (grounded)
        {

            body.AddForce(new Vector2(0, jumpVelocity), ForceMode2D.Impulse);
        }

    }

    public void TakeDamage(GameObject damageSource)
    {
        if (currentState == State.DAMAGE || currentState == State.DEAD || currentState == State.WIN || invincible)
        {
            return;
        }

        StartCoroutine(DamageCoroutine(damageSource.transform));
    }

    IEnumerator DamageCoroutine(Transform damageSourceTransform)
    {
        currentState = State.DAMAGE;
        hVelocity = 0;
        animator?.SetTrigger("Flinch");
        body.AddForce(new Vector2(2f * Mathf.Sign(transform.position.x - damageSourceTransform.position.x), 2f), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        currentState = State.MOVING;
        StartCoroutine(MakeInvincible());
    }

    IEnumerator MakeInvincible()
    {
        invincible = true;
        renderer.color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(1f);
        invincible = false;
        renderer.color = new Color(1, 1, 1, 1f);
    }

    private void OnEnable()
    {
        gameplay.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != State.MOVING)
        {
            return;
        }

        hInput = gameplay.Move.ReadValue<Vector2>().x;
        vVelocity = body.velocity.y;
    }

    private void FixedUpdate()
    {
        // Ground check
        if (body.velocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.BoxCast(body.position, colliderSize, 0, Vector2.down, .02f, groundLayers);
            if (hit.collider != null)
            {
                grounded = true;
            }
            else
            {
                grounded = false;
            }
        }
        else
        {
            grounded = false;
        }

        if (currentState == State.DAMAGE)
        {
            if (grounded)
            {
                body.velocity = Vector2.zero;
            }
        }

        if (currentState == State.TAPING)
        {
            body.velocity = Vector2.zero;

        }

        else if (currentState == State.MOVING)
        {
            float targetVelocity = maxSpeed * hInput;
            float currentVelocity = body.velocity.x;
            float velocityDifference = targetVelocity - currentVelocity;
            body.AddForce(new Vector2(velocityDifference, 0), ForceMode2D.Impulse);
        }


    }
}
