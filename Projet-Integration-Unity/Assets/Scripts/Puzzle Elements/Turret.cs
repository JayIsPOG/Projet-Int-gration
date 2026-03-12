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
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Direction = player.position - transform.position;
        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, Direction, 2f, layerMaskPlayer);
        if (rayInfo)
        {
            if(rayInfo.transform.gameObject.GetComponent<PlayerMovement>())
                StartCoroutine(Kill());
        }
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Die()
    {
        Debug.Log("dies");
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        Destroy(gameObject, 0.5f);
    }
}
