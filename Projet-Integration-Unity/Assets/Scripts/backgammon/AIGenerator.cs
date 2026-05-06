
using static Unity.Burst.Intrinsics.X86.Bmi1;
using static Unity.Mathematics.math;
class simpleDiceGeneratorAI {
  public BoardState Pos;
  public simpleMoveArray moveList;
  public int dice1;
  public int dice2;
  
  public simpleDiceGeneratorAI(int i, int j, simpleMoveArray arr, BoardState pos)
  {
    dice1 = i; // dice1 should be the bigger dice
    dice2 = j;
    moveList = arr;
    Pos = pos;
  }

  public void setDices(int i, int j)
  {
    dice1 = i;
    dice2 = j;
  }

  public void generate()
  {
    moveList.index = 0;

    genForSimple();

    int temp = dice1;
    dice1 = dice2;
    dice2 = temp;

    genForSimple();

    if (moveList.index == 0) { // no moves were possible using 2 dices, check if can use the biggest one (single move)
      genSingleSimpleMove(dice2, 0, 0);

      if (moveList.index == 0) { // no moves were possible using the biggest dice, try using the smallest only
        genSingleSimpleMove(dice1, 0, 0);
      }
    }
  }

  void genSingleSimpleMove(int dice, ushort move_desc, int shift) {
    if(Pos.ai_present == 0) return;
    if (Pos.chips[25] != 0) {
      int to = 25 - dice;
      if (Pos.chips[to] <= 1) moveList.push_back((ushort)(move_desc | ((25 | (dice << 5)) << shift)));
    }
    else {
      uint moves = (uint)((Pos.ai_present >> dice) & ~1);
      int to;
      int from;
      if ((Pos.ai_present & 0b1111110) == Pos.ai_present) { // can bear off
        if(((1 << dice) & Pos.ai_present) != 0) moveList.push_back((ushort)(move_desc | (dice | (dice << 5)) << shift));
        else if(moves == 0) moveList.push_back((ushort)(move_desc | (((31 - lzcnt(Pos.ai_present)) | (dice << 5)) << shift)));
      }

      for (; moves != 0; moves = blsr_u32(moves)) {
        to = (int)tzcnt_u32(moves);
        from = to + dice;
        if (Pos.chips[to] <= 1) moveList.push_back((ushort)(move_desc | ((from | (dice << 5)) << shift)));
      }
    }
  }

