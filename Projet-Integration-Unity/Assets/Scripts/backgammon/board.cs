using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection.Emit;

/////////////////////// Change bearoff logic (dice == 0 doesnt work well, just check if to is out of bounds)
/// use bitboards for rendering ty shit

public class board : MonoBehaviour
{

    public Texture2D dice_texture;
    public Texture2D dice_texture_opponent;
    public Texture2D black_chip_texture;
    public Texture2D white_chip_texture;
    public Texture2D black_pike;
    public Texture2D white_pike;
    public Texture2D rev_black_pike;
    public Texture2D rev_white_pike;

    Learner brain;
    private float xUnit;
    private float yUnit;
    public float stackRatio = 0.5f;
    public float xBorder = 128;
    private float chipUnit;
    bool hasSelected = false;
    private int pick_from;
    private BoardState Pos;
    private interractiveMoves moveManager;

    public int depth = 1;
    public int max_depth = 4;
    public int game_count = 0;
    public int wins = 0;
    Bot bot;
    private int selectedDiceIndex;
    public AudioClip simple_move_sound;
    public AudioClip eat_move_sound;
    public float slide_lenght = 0.1f;
    public float deltaTimeSlide = 0.1f;
    float bot_x;
    float bot_y;
    bool isSliding = false;
    dice_set player_dices;
    dice_set opponent_dices;
    void Start()
    {
        player_dices = new dice_set(dice_texture);
        opponent_dices = new dice_set(dice_texture_opponent);
        brain = new Learner();
        brain.LoadWeights("burger.bin");
        Pos = new BoardState();
        bot = new Bot(Pos, brain, max_depth);
        moveManager = new interractiveMoves(Pos, simple_move_sound, eat_move_sound);
        xUnit = ((float)Screen.width - 2 * xBorder) / 13;
        yUnit = 3 * xUnit;
        chipUnit = xUnit * 0.78125f;
        selectedDiceIndex= 255;
        playPlayer();
    }
    IEnumerator playBot()
    {
        if(Pos.hasPlayerWon()){
            FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0].backgammonCompleted++;
            FindObjectOfType<DataPersistanceManager>().SaveGame();
        }
        opponent_dices.genRandomDices();
        yield return StartCoroutine(RollAllDice(opponent_dices));
        
