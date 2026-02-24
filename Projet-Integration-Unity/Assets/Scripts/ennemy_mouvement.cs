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
    public float sideOffsetX = 1.5f;
public float offsetY = -0.2f;

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
        return;

    float stopDistance = 0.7f;

    // POSITION OFFSET GAUCHE OU DROITE
   Vector3 targetOffsetPosition;

if (transform.position.x < player.position.x)
{
    targetOffsetPosition = player.position + new Vector3(-sideOffsetX, offsetY, 0);
}
else
{
    targetOffsetPosition = player.position + new Vector3(sideOffsetX, offsetY, 0);
}

    float distance = Vector2.Distance(transform.position, targetOffsetPosition);

    if (distance <= stopDistance)
    {
        rb.velocity = Vector2.zero;
        currentPath = null;
        return;
    }

    Node start = gridManager.NodeFromWorld(transform.position);
    Node end = gridManager.NodeFromWorld(targetOffsetPosition);

    if (start == null || end == null)
        return;

    if (!end.walkable)
        return;

    currentPath = gridManager.FindPath(start, end);

    pathIndex = 0;
}

    void SuivreChemin()
{
    if (currentPath == null)
        return;

    if (pathIndex >= currentPath.Count)
    {
        rb.velocity = Vector2.zero;
        currentPath = null;
        return;
    }

    Node targetNode = currentPath[pathIndex];

    Vector2 targetPosition = gridManager.WorldFromNode(targetNode);

    // CALCUL DIRECTION
    float directionX = targetPosition.x - rb.position.x;

    // FLIP ENEMY
    if (directionX > 0.01f)
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
    else if (directionX < -0.01f)
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }

    Vector2 newPos = Vector2.MoveTowards(
        rb.position,
        targetPosition,
        speed * Time.fixedDeltaTime
    );

    rb.MovePosition(newPos);

    if (Vector2.Distance(rb.position, targetPosition) < 0.05f)
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