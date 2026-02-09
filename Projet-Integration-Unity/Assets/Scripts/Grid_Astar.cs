using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GridManager
{
    int largeur = 37;
    int hauteur = 29;
    Node[,] grid;

    public GridManager()
    {
        grid = new Node[largeur, hauteur];

        for(int x = 0; x < largeur; x++) {

            for(int y = 0; y < hauteur; y++)
            {
                grid[x,y] = new Node(x,y,EstLibre(x,y));
            }
        }
    }

    public List<Node> TrouverVoisins(Node node)
{
    List<Node> voisins = new List<Node>();

    int x = node.x;
    int y = node.y;

    // Directions cardinales
    if (x + 1 < largeur)
        voisins.Add(grid[x + 1, y]);

    if (x - 1 >= 0)
        voisins.Add(grid[x - 1, y]);

    if (y + 1 < hauteur)
        voisins.Add(grid[x, y + 1]);

    if (y - 1 >= 0)
        voisins.Add(grid[x, y - 1]);

    // ---- AJOUT DES DIAGONALES ----

    if (x + 1 < largeur && y + 1 < hauteur)
        voisins.Add(grid[x + 1, y + 1]);

    if (x - 1 >= 0 && y + 1 < hauteur)
        voisins.Add(grid[x - 1, y + 1]);

    if (x + 1 < largeur && y - 1 >= 0)
        voisins.Add(grid[x + 1, y - 1]);

    if (x - 1 >= 0 && y - 1 >= 0)
        voisins.Add(grid[x - 1, y - 1]);

    return voisins;
}

public bool EstLibre(int x, int y)
{
    float offsetX = 8f;
    float offsetY = 7f;

    Vector2 point_test_collision = new Vector2(x - offsetX, offsetY - y);

    // Petite marge de sécurité (rayon très petit)
    float rayonTest = 0.2f;

    // On teste seulement la layer Walls
    int objetLayer = LayerMask.GetMask("Default");

    // On regarde s'il y a un collider dans un petit cercle autour du point
    Collider2D hit = Physics2D.OverlapCircle(point_test_collision, rayonTest, objetLayer);
    if(hit != null)
        Debug.Log("Case bloquée : (" + x + ", " + y + ")");
    // S'il y a un collider → la case n'est pas libre
    return hit == null;
}

    public void ResetNodes()
    {
        for (int x = 0; x < largeur; x++)
        {
            for (int y = 0; y < hauteur; y++)
            {
                grid[x, y].gCost = int.MaxValue;
                grid[x, y].parent = null;
            }
        }
    }

    public Node GetNode(int x, int y)
    {
        return grid[x, y];
    }

    public List<Node> FindPath(Node startNode, Node targetNode)
    {
        ResetNodes();

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                (openSet[i].fCost == currentNode.fCost &&
                    openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in TrouverVoisins(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour);


                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }


  int GetDistance(Node a, Node b)
{
    int dstX = Mathf.Abs(a.x - b.x);
    int dstY = Mathf.Abs(a.y - b.y);

    if (dstX > dstY)
        return 14 * dstY + 10 * (dstX - dstY);
    return 14 * dstX + 10 * (dstY - dstX);
}


    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node current = endNode;

        while (current != startNode)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

}
