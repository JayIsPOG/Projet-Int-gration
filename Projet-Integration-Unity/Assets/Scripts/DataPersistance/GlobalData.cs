using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour, IDataPersistance
{
    public int puzzlesCompleted;
    public int fightsCompleted;
    public int backgammonCompleted;
    
    public bool puzzlesKey;
    public bool fightsKey;
    public bool backgammonKey;

    //void Start (){
    //    playerDamageCoefficient = 1;
    //    enemyDamageCoefficient = 1;
    //}
    public void LoadData(GameData data)
    {
        puzzlesCompleted = data.puzzlesCompleted;
        fightsCompleted = data.fightsCompleted;
        backgammonCompleted = data.backgammonCompleted;

        puzzlesKey = data.puzzlesKey;
        fightsKey = data.fightsKey;
        backgammonKey = data.backgammonKey;
    }

    public void SaveData(ref GameData data)
    {
        data.puzzlesCompleted = puzzlesCompleted;
        data.fightsCompleted = fightsCompleted;
        data.backgammonCompleted = backgammonCompleted;

        data.puzzlesKey = puzzlesKey;
        data.fightsKey = fightsKey;
        data.backgammonKey = backgammonKey;
    }
}
