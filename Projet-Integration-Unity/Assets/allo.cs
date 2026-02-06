using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class allo : MonoBehaviour
{
    void Start()
    {
        GridManager gm = new GridManager();

        Node start = gm.GetNode(0, 0);
        Node end = gm.GetNode(10, 10);

        List<Node> path = gm.FindPath(start, end);

        if (path != null)
        {
            //Debug.Log("Chemin trouvé !");
            //Debug.Log("Longueur : " + path.Count);

            foreach (Node n in path)
            {
                //Debug.Log("(" + n.x + ", " + n.y + ")");
            }
        }
        else
        {
            //Debug.Log("Aucun chemin trouvé");
        }
    }

}
