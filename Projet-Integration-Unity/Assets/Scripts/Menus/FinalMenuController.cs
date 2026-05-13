using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FinalMenuController : MonoBehaviour
{

    public Vector3[] finalPath;

    public CanvasGroup menuCanvas, black;
    public GameObject player, gem, gemAudio, grabIcon, endMenu;
    public SpriteRenderer chest;
    public Sprite chest1, chest2;
    private int currentPosition;
    private bool moving, final, loading = false;

    void Start()
    {
        black.alpha = 1;
        loading = false;
    }

    void Update()
    {
        float step = 7 * Time.deltaTime;
        
        black.alpha = Mathf.MoveTowards(black.alpha, 0f, step/7);

        menuCanvas.alpha = Mathf.MoveTowards(menuCanvas.alpha, 0f, step/8);
        
        if(final)
        {
            if (moving == true && player.transform.localPosition == finalPath[currentPosition])
            {
                if (player.transform.localPosition == finalPath[^1])
                {
                    StartCoroutine("End");
                    moving = false;
                }else
                    currentPosition++;
            }
            player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, finalPath[currentPosition], step);
        }else
        {
            player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, finalPath[0], step);
            
        }
    }

    public void Final()
    {
        final = true;
        moving = true;
    }

    public void Quit()
    {
        Debug.Log("Quit Application");
        GetComponent<AudioSource>().Play();
        Application.Quit();
    }

    IEnumerator End()
    {
        grabIcon.SetActive(false);
        gemAudio.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(8f);
        chest.sprite = chest2;
        gem.SetActive(true);
        yield return new WaitForSeconds(2f);
        endMenu.SetActive(true);
    }
}
