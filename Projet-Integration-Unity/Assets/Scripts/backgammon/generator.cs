using System.Security.Cryptography;
using static Unity.Burst.Intrinsics.X86.Bmi1;
abstract public class generator
{
  public BoardState Pos;
  public static readonly uint[] bearoff_mask = { 0, 0b01000000000000000000000000, 0b01100000000000000000000000, 0b01110000000000000000000000, 0b01111000000000000000000000, 0b01111100000000000000000000, 0b01111110000000000000000000 };
  public sbyte scoreMove(int from, int to)
	{
		sbyte score = 0;
		if(Pos.player.chips[from] == 2) score -= 3; // create vulnerable piece
		if(Pos.opponent.chips[to] == 1) score += 5; // eat opponent
		if(Pos.player.chips[to] == 1) {
			score += 2; // Protect a piece
			if(Pos.player.chips[from] == 1) score += 2; // Protect another piece (one moved)
		}
		return score;
	}

  public sbyte scoreBearOff(int from)
  {
    return 10;
  }
  public sbyte scoreBar(int to)
  {
    sbyte score = 0;
    if(Pos.opponent.chips[to] == 1) score += 5; // eat opponent
		if(Pos.player.chips[to] == 1) score += 2; // Protect a piece
    return score;
  }
  public abstract void generate();
}

public class simpleGenerator : generator
{
  public simpleMoveArray moveList;
  public int dice1, dice2;
  uint encode_dice1, encode_dice2;

  public simpleGenerator(int d1, int d2, simpleMoveArray arr, BoardState pos)
  {
    setDices(d1, d2);
    moveList = arr;
    Pos = pos;
  }

  public void setDices(int big_dice, int small_dice)
  {
    dice1 = small_dice;
    dice2 = big_dice;
    encode_dice1 = (uint)(dice1 << 5);
    encode_dice2 = (uint)(dice2 << 5);
  }

  public override void generate()
  {
    moveList.index = 0;

    genTwo();

    setDices(dice1, dice2);

    genTwo();

    if (moveList.index == 0) { // no moves were possible using 2 dices, check if can use the biggest one (single move)
      genOne(0, 0, 0);

      if (moveList.index == 0) { // no moves were possible using the biggest dice, try using the smallest only

        setDices(dice1, dice2);
        genOne(0, 0, 0);

        if (moveList.index == 0) moveList.moveDepth = 0; // no moves are possible...
        else moveList.moveDepth = 1;
      }
      else moveList.moveDepth = 1;
    }
    else moveList.moveDepth = 2;
  }
  void genOne(ushort move_desc, sbyte score, int shift)
  {
    if (Pos.chips[0] != 0) {
      if(Pos.opponent.chips[25 - dice2] <= 1) 
        moveList.push_back(move_desc | (encode_dice2 << shift), (sbyte)(score + scoreBar(dice2)));
    }
    else {
      uint moved, from, to;
      if (Pos.player.canBearOff())
      {
        moved = Pos.player.present & bearoff_mask[dice2];
        for (; moved != 0; moved = blsr_u32(moved))
        {
          from = tzcnt_u32(moved);
          moveList.push_back(move_desc | (from << shift), (sbyte)(score + scoreBearOff(from)));
        }
      }
      moved = (Pos.player.present << dice2) & 0b0111111111111111111111111;
      for (; moved != 0; moved = blsr_u32(moved)) {
          to = tzcnt_u32(moved);
          if(Pos.opponent.chips[25 - to] <= 1)
          {
            from = to - dice2;
            moveList.push_back(move_desc | ((from | encode_dice2) << shift), scoreMove(from, to));
          }
        }
    }
  }
  void genTwo()
  {
    uint bit_mod;
    uint moved;
    uint from;
    uint to;
    if(Pos.player.hasOnBar())
    {
      if(Pos.opponent.chips[25 - dice1] != 0)
      {
        bit_mod = ((Pos.player.chips[0] == 1) ? 1 : 0) | (((Pos.player.chips[dice1] == 0) ? 1 : 0) << dice1);

        Pos.player.chips[0]--;
        Pos.player.chips[dice1]++;
        Pos.player.present ^= bit_mod;
        genOne(encode_dice1, scoreBar(dice1), 8);
        Pos.player.chips[0]++;
        Pos.player.chips[dice1]--;
        Pos.player.present ^= bit_mod;
      }
    }
    else
    {
      if (Pos.player.canBearOff())
      {
        moved = Pos.player.present & bearoff_mask[dice1];
        for (; moved != 0; moved = blsr_u32(moved))
        {
          from = tzcnt_u32(moved);
          bit_mod = ((Pos.player.chips[from] == 1) ? 1 : 0) << from;

          Pos.player.chips[from]--;
          Pos.player.present ^= bit_mod;
          genOne(from, scoreBearOff(from), 8);
          Pos.player.chips[from]++;
          Pos.player.present ^= bit_mod;
        }
      }
      moved = (Pos.player.present << dice1) & 0b0111111111111111111111111;
      for (; moved != 0; moved = blsr_u32(moved)) {
          to = tzcnt_u32(moved);
          if(Pos.opponent.chips[25 - to] <= 1)
          {
            from = to - dice1;
            bit_mod = (((Pos.player.chips[from] == 1) ? 1 : 0) << from) | (((Pos.player.chips[to] == 0) ? 1 : 0) << to);

            Pos.player.present ^= bit_mod;
            Pos.player.chips[from]--;
            Pos.player.chips[to]++;
            genOne(from | encode_dice1, scoreMove(from, to), 8);
            Pos.player.present ^= bit_mod;
            Pos.player.chips[from]++;
            Pos.player.chips[to]--;
          }
        }
    }
  }
}

