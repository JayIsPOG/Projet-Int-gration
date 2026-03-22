using System.Collections;
using UnityEngine;
using Unity.Burst.Intrinsics;
class Bot{

  BoardState Pos;
  public simpleDiceGeneratorAI[] simpleGenPool;
  public doubleDiceGeneratorAI[] doubleGenPool;
  public simpleDiceGeneratorPlayer[] simplePlayerGenPool;
  public unorderedDoubleDiceGeneratorPlayer[] doublePlayerGenPool;
  public simpleMoveArray[] simpleMovesPool;
  public doubleMoveArray[] doubleMovesPool;
  public Learner evaluator;
  int Depth;
  public Bot(BoardState pos, Learner l, int D)
  {
    Depth = D + 1;
    Pos = pos;
    evaluator = l;
    simpleMovesPool = new simpleMoveArray[Depth];
    doubleMovesPool = new doubleMoveArray[Depth];
    doubleGenPool = new doubleDiceGeneratorAI[Depth];
    simpleGenPool = new simpleDiceGeneratorAI[Depth];
    doublePlayerGenPool = new unorderedDoubleDiceGeneratorPlayer[Depth];
    simplePlayerGenPool = new simpleDiceGeneratorPlayer[Depth];
    for(int i = 0; i < Depth; i++){
      simpleMovesPool[i] = new simpleMoveArray();
      doubleMovesPool[i] = new doubleMoveArray();
      doubleGenPool[i] = new doubleDiceGeneratorAI(0, doubleMovesPool[i], pos);
      simpleGenPool[i] = new simpleDiceGeneratorAI(0, 0, simpleMovesPool[i], pos);
      doublePlayerGenPool[i] = new unorderedDoubleDiceGeneratorPlayer(0, doubleMovesPool[i], pos);
      simplePlayerGenPool[i] = new simpleDiceGeneratorPlayer(0, 0, simpleMovesPool[i], pos);
    }
    Depth = D;
  }
  //Minimax must exit if a player has won
  public float minimaxAi(int depth, float alpha, float beta)
  {
    if(depth < 0) return evaluator.evaluatePosition(Pos);
    if(Pos.hasPlayerWon()) return 0.0f;
    simpleMoveArray _simpleMoves = simpleMovesPool[depth];
    doubleMoveArray _doubleMoves = doubleMovesPool[depth];
    simpleDiceGeneratorAI simpleGenerator = simpleGenPool[depth];
    doubleDiceGeneratorAI doubleGenerator = doubleGenPool[depth];
    float totalScore = 0;
    float max = float.MinValue;
    
    float A = 36 * (alpha - 1) + 1;
    float B = 36 * (beta - 0) + 0;
    float AX, BX;

    for(simpleGenerator.dice1 = 2; simpleGenerator.dice1 <= 6; simpleGenerator.dice1++)
      for(simpleGenerator.dice2 = 1; simpleGenerator.dice2 < simpleGenerator.dice1; simpleGenerator.dice2++)
        {
          AX = Mathf.Max(A, 0);
          BX = Mathf.Min(B, 1);

          Debug.Log("gen");
          simpleGenerator.generate();
          Debug.Log(_simpleMoves.size());
          for(int i = 0; i < _simpleMoves.size(); i++){
            max = Mathf.Max(max, evaluateMoveAI(_simpleMoves.moves[i], _simpleMoves.moveDepth - 1, depth - 1, AX, BX));
            AX = Mathf.Max(AX, max);
            //if (AX >= BX) break;
          }
          if(_simpleMoves.size() == 0) max = minimaxPlayer(depth - 1, AX, BX);

          //if(max <= A) return alpha;
          //if(max >= B) return beta;

          totalScore += 2 * max; // car il y a 2 / 36 chances d'avoir une configuration de dé avec deux faces différentes

          A += (1 - max) * 2;
          B += (0 - max) * 2;
          max = float.MinValue;
        }
    for(doubleGenerator.dice = 1; doubleGenerator.dice <= 6; doubleGenerator.dice++)
    {
      AX = Mathf.Max(A, 0);
      BX = Mathf.Min(B, 1);

          Debug.Log("gen");
      doubleGenerator.generate();
      Debug.Log(_doubleMoves.size());
      for(int i = 0; i < _doubleMoves.size(); i++){
        max = Mathf.Max(max, evaluateMoveAI(_doubleMoves.moves[i], _doubleMoves.moveDepth - 1, depth - 1, AX, BX));
        AX = Mathf.Max(AX, max);
        //if (AX >= BX) break;
      }
      if(_doubleMoves.size() == 0) max = minimaxPlayer(depth - 1, AX, BX);

      //if(max <= A) return alpha;
      //if(max >= B) return beta;

      totalScore += max; // car il y a 1 / 36 chances d'avoir une configuration deux dés de faces identiques

      A += 1 - max;
      B += 0 - max;
      max = float.MinValue;
    }
    return totalScore / 36;
  }
  public float minimaxPlayer(int depth, float alpha, float beta)
  {
    if(depth < 0) return evaluator.evaluatePosition(Pos);
    if(Pos.hasAIWon()) return 1.0f;
    simpleMoveArray _simpleMoves = simpleMovesPool[depth];
    doubleMoveArray _doubleMoves = doubleMovesPool[depth];
    simpleDiceGeneratorPlayer simpleGenerator = simplePlayerGenPool[depth];
    unorderedDoubleDiceGeneratorPlayer doubleGenerator = doublePlayerGenPool[depth];
    float totalScore = 0;
    float min = float.MaxValue;
    
    float A = 36 * (alpha - 1) + 1;
    float B = 36 * (beta - 0) + 0;
    float AX, BX;

    for(simpleGenerator.dice1 = 2; simpleGenerator.dice1 <= 6; simpleGenerator.dice1++)
      for(simpleGenerator.dice2 = 1; simpleGenerator.dice2 < simpleGenerator.dice1; simpleGenerator.dice2++)
        {
          AX = Mathf.Max(A, 0);
          BX = Mathf.Min(B, 1);

          Debug.Log("gen");
          simpleGenerator.generate();
          Debug.Log(_simpleMoves.size());
          for(int i = 0; i < _simpleMoves.size(); i++){
            min = Mathf.Min(min, evaluateMovePlayer(_simpleMoves.moves[i], _simpleMoves.moveDepth - 1, depth - 1, AX, BX));
            BX = Mathf.Min(BX, min);
            //if (AX >= BX) break;
          }
          if(_simpleMoves.size() == 0) min = minimaxAi(depth - 1, AX, BX);

          //if(min <= A) return alpha;
          //if(min >= B) return beta;

          totalScore += 2 * min; // car il y a 2 / 36 chances d'avoir une configuration de dé avec deux faces différentes

          A += (1 - min) * 2;
          B += (0 - min) * 2;
          min = float.MaxValue;
        }
    for(doubleGenerator.dice = 1; doubleGenerator.dice <= 6; doubleGenerator.dice++)
    {
      AX = Mathf.Max(A, 0);
      BX = Mathf.Min(B, 1);

          Debug.Log("gen");
      doubleGenerator.generate();
      Debug.Log(_doubleMoves.size());
      for(int i = 0; i < _doubleMoves.size(); i++){
        min = Mathf.Min(min, evaluateMovePlayer(_doubleMoves.moves[i], _doubleMoves.moveDepth - 1, depth - 1, AX, BX));
        BX = Mathf.Min(BX, min);
        //if (AX >= BX) break;
      }
      if(_doubleMoves.size() == 0) min = minimaxAi(depth - 1, AX, BX);

      //if(min <= A) return alpha;
      //if(min >= B) return beta;

      totalScore += min; // car il y a 1 / 36 chances d'avoir une configuration deux dés de faces identiques

      A += 1 - min;
      B += 0 - min;
      min = float.MaxValue;
    }
    return totalScore / 36;
  }

