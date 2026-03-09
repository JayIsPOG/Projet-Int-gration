using System.Collections;
using UnityEngine;
using Unity.Burst.Intrinsics;
class Bot{

  BoardState Pos;
  simpleDiceGeneratorAI simpleGen;
  doubleDiceGeneratorAI doubleGen;
  simpleDiceGeneratorPlayer simplePlayerGen;
  doubleDiceGeneratorPlayer doublePlayerGen;
  simpleMoveArray simpleMoves;
  doubleMoveArray doubleMoves;
  Learner evaluator;
  public Bot(BoardState pos, Learner l)
  {
    Pos = pos;
    evaluator = l;
    simpleMoves = new simpleMoveArray();
    doubleMoves = new doubleMoveArray();
    doubleGen = new doubleDiceGeneratorAI(0, doubleMoves, pos);
    simpleGen = new simpleDiceGeneratorAI(0, 0, simpleMoves, pos);
    doublePlayerGen = new doubleDiceGeneratorPlayer(0, doubleMoves, pos);
    simplePlayerGen = new simpleDiceGeneratorPlayer(0, 0, simpleMoves, pos);
  }
  public void makeForDiceAI(int dice1, int dice2) // selects and plays the move with the highest score
  {
    Pos.playerTurn = false;
    int num = 0;
    uint bestMove = 0;
    double bestScore = double.MinValue;
    if(dice1 == dice2)
    {
      doubleGen.setDice(dice1);
      doubleGen.generate();
      num = doubleMoves.moveDepth - 1;
      for(int i = 0; i < doubleMoves.size(); i++)
      {
        double score = evaluateMoveAI((uint)doubleMoves.moves[i], num);
        if(score > bestScore)
        {
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
        double score = evaluateMoveAI((uint)simpleMoves.moves[i], num);
        if(score > bestScore)
        {
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

  double evaluateMoveAI(uint moveSequence, int num)
  {
    if(num < 0) return evaluator.evaluatePosition(Pos);
    uint move = (moveSequence >> (num * 8)) & 0xff;
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from - dice;
    double eval;
    if(dice == 0) // bearoff move
    {
      uint bit_mod = (uint)(((Pos.chips[from] == -1) ? 1 : 0) << from);

      Pos.chips[from]++;
      Pos.ai_present ^= bit_mod;

      eval = evaluateMoveAI(moveSequence, num - 1);

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

      eval = evaluateMoveAI(moveSequence, num - 1);

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

      eval = evaluateMoveAI(moveSequence, num - 1);

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

    Debug.Log(dice.ToString() + " : " + from.ToString() + ", " + to.ToString());

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
    Pos.playerTurn = true;
    int num = 0;
    uint bestMove = 0;
    double bestScore = double.MaxValue;
    if(dice1 == dice2)
    {
      doublePlayerGen.setDice(dice1);
      doublePlayerGen.generate();
      num = doubleMoves.moveDepth - 1;
      for(int i = 0; i < doubleMoves.size(); i++)
      {
        double score = evaluateMovePlayer((uint)doubleMoves.moves[i], num);
        if(score < bestScore)
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
        double score = evaluateMovePlayer((uint)simpleMoves.moves[i], num);
        if(score < bestScore)
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
  double evaluateMovePlayer(uint moveSequence, int num)
  {
    if(num < 0) return evaluator.evaluatePosition(Pos);
    uint move = (moveSequence >> (num * 8)) & 0xff;
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from + dice;
    double eval;
    if(dice == 0) // bearoff move
    {
      uint bit_mod = (uint)(((Pos.chips[from] == 1) ? 1 : 0) << from);

      Pos.chips[from]--;
      Pos.player_present ^= bit_mod;

      eval = evaluateMovePlayer(moveSequence, num - 1);

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

      eval = evaluateMovePlayer(moveSequence, num - 1);

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

      eval = evaluateMovePlayer(moveSequence, num - 1);

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