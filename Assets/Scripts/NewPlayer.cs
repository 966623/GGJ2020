using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class NewPlayer : MonoBehaviour
{
    Controls controls;
    Controls.GameplayActions gameplay => controls.Gameplay;
    Rigidbody2D body;
    new BoxCollider2D collider;
    public float maxSpeed = 10f;
    public float jumpVelocity = 10f;
    public LayerMask groundLayers;
    float hInput = 0;
    float hVelocity = 0;
    float vVelocity = 0;


    public bool grounded = false;
    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        gameplay.Jump.performed += Jump;
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
        hInput = gameplay.Move.ReadValue<Vector2>().x;
    }

    private void FixedUpdate()
    {
        // Ground check
        if (body.velocity.y <= 0)
        {
            RaycastHit2D hit = Physics2D.BoxCast(body.position, collider.size, 0, Vector2.down, .01f, groundLayers);
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
