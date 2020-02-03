using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{

    public Animator animator;
    public RuntimeAnimatorController closeAnim;
    public Animator shadowAnim;
    public AudioSource audioSource;
    bool activated = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        animator.runtimeAnimatorController = closeAnim;
        shadowAnim.runtimeAnimatorController = closeAnim;
        activated = true;
        gameObject.layer = 8;
        GetComponent<BoxCollider2D>().size = new Vector2(1, 0.3f);
        GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0.15f);
        audioSource.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (activated)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage();
            Activate();
        }
    }
}
