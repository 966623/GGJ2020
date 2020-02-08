using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    AudioSource audioSource;
    public Animator animator;
    bool gotten = false;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gotten)
        {
            return;
        }
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            gotten = true;
            animator.SetTrigger("Get");
            collision.gameObject.GetComponent<Player>().HasKey = true;
            audioSource.Play();
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }
}
