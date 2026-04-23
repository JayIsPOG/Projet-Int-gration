
public class BoardState
{
    // chaque slot de chips montrent les casens en sens antihoraire, les deux dernieres sont pour les chips out pour les joueurs
	public sbyte[] chips; // first is the player bar, last is opponent bar
	public uint player_present;
	public uint ai_present;
	public bool playerTurn;
	public byte player_bearoff;
	public byte ai_bearoff;
	public BoardState()
	{
		set();
	}
	public void set()
	{
		player_bearoff = 0;
		ai_bearoff = 0;
		playerTurn = true;
    chips = new sbyte[26]{0, -2, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0,-5, 5, 0, 0, 0,-3, 0,-5, 0, 0, 0, 0, 2, 0 }; // first is the player bar, last is opponent bar
		player_present = 0;
		ai_present = 0;
		for (int j = 0; j < 26; j++) ai_present |= (uint)(((chips[j] <= -1) ? 1 : 0) << j);
		for (int j = 0; j < 26; j++) player_present |= (uint)(((chips[j] >= 1) ? 1 : 0) << j);
	}
  public bool hasPlayerWon()
	{
		return player_present == 0;
	}
	public bool hasAIWon()
	{
		return ai_present == 0;
	}

	public bool canPlayerBearOff()
	{
		return (player_present & 0b01111110000000000000000000) == player_present;
	}
	public void makeMovePlayer(uint move)
  {
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from + dice;
    if(to >= 25) // bearoff move
    {
      uint bit_mod = (uint)(((chips[from] == 1) ? 1 : 0) << from);

      player_bearoff++;
      chips[from]--;
      player_present ^= bit_mod;
    }
    else if(chips[to] == -1)
    {
      uint bit_to = (1u << to);
      uint bit_mod = (uint)(bit_to | (((chips[from] == 1) ? 1 : 0) << from));
      bit_to |= ((chips[25] == 0) ? 1u : 0u) << 25;

      chips[to] = 1;
      chips[from]--;
      chips[25]--;
      player_present ^= bit_mod;
      ai_present ^= bit_to;
    }
    else
    {
      uint bit_mod = (uint)((((chips[to] == 0) ? 1 : 0) << to) | (((chips[from] == 1) ? 1 : 0) << from));

      chips[from]--;
      chips[to]++;
      player_present ^= bit_mod;
    }
  }
	public void makeMoveAI(uint move)
  {
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from - dice;
    uint bit_mod;

    if(to <= 0) // bearoff move
    {
      bit_mod = (uint)(((chips[from] == -1) ? 1 : 0) << from);

      ai_bearoff++;
      chips[from]++;
      ai_present ^= bit_mod;

    }
    else if(chips[to] == 1)
    {
      uint bit_to = 1u << to;
      bit_mod = (uint)(bit_to | (((chips[from] == -1) ? 1 : 0) << from));
      bit_to |= (chips[0] == 0) ? 1u : 0u;

      chips[to] = -1;
      chips[from]++;
      chips[0]++;
      ai_present ^= bit_mod;
      player_present ^= bit_to;
    }
    else
    {
      bit_mod = (uint)((((chips[to] == 0) ? 1 : 0) << to) | (((chips[from] == -1) ? 1 : 0) << from));

      chips[from]++;
      chips[to]--;
      ai_present ^= bit_mod;
    }
  }
}