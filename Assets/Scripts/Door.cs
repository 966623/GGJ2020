using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string nextLevel = "level_X";
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
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            if (player.HasKey)
            {
                player.SetState(player.winState);
                StartCoroutine(WinCoroutine());
            }
        }
    }

    IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextLevel);
    }
}
