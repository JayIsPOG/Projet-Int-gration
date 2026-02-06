using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public int largeur = 37;
    public int hauteur = 29;

    public Vector2 gridOffset = new Vector2(-8, 8);

    public Color gridColor = Color.green;

    void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int x = 0; x < largeur; x++)
        {
            for (int y = 0; y < hauteur; y++)
            {
                Vector3 pos = new Vector3(
                    x + gridOffset.x,
                    y + gridOffset.y,
                    0
                );

                Gizmos.DrawWireCube(pos, new Vector3(1, 1, 0));
            }
        }
    }
}
