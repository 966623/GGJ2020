using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public LayerMask playerLayer;
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Activate()
    {
     
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(gameObject);
            Activate();
        }
    }

}
