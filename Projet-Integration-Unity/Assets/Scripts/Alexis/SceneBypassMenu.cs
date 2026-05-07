using UnityEngine;
using UnityEngine.SceneManagement;

// Menu de debug: Espace ouvre le menu, Esc ferme. Cliquer charge la scene.
// Implementation OnGUI pour eviter les conflits avec UI Toolkit / Input.
public class SceneBypassMenu : MonoBehaviour
{
    bool isOpen = false;

    PlayerMovement playerMovement;
    Player_attack playerAttack;

    void Start()
    {
        var pm = FindObjectOfType<PlayerMovement>();
        if (pm != null)
        {
            playerMovement = pm;
            playerAttack = pm.GetComponent<Player_attack>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isOpen)
            Open();
        else if (Input.GetKeyDown(KeyCode.Escape) && isOpen)
            Close();
    }

    void Open()
    {
        isOpen = true;
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerAttack != null) playerAttack.enabled = false;
        Time.timeScale = 0f;
    }

    void Close()
    {
        isOpen = false;
        Time.timeScale = 1f;
        if (playerMovement != null) playerMovement.enabled = true;
        if (playerAttack != null) playerAttack.enabled = true;
    }

    void OnGUI()
    {
        if (!isOpen) return;

        // fond noir semi-transparent plein ecran
        Color bg = new Color(0, 0, 0, 0.8f);
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, bg);
        tex.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);

        int count = SceneManager.sceneCountInBuildSettings;
        float btnW = 360f;
        float btnH = 50f;
        float gap = 8f;
        float totalH = count * (btnH + gap) + 80f;
        float startY = (Screen.height - totalH) / 2f;
        float x = (Screen.width - btnW) / 2f;

        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.fontSize = 26;
        title.alignment = TextAnchor.MiddleCenter;
        title.normal.textColor = new Color(1f, 0.85f, 0.35f);
        title.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(x, startY, btnW, 40), "Charger une scene (Esc pour fermer)", title);

        GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = 18;
        btnStyle.fontStyle = FontStyle.Bold;

        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            float y = startY + 60f + i * (btnH + gap);
            if (GUI.Button(new Rect(x, y, btnW, btnH), i + " - " + name, btnStyle))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(i);
            }
        }
    }
}
