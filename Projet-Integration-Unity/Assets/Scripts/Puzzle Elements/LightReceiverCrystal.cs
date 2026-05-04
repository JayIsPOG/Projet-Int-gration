using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightReceiverCrystal : LightEmitter
{
    public bool hitByLight;
    public float waveLenghtReceived;
    public float waveLenghtToOpen = 550;
    public bool open;
    public Sprite spriteLit, spriteUnlit;
    public bool lightPassesThrough;
    public Tilemap tilemap;
    public Color colorOpen, colorClose;

    public override void Update()
    {
        base.Update();
        if(hitByLight)
        {
            if(waveLenghtToOpen <= waveLenghtReceived + 30 && waveLenghtToOpen >= waveLenghtReceived - 30)
                open = true;
            else
                open = false;
        }
        else
            open = false;
        if (open)
        {
            if(GetComponent<SpriteRenderer>().sprite != spriteLit)
            {
                tilemap.color = colorOpen;
                GetComponent<SpriteRenderer>().sprite = spriteLit;
                GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            if(GetComponent<SpriteRenderer>().sprite != spriteUnlit)
            {
                tilemap.color = colorClose;
                GetComponent<SpriteRenderer>().sprite = spriteUnlit;
            }
        }
    }

    public override Vector3 GetRotatedVector(float incomingAngle)
    {
        float outgoingAngle = Mathf.Asin((line.shadowBias * Mathf.Sin(incomingAngle * Mathf.Deg2Rad))/nDensity) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(incomingAngle + 180f, Vector3.forward);
        Vector3 rotatedVector = rotation * transform.up;
        return rotatedVector;
    }
    public override Vector3 GetRayStartPos(float incomingAngle)
    {
        return transform.position + GetRotatedVector(incomingAngle) * 0.25f;
    }
}
