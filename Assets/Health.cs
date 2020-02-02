using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Image image;
    public List<Sprite> sprites;
    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        SetHealth(3);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int i)
    {
        i--;
        if (i < 0 || i >= sprites.Count) return;
        image.sprite = sprites[i];
    }
}
