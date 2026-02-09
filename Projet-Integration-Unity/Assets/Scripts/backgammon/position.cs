using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class position : MonoBehaviour
{
    // chaque slot de chips montrent les casens en sens antihoraire, les deux dernieres sont pour les chips out pour les joueurs
    public int[] chips = new int[26];
    // Start is called before the first frame update
    void Start()
    {
        chips[0] = -2;
        chips[1] = 0;
        chips[2] = 0;
        chips[3] = 0;
        chips[4] = 0;
        chips[5] = 5;
        chips[6] = 0;
        chips[7] = 3;
        chips[8] = 0;
        chips[9] = 0;
        chips[10] = 0;
        chips[11] = -5;
        chips[12] = 5;
        chips[13] = 0;
        chips[14] = 0;
        chips[15] = 0;
        chips[16] = -3;
        chips[17] = 0;
        chips[18] = -5;
        chips[19] = 0;
        chips[20] = 0;
        chips[21] = 0;
        chips[22] = 0;
        chips[23] = 2;
        chips[24] = 0;
        chips[25] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
