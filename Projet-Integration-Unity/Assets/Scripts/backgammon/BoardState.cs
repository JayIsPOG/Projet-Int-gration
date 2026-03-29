
public class sideInfo
{
	public byte[] chips;
	public uint present;
	public sideInfo() // maybe add a bearoff counter idk
	{
		chips = new byte[25]; // 0 is the bar
		present = 0;
	}
	public sideInfo(byte[] c)
	{
		chips = new byte[25];
		present = 0;
		for(int i = 0; i < 25; i++)
		{
			byte count = c[i];
			chips[i] = count;
			if(count >= 1) present |= 1 << i;
		}
	}
  public bool hasWon()
	{
		return present == 0; // all bearOff
	}
	public bool canBearOff()
	{
		return (present & 0b1111110000000000000000000) == present; // all at the 6 end slots
	}
	public bool hasOnBar()
	{
		return (present & 1) != 0;
	}
}
public class BoardState
{
	public sideInfo player;
	public sideInfo opponent;
	public bool playerTurn;
	// Start is called before the first frame update
	public BoardState()
	{
		set();
	}
	public void set()
	{
		byte[] placement = new byte[25]{0, 0, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0};
		player = new sideInfo(placement);
		opponent = new sideInfo(placement);
		playerTurn = false;
	}

	public void nextTurn()
	{
		sideInfo temp = player;
		player = opponent;
		opponent = temp;
		playerTurn = !playerTurn;
	}
}