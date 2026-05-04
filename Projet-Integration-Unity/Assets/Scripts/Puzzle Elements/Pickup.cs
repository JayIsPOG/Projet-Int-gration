using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum levelType { puzzle, fight, backgammon}
    public levelType currentState;
    public bool pickedUp;
    
    void OnTriggerEnter2D()
    {
        pickedUp = true;
        GetComponent<SpriteRenderer>().color = new Color(0,0,0,0);

        switch (currentState) {
        case levelType.puzzle:
            FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0].puzzlesKey = true;
            break;
        case levelType.fight:
            FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0].fightsKey = true;
            Debug.Log("fight");
            break;
        case levelType.backgammon:
            FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0].backgammonKey = true;
            break;
        }
    }
}
