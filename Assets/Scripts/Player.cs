using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class Player : StateMachine
{
    Controls controls;
    public Controls.GameplayActions gameplay => controls.Gameplay;

    [HideInInspector]
    public bool invincible = false;

    [HideInInspector]
    public MovementController movement;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public new SpriteRenderer renderer;
    public float maxSpeed = 10f;
    public float jumpVelocity = 10f;
    public LayerMask platformLayer;
    public LayerMask platformPhysicsLayer;

    [HideInInspector]
    public float airTime = 0;

    [HideInInspector]
    float _speedModifier = 1;
    public float SpeedModifier
    {
        get => _speedModifier;
        set
        {
            _speedModifier = value;
        }

    }

    public event StandardEvent OnKeyGet;
    bool _hasKey;
    public bool HasKey
    {
        get => _hasKey;
        set
        {
            _hasKey = value;
            if (_hasKey)
            {
                OnKeyGet?.Invoke();
            }
        }
    }

    int _health = 3;
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
        }
    }

    #region States
    public PlayerMoveState moveState;
    public PlayerTapingState tapingState;
    public PlayerUntapingState untapingState;
    public PlayerDamageState damageState;
    public PlayerDeadState deadState;

    public PlayerGroundTransition groundTransition;
    public PlayerDamageTransition damageTransition;
    public PlayerDeadTransition deadTransition;
    #endregion States

    #region Tape
    public List<int> initialTapeCount = new List<int>();

    public delegate void TapeCountHandler(Platform.Effect effect);
    public event TapeCountHandler TapeCountIncrement;
    public event TapeCountHandler TapeCountDecrement;

    [HideInInspector]
    public Platform.Effect tapeQueue;
    [HideInInspector]
    public Transform lastHazardHit;

    public int[] tapeInventory = new int[System.Enum.GetNames(typeof(Platform.Effect)).Length];

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



    public delegate bool PlatformConditionFunc(Platform platform);
    public Platform GetInteractablePlatform(PlatformConditionFunc condition)
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(movement.Position + Facing * new Vector2(movement.colliderSize.x, 0), new Vector2(movement.colliderSize.x, movement.colliderSize.y * 0.5f), 0, Vector2.down, movement.colliderSize.y, platformLayer);
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

    #endregion Tape


    private void Awake()
    {
        // init controls
        controls = new Controls();
        controls.Enable();

        // init components
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        movement = GetComponent<MovementController>();
        movement.OnGrounded += (g) => { animator?.SetBool("Grounded", true); };
        movement.OnGrounded += (g) => { airTime = 0; };
        movement.OnLeaveGrounded += (g) => { animator?.SetBool("Grounded", false); };

        // init tape inventory
        foreach (Platform.Effect effect in (Platform.Effect[])Platform.Effect.GetValues(typeof(Platform.Effect)))
        {
            tapeInventory[(int)effect] = initialTapeCount[(int)effect];
        }

        // init states
        moveState = new PlayerMoveState(this);
        tapingState = new PlayerTapingState(this);
        untapingState = new PlayerUntapingState(this);
        damageState = new PlayerDamageState(this);
        deadState = new PlayerDeadState(this);

        //init transitions
        groundTransition = new PlayerGroundTransition(this);
        damageTransition = new PlayerDamageTransition(this);
        deadTransition = new PlayerDeadTransition(this);

        // init state map
        stateMap.Add(damageState, new List<TransitionStatePair>()
            {
                new TransitionStatePair(groundTransition, moveState)
            }
        );

        stateMap.Add(moveState, new List<TransitionStatePair>()
            {
                new TransitionStatePair(damageTransition, damageState)
            }
        );


        stateMap.Add(tapingState, new List<TransitionStatePair>()
            {
                new TransitionStatePair(damageTransition, damageState)
            }
        );


        stateMap.Add(untapingState, new List<TransitionStatePair>()
            {
                new TransitionStatePair(damageTransition, damageState)
            }
        );

        globalTransitions.Add(new TransitionStatePair(deadTransition, deadState));

        SetState(moveState);
    }

    public void TakeDamage(GameObject damageSource)
    {
        if (invincible)
        {
            return;
        }

        lastHazardHit = damageSource.transform;
    }

    private void OnEnable()
    {
        gameplay.Enable();
    }

    public override void OnUpdate(float deltaTime)
    {
        animator.SetFloat("VerticalSpeed", movement.Velocity.y);
    }

    private void FixedUpdate()
    {
        if (movement.grounded)
        {
        }
        else
        {
            airTime += Time.deltaTime;
        }
    }

    #region Movement

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
    public int Facing
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

    public void Move(float scale)
    {
        hInput = scale;
        float targetVelocity = maxSpeed * hInput * SpeedModifier;
        float currentVelocity = movement.Velocity.x;
        float velocityDifference = targetVelocity - currentVelocity;
        movement.ImpulseMove(new Vector2(velocityDifference, 0));
    }

    public void Jump(InputAction.CallbackContext obj)
    {
        if (movement.grounded)
        {
            movement.ImpulseMove(new Vector2(0, jumpVelocity));
        }
    }

    #endregion
}
