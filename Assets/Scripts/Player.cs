using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int tapeCount = 0;
    public int tapeBounceCount = 0;
    public int tapeDashCount = 0;
    public Skin playerSkin;
    public TapeDisplay tapeDisplay;
    Animator animator;
    public Animator shadowAnim;
    public Rigidbody2D body;
    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;
    public SpriteRenderer shadowRenderer;
    public LayerMask wallMask;
    public LayerMask platformMask;
    private float initialSpeed = 10f;

    float invulnTime = 0f;

    internal void DoWin(string nextLevel)
    {
        isWin = true;
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Single);
    }

    public float stunTime = 0.5f;

    bool isDead;
    bool isWin;
    float deadTime = 2f;
    public bool TakeDamage()
    {
        if (invulnTime > 0)
        {
            return false;
        }

        health--;
        healthUI.SetHealth(health);
        if (health <= 0)
        {
            isDead = true;
            audioSource.clip = deadAudio[Random.Range(0, 2)];
            runningAudio.Stop();
            audioSource.Play();
            deadTime = 2f;
            animator.runtimeAnimatorController = playerSkin.death;
            shadowAnim.runtimeAnimatorController = playerSkin.death;
            return false;
        }
        vVelocity = jumpStr * 0.5f;
        onGround = false;
        audioSource.clip = hurtAudio[Random.Range(0, 3)];
        audioSource.Play();
        invulnTime = 2f;
        stunTime = 0.5f;
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        return true;
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
    public List<AudioClip> specialAudio = new List<AudioClip>();
    public List<AudioClip> boingAudio = new List<AudioClip>();
    public List<AudioClip> dashAudio = new List<AudioClip>();
    public List<AudioClip> hurtAudio = new List<AudioClip>();
    public List<AudioClip> deadAudio = new List<AudioClip>();
    AudioSource audioSource;
    public Health healthUI;
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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
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
        shadowAnim.runtimeAnimatorController = playerSkin.idle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (taping || stunTime > 0)
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (isDead)
        {
            if (deadTime < 0)
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }
            else
            {
                deadTime -= Time.deltaTime;
                return;
            }
        }
        if (body.position.y < -35)
        {
            health -= 3;
            TakeDamage();
            return;
        }


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
            shadowAnim.runtimeAnimatorController = playerSkin.run;
            spriteRenderer.flipX = true;
            shadowRenderer.flipX = true;
            runningAudio.volume = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            facingLeft = true;
            xMove += -speed * Time.deltaTime;
            spriteRenderer.flipX = false;
            shadowRenderer.flipX = false;
            animator.runtimeAnimatorController = playerSkin.run;
            shadowAnim.runtimeAnimatorController = playerSkin.run;
            runningAudio.volume = 1;
        }

        if (xMove == 0)
        {
            animator.runtimeAnimatorController = playerSkin.idle;
            shadowAnim.runtimeAnimatorController = playerSkin.idle;
        }

        if (xMove == 0 || !onGround)
        {
            runningAudio.volume = 0;
        }



        //jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
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

        RaycastHit2D[] floorHits = Physics2D.BoxCastAll(body.position, boxCollider.size, 0, Vector2.down, Mathf.Abs(yMove), wallMask);
        foreach (var floorHit in floorHits)
        {
            bool dobreak = false; ;
            if (floorHit.collider != null && vVelocity < 0 && !floorHit.collider.isTrigger)
            {
                vVelocity = 0;
                yMove = -(floorHit.distance);
                onGround = true;
                dobreak = true;
            }

            if (floorHit.collider != null && floorHit.collider.gameObject.CompareTag("Spike"))
            {
                if (TakeDamage())
                {
                    floorHit.collider.gameObject.GetComponent<Spike>()?.PlayAudio();

                }
                dobreak = true;
            }


            if (dobreak)
            {
                break;
            }
        }
        body.MovePosition(body.position + new Vector2(xMove, yMove));

        bool hitAny = false;
        floorHits = Physics2D.BoxCastAll(body.position, boxCollider.size, 0, Vector2.down, Mathf.Abs(0.1f), wallMask);
        foreach (var floorHit in floorHits)
        {

            if ((floorHit.collider == null || floorHit.collider.isTrigger) && vVelocity < 0 && onGround)
            {
            }
            else
            {
                hitAny = true;
            }
        }
        if (!hitAny)
        {
            onGround = false;
        }

        RaycastHit2D[] hitBounces = Physics2D.BoxCastAll(body.position, boxCollider.size * 0.99f, 0, Vector2.down, 0.5f, platformMask);
        foreach (var hitBounce in hitBounces)
        {

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
        }
        if (hitBounces.Length <= 0)
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
                shadowAnim.runtimeAnimatorController = playerSkin.jump;
            }
            else
            {
                animator.runtimeAnimatorController = playerSkin.fall;
                shadowAnim.runtimeAnimatorController = playerSkin.fall;
            }
        }



        //repair
        if (Input.GetKeyDown(KeyCode.Z) && onGround)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                body.position + new Vector2(0, -0f), boxCollider.size * 0.99f, 0, (facingRight ? Vector2.right : Vector2.left) + Vector2.down,
                boxCollider.size.x * 1.4f, platformMask);
            List<RaycastHit2D> hitsList = new List<RaycastHit2D>(hits);
            hitsList.Sort((p1, p2) => p2.transform.position.y.CompareTo(p1.transform.position.y));
            foreach (var hit in hitsList)
            {

                if (hit.collider && hit.distance > 0)
                {


                    PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                    if (platform == null || platform.effect != PlatformA.Effect.NONE || platform.status != PlatformA.Status.FIXED || tapeBounceCount <= 0)
                    {
                        continue;
                    }
                    tapeAction = () =>
                    {

                        platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.BOUNCE);
                        tapeDisplay.gbRemove();
                        tapeBounceCount--;
                    };

                    taping = true;
                    animator.runtimeAnimatorController = playerSkin.tape;
                    shadowAnim.runtimeAnimatorController = playerSkin.tape;
                    audioSource.clip = specialAudio[Random.Range(0, 10)];
                    audioSource.Play();
                    break;
                }
            }



        }

        if (Input.GetKeyDown(KeyCode.X) && onGround)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                body.position + new Vector2(0, -0f), boxCollider.size * 0.99f, 0, (facingRight ? Vector2.right : Vector2.left) + Vector2.down,
                boxCollider.size.x * 1.4f, platformMask);
            List<RaycastHit2D> hitsList = new List<RaycastHit2D>(hits);
            hitsList.Sort((p1, p2) => p2.transform.position.y.CompareTo(p1.transform.position.y));
            foreach (var hit in hitsList)
            {

                if (hit.collider && hit.distance > 0)
                {

                    PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                    if (platform == null || platform.effect != PlatformA.Effect.NONE || platform.status != PlatformA.Status.FIXED || tapeDashCount <= 0)
                    {
                        continue;
                    }
                    tapeAction = () =>
                    {


                        platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.SPEED);
                        tapeDisplay.gdRemove();
                        tapeDashCount--;
                    };

                    taping = true;
                    animator.runtimeAnimatorController = playerSkin.tape;
                    shadowAnim.runtimeAnimatorController = playerSkin.tape;
                    audioSource.clip = specialAudio[Random.Range(0, 10)];
                    audioSource.Play();
                    break;
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.C) && onGround)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                body.position + new Vector2(0, -0f), boxCollider.size * 0.99f, 0, (facingRight ? Vector2.right : Vector2.left) + Vector2.down,
                boxCollider.size.x * 1.4f, platformMask);
            List<RaycastHit2D> hitsList = new List<RaycastHit2D>(hits);
            hitsList.Sort((p1, p2) => p2.transform.position.y.CompareTo(p1.transform.position.y));
            foreach (var hit in hitsList)
            {
                if (hit.collider && hit.distance > 0)

                {

                    PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                    if (platform == null || platform.effect != PlatformA.Effect.NONE || platform.status != PlatformA.Status.BROKEN || tapeCount <= 0)
                    {
                        continue;
                    }
                    tapeAction = () =>
                    {


                        platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.NONE);
                        tapeDisplay.g1Remove();
                        tapeCount--;
                    };

                    taping = true;
                    animator.runtimeAnimatorController = playerSkin.tape;
                    shadowAnim.runtimeAnimatorController = playerSkin.tape;
                    audioSource.clip = tapeAudio[Random.Range(0, 3)];
                    audioSource.Play();
                    break;
                }
            }

        }


        //retrieve
        if (Input.GetKeyDown(KeyCode.V) && onGround)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(
                body.position + new Vector2(0, -0f), boxCollider.size * 0.99f, 0, (facingRight ? Vector2.right : Vector2.left) + Vector2.down,
                boxCollider.size.x * 1.4f, platformMask);
            List<RaycastHit2D> hitsList = new List<RaycastHit2D>(hits);
            hitsList.Sort((p1, p2) => p2.transform.position.y.CompareTo(p1.transform.position.y));
            foreach (var hit in hitsList)
            {

                if (hit.collider)
                {
                    PlatformA platform = hit.collider.gameObject.GetComponent<PlatformA>();
                    if (platform == null || platform.status == PlatformA.Status.BROKEN)
                    {
                        continue;
                    }
                    tapeAction = () =>
                    {

                        switch (platform.effect)
                        {
                            case PlatformA.Effect.NONE:
                                tapeDisplay.g1Add();
                                tapeCount++;
                                platform.SetStatus(PlatformA.Status.BROKEN, PlatformA.Effect.NONE);
                                break;
                            case PlatformA.Effect.BOUNCE:
                                tapeDisplay.gbAdd();
                                tapeBounceCount++;
                                platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.NONE);
                                break;
                            case PlatformA.Effect.SPEED:
                                tapeDisplay.gdAdd();
                                tapeDashCount++;
                                platform.SetStatus(PlatformA.Status.FIXED, PlatformA.Effect.NONE);
                                break;
                            default:
                                break;
                        }
                    };

                    taping = true;
                    animator.runtimeAnimatorController = playerSkin.untape;
                    shadowAnim.runtimeAnimatorController = playerSkin.untape;
                    audioSource.clip = untapeAudio[Random.Range(0, 4)];
                    audioSource.Play();
                    break;
                }
            }

        }
    }
}
