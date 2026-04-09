using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int puzzlesCompleted;
    public int fightsCompleted;
    public int backgammonCompleted;
    
    public bool puzzlesKey;
    public bool fightsKey;
    public bool backgammonKey;

    public GameData(){
        this.puzzlesCompleted = 0;
        this.fightsCompleted = 0;
        this.backgammonCompleted = 0;

        this.puzzlesKey = false;
        this.fightsKey = false;
        this.backgammonKey = false;
    }
}
