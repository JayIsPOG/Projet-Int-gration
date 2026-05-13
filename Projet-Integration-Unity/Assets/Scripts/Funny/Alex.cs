using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alex : MonoBehaviour
{
    public bool isAlexing, canAlex;
    public float min = 30f,max = 120f;
    public AudioClip[] audioClips;
    private List<KeyCode> secretCode = new List<KeyCode>()
    {
        KeyCode.UpArrow,
        KeyCode.UpArrow,
        KeyCode.DownArrow,
        KeyCode.DownArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.LeftArrow,
        KeyCode.RightArrow,
        KeyCode.B,
        KeyCode.A
    };

    private List<KeyCode> inputBuffer = new List<KeyCode>();

    void Update()
    {
        if(!isAlexing && canAlex)
            StartCoroutine(Alexing());
        // Loop through all possible keys
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                inputBuffer.Add(key);

                // Keep buffer same length as code
                if (inputBuffer.Count > secretCode.Count)
                {
                    inputBuffer.RemoveAt(0);
                }

                CheckCode();
            }
        }
    }

    void CheckCode()
    {
        if (inputBuffer.Count < secretCode.Count)
            return;

        for (int i = 0; i < secretCode.Count; i++)
        {
            if (inputBuffer[i] != secretCode[i])
                return;
        }

        ActivateSecret();
    }

    void ActivateSecret()
    {
        Debug.Log("Secret code activated!");
        canAlex = true;
    }

    IEnumerator Alexing()
    {
        isAlexing = true;
        yield return new WaitForSeconds(Random.Range(min, max));
        transform.GetComponent<AudioSource>().clip = audioClips[Random.Range(0, audioClips.Length)];
        yield return new WaitForSeconds(0.35f);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2f);
        transform.GetComponent<AudioSource>().Stop();
        transform.GetChild(0).gameObject.SetActive(false);
        isAlexing = false;
    }
}
