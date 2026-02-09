using System.Numerics;
using UnityEngine;

public class board : MonoBehaviour
{
    public Texture2D black_chip_texture;
    public Texture2D white_chip_texture;
    public Texture2D black_pike;
    public Texture2D white_pike;
    public Texture2D rev_black_pike;
    public Texture2D rev_white_pike;

    private float xUnit;
    private float yUnit;
    public float xBorder = 50;
    private float chipUnit;
    private Texture2D selected_chip = null;
    private int pick_from;
    private int chip_turn;
    public position pos;
    void Start()
    {
        xUnit = ((float)Screen.width - 2 * xBorder) / 13;
        yUnit = 3 * xUnit;
        chipUnit = xUnit * 0.8f;

        chip_turn = -1;
    }

    // Update is called once per frame
    int getMouseIndex()
    {
        int index = (int)((Input.mousePosition.x - xBorder) / xUnit);

        if(Input.mousePosition.y >= Screen.height - yUnit)
        {
            if(0 <= index && index < 6) return 11 - index;
            else if(6 < index && index < 13) return 12 - index;
        }
        if(Input.mousePosition.y <= yUnit)
        {
            if(0 <= index && index < 6) return index + 12;
            if(6 < index && index < 13) return index + 11;
        }
        return -1;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if(selected_chip == null)
            {
                int index = getMouseIndex();
                if(index != -1) {
                    if(pos.chips[index] * chip_turn > 0)
                    {
                        selected_chip = (chip_turn < 0) ?  black_chip_texture : white_chip_texture;
                        pos.chips[index] -= chip_turn;
                        pick_from = index;
                    }
                }
            }
        }
        else
        {
            if(selected_chip != null)
            {
                int index = getMouseIndex();
                if(index != -1)
                {
                    pos.chips[index] += chip_turn;
                    chip_turn = -chip_turn;
                }
                else
                {
                    pos.chips[pick_from] += chip_turn;
                }
            }
            selected_chip = null;
        }
    }

    void OnGUI()
    {
        xUnit = ((float)Screen.width - 2 * xBorder) / 13;
        yUnit = 3 * xUnit;
        chipUnit = xUnit * 0.8f;

        for(float i = 0; i < 6; i+=2)
        {
            float x = i * xUnit + xBorder;
            GUI.DrawTexture(new Rect(x, 0, xUnit, yUnit), rev_black_pike);
            GUI.DrawTexture(new Rect(x, Screen.height - yUnit, xUnit, yUnit), white_pike);
            x = (i + 1) * xUnit + xBorder;
            GUI.DrawTexture(new Rect(x, 0, xUnit, yUnit), rev_white_pike);
            GUI.DrawTexture(new Rect(x, Screen.height - yUnit, xUnit, yUnit), black_pike);
        }
        
        GUI.DrawTexture(new Rect(6 * xUnit + xBorder + xUnit / 4, 0, xUnit / 2, Screen.height), Texture2D.grayTexture);
        
        for(float i = 7; i < 13; i+=2)
        {
            float x = i * xUnit + xBorder;
            GUI.DrawTexture(new Rect(x, 0, xUnit, yUnit), rev_black_pike);
            GUI.DrawTexture(new Rect(x, Screen.height - yUnit, xUnit, yUnit), white_pike);
            x = (i + 1) * xUnit + xBorder;
            GUI.DrawTexture(new Rect(x, 0, xUnit, yUnit), rev_white_pike);
            GUI.DrawTexture(new Rect(x, Screen.height - yUnit, xUnit, yUnit), black_pike);
        }

        for(int i = 0; i < 6; i++)
        {
            float x = (12 - i) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < pos.chips[i]; j++) GUI.DrawTexture(new Rect(x, j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
            
            x = i * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
        }
        
        for(int i = 6; i < 12; i++)
        {
            float x = (11 - i) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < pos.chips[i]; j++) GUI.DrawTexture(new Rect(x, j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
            
            x = (i + 1) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
        }

        if(selected_chip != null) GUI.DrawTexture(new Rect(Input.mousePosition.x - chipUnit / 2, Screen.height - Input.mousePosition.y - chipUnit / 2, chipUnit, chipUnit), selected_chip);

    }
}
