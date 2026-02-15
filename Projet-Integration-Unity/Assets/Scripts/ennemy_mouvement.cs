using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class ennemy_mouvement : MonoBehaviour
{
    public float speed = 3f;
    public Transform player;

    private Rigidbody2D rb;
    private GridManager gridManager;
    public Tilemap tilemap;
    private List<Node> currentPath;
    private int pathIndex = 0;

    private Vector3 anciennepos;

    void Awake()
    {
    
    rb = GetComponent<Rigidbody2D>();
    tilemap = GameObject.Find("Tilemap Fountain").GetComponent<Tilemap>();

    gridManager = new GridManager(tilemap);


    if (player == null)
    {
        GameObject p = GameObject.FindWithTag("Player");

        if (p != null)
        {
            player = p.transform;
        }
        else
        {
        }
    }

    if (player != null)
    {
        anciennepos = player.position;
        ChoisirDestination();
    }
}


    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector2.Distance(anciennepos, player.transform.position);

        if (distance > 0.5f)
        {
            anciennepos = player.transform.position;
            ChoisirDestination();
        }

        SuivreChemin();
    }

    void ChoisirDestination()
    {

        if (player == null)
        {
            return;
        }

        float offsetX = 8f;
        float offsetY = 7f;

        int startX = Mathf.FloorToInt(transform.position.x + offsetX);
        int startY = Mathf.FloorToInt(offsetY - transform.position.y);

        int targetX = Mathf.FloorToInt(player.transform.position.x + offsetX);
        int targetY = Mathf.FloorToInt(offsetY - player.transform.position.y);


        if (startX < 0 || startX >= 37 || startY < 0 || startY >= 29)
        {
            return;
        }

        if (targetX < 0 || targetX >= 37 || targetY < 0 || targetY >= 29)
        {
            return;
        }

        Node start = gridManager.GetNode(startX, startY);
        Node end = gridManager.GetNode(targetX, targetY);

        if (!end.walkable)
        {
            return;
        }

        currentPath = gridManager.FindPath(start, end);

        if (currentPath == null)
        {
        }
        else
        {
        }

        pathIndex = 0;
    }

    void SuivreChemin()
    {
        if (currentPath == null)
        {
            return;
        }

        if (pathIndex >= currentPath.Count)
        {
            rb.velocity = Vector2.zero;
            currentPath = null;
            return;
        }

        Node targetNode = currentPath[pathIndex];

        float worldX = targetNode.x - 8f;
        float worldY = 7f - targetNode.y;

        Vector2 targetPosition = new Vector2(worldX, worldY);


        Vector2 direction = (targetPosition - rb.position).normalized;

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);


        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            pathIndex++;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = Vector2.zero;
        currentPath = null;
    }
}