using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Turret : MonoBehaviour
{
    public Transform player;
    private Vector2 Direction;
    public LayerMask layerMaskPlayer;
    public GameObject explosionEffect;
    public LineRenderer line;
    public LineRenderer linePrefab;
    public GameObject effectKill;
    public float range = 3f;
    private bool killing;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);

        if(player != null)
            Gizmos.DrawLine(transform.position, player.position);
        
    }
    void Update()
    {
        Direction = player.position - transform.position + new Vector3(0,1,0);
        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position + new Vector3(0,1,0), Direction, range, layerMaskPlayer);
        
        if (rayInfo)
        {
            if(rayInfo.transform.gameObject.GetComponent<PlayerMovement>())
            {
                if(!killing)
                    StartCoroutine(Kill());
                
                if(line == null)
                {
                    line = Instantiate(linePrefab, new Vector3(0,0,0), Quaternion.identity);
                    line.GetComponent<LightLine>().waveLenght = 740;
                }
                else
                {
                    line.SetPosition(1, rayInfo.point);
                    line.SetPosition(0, transform.position + new Vector3(0,1,0));
                }
                
            }
            else
                StopCoroutine("Kill");
        }
    }

    IEnumerator Kill()
    {
        killing = true;
        yield return new WaitForSeconds(0.1f);
        player.gameObject.GetComponent<PlayerMovement>().movementSpeed = 0;
        yield return new WaitForSeconds(0.1f);
        player.gameObject.GetComponent<PlayerMovement>().enabled = false;
        player.gameObject.GetComponent<SpriteRenderer>().color = player.gameObject.GetComponent<SpriteRenderer>().color - new Color(0,0,0,255);
        GameObject effect = Instantiate(effectKill, player.position, Quaternion.identity);
        Destroy(effect, 5f);
        yield return new WaitForSeconds(3f);
        FindObjectsByType<SceneManagerPuzzle>(FindObjectsSortMode.None)[0].CloseScene(SceneManager.GetActiveScene().name);
        killing = false;
    }

    public void Die()
    {
        Debug.Log("dies");
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        Destroy(gameObject, 0.5f);
    }
}
