using System.Collections.Concurrent;

public class genOdds
{
  uint[] mask;
  int[,] eating_odds;
  int[,] blocking_odds;
  void innitMask()
  {
    mask[0] = 0;
    uint bit = 0b10;
    for(int to = 1; to < 25; to++)
    {
      uint mask = 0;
      for(int d1 = 1; d1 <= 6; d1++)
      {
        for(int d2 = 1; d2 < d1; d2++)
        {
          int from = to + d1;
          if(from < 0)
          mask |= bit >> d1;
          mask |= bit >> d2;
        }
      }
      bit <<= 1;
    }
  }
  public genOdds()
  {
    mask = new uint[25];
    eating_odds = new int[25, 25]; // first is the piece placed other are the opponent pieces
    blocking_odds = new int[25, 25];
    for(int i = 0; i < 25; i++)
      for(int j = 0; j < 25; j++)
      {
        eating_odds[i, j] = 0;
        blocking_odds[i, j] = 0;
      }
  }
}