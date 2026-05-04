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
    public GameObject circle;
    public float range = 3f;
    private bool killing;
    private AudioSource audioSource;
    public AudioClip[] audioClips, audioClipsDie;
    private bool speaking;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
        circle.transform.localScale = new Vector3(range, range, range);

        if(player != null)
            Gizmos.DrawLine(transform.position + new Vector3(0,1,0), player.position);
        
    }
    void Update()
    {
        circle.transform.localScale = new Vector3(range, range, range);
        Direction = player.position - transform.position;
        RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, Direction, range, layerMaskPlayer);
        
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
        if(Vector3.Distance(player.position, transform.position) <= range * 2)
        {
            if(!speaking)
                StartCoroutine("Speak");
        }
    }
    IEnumerator Speak()
    {
        speaking = true;
        int num = Random.Range(0, audioClips.Length * 4);
        if(num < audioClips.Length && !audioSource.isPlaying)
        {
            audioSource.clip = audioClips[num];
            audioSource.Play();
        }
        yield return new WaitForSeconds(10f);
        speaking = false;
    }
    

    IEnumerator Kill()
    {
        killing = true;
        yield return new WaitForSeconds(0.1f);
        player.gameObject.GetComponent<PlayerMovement>().movementSpeed = 0;
        yield return new WaitForSeconds(0.1f);
        player.gameObject.GetComponent<PlayerMovement>().enabled = false;
        player.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = player.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color - new Color(0,0,0,255); //body
        player.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = player.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color - new Color(0,0,0,255); //head
        player.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = player.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().color - new Color(0,0,0,255); //head
        player.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = player.GetChild(0).GetChild(1).gameObject.GetComponent<SpriteRenderer>().color - new Color(0,0,0,255); //leg1
        player.GetChild(0).GetChild(2).gameObject.GetComponent<SpriteRenderer>().color = player.GetChild(0).GetChild(2).gameObject.GetComponent<SpriteRenderer>().color - new Color(0,0,0,255); //leg2
        GameObject effect = Instantiate(effectKill, player.position, Quaternion.identity);
        Destroy(effect, 5f);
        yield return new WaitForSeconds(2f);
        FindObjectsByType<SceneManagerPuzzle>(FindObjectsSortMode.None)[0].CloseScene(SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(3f);
        killing = false;
    }

    public IEnumerator Die()
    {
        audioSource.clip = audioClipsDie[Random.Range(0, audioClipsDie.Length)];
        audioSource.Play();
        //Debug.Log("dies");
        yield return new WaitForSeconds(2.5f);
        GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);
        Destroy(gameObject, 0.5f);
    }
}
