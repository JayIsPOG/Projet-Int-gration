using Unity.Burst.Intrinsics;
using UnityEngine;

class interractiveMoves
{
  bool AreDiceDoubles;
  simpleMoveArray simpleMoves;
  doubleMoveArray doubleMoves;
  simpleDiceGeneratorPlayer singleGenerator;
  doubleDiceGeneratorPlayer doubleGenerator;
  BoardState Pos;
  uint moveSequence;
  public AudioClip simple_move_sound;
  public AudioClip eat_move_sound;
  public int movesTodo;
  public int movesDone;
  static readonly uint[] masks = new uint[4]{0xff, 0xffff, 0xffffff, 0xffffffff};
  public interractiveMoves(BoardState pos, AudioClip s, AudioClip e)
  {
    Pos = pos;
    movesTodo = 0;
    simpleMoves = new simpleMoveArray();
    doubleMoves = new doubleMoveArray();
    doubleGenerator = new doubleDiceGeneratorPlayer(0, doubleMoves, pos);
    singleGenerator = new simpleDiceGeneratorPlayer(0, 0, simpleMoves, pos);
    eat_move_sound = e;
    simple_move_sound = s;
  }
  public void generate(int dice1, int dice2)
  {
    moveSequence = 0;
    movesDone = 0;
    movesTodo = 0;
    if(dice1 == dice2)
    {
      AreDiceDoubles = true;
      doubleGenerator.setDice(dice1);
      doubleGenerator.generate();
      uint sampleMove = doubleGenerator.moveList.moves[0];
      for(; sampleMove != 0; sampleMove >>= 8) movesTodo++;
    }
    else
    {
      AreDiceDoubles = false;
      if(dice1 > dice2) singleGenerator.setDices(dice1, dice2);
      else singleGenerator.setDices(dice2, dice1);
      singleGenerator.generate();
      uint sampleMove = singleGenerator.moveList.moves[0];
      for(; sampleMove != 0; sampleMove >>= 8) movesTodo++;
    }
  }

  public bool isMoveValid(uint move)
  {
    move = moveSequence | (move << (8 * movesDone));
    uint mask = masks[movesDone];
    
    if (AreDiceDoubles)
    {
      for(int i = 0; i < doubleMoves.size(); i++)
        if((doubleMoves.moves[i] & mask) == move) return true;
      return false;
    }
    for(int i = 0; i < simpleMoves.size(); i++)
      if((simpleMoves.moves[i] & mask) == move) return true;
    return false;
  }

  public bool placeChip(int from, int dice)
  {
    int to = from + dice;
    if(Pos.chips[to] < 0)
    { 
      AudioSource.PlayClipAtPoint(eat_move_sound, Camera.main.transform.position);
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
      AudioSource.PlayClipAtPoint(simple_move_sound, Camera.main.transform.position);
      uint bit_mod = (uint)(((Pos.chips[to] == 0) ? 1 : 0) << to);

      Pos.chips[to]++;
      Pos.player_present ^= bit_mod;
    }
    
    moveSequence |= (uint)(from | (dice << 5)) << (8 * movesDone);
    movesDone++;
    return movesTodo == movesDone;
  }
  public bool makeBearoffMove(int from)
  {
    AudioSource.PlayClipAtPoint(simple_move_sound, Camera.main.transform.position);
    Pos.player_bearoff++;
    
    moveSequence |= (uint)(from) << (8 * movesDone);
    movesDone++;
    return movesTodo == movesDone;
  }
}