        uint moveSequence = bot.bestMoveAI(opponent_dices.dices[0], opponent_dices.dices[1], depth);
        for(; moveSequence != 0; moveSequence >>= 8) 
        {
            uint move = moveSequence & 0xff;
            int dice = (int)(move >> 5);
            yield return StartCoroutine(makeMoveAI(move));
            opponent_dices.removeDice(dice);
        }
        playPlayer();
    }
    IEnumerator RollAllDice(dice_set set)
    {
        for (int i = 0; i < set.dice_count; i++)
            StartCoroutine(set.dice_animations[i].playAnimation());
        
        yield return null;

        bool anyRolling = true;
        while (anyRolling)
        {
            anyRolling = false;
            for (int i = 0; i < set.dice_count; i++)
                if (set.dice_animations[i].is_rolling) anyRolling = true;
            yield return null;
        }
    }
    void playPlayer()
    {
        if(Pos.hasAIWon()) SceneManager.LoadScene("Main_Menu"); // fix
        Pos.playerTurn = true;
        player_dices.genRandomDices(); // S
        moveManager.generate(player_dices.dices[0], player_dices.dices[1]);
        if(moveManager.movesTodo <= 0){ // player must skip his turn
            StartCoroutine(playBot());
        }
        else StartCoroutine(RollAllDice(player_dices));
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
        if(Input.mousePosition.x <= xUnit && index < player_dices.dice_count) return index;
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
                        if(Pos.canPlayerBearOff() && selectedDiceIndex != 255 && pick_from + player_dices.dices[selectedDiceIndex] >= 25 && moveManager.isMoveValid((uint)pick_from))
                        {
                            bool isFinished = moveManager.makeBearoffMove(pick_from);
                            player_dices.removeDiceAt(selectedDiceIndex);
                            if(Pos.hasPlayerWon()) {
                                FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0].backgammonCompleted++;
                                FindObjectOfType<DataPersistanceManager>().SaveGame();
                            }
                            if(isFinished) StartCoroutine(playBot());
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
                if(index != 255 && selectedDiceIndex != 255 && pick_from + player_dices.dices[selectedDiceIndex] == index && moveManager.isMoveValid((uint)((uint)pick_from | (uint)(player_dices.dices[selectedDiceIndex] << 5))))
                {
                    bool isFinished = moveManager.placeChip(pick_from, player_dices.dices[selectedDiceIndex]);
                    player_dices.removeDiceAt(selectedDiceIndex);
                    if(Pos.hasPlayerWon()){
                        FindObjectsByType<GlobalData>(FindObjectsSortMode.None)[0].backgammonCompleted++;
                        FindObjectOfType<DataPersistanceManager>().SaveGame();
                    }
                    if(isFinished) StartCoroutine(playBot());
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
            float x = (13 - i) * xUnit + (xUnit - chipUnit) * 0.5f + xBorder;
            for(int j = 0; j < Pos.chips[i]; j++) GUI.DrawTexture(new Rect(x,  j * chipUnit * stackRatio, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit * stackRatio, chipUnit, chipUnit), black_chip_texture);
            
            x = (i - 1) * xUnit + (xUnit - chipUnit) * 0.5f + xBorder;
            for(int j = 0; j < Pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit * stackRatio, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit * stackRatio, chipUnit, chipUnit), black_chip_texture);
        }
        
        for(int i = 7; i < 13; i++)
        {
            float x = (12 - i) * xUnit + (xUnit - chipUnit) * 0.5f + xBorder;
            for(int j = 0; j < Pos.chips[i]; j++) GUI.DrawTexture(new Rect(x,  j * chipUnit * stackRatio, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i]; j--) GUI.DrawTexture(new Rect(x, -j * chipUnit * stackRatio, chipUnit, chipUnit), black_chip_texture);
            
            x = i * xUnit + (xUnit - chipUnit) * 0.5f + xBorder;
            for(int j = 0; j < Pos.chips[i + 12]; j++) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit - j * chipUnit * stackRatio, chipUnit, chipUnit), white_chip_texture);
            for(int j = 0; j > Pos.chips[i + 12]; j--) GUI.DrawTexture(new Rect(x, Screen.height - chipUnit + j * chipUnit * stackRatio, chipUnit, chipUnit), black_chip_texture);
        }

        for(int i = 0; i < player_dices.dice_count; i++)
            GUI.DrawTexture(new Rect(0, i * xUnit, xUnit, xUnit), player_dices.dice_animations[i].texture);

        for(int i = 0; i < opponent_dices.dice_count; i++)
            GUI.DrawTexture(new Rect(Screen.width - xUnit, i * xUnit, xUnit, xUnit), opponent_dices.dice_animations[i].texture);

        GUIStyle font = new GUIStyle(GUI.skin.label);
        font.fontSize = 40;
        font.alignment = TextAnchor.MiddleCenter;

        if(Pos.chips[25] < 0)
        {
            font.normal.textColor = Color.white;
            GUI.DrawTexture(new Rect(xUnit * 6.5f + xBorder - chipUnit * 0.5f, Screen.height - chipUnit, chipUnit, chipUnit), black_chip_texture);
            GUI.Label(new Rect(xUnit * 6.5f + xBorder - chipUnit * 0.5f, Screen.height - chipUnit, chipUnit, chipUnit), (-Pos.chips[25]).ToString(), font);
        }
        
        if(Pos.chips[0] > 0)
        {
            font.normal.textColor = Color.black;
            GUI.DrawTexture(new Rect(xUnit * 6.5f + xBorder - chipUnit * 0.5f, 0, chipUnit, chipUnit), white_chip_texture);
            GUI.Label(new Rect(xUnit * 6.5f + xBorder - chipUnit * 0.5f, 0, chipUnit, chipUnit), Pos.chips[0].ToString(), font);
        }

        if(hasSelected) GUI.DrawTexture(new Rect(Input.mousePosition.x - chipUnit * 0.5f, Screen.height - Input.mousePosition.y - chipUnit * 0.5f, chipUnit, chipUnit), white_chip_texture);
        if(isSliding) GUI.DrawTexture(new Rect(bot_x, bot_y, chipUnit, chipUnit), black_chip_texture);
    }

    IEnumerator makeMoveAI(uint move)
    {
        int dice = (int)(move >> 5);
        int from = (int)(move & 0b11111);
        int to = from - dice;
        uint bit_mod;

        if(dice == 0) // bearoff move
        {
            AudioSource.PlayClipAtPoint(simple_move_sound, Camera.main.transform.position);
            bit_mod = (uint)(((Pos.chips[from] == -1) ? 1 : 0) << from);

            Pos.ai_bearoff++;
            Pos.chips[from]++;
            Pos.ai_present ^= bit_mod;

        }
        else if(Pos.chips[to] == 1)
        {
            uint bit_to = 1u << to;
            bit_mod = (uint)(bit_to | (((Pos.chips[from] == -1) ? 1 : 0) << from));
            bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

            Pos.chips[from]++;
            yield return StartCoroutine(moveChipAnimation(from, to));
            AudioSource.PlayClipAtPoint(eat_move_sound, Camera.main.transform.position);
            Pos.chips[0]++;
            Pos.chips[to] = -1;
            Pos.ai_present ^= bit_mod;
            Pos.player_present ^= bit_to;
        }
        else
        {
            bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == -1) ? 1 : 0) << from));

            Pos.chips[from]++;
            yield return StartCoroutine(moveChipAnimation(from, to));
            AudioSource.PlayClipAtPoint(simple_move_sound, Camera.main.transform.position);
            Pos.chips[to]--;
            Pos.ai_present ^= bit_mod;
        }
    }
    float calcXPosition(int slot)
    {
        if(slot == 0 || slot == 25) return xUnit * 6.5f + xBorder - chipUnit * 0.5f;
        if(slot <= 6) return (13 - slot) * xUnit + xBorder + (xUnit - chipUnit) * 0.5f;
        if(slot <= 12) return (12 - slot) * xUnit + xBorder + (xUnit - chipUnit) * 0.5f; // simplify later
        if(slot <= 18) return (slot - 13) * xUnit + xBorder + (xUnit - chipUnit) * 0.5f;
        return (slot - 12) * xUnit + xBorder + (xUnit - chipUnit) * 0.5f;
    }
    float calcYPosition(int slot)
    {
        if(slot == 0) return 0;
        if(slot == 25) return Screen.height - chipUnit;
        if(slot <= 12) return Mathf.Abs(Pos.chips[slot]) * chipUnit * stackRatio;
        return Screen.height - chipUnit - Mathf.Abs(Pos.chips[slot]) * chipUnit * stackRatio;
    }
    IEnumerator moveChipAnimation(int from, int to)
    {
        isSliding = true;
        bot_x = calcXPosition(from);
        bot_y = calcYPosition(from);

        float dx = calcXPosition(to) - bot_x;
        float dy = calcYPosition(to) - bot_y;
        int step_num = (int)(Mathf.Sqrt(dx*dx + dy*dy) / slide_lenght);
        dx /= step_num;
        dy /= step_num;
        
        for(int i = 0; i < step_num; i++)
        {
            bot_x += dx;
            bot_y += dy;
            yield return new WaitForSeconds(deltaTimeSlide);
        }
        isSliding = false;
    }
}