using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    AudioSource audioSource;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            gotten = true;
            collision.gameObject.GetComponent<OldPlayer>().hasKey = true;
            collision.gameObject.GetComponent<OldPlayer>().tapeDisplay.GotKey();
            audioSource.Play();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(transform.GetChild(0).gameObject);
        }
    }
}
