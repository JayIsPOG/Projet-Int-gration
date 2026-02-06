using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    Vector3 spawn_1 = new Vector3(-7f,1f,0);//5.67f
      Vector3 spawn_2 = new Vector3(25.6f,0.5f,0);
      Quaternion spawnRotation = Quaternion.identity;
      float timer = 0f;
      float spawn_delay = 3f;

    
    void Start()
    {
        Instantiate(prefab,spawn_1,spawnRotation);
    }

    // Update is called once per frame
    void Update()
    {
        /*timer += Time.deltaTime;
        if (timer >= spawn_delay )
        {
            Vector3 choix = Random.value < 0.5f ? spawn_1 : spawn_2;
            Instantiate(prefab,choix,spawnRotation);
            timer = 0f;
        }*/
    }


}
