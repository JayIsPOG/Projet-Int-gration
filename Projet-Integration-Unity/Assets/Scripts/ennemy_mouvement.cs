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
            Debug.Log("Player trouvé automatiquement : " + player.name);
        }
        else
        {
            Debug.LogError("Aucun objet avec le tag Player trouvé !");
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
            Debug.LogError("Player est null dans FixedUpdate !");
            return;
        }

        float distance = Vector2.Distance(anciennepos, player.transform.position);
        Debug.Log(player.localPosition);
        Debug.Log("Distance joueur depuis dernier calcul : " + distance);

        if (distance > 0.5f)
        {
            Debug.Log("JOUEUR A BOUGÉ → recalcul du chemin");
            anciennepos = player.transform.position;
            ChoisirDestination();
        }
        else
        {
            Debug.Log("Joueur pas assez déplacé, on continue le chemin actuel");
        }

        SuivreChemin();
    }

    void ChoisirDestination()
    {
        Debug.Log("---- CHOISIR DESTINATION ----");

        if (player == null)
        {
            Debug.LogError("Player null dans ChoisirDestination");
            return;
        }

        float offsetX = 8f;
        float offsetY = 7f;

        int startX = Mathf.FloorToInt(transform.position.x + offsetX);
        int startY = Mathf.FloorToInt(offsetY - transform.position.y);

        int targetX = Mathf.FloorToInt(player.transform.position.x + offsetX);
        int targetY = Mathf.FloorToInt(offsetY - player.transform.position.y);

        Debug.Log($"Start grille : {startX}, {startY}");
        Debug.Log($"Target grille : {targetX}, {targetY}");

        if (startX < 0 || startX >= 37 || startY < 0 || startY >= 29)
        {
            Debug.LogError("Start hors de la grille !");
            return;
        }

        if (targetX < 0 || targetX >= 37 || targetY < 0 || targetY >= 29)
        {
            Debug.LogError("Target hors de la grille !");
            return;
        }

        Node start = gridManager.GetNode(startX, startY);
        Node end = gridManager.GetNode(targetX, targetY);

        if (!end.walkable)
        {
            Debug.LogWarning("Case cible NON WALKABLE !");
            return;
        }

        currentPath = gridManager.FindPath(start, end);

        if (currentPath == null)
        {
            Debug.LogError("AUCUN CHEMIN TROUVÉ PAR A* !");
        }
        else
        {
            Debug.Log("Chemin trouvé ! Longueur : " + currentPath.Count);
        }

        pathIndex = 0;
    }

    void SuivreChemin()
    {
        if (currentPath == null)
        {
            Debug.Log("Pas de chemin actuel");
            return;
        }

        if (pathIndex >= currentPath.Count)
        {
            Debug.Log("Chemin terminé");
            rb.velocity = Vector2.zero;
            currentPath = null;
            return;
        }

        Node targetNode = currentPath[pathIndex];

        float worldX = targetNode.x - 8f;
        float worldY = 7f - targetNode.y;

        Vector2 targetPosition = new Vector2(worldX, worldY);

        Debug.Log($"Déplacement vers noeud {pathIndex} : {targetPosition}");

        Vector2 direction = (targetPosition - rb.position).normalized;

        rb.velocity = direction * speed;

        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            Debug.Log("Noeud atteint → suivant");
            pathIndex++;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision détectée → arrêt");
        rb.velocity = Vector2.zero;
        currentPath = null;
    }
}