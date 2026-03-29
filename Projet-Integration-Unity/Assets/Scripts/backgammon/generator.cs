abstract public class generator
{
  public BoardState Pos;
  public abstract sbyte scoreMove(int from, int to);
  public abstract sbyte scoreBearOff(int from);
  public abstract sbyte scoreBar(int to);
  public abstract void generate();
}
public abstract class generatorPlayer : generator
{
  public static readonly uint[] bearoff_mask = { 0, 0b01000000000000000000000000, 0b01100000000000000000000000, 0b01110000000000000000000000, 0b01111000000000000000000000, 0b01111100000000000000000000, 0b01111110000000000000000000 };
  public override sbyte scoreMove(int from, int to)
	{
		sbyte score = 0;
		if(Pos.chips[from] == 2) score -= 3; // create vulnerable piece
		if(Pos.chips[to] == -1) score += 5; // eat opponent
		if(Pos.chips[to] == 1) {
			score += 2; // Protect a piece
			if(Pos.chips[from] == 1) score += 2; // Protect another piece (one moved)
		}
		return score;
	}

  public override sbyte scoreBearOff(int from)
  {
    return 10;
  }
  public override sbyte scoreBar(int to)
  {
    sbyte score = 0;
    if(Pos.chips[to] == -1) score += 5; // eat opponent
		if(Pos.chips[to] == 1) score += 2; // Protect a piece
    return score;
  }
}
public abstract class generatorAI : generator
{
  public static readonly uint[] bearoff_mask = { 0, 0b10, 0b110, 0b1110, 0b11110, 0b111110, 0b1111110 };
  public override sbyte scoreMove(int from, int to)
	{
		sbyte score = 0;
		if(Pos.chips[from] == -2) score -= 3; // create vulnerable piece
		if(Pos.chips[to] == 1) score += 5; // eat opponent
		if(Pos.chips[to] == -1) {
			score += 2; // Protect a piece
			if(Pos.chips[from] == -1) score += 2; // Protect another piece (one moved)
		}
		return score;
	}
  public override sbyte scoreBearOff(int from)
  {
    return 10;
  }
  public override sbyte scoreBar(int to)
  {
    sbyte score = 0;
    if(Pos.chips[to] == 1) score += 5; // eat opponent
		if(Pos.chips[to] == -1) score += 2; // Protect a piece
    return score;
  }
}