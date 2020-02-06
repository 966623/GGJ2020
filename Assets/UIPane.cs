using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPane : MonoBehaviour
{
    public Player player;
    public GameObject tapePrefab;
    public GameObject bounceTapePrefab;
    public GameObject dashTapePrefab;

    public Transform tapePanel;
    public Transform bounceTapePanel;
    public Transform dashTapePanel;
    // Start is called before the first frame update
    void Start()
    {
        player.TapeCountChanged.AddListener(UpdateTape);
        UpdateTape();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateTape()
    {
        while (tapePanel.transform.childCount > player.tapeCount)
        {
            tapePanel.transform.GetChild(0).parent = null;
        }
        while (tapePanel.transform.childCount < player.tapeCount)
        {
            Instantiate(tapePrefab, tapePanel);
        }

        while (bounceTapePanel.transform.childCount > player.bounceTapeCount)
        {
            bounceTapePanel.transform.GetChild(0).parent = null;
        }
        while (bounceTapePanel.transform.childCount < player.bounceTapeCount)
        {
            Instantiate(bounceTapePrefab, bounceTapePanel);
        }

        while (dashTapePanel.transform.childCount > player.dashTapeCount)
        {
            dashTapePanel.transform.GetChild(0).parent = null;
        }
        while (dashTapePanel.transform.childCount < player.dashTapeCount)
        {
            Instantiate(dashTapePrefab, dashTapePanel);
        }
    }
}