public class doubleGenerator : generator
{
  public doubleMoveArray moveList;
  public int dice;
  uint encode_dice;
  public doubleGenerator(byte d, doubleMoveArray arr, BoardState pos)
  {
    setDice(d);
    moveList = arr;
    Pos = pos;
  }
  public override void generate() {
    moveList.index = 0;
    int n;
    for (n = 3; n >= 0 && moveList.index == 0; n--) 
      gen(0, 0, Pos.player.present, 0, n);
      
    if(moveList.index == 0) moveList.moveDepth = 0;
    else moveList.moveDepth = n + 2;
  }
  public void setDice(int d)
  {
    dice = d;
    encode_dice = (uint)(d << 5);
  }
  void gen(uint move_desc, sbyte score, uint present, int shift, int n)
  {
    uint bit_mod, moved, from, to;
    if(n == 0)
    {
      if (Pos.chips[0] != 0) {
        if(Pos.opponent.chips[25 - dice] <= 1)
          moveList.push_back(move_desc | (encode_dice << shift), (sbyte)(score + scoreBar(dice)));
      }
      else {
        if (Pos.player.canBearOff())
        {
          moved = present & bearoff_mask[dice];
          for (; moved != 0; moved = blsr_u32(moved))
          {
            from = tzcnt_u32(moved);
            moveList.push_back(move_desc | (from << shift), (sbyte)(score + scoreBearOff(from)));
          }
        }
        moved = (present << dice) & 0b0111111111111111111111111;
        for (; moved != 0; moved = blsr_u32(moved)) {
            to = tzcnt_u32(moved);
            if(Pos.opponent.chips[25 - to] <= 1)
            {
              from = to - dice;
              moveList.push_back(move_desc | ((from | encode_dice) << shift), scoreMove(from, to));
            }
          }
      }
      return;
    }

    if(Pos.player.hasOnBar())
    {
      if(Pos.opponent.chips[25 - dice] <= 1)
      {
        bit_mod = ((Pos.player.chips[0] == 1) ? 1 : 0) | (((Pos.player.chips[dice] == 0) ? 1 : 0) << dice);

        Pos.player.chips[0]--;
        Pos.player.chips[dice]++;
        present ^= bit_mod;
        gen(move_desc | (encode_dice << shift), scoreBar(dice), present, shift + 8, n - 1);
        Pos.player.chips[0]++;
        Pos.player.chips[dice]--;
        present ^= bit_mod;
      }
    }
    else
    {
      if (Pos.player.canBearOff())
      {
        moved = present & bearoff_mask[dice];
        for (; moved != 0; moved = blsr_u32(moved))
        {
          from = tzcnt_u32(moved);
          bit_mod = ((Pos.player.chips[from] == 1) ? 1 : 0) << from;

          Pos.player.chips[from]--;
          present ^= bit_mod;
          gen(move_desc | (from << shift), scoreBearOff(from), present, shift + 8, n - 1);
          Pos.player.chips[from]++;
          present ^= bit_mod;
          present ^= 1 << from; // remove piece moved to remove order
        }
      }
      moved = (present << dice) & 0b0111111111111111111111111;
      for (; moved != 0; moved = blsr_u32(moved)) {
          to = tzcnt_u32(moved);
          if(Pos.opponent.chips[25 - to] <= 1)
          {
            from = to - dice;
            bit_mod = (((Pos.player.chips[from] == 1) ? 1 : 0) << from) | (((Pos.player.chips[to] == 0) ? 1 : 0) << to);

            present ^= bit_mod;
            Pos.player.chips[from]--;
            Pos.player.chips[to]++;
            gen(move_desc | ((from | encode_dice) << shift), scoreMove(from, to), present, shift + 8, n - 1);
            present ^= bit_mod;
            present ^= 1 << from;
            Pos.player.chips[from]++;
            Pos.player.chips[to]--;
          }
        }
    }
  }
}

