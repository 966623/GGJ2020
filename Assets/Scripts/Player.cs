using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class Player : MonoBehaviour
{
    Controls controls;
    Controls.GameplayActions gameplay => controls.Gameplay;
    Rigidbody2D body;
    new BoxCollider2D collider;
    new SpriteRenderer renderer;
    Animator animator;
    public float maxSpeed = 10f;
    public float jumpVelocity = 10f;
    public LayerMask groundLayers;
    public LayerMask platformLayer;
    bool canMove = true;



    public int initialTapeCount = 0;
    public int initialBounceTapeCount = 0;
    public int initialDashTapeCount = 0;

    public UnityEvent TapeCountChanged;
    int _tapeCount = 0;
    public int tapeCount
    {
        get
        {
            return _tapeCount;
        }
        set
        {
            _tapeCount = value;
            TapeCountChanged.Invoke();
        }
    }

    int _bounceTapeCount = 0;
    public int bounceTapeCount
    {
        get
        {
            return _bounceTapeCount;
        }
        set
        {
            _bounceTapeCount = value;
            TapeCountChanged.Invoke();
        }
    }

    int _dashTapeCount = 0;
    public int dashTapeCount
    {
        get
        {
            return _dashTapeCount;
        }
        set
        {
            _dashTapeCount = value;
            TapeCountChanged.Invoke();
        }
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
        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gameplay.Jump.performed += Jump;
        gameplay.Downgrade.performed += TryDowngrade;
        gameplay.ApplyTape.performed += TryApplyTape;
        gameplay.BounceTape.performed += ApplyBounce;
        gameplay.DashTape.performed += ApplyDash;

        tapeCount = initialTapeCount;
        bounceTapeCount = initialBounceTapeCount;
        dashTapeCount = initialDashTapeCount;
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
        if (!canMove || !grounded || tapeCount <= 0)
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

        StartCoroutine(FixPlatform(hitPlatform));
    }

    IEnumerator UpgradePlatform(Platform platform, Platform.Effect effect)
    {
        canMove = false;
        hInput = 0;
        animator?.SetTrigger("Upgrade");
        yield return new WaitForSeconds(0.5f);
        tapeCount--;
        platform.currentEffect = effect;
        canMove = true;
    }

    private void TryApplyTape(InputAction.CallbackContext obj)
    {
        if (!canMove || !grounded || tapeCount <= 0)
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
        canMove = false;
        hInput = 0;
        animator?.SetTrigger("Upgrade");
        yield return new WaitForSeconds(0.5f);
        tapeCount--;
        platform.isFixed = true;
        canMove = true;
    }

    private void TryDowngrade(InputAction.CallbackContext obj)
    {
        if (!canMove || !grounded)
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
        canMove = false;
        hInput = 0;
        animator?.SetTrigger("Downgrade");
        yield return new WaitForSeconds(0.5f);

        switch (platform.currentEffect)
        {
            case Platform.Effect.NONE:
                tapeCount++;
                platform.isFixed = false;
                break;
            case Platform.Effect.DASH:
                break;
            case Platform.Effect.BOUNCE:
                break;
            default:
                break;
        }

        canMove = true;
    }

    public delegate bool PlatformConditionFunc(Platform platform);
    private Platform GetInteractablePlatform(PlatformConditionFunc condition)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(body.position + Facing * new Vector2(collider.size.x, 0), new Vector2(collider.size.x, collider.size.y * 0.5f), 0, Vector2.down, collider.size.y, platformLayer);
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
        if (!canMove)
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
            RaycastHit2D hit = Physics2D.BoxCast(body.position, collider.size, 0, Vector2.down, .02f, groundLayers);
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

        float targetVelocity = maxSpeed * hInput;
        float currentVelocity = body.velocity.x;
        float velocityDifference = targetVelocity - currentVelocity;
        body.AddForce(new Vector2(velocityDifference, 0), ForceMode2D.Impulse);


    }
}
