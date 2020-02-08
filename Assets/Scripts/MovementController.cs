using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
public class MovementController : MonoBehaviour
{
    Rigidbody2D body;
    public Vector2 colliderSize;
    public LayerMask groundLayers;

  
    public delegate void StandardEvent();
    public event StandardEvent OnGrounded;
    public event StandardEvent OnLeaveGrounded;

    bool _grounded = false;
    public bool grounded
    {
        get
        {
            return _grounded;
        }
        set
        {
            _grounded = value;
            if (_grounded) OnGrounded?.Invoke();
            else OnLeaveGrounded?.Invoke();
        }
    }

    public Vector2 Velocity
    {
        get
        {
            return body.velocity;
        }
        set
        {
            body.velocity = value;
        }
    }

    public Vector2 Position
    {
        get
        {
            return body.position;
        }
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {

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
    }

    public void ImpulseMove(Vector2 force)
    {
        body.AddForce(force, ForceMode2D.Impulse);
    }
}
