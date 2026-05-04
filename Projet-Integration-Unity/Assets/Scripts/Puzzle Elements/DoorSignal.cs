using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSignal : MonoBehaviour
{
    public GameObject[] gameObjects;
    public bool open;
    void Update()
    {
        bool shouldOpen = true;
        foreach(GameObject obj in gameObjects)
        {
            
            if(obj.GetComponent<LightReceiverCrystal>())
            {
                if (!obj.GetComponent<LightReceiverCrystal>().open)
                    shouldOpen = false;
            }
            if(obj.GetComponent<LightReceiver>())
            {
                if (!obj.GetComponent<LightReceiver>().open)
                    shouldOpen = false;
            }
            if(obj.GetComponent<Pickup>())
            {
                if (!obj.GetComponent<Pickup>().pickedUp)
                    shouldOpen = false;
            }
            
        }
        open = shouldOpen;
    }
}
