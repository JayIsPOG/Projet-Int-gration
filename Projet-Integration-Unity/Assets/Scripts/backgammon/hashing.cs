
using System.Collections.Generic;
using System.Resources;
using Unity.Burst.Intrinsics;

public class evalInfo
{
	public sbyte ldepth;
	public sbyte udepth;
	public float upper_bound;
	public float lower_bound;
	public ulong key;
  public evalInfo(sbyte ld, sbyte ud, float u, float l, ulong k)
  {
    ldepth = ld;
    udepth = ud;
    key = k;
    upper_bound = u;
    lower_bound = l;
  }
  public evalInfo()
  {
    reset(0);
  }

  public void reset(ulong k)
  {
    ldepth = -127;
    udepth = -127;
    upper_bound = float.MaxValue;
    lower_bound = float.MinValue;
    key = k;
  }
  public void storeUpper(sbyte depth, float eval) {
    if(udepth <= depth) {
        udepth = depth;
        upper_bound = eval;
    }
  }
  public void storeLower(sbyte depth, float eval) {
    if(ldepth <= depth) {
        ldepth = depth;
        lower_bound = eval;
    }
  }

  public void storeAll(sbyte depth, float eval) {
    if(udepth <= depth) {
        udepth = depth;
        upper_bound = eval;
    }
    if(ldepth <= depth) {
        ldepth = depth;
        lower_bound = eval;
    }
  }
};
public class TranspositionTable
{
  public ulong size;
  public ulong[,] aiHash;
  public ulong[,] playerHash;
  public ulong[] turnHash;
  public evalInfo[] table;
  public TranspositionTable(int Size)
  {
    aiHash = new ulong[26, 15];
    playerHash = new ulong[26, 15];
    turnHash = new ulong[2];
    System.Random rng = new System.Random();
    size = (ulong)Size;
    table = new evalInfo[Size];

    for(int i = 0; i < Size; i++) table[i] = new evalInfo();

    for(int i = 0; i < 26; i++)
      for(int j = 0; j < 15; j++)
      {
        aiHash[i, j] = ((ulong)rng.Next() << 32) | (ulong)rng.Next();
        playerHash[i, j] = ((ulong)rng.Next() << 32) | (ulong)rng.Next();
      }

    turnHash[0] = ((ulong)rng.Next() << 32) | (ulong)rng.Next();
    turnHash[1] = ((ulong)rng.Next() << 32) | (ulong)rng.Next();
  }
  public ulong key(BoardState pos)
  {
    ulong hash = 0;
    uint slot;

    uint temp = pos.ai_present;
    for (; temp != 0; temp = X86.Bmi1.blsr_u32(temp)) {
      slot = X86.Bmi1.tzcnt_u32(temp);
      hash ^= aiHash[slot, -pos.chips[slot] - 1];
    }

    temp = pos.player_present;
    for (; temp != 0; temp = X86.Bmi1.blsr_u32(temp)) {
      slot = X86.Bmi1.tzcnt_u32(temp);
      hash ^= playerHash[slot, pos.chips[slot] - 1];
    }
    hash ^= turnHash[pos.playerTurn ? 1 : 0];
    return hash;
  }
  public evalInfo get(ulong key)
  {
    return table[key % size];
  }
  public evalInfo get(BoardState pos)
  {
    return table[key(pos) % size];
  }

  public int getIndex(ulong key)
  {
    return (int)(key % size);
  }
  public int getIndex(BoardState pos)
  {
    return (int)(key(pos) % size);
  }
};