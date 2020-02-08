using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPane : MonoBehaviour
{
    public Player player;
    public List<GameObject> tapePrefabs;
    public List<Transform> tapePanels;
    // Start is called before the first frame update
    void Start()
    {
        player.TapeCountIncrement += Player_TapeCountIncrement;
        player.TapeCountDecrement += Player_TapeCountDecrement;
        InitTape();        
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
