using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapeDisplay : MonoBehaviour
{
    public HorizontalLayoutGroup g1;
    public HorizontalLayoutGroup gb;
    public HorizontalLayoutGroup gd;

    public GameObject t1;
    public GameObject tb;
    public GameObject td;

    public GameObject mainPanel;

    public Image keyImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotKey()
    {
        keyImage.color = new Color(1, 1, 1, 1);
    }

    public void g1Add()
    {
        GameObject newObject = Instantiate(t1);
        newObject.transform.parent = g1.transform;
        newObject.transform.localScale = new Vector3(1,1,1);
    }

    public void g1Remove()
    {
        g1.transform.GetChild(0).parent = null;
    }

    public void gbAdd()
    {
        GameObject newObject = Instantiate(tb);
        newObject.transform.parent = gb.transform;
        newObject.transform.localScale = new Vector3(1,1,1);
    }

    public void gbRemove()
    {
        gb.transform.GetChild(0).parent = null;
    }

    public void gdAdd()
    {
        GameObject newObject = Instantiate(td);
        newObject.transform.parent = gd.transform;
        newObject.transform.localScale = new Vector3(1,1,1);
    }

    public void gdRemove()
    {
        gd.transform.GetChild(0).parent = null;
    }
}
