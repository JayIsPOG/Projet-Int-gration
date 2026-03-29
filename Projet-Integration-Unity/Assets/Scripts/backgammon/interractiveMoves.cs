using UnityEngine;
using Unity.Burst.Intrinsics;

class interractiveMoves
{
  bool AreDiceDoubles;
  simpleMoveArray simpleMoves;
  doubleMoveArray doubleMoves;
  simpleGenerator simpleGen;
  orderedDoubleGenerator doubleGen;
  BoardState Pos;
  uint moveSequence;
  public int moveTodo;
  public int movesDone;
  public interractiveMoves(BoardState pos)
  {
    Pos = pos;
    moveTodo = 0;
    simpleMoves = new simpleMoveArray();
    doubleMoves = new doubleMoveArray();
    doubleGen = new orderedDoubleGenerator(0, doubleMoves, pos);
    simpleGen = new simpleGenerator(0, 0, simpleMoves, pos);
  }

  public void generate(int dice1, int dice2)
  {
    moveSequence = 0;
    if(dice1 == dice2)
    {
      AreDiceDoubles = true;
      doubleGen.setDice(dice1);
      doubleGen.generate();
      moveTodo = doubleMoves.moveDepth - 1;
    }
    else
    {
      AreDiceDoubles = false;
      if(dice1 > dice2) simpleGen.setDices(dice1, dice2);
      else simpleGen.setDices(dice2, dice1);
      simpleGen.generate();
      moveTodo = simpleMoves.moveDepth - 1;
    }
  }

  public bool isMoveValid(uint move)
  {
    move = moveSequence | (move << (8 * movesDone));
    int mask = (1 << (8 * (movesDone + 1))) - 1;
    if (AreDiceDoubles)
    {
      for(int i = 0; i < doubleMoves.size(); i++)
        if(doubleMoves.moves[i] & mask == move) return true;
      return false;
    }
    for(int i = 0; i < simpleMoves.size(); i++)
      if(simpleMoves.moves[i] & mask == move) return true;
    return false;
  }

  public bool placeChip(int from, int dice)
  {
    int to = from + dice;
    int opponent_to = 25 - to;
    if(Pos.opponent.chips[opponent_to] == 1)
    { 
      uint player_mod = 1u << to;
      uint opponent_mod = (1u << opponent_to) | ((Pos.opponent.chips[0] == 0) ? 1u : 0u);

      Pos.player.chips[to] = 1;
      Pos.opponent.chips[opponent_to] = 0;
      Pos.opponent.chips[0]++;
      Pos.player_present ^= player_mod;
      Pos.ai_present ^= opponent_mod;
    }
    else
    {
      uint bit_mod = (uint)(((Pos.chips[to] == 0) ? 1 : 0) << to);

      Pos.player.chips[to]++;
      Pos.player.present ^= bit_mod;
    }
    
    moveSequence |= (uint)(from | (dice << 5)) << (8 * movesDone);
    movesDone++;
    return movesDone == moveTodo;
  }
}