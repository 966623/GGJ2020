using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPane : MonoBehaviour
{
    public Player player;
    public List<GameObject> tapePrefabs;
    public List<Transform> tapePanels;

    public Transform heartsPanel;
    public GameObject heartPrefab;

    public HeartUI[] hearts = new HeartUI[3];
    // Start is called before the first frame update
    void Start()
    {
        player.TapeCountIncrement += Player_TapeCountIncrement;
        player.TapeCountDecrement += Player_TapeCountDecrement;
        InitTape();
        InitHearts();
        player.OnHealthChanged += Player_OnHealthChanged;
    }

    private void Player_OnHealthChanged()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i + 1> player.Health)
            {
                hearts[i].animator.SetBool("Active", false);
            }
            else
            {
                hearts[i].animator.SetBool("Active", true);
            }
        }

    }

    private void InitTape()
    {
        foreach (Platform.Effect effect in (Platform.Effect[])Platform.Effect.GetValues(typeof(Platform.Effect)))
        {
            for (int i = 0; i < player.GetTapeCount(effect); i++)
            {
                Instantiate(tapePrefabs[(int)effect], tapePanels[(int)effect]);
            }
        }
    }

    private void InitHearts()
    {

        for(int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = Instantiate(heartPrefab, heartsPanel).GetComponent<HeartUI>();
        }
        
    }

    private void Player_TapeCountDecrement(Platform.Effect effect)
    {
        tapePanels[(int)effect].transform.GetChild(0).SetParent(null);
    }

    private void Player_TapeCountIncrement(Platform.Effect effect)
    {
        Instantiate(tapePrefabs[(int)effect], tapePanels[(int)effect]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
