
public class BoardState
{
    // chaque slot de chips montrent les casens en sens antihoraire, les deux dernieres sont pour les chips out pour les joueurs
	public sbyte[] chips; // first is the player bar, last is opponent bar
	public uint player_present;
	public uint ai_present;
	public int player_borneoff;
	public int ai_borneoff;
	public bool playerTurn;
	// Start is called before the first frame update
	public BoardState()
	{
		set();
	}
	public void set()
	{
		playerTurn = true;
    chips = new sbyte[26]{0, -2, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0,-5, 5, 0, 0, 0,-3, 0,-5, 0, 0, 0, 0, 2, 0 }; // first is the player bar, last is opponent bar
		player_present = 0;
		ai_present = 0;
		player_borneoff = 0;
		ai_borneoff = 0;
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
}