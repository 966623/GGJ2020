using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int tapeCount = 0;
    public int tapeBounceCount = 0;
    public int tapeDashCount = 0;
    public Skin playerSkin;
    public TapeDisplay tapeDisplay;
    Animator animator;
    public Rigidbody2D body;
    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;
    public LayerMask wallMask;
    public LayerMask platformMask;
    private float initialSpeed = 10f;

    public float invulnTime = 2f;
    public float stunTime = 0.5f;
    public void TakeDamage()
    {
        if (invulnTime > 0)
        {
            return;
        }

        health--;
        if (health <= 0)
        {
            Destroy(gameObject);

        }
        vVelocity = jumpStr * 0.5f;
        onGround = false;
        audioSource.clip = hurtAudio[Random.Range(0, 3)];
        audioSource.Play();
        invulnTime = 2f;
        stunTime = 0.5f;
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
    }

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
    public List<AudioClip> hurtAudio = new List<AudioClip>();
    AudioSource audioSource;
    public AudioSource runningAudio;

    public int health = 3;
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

        for (int i = 0; i < tapeCount; i++)
        {
            tapeDisplay.g1Add();
        }
        for (int i = 0; i < tapeBounceCount; i++)
        {
            tapeDisplay.gbAdd();
        }
        for (int i = 0; i < tapeDashCount; i++)
        {
            tapeDisplay.gdAdd();
        }


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
        invulnTime -= Time.deltaTime;
        if (invulnTime <= 0)
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
        stunTime -= Time.deltaTime;
        if (stunTime > 0)
        {
            return;
        }

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

        body.gravityScale = 0;

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
        if (floorHit.collider != null && vVelocity < 0 && !floorHit.collider.isTrigger)
        {
            vVelocity = 0;
            yMove = -(floorHit.distance);
            onGround = true;
        }

        if (floorHit.collider != null && floorHit.collider.gameObject.CompareTag("Spike"))
        {
            TakeDamage();

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
                body.position + new Vector2(0, -0.5f), boxCollider.size * 0.99f, 0, facingRight ? Vector2.right : Vector2.left,
                boxCollider.size.x, platformMask);
            if (hit.collider && hit.distance > 0)
            {
               

                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null || platform.effect != PlatformA.Effect.NONE || tapeBounceCount <= 0)
                {
                    return;
                }
                tapeAction = () =>
                {

                    platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.BOUNCE);
                    tapeDisplay.gbRemove();
                    tapeBounceCount--;
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
                body.position + new Vector2(0, -0.5f), boxCollider.size * 0.99f, 0, facingRight ? Vector2.right : Vector2.left,
                boxCollider.size.x , platformMask);
            if (hit.collider && hit.distance > 0)
            {
               
                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null || platform.effect != PlatformA.Effect.NONE || tapeDashCount <= 0)
                {
                    return;
                }
                tapeAction = () =>
                {


                    platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.SPEED);
                    tapeDisplay.gdRemove();
                    tapeDashCount--;
                };

                taping = true;
                animator.runtimeAnimatorController = playerSkin.tape;
                audioSource.clip = tapeAudio[Random.Range(0, 5)];
                audioSource.Play();
            }

        }

        if (Input.GetKeyDown(KeyCode.C) && onGround)
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                body.position + new Vector2(0, -0.5f), boxCollider.size * 0.99f, 0, facingRight ? Vector2.right : Vector2.left,
                boxCollider.size.x, platformMask);
            if (hit.collider && hit.distance > 0)
            {
               
                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null || platform.effect != PlatformA.Effect.NONE || tapeCount <= 0)
                {
                    return;
                }
                tapeAction = () =>
                {


                    platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.NONE);
                    tapeDisplay.g1Remove();
                    tapeCount--;
                };

                taping = true;
                animator.runtimeAnimatorController = playerSkin.tape;
                audioSource.clip = tapeAudio[Random.Range(0, 5)];
                audioSource.Play();
            }

        }


        //retrieve
        //repair
        if (Input.GetKeyDown(KeyCode.V) && onGround)
        {
            RaycastHit2D hit = Physics2D.BoxCast(
                body.position + new Vector2(0, -0.5f), new Vector2(1, 1), 0, facingRight ? Vector2.right : Vector2.left,
                1, platformMask);
            if (hit.collider)
            {
                PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                if (platform == null || platform.status == PlatformA.Status.BROKEN)
                {
                    return;
                }
                tapeAction = () =>
                {

                    switch (platform.effect)
                    {
                        case PlatformA.Effect.NONE:
                            tapeDisplay.g1Add();
                            tapeCount++;
                            break;
                        case PlatformA.Effect.BOUNCE:
                            tapeDisplay.gbAdd();
                            tapeBounceCount++;
                            break;
                        case PlatformA.Effect.SPEED:
                            tapeDisplay.gdAdd();
                            tapeDashCount++;
                            break;
                        default:
                            break;
                    }
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
