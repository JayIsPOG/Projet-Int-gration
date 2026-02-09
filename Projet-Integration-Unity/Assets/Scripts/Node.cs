public class Node
{
    public int x;
    public int y;

    public bool walkable;

    public int gCost;
    public int hCost;

    public Node parent;

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node(int x, int y, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;

        gCost = int.MaxValue;
    }

    public void Reset()
    {
        gCost = int.MaxValue;
        hCost = 0;
        parent = null;
    }
}
