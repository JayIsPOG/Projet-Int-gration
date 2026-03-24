
using System.Collections.Generic;

public class evalInfo
{
	public sbyte depth;
	public float upper_bound;
	public float lower_bound;
	//public byte type; // 0 = EXACT, 1 = UPPER_BOUND, 2 = LOWER_BOUND
	public ulong key;
  public evalInfo(sbyte d, float u, float l, ulong k)
  {
    depth = d;
    key = k;
    upper_bound = u;
    lower_bound = l;

  }
};
public class TranspositionTable
{
  public ulong size;
  public ulong[,] slotHash;
  public ulong[] turnHash;
  public evalInfo[] table;
  public TranspositionTable(int Size)
  {
    slotHash = new ulong[26, 15*2+1];
    turnHash = new ulong[2];
    System.Random rng = new System.Random();
    size = (ulong)Size;
    table = new evalInfo[Size];

    for(int i = 0; i < Size; i++) table[i] = new evalInfo(0, 0, 0, 0);

    for(int i = 0; i < 26; i++)
      for(int j = 0; j < 15 * 2 + 1; j++)
        slotHash[i, j] = ((ulong)rng.Next() << 32) | (ulong)rng.Next();

    turnHash[0] = ((ulong)rng.Next() << 32) | (ulong)rng.Next();
    turnHash[1] = ((ulong)rng.Next() << 32) | (ulong)rng.Next();
  }
  public ulong key(BoardState pos)
  {
    ulong hash = 0;
    for(int i = 0; i < 26; i++) hash ^= slotHash[i, pos.chips[i] + 15]; // can iterate trough the bitboard to make faster
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