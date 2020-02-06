using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{

    BoxCollider2D boxCollider2D;
    public GameObject beam;
    public GameObject beamShadow;
    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float timer = 2f;
    public bool state = false;
    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 2f;
            beam.SetActive(state);
            beamShadow.SetActive(state);
            boxCollider2D.isTrigger = !state;
            state = !state;
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            audioSource.Play();
            if (collision.gameObject.GetComponent<OldPlayer>().transform.position.x < transform.position.x)
            {
                collision.gameObject.GetComponent<OldPlayer>().body.gravityScale = 1;
                collision.gameObject.GetComponent<OldPlayer>().body.AddForce(new Vector2(-50, 0));
            }
            else
            {
                collision.gameObject.GetComponent<OldPlayer>().body.gravityScale = 1;
                collision.gameObject.GetComponent<OldPlayer>().body.AddForce(new Vector2(50, 0));
            }
            collision.gameObject.GetComponent<OldPlayer>().TakeDamage();
        }
    }
}
