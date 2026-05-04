using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightReceiver : MonoBehaviour
{
    public bool hitByLight;
    public float waveLenghtReceived;
    public float waveLenghtToOpen = 550;
    public bool open;
    public Sprite spriteLit, spriteUnlit;
    public bool lightPassesThrough;
    public GameObject symbols;
    public Tilemap tilemap;
    public Color colorOpen, colorClose;

    void Update()
    {
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
                GetComponent<SpriteRenderer>().sprite = spriteLit;
                symbols.SetActive(true);
                GetComponent<AudioSource>().Play();
                tilemap.color = colorOpen;
            }
        }
        else{
            if(GetComponent<SpriteRenderer>().sprite != spriteUnlit)
            {
                GetComponent<SpriteRenderer>().sprite = spriteUnlit;
                symbols.SetActive(false);
                tilemap.color = colorClose;
            }
        }
    }
}