  public void makeForDiceAI(int dice1, int dice2) // selects and plays the move with the highest score
  {
    simpleMoveArray simpleMoves = simpleMovesPool[Depth];
    doubleMoveArray doubleMoves = doubleMovesPool[Depth];
    simpleDiceGeneratorAI simpleGen = simpleGenPool[Depth];
    doubleDiceGeneratorAI doubleGen = doubleGenPool[Depth];
    Pos.playerTurn = false;
    int num = 0;
    uint bestMove = 0;
    float bestScore = 0.0f;
    if(dice1 == dice2)
    {
      doubleGen.setDice(dice1);
          Debug.Log("gen");
      doubleGen.generate();
      num = doubleMoves.moveDepth - 1;
      for(int i = 0; i < doubleMoves.size(); i++)
      {
        float score = evaluateMoveAI((uint)doubleMoves.moves[i], num, Depth - 1, bestScore, 1.0f);
        if(score >= bestScore)
        {
          if(score < 0 || score > 1) Debug.Log(score);
          bestScore = score;
          bestMove = doubleMoves.moves[i];
        }
      }
    }
    else
    {
      if(dice1 > dice2) simpleGen.setDices(dice1, dice2);
      else simpleGen.setDices(dice2, dice1);
      simpleGen.generate();
      num = simpleMoves.moveDepth - 1;
      for(int i = 0; i < simpleMoves.size(); i++)
      {
        float score = evaluateMoveAI((uint)simpleMoves.moves[i], num, Depth - 1, bestScore, 1.0f);
        if(score >= bestScore)
        {
          if(score < 0 || score > 1) Debug.Log(score);
          bestScore = score;
          bestMove = simpleMoves.moves[i];
        }
      }
    }
    for(; num >= 0; num--)
    {
      uint move = (bestMove >> (num * 8)) & 0xff;
      makeMoveAI(move);
    }
  }

