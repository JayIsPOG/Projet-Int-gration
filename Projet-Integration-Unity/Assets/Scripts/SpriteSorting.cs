using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSorting : MonoBehaviour
{
    public int offset;
    // Update is called once per frame
    void Update()
    {
        try{
            GetComponent<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(-transform.position.y * 100) + offset;
        }catch{}
        try{
            LineRenderer ln = GetComponent<LineRenderer>();

            if(ln.GetPosition(1).y < ln.GetPosition(0).y)
            {
                GetComponent<LineRenderer>().sortingOrder = Mathf.RoundToInt(-GetComponent<LineRenderer>().GetPosition(1).y * 100) + offset;
            }else{
                GetComponent<LineRenderer>().sortingOrder = Mathf.RoundToInt(-GetComponent<LineRenderer>().GetPosition(0).y * 100) + offset;
            }
        }catch{}
    }
}