  void genForSimple() {
    if (Pos.chips[25] != 0) {
      uint bit_to;
      uint bit_mod;
      int to = (int)(25 - dice1);
      if (Pos.chips[to] == 1) {
        bit_to = (uint)(1u << to);
        bit_mod = (uint)(bit_to | (((Pos.chips[25] == -1) ? 1 : 0) << 25));
        bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

        Pos.chips[to] = -1;
        Pos.chips[25]++;
        Pos.chips[0]++;
        Pos.ai_present ^= bit_mod;
        Pos.player_present ^= bit_to;

        genSingleSimpleMove(dice2, (ushort)(25 | (dice1 << 5)), 8);

        Pos.chips[to] = 1;
        Pos.chips[25]--;
        Pos.chips[0]--;
        Pos.ai_present ^= bit_mod;
        Pos.player_present ^= bit_to;
      }
      else if (Pos.chips[to] < 1) {
        bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[25] == -1) ? 1 : 0) << 25));

        Pos.chips[25]++;
        Pos.chips[to]--;
        Pos.ai_present ^= bit_mod;

        genSingleSimpleMove(dice2, (ushort)(25 | (dice1 << 5)), 8);

        Pos.chips[to]++;
        Pos.chips[25]--;
        Pos.ai_present ^= bit_mod;
      }
    }
    else {
      uint moves = (Pos.ai_present >> dice1) & ~1u;
      uint bit_mod;
      uint bit_to;
      int from;
      int to;
      if ((Pos.ai_present & 0b1111110) == Pos.ai_present) { // can bear off
        if(((1 << dice1) & Pos.ai_present) != 0) {
          bit_mod = (uint)(((Pos.chips[dice1] == -1) ? 1 : 0) << dice1);

          Pos.chips[dice1]++;
          Pos.ai_present ^= bit_mod;
          genSingleSimpleMove(dice2, (ushort)(dice1 | (dice1 << 5)), 8);
          Pos.chips[dice1]--;
          Pos.ai_present ^= bit_mod;
        }
        else if(moves == 0) {
          from = (int)(31 - lzcnt(Pos.ai_present));
          bit_mod = (uint)(((Pos.chips[from] == -1) ? 1 : 0) << from);

          Pos.chips[from]++;
          Pos.ai_present ^= bit_mod;
          genSingleSimpleMove(dice2, (ushort)(from | (dice1 << 5)), 8);
          Pos.chips[from]--;
          Pos.ai_present ^= bit_mod;
        }
      }
    
      for (; moves != 0; moves = blsr_u32(moves)) {
        to = (int)tzcnt_u32(moves);
        from = to + dice1;
        if (Pos.chips[to] == 1) {
          bit_to = (1u << to);
          bit_mod = (uint)(bit_to | (((Pos.chips[from] == -1) ? 1 : 0) << from));
          bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

          Pos.chips[to] = -1;
          Pos.chips[from]++;
          Pos.chips[0]++;
          Pos.ai_present ^= bit_mod;
          Pos.player_present ^= bit_to;

          genSingleSimpleMove(dice2, (ushort)(from | (dice1 << 5)), 8);

          Pos.chips[to] = 1;
          Pos.chips[from]--;
          Pos.chips[0]--;
          Pos.ai_present ^= bit_mod;
          Pos.player_present ^= bit_to;
        }
        else if (Pos.chips[to] < 1) {
          bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == -1) ? 1 : 0) << from));

          Pos.chips[from]++;
          Pos.chips[to]--;
          Pos.ai_present ^= bit_mod;

          genSingleSimpleMove(dice2, (ushort)(from | (dice1 << 5)), 8);

          Pos.chips[to]++;
          Pos.chips[from]--;
          Pos.ai_present ^= bit_mod;
        }
      }
    }
  }
}

class doubleDiceGeneratorAI
{
  public BoardState Pos;
  public doubleMoveArray moveList;
  public int dice;
  public doubleDiceGeneratorAI(byte i, doubleMoveArray arr, BoardState pos)
  {
    dice = i;
    moveList = arr;
    Pos = pos;
  }
  public void generate() {
    moveList.index = 0;
    int n;
    for (n = 3; n >= 0 && moveList.index == 0; n--) 
      genForDouble(n, 0, Pos.ai_present, 0);
  }
  public void setDice(int i)
  {
    dice = i;
  }
  void genForDouble(int dice_index, uint move_desc, uint self_present, int shift) {
    if(self_present == 0) return;
    if (dice_index > 0) {
      if (Pos.chips[25] != 0) {
        uint bit_to;
        uint bit_mod;
        int to = (int)(25 - dice);
        if (Pos.chips[to] == 1) {
          bit_to = (uint)(1 << to);
          bit_mod = (uint)(bit_to | (((Pos.chips[25] == -1) ? 1 : 0) << 25));
          bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

          Pos.chips[to] = -1;
          Pos.chips[25]++;
          Pos.chips[0]++;
          Pos.ai_present ^= bit_mod;
          Pos.player_present ^= bit_to;

          genForDouble(dice_index - 1, (uint)(move_desc | ((25 | (dice << 5)) << shift)), self_present ^ bit_mod, shift + 8);

          Pos.chips[to] = 1;
          Pos.chips[25]--;
          Pos.chips[0]--;
          Pos.ai_present ^= bit_mod;
          Pos.player_present ^= bit_to;
        }
        else if (Pos.chips[to] < 1) {
          bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[25] == -1) ? 1 : 0) << 25));

          Pos.chips[25]++;
          Pos.chips[to]--;
          Pos.ai_present ^= bit_mod;

          genForDouble(dice_index - 1, (uint)(move_desc | ((25 | (dice << 5)) << shift)), self_present ^ bit_mod, shift + 8);