  public void makeForDiceAIRandom(int dice1, int dice2)
  {
    simpleMoveArray simpleMoves = simpleMovesPool[Depth];
    doubleMoveArray doubleMoves = doubleMovesPool[Depth];
    simpleDiceGeneratorAI simpleGen = simpleGenPool[Depth];
    doubleDiceGeneratorAI doubleGen = doubleGenPool[Depth];
    Pos.playerTurn = false;
    int num = 0;
    uint bestMove = 0;
    if(dice1 == dice2)
    {
      doubleGen.setDice(dice1);
      doubleGen.generate();
      num = doubleMoves.moveDepth - 1;
      bestMove = doubleMoves.moves[Random.Range(0, doubleMoves.size())];
    }
    else
    {
      if(dice1 > dice2) simpleGen.setDices(dice1, dice2);
      else simpleGen.setDices(dice2, dice1);
      simpleGen.generate();
      num = simpleMoves.moveDepth - 1;
      bestMove = simpleMoves.moves[Random.Range(0, simpleMoves.size())];
    }
    for(; num >= 0; num--)
    {
      uint move = (bestMove >> (num * 8)) & 0xff;
      makeMoveAI(move);
    }
  }

  float evaluateMoveAI(uint moveSequence, int num, int depth, float alpha, float beta)
  {
    if(num < 0) return minimaxPlayer(depth, alpha, beta);
    uint move = (moveSequence >> (num * 8)) & 0xff;
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from - dice;
    float eval;
    if(dice == 0) // bearoff move
    {
      uint bit_mod = (uint)(((Pos.chips[from] == -1) ? 1 : 0) << from);

      Pos.chips[from]++;
      Pos.ai_present ^= bit_mod;

      eval = evaluateMoveAI(moveSequence, num - 1, depth, alpha, beta);

      Pos.chips[from]--;
      Pos.ai_present ^= bit_mod;

    }
    else if(Pos.chips[to] == 1)
    {
      uint bit_to = (1u << to);
      uint bit_mod = (uint)(bit_to | (((Pos.chips[from] == -1) ? 1 : 0) << from));
      bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

      Pos.chips[to] = -1;
      Pos.chips[from]++;
      Pos.chips[0]++;
      Pos.ai_present ^= bit_mod;
      Pos.player_present ^= bit_to;

      eval = evaluateMoveAI(moveSequence, num - 1, depth, alpha, beta);

      Pos.chips[to] = 1;
      Pos.chips[from]--;
      Pos.chips[0]--;
      Pos.ai_present ^= bit_mod;
      Pos.player_present ^= bit_to;
    }
    else
    {
      uint bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == -1) ? 1 : 0) << from));

      Pos.chips[from]++;
      Pos.chips[to]--;
      Pos.ai_present ^= bit_mod;

      eval = evaluateMoveAI(moveSequence, num - 1, depth, alpha, beta);

      Pos.chips[to]++;
      Pos.chips[from]--;
      Pos.ai_present ^= bit_mod;
    }
    return eval;
  }

  void makeMoveAI(uint move) // can be reused, not pasted
  {
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from - dice;
    uint bit_mod;

    if(dice == 0) // bearoff move
    {
      bit_mod = (uint)(((Pos.chips[from] == -1) ? 1 : 0) << from);

      Pos.chips[from]++;
      Pos.ai_present ^= bit_mod;

    }
    else if(Pos.chips[to] == 1)
    {
      uint bit_to = 1u << to;
      bit_mod = (uint)(bit_to | (((Pos.chips[from] == -1) ? 1 : 0) << from));
      bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

      Pos.chips[to] = -1;
      Pos.chips[from]++;
      Pos.chips[0]++;
      Pos.ai_present ^= bit_mod;
      Pos.player_present ^= bit_to;
    }
    else
    {
      bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == -1) ? 1 : 0) << from));

      Pos.chips[from]++;
      Pos.chips[to]--;
      Pos.ai_present ^= bit_mod;
    }
  }


  public void makeForDicePlayer(int dice1, int dice2) // selects and plays the move with the lowest score
  {
    simpleMoveArray simpleMoves = simpleMovesPool[Depth];
    doubleMoveArray doubleMoves = doubleMovesPool[Depth];
    simpleDiceGeneratorPlayer simplePlayerGen = simplePlayerGenPool[Depth];
    unorderedDoubleDiceGeneratorPlayer doublePlayerGen = doublePlayerGenPool[Depth];
    Pos.playerTurn = true;
    int num = 0;
    uint bestMove = 0;
    float bestScore = 1.0f;
    if(dice1 == dice2)
    {
      doublePlayerGen.setDice(dice1);
      doublePlayerGen.generate();
      num = doubleMoves.moveDepth - 1;
      for(int i = 0; i < doubleMoves.size(); i++)
      {
        float score = evaluateMovePlayer((uint)doubleMoves.moves[i], num, Depth - 1, 0.0f, bestScore);
        if(score <= bestScore)
        {
          bestScore = score;
          bestMove = doubleMoves.moves[i];
        }
      }
    }
    else
    {
      if(dice1 > dice2) simplePlayerGen.setDices(dice1, dice2);
      else simplePlayerGen.setDices(dice2, dice1);
      simplePlayerGen.generate();
      num = simpleMoves.moveDepth - 1;
      for(int i = 0; i < simpleMoves.size(); i++)
      {
        float score = evaluateMovePlayer((uint)simpleMoves.moves[i], num, Depth - 1, 0.0f, bestScore);
        if(score <= bestScore)
        {
          bestScore = score;
          bestMove = simpleMoves.moves[i];
        }
      }
    }
    for(; num >= 0; num--)
    {
      uint move = (bestMove >> (num * 8)) & 0xff;
      makeMovePlayer(move);
    }
  }

  public void makeForDicePlayerRandom(int dice1, int dice2) // selects and plays the move with the lowest score
  {
    simpleMoveArray simpleMoves = simpleMovesPool[Depth];
    doubleMoveArray doubleMoves = doubleMovesPool[Depth];
    simpleDiceGeneratorPlayer simplePlayerGen = simplePlayerGenPool[Depth];
    unorderedDoubleDiceGeneratorPlayer doublePlayerGen = doublePlayerGenPool[Depth];
    Pos.playerTurn = true;
    int num = 0;
    uint bestMove = 0;
    float bestScore = 1.0f;
    if(dice1 == dice2)
    {
      doublePlayerGen.setDice(dice1);
      doublePlayerGen.generate();
      num = doubleMoves.moveDepth - 1;
      bestMove = doubleMoves.moves[Random.Range(0, doubleMoves.size())];
    }
    else
    {
      if(dice1 > dice2) simplePlayerGen.setDices(dice1, dice2);
      else simplePlayerGen.setDices(dice2, dice1);
      simplePlayerGen.generate();
      num = simpleMoves.moveDepth - 1;
      bestMove = simpleMoves.moves[Random.Range(0, simpleMoves.size())];
    }
    for(; num >= 0; num--)
    {
      uint move = (bestMove >> (num * 8)) & 0xff;
      makeMovePlayer(move);
    }
  }
  float evaluateMovePlayer(uint moveSequence, int num, int depth, float alpha, float beta)
  {
    if(num < 0) return minimaxAi(depth, alpha, beta);
    uint move = (moveSequence >> (num * 8)) & 0xff;
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from + dice;
    if(to >= 26 || to < 0) {
      Debug.Log("To : " + to.ToString() + "\nFrom : " + from.ToString() + "\nDice : " + dice.ToString() + "\nNum : " + num.ToString());
    }
    float eval;
    if(dice == 0) // bearoff move
    {
      uint bit_mod = (uint)(((Pos.chips[from] == 1) ? 1 : 0) << from);

      Pos.chips[from]--;
      Pos.player_present ^= bit_mod;

      eval = evaluateMovePlayer(moveSequence, num - 1, depth, alpha, beta);

      Pos.chips[from]++;
      Pos.player_present ^= bit_mod;
    }
    else if(Pos.chips[to] == -1)
    {
      uint bit_to = (1u << to);
      uint bit_mod = (uint)(bit_to | (((Pos.chips[from] == 1) ? 1 : 0) << from));
      bit_to |= ((Pos.chips[25] == 0) ? 1u : 0u) << 25;

      Pos.chips[to] = 1;
      Pos.chips[from]--;
      Pos.chips[25]--;
      Pos.player_present ^= bit_mod;
      Pos.ai_present ^= bit_to;

      eval = evaluateMovePlayer(moveSequence, num - 1, depth, alpha, beta);

      Pos.chips[to] = -1;
      Pos.chips[from]++;
      Pos.chips[25]++;
      Pos.player_present ^= bit_mod;
      Pos.ai_present ^= bit_to;
    }
    else
    {
      uint bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == 1) ? 1 : 0) << from));

      Pos.chips[from]--;
      Pos.chips[to]++;
      Pos.player_present ^= bit_mod;

      eval = evaluateMovePlayer(moveSequence, num - 1, depth, alpha, beta);

      Pos.chips[to]--;
      Pos.chips[from]++;
      Pos.player_present ^= bit_mod;
    }
    return eval;
  }

  void makeMovePlayer(uint move) // can be reused, not pasted
  {
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from + dice;
    if(dice == 0) // bearoff move
    {
      uint bit_mod = (uint)(((Pos.chips[from] == 1) ? 1 : 0) << from);

      Pos.chips[from]--;
      Pos.player_present ^= bit_mod;
    }
    else if(Pos.chips[to] == -1)
    {
      uint bit_to = (1u << to);
      uint bit_mod = (uint)(bit_to | (((Pos.chips[from] == 1) ? 1 : 0) << from));
      bit_to |= ((Pos.chips[25] == 0) ? 1u : 0u) << 25;

      Pos.chips[to] = 1;
      Pos.chips[from]--;
      Pos.chips[25]--;
      Pos.player_present ^= bit_mod;
      Pos.ai_present ^= bit_to;
    }
    else
    {
      uint bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == 1) ? 1 : 0) << from));

      Pos.chips[from]--;
      Pos.chips[to]++;
      Pos.player_present ^= bit_mod;
    }
  }
}