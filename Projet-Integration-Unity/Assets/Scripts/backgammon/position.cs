using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class position : MonoBehaviour
{
    // chaque slot de chips montrent les casens en sens antihoraire, les deux dernieres sont pour les chips out pour les joueurs
    public int[] chips;
    // Start is called before the first frame update
    void Start()
    {
        chips = new int[]{-2, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, -5, 5, 0, 0, 0, -3, 0, -5, 0, 0, 0, 0, 2, 0, 0};
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
