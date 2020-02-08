using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPhysics : MonoBehaviour
{
    public delegate void CollisionEvent(Collision2D collision);
    public event CollisionEvent OnCollision;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision?.Invoke(collision);
    }
}
