using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSignal : MonoBehaviour
{
    public GameObject[] gameObjects;
    public bool open;
    void Update()
    {
        foreach(GameObject obj in gameObjects)
        {
            bool shouldOpen = true;
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
            if(obj.GetComponent<LightReceiver>())
            {
                if (!obj.GetComponent<LightReceiver>().open)
                    shouldOpen = false;
            }
            open = shouldOpen;
        }
    }
}
