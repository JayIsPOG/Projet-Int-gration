using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReceiver : MonoBehaviour
{
    public bool hitByLight;
    public float waveLenghtReceived;
    public float waveLenghtToOpen = 550;
    public bool open;

    void Update()
    {
        if(hitByLight)
        {
            if(waveLenghtToOpen <= waveLenghtReceived + 10 && waveLenghtToOpen >= waveLenghtReceived - 10)
                open = true;
            else
                open = false;
        }
        else
            open = false;
    }
}
