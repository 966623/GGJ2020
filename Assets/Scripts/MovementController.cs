using System;
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

    float _desiredVelocity;
    public float DesiredVelocity
    {
        get => _desiredVelocity;
        set
        {
            _desiredVelocity = Mathf.Clamp(value, -maxSpeed, maxSpeed);
        }
    }

    bool _wantJump;
    public bool WantJump
    {
        get => _wantJump;
        set
        {
            _wantJump = value;
        }
    }

    [SerializeField]
    float maxSpeed = 10f;
    [SerializeField]
    float maxAccel = 1f;
    [SerializeField]
    float maxAirAccel = 1f;
    [SerializeField]
    float jumpHeight = 1f;

    [SerializeField]
    float maxGroundAngle = 50f;

    float minGroundDotProduct;

    [SerializeField, Min(0f)]
    float maxSnapSpeed = 6f;

    [SerializeField, Min(0f)]
    float maxSnapDistance = 1f;

    [HideInInspector]
    public float jumpModifier = 1;
    [HideInInspector]
    public float speedModifier = 1;

    bool wantOverrideVelocity = false;
    Vector2 velocityOverride = Vector2.zero;

    Vector2 contactNormal;

    int stepsSinceLastGrounded, stepsSinceLastJump;

    public delegate void GroundEvent(GameObject ground);
    public event GroundEvent OnGrounded;
    public event GroundEvent OnLeaveGrounded;

    public bool OnGround => _numGroundContacts > 0;
    //{
    //    get
    //    {
    //        return _onGround;
    //    }
    //    set
    //    {
    //        _onGround = value;
    //        if (_onGround)
    //        {
    //            OnGrounded?.Invoke(gameObject);
    //        }
    //        else
    //        {
    //            OnLeaveGrounded?.Invoke(gameObject);
    //        }
    //    }
    //}

    int _numGroundContacts = 0;
    int NumGroundContacts
    {
        get => _numGroundContacts;
        set
        {
            _numGroundContacts = value;
           
        }
    }

    public Vector2 velocity;

    public Vector2 Position
    {
        get
        {
            return body.position;
        }
    }

    bool wantStopMovement = false;

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        velocity = body.velocity;
        OnValidate();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StopMovement()
    {
        wantStopMovement = true;
    }

    private void FixedUpdate()
    {
        UpdateState();

        // Update horizontal movement
        AdjustVelocity();

        // Jump
        if (WantJump && OnGround)
        {
            stepsSinceLastJump = 0;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * jumpHeight * jumpModifier);
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0);
            }
            velocity.y += jumpSpeed;
        }
        else
        {
            WantJump = false;
        }

        if (wantStopMovement)
        {
            body.velocity = Vector2.zero;
            DesiredVelocity = 0;
            wantStopMovement = false;
        }
        else if (wantOverrideVelocity)
        {
            body.velocity = velocityOverride;
            DesiredVelocity = velocityOverride.x;
            stepsSinceLastJump = 0;
            wantOverrideVelocity = false;
            velocityOverride = Vector2.zero;
        }
        else
        {
            body.velocity = velocity;
        }

        if (OnGround)
        {
            OnGrounded?.Invoke(gameObject);
        }
        else
        {
            OnLeaveGrounded?.Invoke(gameObject);
        }



        ClearState();
    }

    private void ClearState()
    {
        NumGroundContacts = 0;
        contactNormal = Vector2.zero;
    }

    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround())
        {
            stepsSinceLastGrounded = 0;
            if (NumGroundContacts > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
        {
            return false;
        }

        RaycastHit2D hit = Physics2D.BoxCast(Position, colliderSize * .9f, 0, Vector2.down, maxSnapDistance, groundLayers);

        if (hit.collider != null)
        {
            if (hit.normal.y < minGroundDotProduct)
            {
                return false;
            }


            NumGroundContacts = 1;
            contactNormal = hit.normal;
            float speed = velocity.magnitude;
            float dot = Vector3.Dot(velocity, hit.normal);
            //body.MovePosition(Position + Vector2.down * hit.distance);
            if (dot > 0f)
            {
                velocity = (velocity - hit.normal * dot).normalized * speed;
            }
            return true;
        }

        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                NumGroundContacts++;
                contactNormal += normal;
            }
            else
            {
                contactNormal += Vector2.zero;
            }
        }
    }

    Vector2 ProjectOnContactPlane(Vector2 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    void AdjustVelocity()
    {
        float accel = OnGround ? maxAccel : maxAirAccel;
        float maxVelocityChanged = accel * Time.deltaTime;

        Vector2 projectedParallel = ProjectOnContactPlane(Vector2.right).normalized;
        float currentProjectedVelocity = Vector2.Dot(velocity, projectedParallel);
        float newProjectedVelocity = Mathf.MoveTowards(currentProjectedVelocity, DesiredVelocity, maxVelocityChanged);
        velocity += projectedParallel * (newProjectedVelocity - currentProjectedVelocity);
    }

    public void ForceMove(Vector2 force)
    {
        velocityOverride = force;
        wantOverrideVelocity = true;
    }

    private void OnDrawGizmos()
    {
        if (body == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Position, Position + contactNormal);
    }
}