public class orderedDoubleGenerator : generator
{
  public doubleMoveArray moveList;
  public int dice;
  uint encode_dice;
  public orderedDoubleGenerator(byte d, doubleMoveArray arr, BoardState pos)
  {
    setDice(d);
    moveList = arr;
    Pos = pos;
  }
  public override void generate() {
    moveList.index = 0;
    int n;
    for (n = 3; n >= 0 && moveList.index == 0; n--) 
      gen(0, 0, 0, n);
      
    if(moveList.index == 0) moveList.moveDepth = 0;
    else moveList.moveDepth = n + 2;
  }
  public void setDice(int d)
  {
    dice = d;
    encode_dice = (uint)(d << 5);
  }
  void gen(uint move_desc, sbyte score, int shift, int n)
  {
    uint bit_mod, moved, from, to;
    if(n == 0)
    {
      if (Pos.chips[0] != 0) {
        if(Pos.opponent.chips[25 - dice] <= 1)
          moveList.push_back(move_desc | (encode_dice << shift), (sbyte)(score + scoreBar(dice)));
      }
      else {
        if (Pos.player.canBearOff())
        {
          moved = Pos.player.present & bearoff_mask[dice];
          for (; moved != 0; moved = blsr_u32(moved))
          {
            from = tzcnt_u32(moved);
            moveList.push_back(move_desc | (from << shift), (sbyte)(score + scoreBearOff(from)));
          }
        }
        moved = (Pos.player.present << dice) & 0b0111111111111111111111111;
        for (; moved != 0; moved = blsr_u32(moved)) {
            to = tzcnt_u32(moved);
            if(Pos.opponent.chips[25 - to] <= 1)
            {
              from = to - dice;
              moveList.push_back(move_desc | ((from | encode_dice) << shift), scoreMove(from, to));
            }
          }
      }
      return;
    }

    if(Pos.player.hasOnBar())
    {
      if(Pos.opponent.chips[25 - dice] <= 1)
      {
        bit_mod = ((Pos.player.chips[0] == 1) ? 1 : 0) | (((Pos.player.chips[dice] == 0) ? 1 : 0) << dice);

        Pos.player.chips[0]--;
        Pos.player.chips[dice]++;
        Pos.player.present ^= bit_mod;
        gen(move_desc | (encode_dice << shift), scoreBar(dice), shift + 8, n - 1);
        Pos.player.chips[0]++;
        Pos.player.chips[dice]--;
        Pos.player.present ^= bit_mod;
      }
    }
    else
    {
      if (Pos.player.canBearOff())
      {
        moved = Pos.player.present & bearoff_mask[dice];
        for (; moved != 0; moved = blsr_u32(moved))
        {
          from = tzcnt_u32(moved);
          bit_mod = ((Pos.player.chips[from] == 1) ? 1 : 0) << from;

          Pos.player.chips[from]--;
          Pos.player.present ^= bit_mod;
          gen(move_desc | (from << shift), scoreBearOff(from), shift + 8, n - 1);
          Pos.player.chips[from]++;
          Pos.player.present ^= bit_mod;
        }
      }
      moved = (Pos.player.present << dice) & 0b0111111111111111111111111;
      for (; moved != 0; moved = blsr_u32(moved)) {
          to = tzcnt_u32(moved);
          if(Pos.opponent.chips[25 - to] <= 1)
          {
            from = to - dice;
            bit_mod = (((Pos.player.chips[from] == 1) ? 1 : 0) << from) | (((Pos.player.chips[to] == 0) ? 1 : 0) << to);

            Pos.player.present ^= bit_mod;
            Pos.player.chips[from]--;
            Pos.player.chips[to]++;
            gen(move_desc | ((from | encode_dice) << shift), scoreMove(from, to), shift + 8, n - 1);
            Pos.player.present ^= bit_mod;
            Pos.player.chips[from]++;
            Pos.player.chips[to]--;
          }
        }
    }
  }
}