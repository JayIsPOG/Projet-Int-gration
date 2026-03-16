using UnityEngine;
using System.Collections.Generic;
/*
there is an offset of one to the rendering and picking chips(cuz lowest board index = 1, bar = 0)
*/

public class board : MonoBehaviour
{

    public Texture2D dice_texture;
    private Texture2D[] dice_faces = new Texture2D[6];
    public Texture2D black_chip_texture;
    public Texture2D white_chip_texture;
    public Texture2D black_pike;
    public Texture2D white_pike;
    public Texture2D rev_black_pike;
    public Texture2D rev_white_pike;

    Learner brain;
    private float xUnit;
    private float yUnit;
    public float xBorder = 50;
    private float chipUnit;
    private Texture2D selected_chip = null;
    private int pick_from;
    private BoardState Pos;
    private interractiveMoves moveGenerator;

    private List<int> dices;
    Bot bot;
    private int selectedDiceIndex;
    void rollDices()
    {
        dices.Clear();
        dices.Add(Random.Range(1, 7));
        dices.Add(Random.Range(1, 7));
        if(dices[0] == dices[1])
        {
            dices.Add(dices[0]);
            dices.Add(dices[0]);
        }
        moveGenerator.generate(dices[0], dices[1]);
        if(moveGenerator.moveTodo <= 0){ // player must skip his turn
            bot.makeForDiceAI(Random.Range(1, 7), Random.Range(1, 7));
            rollDices();
        }
    }
    void Start()
    {
        brain = new Learner();
        brain.LoadWeights("new_weights.bin");
        Pos = new BoardState();
        bot = new Bot(Pos, brain);
        moveGenerator = new interractiveMoves(Pos);
        xUnit = ((float)Screen.width - 2 * xBorder) / 13;
        yUnit = 3 * xUnit;
        chipUnit = xUnit * 0.8f;

        int faceHeight = dice_texture.height / 6;
        int faceWidth = dice_texture.width;

        for(int i = 0; i < 6; i++)
        {
            Color[] pixels = dice_texture.GetPixels(0, i * faceHeight, faceWidth, faceHeight);
            dice_faces[5 - i] = new Texture2D(faceWidth, faceHeight);
            dice_faces[5 - i].SetPixels(pixels);
            dice_faces[5 - i].Apply();
        }
        dices = new List<int>();
        rollDices();
        selectedDiceIndex= 255;
    }

    // Update is called once per frame

    int getMouseIndex()
    {
        int index = (int)((Input.mousePosition.x - xBorder) / xUnit);

        if(Input.mousePosition.y >= Screen.height - yUnit)
        {
            if(0 <= index && index < 6) return 12 - index;
            else if(6 < index && index < 13) return 13 - index;
        }
        
        if(Input.mousePosition.y >= Screen.height - chipUnit && index == 6) return 0; // player bar;

        if(Input.mousePosition.y <= yUnit)
        {
            if(0 <= index && index < 6) return index + 13;

            if(6 < index && index < 13) return index + 12;
        }
        return 255;
    }
    int getDiceIndex()
    {
        int index = (int)((Screen.height - Input.mousePosition.y) / chipUnit);
        if(Input.mousePosition.x <= chipUnit && index < dices.Count) return index;
        else return 255;
    }
    void Update()
    {
      /*System.Random rng = new System.Random();
      bot.makeForDicePlayer(rng.Next(1, 7), rng.Next(1, 7));
      System.Threading.Thread.Sleep(1000);
      bot.makeForDiceAI(rng.Next(1, 7), rng.Next(1, 7));
      System.Threading.Thread.Sleep(1000);*/
      Debug.Log(bot.evaluator.evaluatePosition(Pos));
        xUnit = ((float)Screen.width - 2 * xBorder) / 13;
        yUnit = 3 * xUnit;
        chipUnit = xUnit * 0.8f;

        if (Input.GetMouseButton(0))
        {
            if(selected_chip == null)
            {
                int index = getMouseIndex();
                if(index != 255) {
                    if(Pos.chips[index] > 0)
                    {
                        selected_chip = white_chip_texture;
                        Pos.chips[index]--;
                        Pos.player_present ^= (Pos.chips[index] == 0 ? 1u : 0u) << index;
                        pick_from = index;
                    }
                }
                int dice_index = getDiceIndex();
                if(dice_index != 255) selectedDiceIndex = dice_index;
            }
        }
        else
        {
            if(selected_chip != null)
            {
                int index = getMouseIndex();
                if(index != 255 && selectedDiceIndex != 255 && pick_from + dices[selectedDiceIndex] == index && moveGenerator.isMoveValid((uint)((uint)pick_from | (uint)(dices[selectedDiceIndex] << 5))))
                {
                    if(moveGenerator.placeChip(pick_from, dices[selectedDiceIndex])) {
                        bot.makeForDiceAI(Random.Range(1, 7), Random.Range(1, 7));
                        rollDices();
                    }
                    else dices.RemoveAt(selectedDiceIndex);
                    selectedDiceIndex= 255;
                }
                else
                {
                    Pos.player_present ^= (Pos.chips[pick_from] == 0 ? 1u : 0u) << pick_from;
                    Pos.chips[pick_from]++;
                }
            }
            selected_chip = null;
        }        
    }

    void OnGUI()
    {
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

        for(int i = 1; i < 7; i++)
        {
            float x = (13 - i) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < Pos.chips[i]; j++) GUI.DrawTexture(new Rect(x, j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
            
            x = (i - 1) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < Pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
        }
        
        for(int i = 7; i < 13; i++)
        {
            float x = (12 - i) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < Pos.chips[i]; j++) GUI.DrawTexture(new Rect(x, j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
            
            x = i * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < Pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit / 2, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit / 2, chipUnit, chipUnit), black_chip_texture);
        }

        for(int i = 0; i < dices.Count; i++)
            GUI.DrawTexture(new Rect(0, i * chipUnit, chipUnit, chipUnit), dice_faces[dices[i] - 1]);

        if(Pos.chips[25] < 0)
        {
            GUI.DrawTexture(new Rect(6 * xUnit + xBorder + xUnit / 4, Screen.height - chipUnit, chipUnit, chipUnit), black_chip_texture);
            GUI.Label(new Rect(6 * xUnit + xBorder + xUnit / 4, Screen.height - chipUnit, chipUnit, chipUnit), (-Pos.chips[25]).ToString());
        }
        
        if(Pos.chips[0] > 0)
        {
            GUI.DrawTexture(new Rect(6 * xUnit + xBorder + xUnit / 4, 0, chipUnit, chipUnit), white_chip_texture);
            GUI.Label(new Rect(6 * xUnit + xBorder + xUnit / 4, 0, chipUnit, chipUnit), Pos.chips[0].ToString());
        }

        if(selected_chip != null) GUI.DrawTexture(new Rect(Input.mousePosition.x - chipUnit / 2, Screen.height - Input.mousePosition.y - chipUnit / 2, chipUnit, chipUnit), selected_chip);
    }
}