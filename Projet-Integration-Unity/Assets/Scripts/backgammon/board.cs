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
    public float xBorder = 128;
    private float chipUnit;
    bool hasSelected = false;
    private int pick_from;
    private BoardState Pos;
    private interractiveMoves moveGenerator;

    public int depth = 1;
    public int max_depth = 4;
    public int game_count = 0;
    public int wins = 0;

    private List<int> dices;
    Bot bot;
    private int selectedDiceIndex;
    public burger[] dice_animations;
    public AudioClip simple_move_sound;
    public AudioClip eat_move_sound;
    void Start()
    {
        dice_animations = new burger[4];
        for(int i = 0; i < 4; i++) dice_animations[i] = new burger(dice_texture);
        brain = new Learner();
        brain.LoadWeights("burger.bin");
        Pos = new BoardState();
        bot = new Bot(Pos, brain, max_depth);
        moveGenerator = new interractiveMoves(Pos, simple_move_sound, eat_move_sound);
        xUnit = ((float)Screen.width - 2 * xBorder) / 13;
        yUnit = 3 * xUnit;
        chipUnit = xUnit * 0.78125f;

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
        playPlayer();
        selectedDiceIndex= 255;
    }
    void playBot()
    {
        if(Pos.hasPlayerWon()) Application.Quit();
        bot.makeForDiceAI(Random.Range(1, 7), Random.Range(1, 7), depth);
        playPlayer();
    }
    void playPlayer()
    {
        if(Pos.hasAIWon()) Application.Quit(); // fix
        Pos.playerTurn = true;
        dices.Clear();
        dices.Add(Random.Range(1, 7));
        dice_animations[0].setOrientation(dices[0]);
        dices.Add(Random.Range(1, 7));
        dice_animations[1].setOrientation(dices[1]);
        if(dices[0] == dices[1])
        {
            dices.Add(dices[0]);
            dice_animations[2].setOrientation(dices[2]);
            dices.Add(dices[0]);
            dice_animations[3].setOrientation(dices[3]);
        }
        moveGenerator.generate(dices[0], dices[1]);
        if(moveGenerator.moveTodo <= 0){ // player must skip his turn
            playBot();
        }
        else {
            for(int i = 0; i < dices.Count; i++)
                StartCoroutine(dice_animations[i].playAnimation());
        }
    }

    void removeDiceAt(int index)
    {
        dices.RemoveAt(index);
        burger temp = dice_animations[index];
        for(int i = index + 1; i < 4; i++) dice_animations[i - 1] = dice_animations[i];
        dice_animations[3] = temp;
    }

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
        int index = (int)((Screen.height - Input.mousePosition.y) / xUnit);
        if(Input.mousePosition.x <= xUnit && index < dices.Count) return index;
        else return 255;
    }
    void Update()
    {
        /*System.Random rng = new System.Random();
        bot.makeForDicePlayer(rng.Next(1, 7), rng.Next(1, 7), depth);
        if(Pos.hasPlayerWon())
        {
            Pos.set();
            game_count++;
            wins++;
        }
        bot.makeForDiceAI(rng.Next(1, 7), rng.Next(1, 7), 0);
        if(Pos.hasAIWon()) 
        {
            Pos.set();
            game_count++;
        }*/
      
        Debug.Log(bot.evaluator.evaluatePosition(Pos));
        xUnit = ((float)Screen.width - 2 * xBorder) / 13;
        yUnit = 3 * xUnit;
        chipUnit = xUnit * 0.78125f;

        if (Input.GetMouseButton(0))
        {
            if(!hasSelected)
            {
                pick_from = getMouseIndex();
                if(pick_from != 255) {
                    if(Pos.chips[pick_from] > 0)
                    {
                        Pos.chips[pick_from]--;
                        Pos.player_present ^= (Pos.chips[pick_from] == 0 ? 1u : 0u) << pick_from;
                        if(Pos.canPlayerBearOff() && selectedDiceIndex != 255 && pick_from + dices[selectedDiceIndex] >= 25 && moveGenerator.isMoveValid((uint)pick_from))
                        {
                            if(moveGenerator.makeBearoffMove(pick_from)) {
                                playBot();
                            }
                            else {
                                removeDiceAt(selectedDiceIndex);
                                if(Pos.hasPlayerWon()) Application.Quit();
                            }
                            selectedDiceIndex = 255;
                        }
                        else hasSelected = true;
                    }
                }
                int dice_index = getDiceIndex();
                if(dice_index != 255) selectedDiceIndex = dice_index;
            }
        }
        else
        {
            if(hasSelected)
            {
                int index = getMouseIndex();
                if(index != 255 && selectedDiceIndex != 255 && pick_from + dices[selectedDiceIndex] == index && moveGenerator.isMoveValid((uint)((uint)pick_from | (uint)(dices[selectedDiceIndex] << 5))))
                {
                    if(moveGenerator.placeChip(pick_from, dices[selectedDiceIndex])) {
                        playBot();
                    }
                    else {
                        removeDiceAt(selectedDiceIndex);
                        if(Pos.hasPlayerWon()) Application.Quit();
                    }
                    selectedDiceIndex= 255;
                }
                else
                {
                    Pos.player_present ^= (Pos.chips[pick_from] == 0 ? 1u : 0u) << pick_from;
                    Pos.chips[pick_from]++;
                }
            }
            hasSelected = false;
        }
    }

    void OnGUI()
    {
        for(float i = 0; i < 6; i += 2)
        {
            float x = i * xUnit + xBorder;
            GUI.DrawTexture(new Rect(x, 0, xUnit, yUnit), rev_black_pike);
            GUI.DrawTexture(new Rect(x, Screen.height - yUnit, xUnit, yUnit), white_pike);
            x = (i + 1) * xUnit + xBorder;
            GUI.DrawTexture(new Rect(x, 0, xUnit, yUnit), rev_white_pike);
            GUI.DrawTexture(new Rect(x, Screen.height - yUnit, xUnit, yUnit), black_pike);
        }
        
        GUI.DrawTexture(new Rect(6 * xUnit + xBorder + xUnit * 0.25f, 0, xUnit * 0.5f, Screen.height), Texture2D.grayTexture);
        
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
            for(int j = 0; j < Pos.chips[i]; j++) GUI.DrawTexture(new Rect(x,  j * chipUnit * 0.5f, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit * 0.5f, chipUnit, chipUnit), black_chip_texture);
            
            x = (i - 1) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < Pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit * 0.5f, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit * 0.5f, chipUnit, chipUnit), black_chip_texture);
        }
        
        for(int i = 7; i < 13; i++)
        {
            float x = (12 - i) * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < Pos.chips[i]; j++) GUI.DrawTexture(new Rect(x,  j * chipUnit * 0.5f, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit * 0.5f, chipUnit, chipUnit), black_chip_texture);
            
            x = i * xUnit + (xUnit - chipUnit) / 2 + xBorder;
            for(int j = 0; j < Pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit * 0.5f, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit * 0.5f, chipUnit, chipUnit), black_chip_texture);
        }

        for(int i = 0; i < dices.Count; i++)
            GUI.DrawTexture(new Rect(0, i * xUnit, xUnit, xUnit), dice_animations[i].texture);

        GUIStyle font = new GUIStyle(GUI.skin.label);
        font.fontSize = 40;
        font.alignment = TextAnchor.MiddleCenter;

        if(Pos.chips[25] < 0)
        {
            font.normal.textColor = Color.white;
            GUI.DrawTexture(new Rect(xUnit * 13.0f / 2 + xBorder - chipUnit * 0.5f, Screen.height - chipUnit, chipUnit, chipUnit), black_chip_texture);
            GUI.Label(new Rect(xUnit * 13.0f / 2 + xBorder - chipUnit * 0.5f, Screen.height - chipUnit, chipUnit, chipUnit), (-Pos.chips[25]).ToString(), font);
        }
        
        if(Pos.chips[0] > 0)
        {
            font.normal.textColor = Color.black;
            GUI.DrawTexture(new Rect(xUnit * 13.0f / 2 + xBorder - chipUnit * 0.5f, 0, chipUnit, chipUnit), white_chip_texture);
            GUI.Label(new Rect(xUnit * 13.0f / 2 + xBorder - chipUnit * 0.5f, 0, chipUnit, chipUnit), Pos.chips[0].ToString(), font);
        }

        if(hasSelected) GUI.DrawTexture(new Rect(Input.mousePosition.x - chipUnit * 0.5f, Screen.height - Input.mousePosition.y - chipUnit * 0.5f, chipUnit, chipUnit), white_chip_texture);
    }
}