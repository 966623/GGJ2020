using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Skin playerSkin;
    Animator animator;
    Rigidbody2D body;
    BoxCollider2D collider;
    SpriteRenderer renderer;
    public LayerMask wallMask;
    public LayerMask platformMask;
    private float initialSpeed = 10f;
    public float speed = 10f;
    public float startupTime = 0.1f;
    public bool onGround = false;
    float vVelocity = 0;
    float gravity = -20f;

    int faceDir = 1;

    bool facingRight { get { return faceDir == 1; } set { faceDir = value ? 1 : -1; } }
    bool facingLeft { get { return faceDir == -1; } set { faceDir = value ? -1 : 1; } }


    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        initialSpeed = speed;
        animator.runtimeAnimatorController = playerSkin.idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float xMove = 0;
        float yMove = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            facingRight = true;
            xMove += speed * Time.fixedDeltaTime;
            animator.runtimeAnimatorController = playerSkin.run;
            renderer.flipX = true;
            //body.MovePosition(body.position + new Vector2(speed * Time.fixedDeltaTime, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            facingLeft = true;
            xMove += -speed * Time.fixedDeltaTime;
            renderer.flipX = false;
            animator.runtimeAnimatorController = playerSkin.run;
            //body.MovePosition(body.position + new Vector2(-speed * Time.fixedDeltaTime, 0));
        }

        if (xMove == 0)
        {
            animator.runtimeAnimatorController = playerSkin.idle;
        }




        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            vVelocity = 8f;
            onGround = false;
        }

        vVelocity += gravity * Time.fixedDeltaTime;
        yMove = vVelocity * Time.fixedDeltaTime;

        RaycastHit2D hit = Physics2D.BoxCast(body.position, collider.size * 0.95f, 0, Vector2.down, Mathf.Abs(yMove), wallMask);
        if (hit.collider && vVelocity <= 0)
        {
            vVelocity = 0;
            yMove = -hit.distance - 0.00001f;
            onGround = true;
        }

        body.MovePosition(body.position + new Vector2(xMove, yMove));


        RaycastHit2D hitBounce = Physics2D.BoxCast(body.position, collider.size * 0.99f, 0, Vector2.down, 0.5f, platformMask);
        if (hitBounce.collider && onGround)
        {
            PlatformA platform = hitBounce.collider.GetComponent<PlatformA>();
            if (platform != null && platform.effect == PlatformA.Effect.BOUNCE)
            {
                vVelocity = 12f;
                onGround = false;
            }
            if (platform != null && platform.effect == PlatformA.Effect.SPEED)
            {
                speed = initialSpeed * 2;
            }
            else
            {
                if (onGround)
                {
                    speed = initialSpeed;
                }
            }
        }
        else
        {
            if (onGround)
            {
                speed = initialSpeed;
            }
        }

        if (!onGround)
        {
            if (vVelocity > 0)
            {
                animator.runtimeAnimatorController = playerSkin.jump;
            }
            else
            {
                animator.runtimeAnimatorController = playerSkin.fall;
            }
        }
    }

    private void Update()
    {
        //repair
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                body.position + new Vector2(0, -0.5f), new Vector2(1, 1), 0, facingRight ? Vector2.right : Vector2.left,
                1, platformMask);
            if (hit.collider)
            {
                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null || platform.effect != PlatformA.Effect.NONE)
                {
                    return;
                }

                platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.BOUNCE);
            }

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                body.position + new Vector2(0, -0.5f), new Vector2(1, 1), 0, facingRight ? Vector2.right : Vector2.left,
                1, platformMask);
            if (hit.collider)
            {
                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null || platform.effect != PlatformA.Effect.NONE)
                {
                    return;
                }

                platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.SPEED);
            }

        }


        //retrieve
        //repair
        if (Input.GetKeyDown(KeyCode.C))
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                body.position + new Vector2(0, -0.5f), new Vector2(1, 1), 0, facingRight ? Vector2.right : Vector2.left,
                1, platformMask);
            if (hit.collider)
            {
                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null)
                {
                    return;
                }

                platform.SetStatus(PlatformA.Status.BROKEN, PlatformA.Effect.NONE);
            }

        }
    }
}
