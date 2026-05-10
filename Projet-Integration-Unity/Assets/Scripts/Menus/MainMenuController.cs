using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{

    public Vector3[] puzzlePath;
    public Vector3[] backgammonPath;
    public Vector3[] fightPath;
    public Vector3[] titlePath;
    public GameObject[] keys;

    public GameObject menu, atrium, finalButton;
    public CanvasGroup menuCanvas, black;
    public GameObject player;
    private int currentPosition;
    private bool moving, puzzle = false, backgammon = false, fight = false, title = true, loading = false;
    private string lvlName = "Puzzle 1";
    public string puzzleLvl,fightLvl,backgammonLvl, aboutScene;

    void Start()
    {
        black.alpha = 1;
        loading = false;
        MainMenu();
    }

    void Update()
    {
        GlobalData gd = FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0];

        keys[0].SetActive(gd.puzzlesKey);
        keys[1].SetActive(gd.fightsKey);
        keys[2].SetActive(gd.backgammonKey);
        
        finalButton.SetActive(gd.puzzlesKey && gd.fightsKey && gd.backgammonKey);

        float step = 7 * Time.deltaTime;
        if(loading == true)
        {
            black.alpha = Mathf.MoveTowards(black.alpha, 1f, step/7);
            if(black.alpha == 1f){
                SceneManager.LoadScene(lvlName);
            }
        }
        else
        {
            black.alpha = Mathf.MoveTowards(black.alpha, 0f, step/7);
        }
        

        if(title)
        {
            if(!menu.activeInHierarchy)
                menu.SetActive(true);
            menuCanvas.alpha = Mathf.MoveTowards(menuCanvas.alpha, 1f, step/4);
        }
        else
        {
            menuCanvas.alpha = Mathf.MoveTowards(menuCanvas.alpha, 0f, step/8);
            if(menu.activeInHierarchy && menuCanvas.alpha == 0f)
                menu.SetActive(false);
        }


        if(puzzle)
        {
            if (moving)
            {
                player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, puzzlePath[currentPosition], step);
            }
            if (moving == true && player.transform.localPosition == puzzlePath[currentPosition])
            {
                if (player.transform.localPosition == puzzlePath[^1])
                {
                    lvlName = puzzleLvl;
                    loading = true;
                }else
                    currentPosition++;
            }
        }

        if(backgammon)
        {
            if (moving)
            {
                player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, backgammonPath[currentPosition], step);
            }
            if (moving == true && player.transform.localPosition == backgammonPath[currentPosition])
            {
                if (player.transform.localPosition == backgammonPath[^1])
                {
                    lvlName = backgammonLvl;
                    loading = true;
                }else
                    currentPosition++;
            }
        }

        if(fight)
        {
            if (moving)
            {
                player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, fightPath[currentPosition], step);
            }
            if (moving == true && player.transform.localPosition == fightPath[currentPosition])
            {
                if (player.transform.localPosition == fightPath[^1])
                {
                    lvlName = fightLvl;
                    loading = true;
                }else
                    currentPosition++;
            }
        }

        if(title)
        {
            if (moving)
            {
                player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, titlePath[currentPosition], step);
            }
            if (moving == true && player.transform.localPosition == titlePath[currentPosition])
            {
                if (player.transform.localPosition == titlePath[^1])
                {
                }else
                    currentPosition++;
            }
        }
        
        if(!title && !fight && !puzzle && !backgammon)
        {
            if (moving)
            {
                player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, new Vector3(0,-3,0), step);
            }
        }
    }
    public void MainMenu()
    {
        title = true;
        moving = true;
        atrium.SetActive(false);
    }

    public void Play()
    {
        GetComponent<AudioSource>().Play();
        title = false;
        atrium.SetActive(true);
    }

    public void About()
    {
        GetComponent<AudioSource>().Play();
        lvlName = aboutScene;
        loading = true;
    }

    public void Puzzle()
    {
        puzzle = true;
        moving = true;
    }

    public void Backgammon()
    {
        backgammon = true;
        moving = true;
    }

    public void Fight()
    {
        fight = true;
        moving = true;
    }

    public void Quit()
    {
        GetComponent<AudioSource>().Play();
        Application.Quit();
    }
}