          Pos.ai_present ^= bit_mod;
          Pos.chips[to]++;
          Pos.chips[25]--;
        }
      }
      else {
        uint moves = (uint)((self_present >> dice) & ~1u);
        uint bit_mod;
        uint bit_to;
        int from;
        int to;

      if ((Pos.ai_present & 0b1111110) == Pos.ai_present) { // can bear off
        if(((1 << dice) & Pos.ai_present) != 0) {
          if(((1 << dice) & self_present) != 0) {
            bit_mod = (uint)(((Pos.chips[dice] == -1) ? 1 : 0) << dice);

            Pos.ai_present ^= bit_mod;
            Pos.chips[dice]++;
            genForDouble(dice_index - 1, (uint)(move_desc | ((dice | (dice << 5)) << shift)), self_present ^ bit_mod, shift + 8);
            Pos.ai_present ^= bit_mod;
            Pos.chips[dice]--;
            self_present &= ~(1u << dice);
          }
        }
        else {
          from = (int)(31 - lzcnt(Pos.ai_present));
          if(from <= dice && ((1 << from) & self_present) != 0) {
            bit_mod = (uint)(((Pos.chips[from] == -1) ? 1 : 0) << from);

            Pos.chips[from]++;
            Pos.ai_present ^= bit_mod;
            genForDouble(dice_index - 1,  (uint)(move_desc | ((from | (dice << 5)) << shift)), self_present ^ bit_mod, shift + 8);
            Pos.chips[from]--;
            Pos.ai_present ^= bit_mod;
            self_present &= ~(1u << from);
          }
        }
      }

        for (; moves != 0; moves = blsr_u32(moves)) {
          to = (int)tzcnt_u32(moves);
          from = to + dice;
          if (Pos.chips[to] == 1) {
            bit_to = (1u << to);
            bit_mod = (uint)(bit_to | (((Pos.chips[from] == -1) ? 1 : 0) << from));
            bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

            Pos.chips[to] = -1;
            Pos.chips[from]++;
            Pos.chips[0]++;
            Pos.ai_present ^= bit_mod;
            Pos.player_present ^= bit_to;

            genForDouble(dice_index - 1, (uint)(move_desc | ((from | (dice << 5)) << shift)), self_present ^ bit_mod, shift + 8);

            Pos.chips[to] = 1;
            Pos.chips[from]--;
            Pos.chips[0]--;
            Pos.ai_present ^= bit_mod;
            self_present &= ~(1u << from);
            Pos.player_present ^= bit_to;
          }
          else if (Pos.chips[to] < 1) {
            bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == -1) ? 1 : 0) << from));

            Pos.chips[from]++;
            Pos.chips[to]--;
            Pos.ai_present ^= bit_mod;

            genForDouble(dice_index - 1, (uint)(move_desc | ((from | (dice << 5)) << shift)), self_present ^ bit_mod, shift + 8);

            Pos.chips[to]++;
            Pos.chips[from]--;
            Pos.ai_present ^= bit_mod;
            self_present &= ~(1u << from);
          }
        }
      }
    }
    else {
      if (Pos.chips[25] != 0) {
        int to = 25 - dice;
        if (Pos.chips[to] <= 1) moveList.push_back((uint)(move_desc | ((25 | (dice << 5)) << shift)));
      }
      else {
        uint moves = (uint)((self_present >> dice) & ~1u);
        int to;
        int from;

        if ((Pos.ai_present & 0b1111110) == Pos.ai_present) { // can bear off
          if(((1 << dice) & Pos.ai_present) != 0) {
            if(((1 << dice) & self_present) != 0)
              moveList.push_back((uint)(move_desc | (dice | (dice << 5)) << shift));
          }
          else
          {
            from = (int)(31 - lzcnt(Pos.ai_present));
            if(from <= dice && ((1 << from) & self_present) != 0)
              moveList.push_back((uint)(move_desc | ((from | (dice << 5)) << shift)));
          }
        }

        for (; moves != 0; moves = blsr_u32(moves)) {
          to = (int)tzcnt_u32(moves);
          from = to + dice;
          if (Pos.chips[to] <= 1) moveList.push_back((uint)(move_desc | ((from | (dice << 5)) << shift)));
        }
      }
    }
  }
}