using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Skin playerSkin;
    Animator animator;
    Rigidbody2D body;
    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;
    public LayerMask wallMask;
    public LayerMask platformMask;
    private float initialSpeed = 10f;
    public float speed = 10f;
    public float startupTime = 0.1f;
    public bool onGround = false;
    public bool taping = false;
    float vVelocity = 0;
    public float gravity = -80f;
    public float jumpStr = 20f;
    public List<AudioClip> jumpAudio = new List<AudioClip>();
    public List<AudioClip> untapeAudio = new List<AudioClip>();
    public List<AudioClip> tapeAudio = new List<AudioClip>();
    public List<AudioClip> boingAudio = new List<AudioClip>();
    public List<AudioClip> dashAudio = new List<AudioClip>();
    AudioSource audioSource;
    public AudioSource runningAudio;

    int faceDir = 1;

    bool facingRight { get { return faceDir == 1; } set { faceDir = value ? 1 : -1; } }
    bool facingLeft { get { return faceDir == -1; } set { faceDir = value ? -1 : 1; } }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        if (taping)
        {
            return;
        }

    }

    float tapeTime = 0;
    delegate void TapeAction();
    TapeAction tapeAction;
    public bool hasKey;

    private void Update()
    {
        if (taping)
        {
            runningAudio.volume = 0;
            tapeTime += Time.deltaTime;
            if (tapeTime >= 0.5)
            {
                tapeTime = 0;
                taping = false;
                tapeAction();
            }
            return;
        }

        float xMove = 0;
        float yMove = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            facingRight = true;
            xMove += speed * Time.deltaTime;
            animator.runtimeAnimatorController = playerSkin.run;
            spriteRenderer.flipX = true;
            runningAudio.volume = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            facingLeft = true;
            xMove += -speed * Time.deltaTime;
            spriteRenderer.flipX = false;
            animator.runtimeAnimatorController = playerSkin.run;
            runningAudio.volume = 1;
        }

        if (xMove == 0)
        {
            animator.runtimeAnimatorController = playerSkin.idle;
        }

        if (xMove == 0 || !onGround)
        {
            runningAudio.volume = 0;
        }



        //jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SpacePressed");
            if (onGround)
            {
                vVelocity = jumpStr;
                onGround = false;
                if (speed > initialSpeed)
                {
                    audioSource.clip = dashAudio[Random.Range(0, 2)];
                    audioSource.Play();
                }
                else
                {
                    audioSource.clip = jumpAudio[Random.Range(0, 3)];
                    audioSource.Play();
                }
            }
        }

        vVelocity += gravity * Time.deltaTime;
        yMove = vVelocity * Time.deltaTime;

        RaycastHit2D floorHit = Physics2D.BoxCast(body.position, boxCollider.size, 0, Vector2.down, Mathf.Abs(yMove), wallMask);
        if (floorHit.collider != null && vVelocity < 0)
        {
            vVelocity = 0;
            yMove = -(floorHit.distance - 0.00001f);
            onGround = true;
        }


        body.MovePosition(body.position + new Vector2(xMove, yMove));

        floorHit = Physics2D.BoxCast(body.position, boxCollider.size, 0, Vector2.down, Mathf.Abs(0.1f), wallMask);
        if (floorHit.collider == null && vVelocity < 0 && onGround)
        {
            onGround = false;
        }

        RaycastHit2D hitBounce = Physics2D.BoxCast(body.position, boxCollider.size * 0.99f, 0, Vector2.down, 0.5f, platformMask);
        if (hitBounce.collider && onGround)
        {
            PlatformA platform = hitBounce.collider.GetComponent<PlatformA>();
            if (platform != null && platform.effect == PlatformA.Effect.BOUNCE)
            {
                audioSource.clip = boingAudio[Random.Range(0, 2)];
                audioSource.Play();
                vVelocity = jumpStr * 2;
                onGround = false;
            }
            if (platform != null && platform.effect == PlatformA.Effect.SPEED)
            {

                speed = initialSpeed * 3;
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


        //repair
        if (Input.GetKeyDown(KeyCode.Z) && onGround)
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
                tapeAction = () =>
                {

                    platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.BOUNCE);
                };

                taping = true;
                animator.runtimeAnimatorController = playerSkin.tape;
                audioSource.clip = tapeAudio[Random.Range(0, 5)];
                audioSource.Play();
            }



        }

        if (Input.GetKeyDown(KeyCode.X) && onGround)
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
                tapeAction = () =>
                {


                    platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.SPEED);
                };

                taping = true;
                animator.runtimeAnimatorController = playerSkin.tape;
                audioSource.clip = tapeAudio[Random.Range(0, 5)];
                audioSource.Play();
            }

        }


        //retrieve
        //repair
        if (Input.GetKeyDown(KeyCode.C) && onGround)
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                body.position + new Vector2(0, -0.5f), new Vector2(1, 1), 0, facingRight ? Vector2.right : Vector2.left,
                1, platformMask);
            if (hit.collider)
            {
                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null || platform.effect == PlatformA.Effect.NONE)
                {
                    return;
                }
                tapeAction = () =>
                {


                    platform.SetStatus(PlatformA.Status.BROKEN, PlatformA.Effect.NONE);
                };

                taping = true;
                animator.runtimeAnimatorController = playerSkin.untape;
                audioSource.clip = untapeAudio[Random.Range(0, 4)];
                audioSource.Play();
            }

        }
    }
}
