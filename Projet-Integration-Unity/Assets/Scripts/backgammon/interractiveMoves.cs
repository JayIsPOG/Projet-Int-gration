using UnityEngine;
using Unity.Burst.Intrinsics;

class interractiveMoves
{
  bool AreDiceDoubles;
  simpleMoveArray simpleMoves;
  doubleMoveArray doubleMoves;
  simpleDiceGeneratorPlayer singleGenerator;
  doubleDiceGeneratorPlayer doubleGenerator;
  BoardState Pos;
  uint moveSequence;
  public int moveTodo;
  public interractiveMoves(BoardState pos)
  {
    Pos = pos;
    moveTodo = 0;
    simpleMoves = new simpleMoveArray();
    doubleMoves = new doubleMoveArray();
    doubleGenerator = new doubleDiceGeneratorPlayer(0, doubleMoves, pos);
    singleGenerator = new simpleDiceGeneratorPlayer(0, 0, simpleMoves, pos);
  }

  public void generate(int dice1, int dice2)
  {
    moveSequence = 0;
    if(dice1 == dice2)
    {
      AreDiceDoubles = true;
      doubleGenerator.setDice(dice1);
      doubleGenerator.generate();
      moveTodo = doubleMoves.moveDepth;
    }
    else
    {
      AreDiceDoubles = false;
      if(dice1 > dice2) singleGenerator.setDices(dice1, dice2);
      else singleGenerator.setDices(dice2, dice1);
      singleGenerator.generate();
      moveTodo = simpleMoves.moveDepth;
    }
  }

  public bool isMoveValid(uint move)
  {
    move = (moveSequence << 8) | move;
    int shift = (moveTodo - 1) * 8;
    if (AreDiceDoubles)
    {
      for(int i = 0; i < doubleMoves.size(); i++)
        if((doubleMoves.moves[i] >> shift) == move) return true;
      return false;
    }
    for(int i = 0; i < simpleMoves.size(); i++)
      if((simpleMoves.moves[i] >> shift) == move) return true;
    return false;
  }

  public bool placeChip(int from, int dice)
  {
    int to = from + dice;
    if(Pos.chips[to] < 0)
    { 
      uint bit_to = 1u << to;
      uint bit_mod = bit_to;
      bit_to |= ((Pos.chips[25] == 0) ? 1u : 0u) << 25;

      Pos.chips[to] = 1;
      Pos.chips[25]--;
      Pos.player_present ^= bit_mod;
      Pos.ai_present ^= bit_to;
    }
    else
    {
      uint bit_mod = (uint)(((Pos.chips[to] == 0) ? 1 : 0) << to);

      Pos.chips[to]++;
      Pos.player_present ^= bit_mod;
    }
    
    moveTodo--;
    moveSequence <<= 8;
    moveSequence |= (uint)(from | (dice << 5));
    return moveTodo <= 0;
  }
}