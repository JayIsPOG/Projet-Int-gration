using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightLine : MonoBehaviour
{
    public float nDensity, velocity, waveLenght, waveLenghtInDensity;
    public LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = 299792458 / nDensity;
        waveLenghtInDensity = waveLenght / nDensity;
        Color color = Convert(waveLenghtInDensity);
        line.endColor = color;
        line.startColor = color;
    }

    Color Convert(float w)
    {
        float red, green, blue;
        if (w >= 380 && w < 440)
        {
            red   = -(w - 440) / (440 - 380);
            green = 0.0f;
            blue  = 1.0f;
        }
        else if (w >= 440 && w < 490)
        {
            red   = 0.0f;
            green = (w - 440) / (490 - 440);
            blue  = 1.0f;
        }
        else if (w >= 490 && w < 510)
        {
            red   = 0.0f;
            green = 1.0f;
            blue  = -(w - 510) / (510 - 490);
        }
        else if (w >= 510 && w < 580)
        {
            red   = (w - 510) / (580 - 510);
            green = 1.0f;
            blue  = 0.0f;
        }
        else if (w >= 580 && w < 645)
        {
            red   = 1.0f;
            green = -(w - 645) / (645 - 580);
            blue  = 0.0f;
        }
        else if (w >= 645 && w < 781)
        {
            red   = 1.0f;
            green = 0.0f;
            blue  = 0.0f;
        }
        else
        {
            red   = 0.0f;
            green = 0.0f;
            blue  = 0.0f;
        }

        return new Color(red, green, blue, 1f);
    }
}
