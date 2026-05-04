using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PressurePlate : MonoBehaviour
{
    public bool pressed;
    public GameObject symbols;
    public Tilemap tilemap;
    public Color colorOpen, colorClose;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        symbols.SetActive(pressed);
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == 3 || other.gameObject.layer ==  1)
        {
            tilemap.color = colorOpen;
            pressed = true;
        }  
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == 3 || other.gameObject.layer == 1)
        {
            tilemap.color = colorClose;
            pressed = false;
        }
    }
}
