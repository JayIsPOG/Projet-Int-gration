using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool pressed;
    public GameObject symbols;
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
            pressed = true;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == 3 || other.gameObject.layer == 1)
            pressed = false;
    }
}
