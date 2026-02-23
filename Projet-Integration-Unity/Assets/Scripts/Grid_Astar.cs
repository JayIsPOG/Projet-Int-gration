using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager
{
    float rayonAgent = 0.4f; // ajuste selon la taille de ton cube
    LayerMask wallLayer;

    int largeur = 37;
    int hauteur = 29;

    Node[,] grid;

    Tilemap tilemap;

    float offsetX = 8f;
    float offsetY = 7f;

    public GridManager(Tilemap tilemap)
{
    this.tilemap = tilemap;

    wallLayer = LayerMask.GetMask("Ground");

    grid = new Node[largeur, hauteur];

    for (int x = 0; x < largeur; x++)
    {
        for (int y = 0; y < hauteur; y++)
        {
            Vector3 worldPos = new Vector3(x - offsetX, offsetY - y, 0);

            bool walkable = Physics2D.OverlapCircle(worldPos, rayonAgent, wallLayer) == null;

            grid[x, y] = new Node(x, y, walkable);
        }
    }
}


    public Node GetNode(int x, int y)
    {
        return grid[x, y];
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

    public List<Node> TrouverVoisins(Node node)
    {
        List<Node> voisins = new List<Node>();

        int x = node.x;
        int y = node.y;

        bool droite = x + 1 < largeur && grid[x + 1, y].walkable;
        bool gauche = x - 1 >= 0 && grid[x - 1, y].walkable;
        bool haut = y + 1 < hauteur && grid[x, y + 1].walkable;
        bool bas = y - 1 >= 0 && grid[x, y - 1].walkable;

        if (droite) voisins.Add(grid[x + 1, y]);
        if (gauche) voisins.Add(grid[x - 1, y]);
        if (haut) voisins.Add(grid[x, y + 1]);
        if (bas) voisins.Add(grid[x, y - 1]);

        // diagonales autorisées seulement si pas de mur
        if (droite && haut && grid[x + 1, y + 1].walkable)
            voisins.Add(grid[x + 1, y + 1]);

        if (gauche && haut && grid[x - 1, y + 1].walkable)
            voisins.Add(grid[x - 1, y + 1]);

        if (droite && bas && grid[x + 1, y - 1].walkable)
            voisins.Add(grid[x + 1, y - 1]);

        if (gauche && bas && grid[x - 1, y - 1].walkable)
            voisins.Add(grid[x - 1, y - 1]);

        return voisins;
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
                    openSet[i].fCost == currentNode.fCost &&
                    openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in TrouverVoisins(currentNode))
            {
                if (closedSet.Contains(neighbour))
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