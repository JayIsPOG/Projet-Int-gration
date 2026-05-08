using System.Collections;
using UnityEngine;
using Unity.Burst.Intrinsics;
using System.Diagnostics;
using System;
class Bot{
  TranspositionTable TT;
  BoardState Pos;
  public simpleDiceGeneratorAI[] simpleGenPool;
  public doubleDiceGeneratorAI[] doubleGenPool;
  public simpleDiceGeneratorPlayer[] simplePlayerGenPool;
  public unorderedDoubleDiceGeneratorPlayer[] doublePlayerGenPool;
  public simpleMoveArray[] simpleMovesPool;
  public doubleMoveArray[] doubleMovesPool;
  public Learner evaluator;
  public Bot(BoardState pos, Learner l, int max_depth)
  {
    TT = new TranspositionTable(33554467); // 33554467 est un nombre prime relativement gros (les nombre primes sont meilleurs car on utilise modulo, cela r/)
    max_depth++;
    Pos = pos;
    evaluator = l;
    simpleMovesPool = new simpleMoveArray[max_depth];
    doubleMovesPool = new doubleMoveArray[max_depth];
    doubleGenPool = new doubleDiceGeneratorAI[max_depth];
    simpleGenPool = new simpleDiceGeneratorAI[max_depth];
    doublePlayerGenPool = new unorderedDoubleDiceGeneratorPlayer[max_depth];
    simplePlayerGenPool = new simpleDiceGeneratorPlayer[max_depth];
    for(int i = 0; i < max_depth; i++){
      simpleMovesPool[i] = new simpleMoveArray();
      doubleMovesPool[i] = new doubleMoveArray();
      doubleGenPool[i] = new doubleDiceGeneratorAI(0, doubleMovesPool[i], pos);
      simpleGenPool[i] = new simpleDiceGeneratorAI(0, 0, simpleMovesPool[i], pos);
      doublePlayerGenPool[i] = new unorderedDoubleDiceGeneratorPlayer(0, doubleMovesPool[i], pos);
      simplePlayerGenPool[i] = new simpleDiceGeneratorPlayer(0, 0, simpleMovesPool[i], pos);
    }
  }
  public float minimaxAi(int depth, float alpha, float beta)
  {
    //Pos.playerTurn = false;
    //return evaluator.evaluatePosition(Pos);

    if(Pos.hasPlayerWon()) return 0.0f;

    Pos.playerTurn = false;
    ulong key = TT.key(Pos);
    int index = TT.getIndex(key);
    evalInfo entry = TT.table[index];
    if (entry.key == key) {
      if (entry.upper_bound == entry.lower_bound && entry.udepth == entry.ldepth && entry.udepth >= depth) 
        return entry.upper_bound;

      if(entry.udepth >= depth) {
        if(entry.upper_bound <= alpha) return entry.upper_bound;
        beta = Mathf.Min(beta, entry.upper_bound);
      }

      if(entry.ldepth >= depth) {
        if(entry.lower_bound >= beta) return entry.lower_bound;
        alpha = Mathf.Max(alpha, entry.lower_bound);
      }
    }
    else entry.reset(key);

    if(depth < 0) {
      float eval = evaluator.evaluatePosition(Pos);
      entry.storeAll((sbyte)depth, eval);
      return eval;
    }

    simpleMoveArray _simpleMoves = simpleMovesPool[depth];
    doubleMoveArray _doubleMoves = doubleMovesPool[depth];
    simpleDiceGeneratorAI simpleGenerator = simpleGenPool[depth];
    doubleDiceGeneratorAI doubleGenerator = doubleGenPool[depth];

    float max, childMin, childMax;
    float upper_bound = 1.0f;
    float lower_bound = 0.0f;


    for(int d1 = 2; d1 <= 6; d1++)
      for(int d2 = 1; d2 < d1; d2++)
        {
          max = float.MinValue;

          simpleGenerator.setDices(d1, d2);

          childMin = Mathf.Max(18 * (alpha - upper_bound) + 1, 0);
          childMax = Mathf.Min(18 * (beta - lower_bound), 1);

          simpleGenerator.generate();
          for(int i = 0; i < _simpleMoves.size(); i++){
            max = Mathf.Max(max, evaluateMoveAI(_simpleMoves.moves[i], depth - 1, childMin, childMax));
            childMin = Mathf.Max(childMin, max);
            if(childMin >= childMax) break; // reg alpha beta pruning
          }
          if(_simpleMoves.size() == 0) max = minimaxPlayer(depth - 1, childMin, childMax);

          // car il y a 2 / 36 chances d'avoir une configuration de dé avec deux faces différentes
          upper_bound -= (1.0f - max) * (2.0f / 36.0f);
          lower_bound += max * (2.0f / 36.0f);

          if(upper_bound <= alpha) {
            entry.storeUpper((sbyte)depth, upper_bound);
            return upper_bound;
          }

          if(lower_bound >= beta) {
            entry.storeLower((sbyte)depth, lower_bound);
            return lower_bound;
          }
        }
    for(int d = 1; d <= 6; d++)
    {
      max = float.MinValue;
          
      doubleGenerator.setDice(d);

      childMin = Mathf.Max(36 * (alpha - upper_bound) + 1, 0);
      childMax = Mathf.Min(36 * (beta - lower_bound), 1);

      doubleGenerator.generate();
      for(int i = 0; i < _doubleMoves.size(); i++){
        max = Mathf.Max(max, evaluateMoveAI(_doubleMoves.moves[i], depth - 1, childMin, childMax));
        childMin = Mathf.Max(childMin, max);
        if (childMin >= childMax) break;
      }
      if(_doubleMoves.size() == 0) max = minimaxPlayer(depth - 1, childMin, childMax);

       // car il y a 1 / 36 chances dee identiques;
      upper_bound -= (1.0f - max) * (1.0f / 36.0f);
      lower_bound += max * (1.0f / 36.0f);

      if(upper_bound <= alpha) {
        entry.storeUpper((sbyte)depth, upper_bound);
        return upper_bound;
      }

      if(lower_bound >= beta) {
        entry.storeLower((sbyte)depth, lower_bound);
        return lower_bound;
      }
    }
    entry.storeAll((sbyte)depth, upper_bound);
    return upper_bound; // car upper_bound == lower_bound a la fin
  }
  public float minimaxPlayer(int depth, float alpha, float beta)
  {
    //Pos.playerTurn = true;
    //return evaluator.evaluatePosition(Pos);

    if(Pos.hasAIWon()) return 1.0f;

    Pos.playerTurn = true;
    ulong key = TT.key(Pos);
    int index = TT.getIndex(key);
    evalInfo entry = TT.table[index]; // obtien les paramètres d'évaluation de la position
    if (entry.key == key) { // confirme si on la vue précédement
      if (entry.upper_bound == entry.lower_bound && entry.udepth == entry.ldepth && entry.udepth >= depth) // confirme si elle a été explirée en détail (plus grande depth)
        return entry.upper_bound;

      if(entry.udepth >= depth) {
        if(entry.upper_bound <= alpha) return entry.upper_bound;
        beta = Mathf.Min(beta, entry.upper_bound); // permet de diminuer beta en ce basant sur le max évalué précédement
      }

      if(entry.ldepth >= depth) {
        if(entry.lower_bound >= beta) return entry.lower_bound;
        alpha = Mathf.Max(alpha, entry.lower_bound); // permet d'augmenter alpha en ce basant sur le min évalué précédement
      }
    }
    else entry.reset(key);

    // on retourne l'évaluation à la fin de l'arbre
    if(depth < 0) {
      float eval = evaluator.evaluatePosition(Pos);
      entry.storeAll((sbyte)depth, eval);
      return eval;
    }

    simpleMoveArray _simpleMoves = simpleMovesPool[depth];
    doubleMoveArray _doubleMoves = doubleMovesPool[depth];
    simpleDiceGeneratorPlayer simpleGenerator = simplePlayerGenPool[depth];
    unorderedDoubleDiceGeneratorPlayer doubleGenerator = doublePlayerGenPool[depth];
    
    float min, childMin, childMax;
    float upper_bound = 1;
    float lower_bound = 0;

    for(int d1 = 2; d1 <= 6; d1++)
      for(int d2 = 1; d2 < d1; d2++)
        {
          min = float.MaxValue;

          simpleGenerator.setDices(d1, d2);

          childMin = Mathf.Max(18 * (alpha - upper_bound) + 1, 0);
          // childMin est le alpha du child, on l'obtien avec la contition de prune (upper_bound <= alpha)
          // upper_bound - (1 - val) * (2 / 36) <= alpha
          // (1 - val) * (2 / 36) >= - (alpha - upper_bound)
          // 1 - val >= - (alpha - upper_bound) * (36 / 2)
          // val <= (alpha - upper_bound) * (36 / 2) + 1
          childMax = Mathf.Min(18 * (beta - lower_bound), 1);
          // childMax est le beta du child, on l'obtien avec la contition de prune (upper_bound >= beta)

          simpleGenerator.generate();
          for(int i = 0; i < _simpleMoves.size(); i++) {
            min = Mathf.Min(min, evaluateMovePlayer(_simpleMoves.moves[i], depth - 1, childMin, childMax));
            childMax = Mathf.Min(childMax, min);
            if (childMin >= childMax) break; // Si la valeur est plus petite que le minimum, on prune
          }
          if(_simpleMoves.size() == 0) min = minimaxAi(depth - 1, childMin, childMax); // skip son tour

          // car il y a 2 / 36 chances d'avoir une configuration de dé avec deux faces différentes
          upper_bound -= (1.0f - min) * (2.0f / 36.0f);
          lower_bound += min * (2.0f / 36.0f);

          
          if(upper_bound <= alpha) { // on ne pourra jamais atteindre la valeur minimale (alpha), on prune
            entry.storeUpper((sbyte)depth, upper_bound);
            return upper_bound;
          }

          if(lower_bound >= beta) {// on ne pourra jamais atteindre la valeur maximale (bata), on prune
            entry.storeLower((sbyte)depth, lower_bound);
            return lower_bound;
          }
        }
    // même fonctionnement que la dernière for loop, mais pour des dés identiques
    for(int d = 1; d <= 6; d++)
    {
      min = float.MaxValue;

      doubleGenerator.setDice(d);

      childMin = Mathf.Max(36 * (alpha - upper_bound) + 1, 0);
      childMax = Mathf.Min(36 * (beta - lower_bound), 1);

      doubleGenerator.generate();
      for(int i = 0; i < _doubleMoves.size(); i++){
        min = Mathf.Min(min, evaluateMovePlayer(_doubleMoves.moves[i], depth - 1, childMin, childMax));
        childMax = Mathf.Min(childMax, min);
        if (childMin >= childMax) break;
      }
      if(_doubleMoves.size() == 0) min = minimaxAi(depth - 1, childMin, childMax);

      // car il y a 1 / 36 chances dee identiques;
      upper_bound -= (1.0f - min) * (1.0f / 36.0f);
      lower_bound += min * (1.0f / 36.0f);
      
      
      if(upper_bound <= alpha) {
        entry.storeUpper((sbyte)depth, upper_bound);
        return upper_bound;
      }

      if(lower_bound >= beta) {
        entry.storeLower((sbyte)depth, lower_bound);
        return lower_bound;
      }
    }
    entry.storeAll((sbyte)depth, upper_bound);
    return upper_bound;
  }

  public uint bestMoveAI(int dice1, int dice2, int depth) // selects and plays the move with the highest score
  {
    
    simpleMoveArray simpleMoves = simpleMovesPool[depth];
    doubleMoveArray doubleMoves = doubleMovesPool[depth];
    simpleDiceGeneratorAI simpleGen = simpleGenPool[depth];
    doubleDiceGeneratorAI doubleGen = doubleGenPool[depth];
    uint bestMove = 0;
    float bestScore = float.MinValue;
    if(dice1 == dice2)
    {
      doubleGen.setDice(dice1);
      doubleGen.generate();
      for(int i = 0; i < doubleMoves.size(); i++)
      {
        float score = evaluateMoveAI((uint)doubleMoves.moves[i], depth - 1, Mathf.Max(bestScore, 0), 1.0f);
        if(score >= bestScore)
        {
          bestScore = score;
          bestMove = doubleMoves.moves[i];
        }
      }
    }
    else
    {
      if(dice1 > dice2) simpleGen.setDices(dice1, dice2);
      else simpleGen.setDices(dice2, dice1);
      simpleGen.generate();
      for(int i = 0; i < simpleMoves.size(); i++)
      {
        float score = evaluateMoveAI((uint)simpleMoves.moves[i], depth - 1, Mathf.Max(bestScore, 0), 1.0f);
        if(score >= bestScore)
        {
          bestScore = score;
          bestMove = simpleMoves.moves[i];
        }
      }
    }
    return bestMove;
  }

  float evaluateMoveAI(uint moveSequence, int depth, float alpha, float beta)
  {
    if(moveSequence == 0) return minimaxPlayer(depth, alpha, beta);
    uint move = moveSequence & 0xff;
    moveSequence >>= 8;
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from - dice;
    float eval;
    if(to <= 0) // bearoff move
    {
      uint bit_mod = (uint)(((Pos.chips[from] == -1) ? 1 : 0) << from);

      Pos.ai_bearoff++;
      Pos.chips[from]++;
      Pos.ai_present ^= bit_mod;

      eval = evaluateMoveAI(moveSequence, depth, alpha, beta);

      Pos.chips[from]--;
      Pos.ai_present ^= bit_mod;
      Pos.ai_bearoff--;

    }
    else if(Pos.chips[to] == 1)
    {
      uint bit_to = (1u << to);
      uint bit_mod = (uint)(bit_to | (((Pos.chips[from] == -1) ? 1 : 0) << from));
      bit_to |= (Pos.chips[0] == 0) ? 1u : 0u;

      Pos.chips[to] = -1;
      Pos.chips[from]++;
      Pos.chips[0]++;
      Pos.ai_present ^= bit_mod;
      Pos.player_present ^= bit_to;

      eval = evaluateMoveAI(moveSequence, depth, alpha, beta);

      Pos.chips[to] = 1;
      Pos.chips[from]--;
      Pos.chips[0]--;
      Pos.ai_present ^= bit_mod;
      Pos.player_present ^= bit_to;
    }
    else
    {
      uint bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == -1) ? 1 : 0) << from));

      Pos.chips[from]++;
      Pos.chips[to]--;
      Pos.ai_present ^= bit_mod;

      eval = evaluateMoveAI(moveSequence, depth, alpha, beta);

      Pos.chips[to]++;
      Pos.chips[from]--;
      Pos.ai_present ^= bit_mod;
    }
    return eval;
  }

  public uint bestMovePlayer(int dice1, int dice2, int depth)
  {
    simpleMoveArray simpleMoves = simpleMovesPool[depth];
    doubleMoveArray doubleMoves = doubleMovesPool[depth];
    simpleDiceGeneratorPlayer simplePlayerGen = simplePlayerGenPool[depth];
    unorderedDoubleDiceGeneratorPlayer doublePlayerGen = doublePlayerGenPool[depth];
    uint bestMove = 0;
    float bestScore = float.MaxValue;
    if(dice1 == dice2)
    {
      doublePlayerGen.setDice(dice1);
      doublePlayerGen.generate();
      for(int i = 0; i < doubleMoves.size(); i++)
      {
        float score = evaluateMovePlayer((uint)doubleMoves.moves[i], depth - 1, 0.0f,  Mathf.Max(bestScore, 1));
        if(score <= bestScore)
        {
          bestScore = score;
          bestMove = doubleMoves.moves[i];
        }
      }
    }
    else
    {
      if(dice1 > dice2) simplePlayerGen.setDices(dice1, dice2);
      else simplePlayerGen.setDices(dice2, dice1);
      simplePlayerGen.generate();
      for(int i = 0; i < simpleMoves.size(); i++)
      {
        float score = evaluateMovePlayer((uint)simpleMoves.moves[i], depth - 1, 0.0f, Mathf.Max(bestScore, 1));
        if(score <= bestScore)
        {
          bestScore = score;
          bestMove = simpleMoves.moves[i];
        }
      }
    }
    return bestMove;
  }
  float evaluateMovePlayer(uint moveSequence, int depth, float alpha, float beta)
  {
    if(moveSequence == 0) return minimaxAi(depth, alpha, beta);
    uint move = moveSequence & 0xff;
    moveSequence >>= 8;
    int dice = (int)(move >> 5);
    int from = (int)(move & 0b11111);
    int to = from + dice;
    float eval;
    if(to >= 25) // bearoff move
    {
      uint bit_mod = (uint)(((Pos.chips[from] == 1) ? 1 : 0) << from);

      Pos.player_bearoff++;
      Pos.chips[from]--;
      Pos.player_present ^= bit_mod;

      eval = evaluateMovePlayer(moveSequence, depth, alpha, beta);

      Pos.player_bearoff--;
      Pos.chips[from]++;
      Pos.player_present ^= bit_mod;
    }
    else if(Pos.chips[to] == -1)
    {
      uint bit_to = (1u << to);
      uint bit_mod = (uint)(bit_to | (((Pos.chips[from] == 1) ? 1 : 0) << from));
      bit_to |= ((Pos.chips[25] == 0) ? 1u : 0u) << 25;

      Pos.chips[to] = 1;
      Pos.chips[from]--;
      Pos.chips[25]--;
      Pos.player_present ^= bit_mod;
      Pos.ai_present ^= bit_to;

      eval = evaluateMovePlayer(moveSequence, depth, alpha, beta);

      Pos.chips[to] = -1;
      Pos.chips[from]++;
      Pos.chips[25]++;
      Pos.player_present ^= bit_mod;
      Pos.ai_present ^= bit_to;
    }
    else
    {
      uint bit_mod = (uint)((((Pos.chips[to] == 0) ? 1 : 0) << to) | (((Pos.chips[from] == 1) ? 1 : 0) << from));

      Pos.chips[from]--;
      Pos.chips[to]++;
      Pos.player_present ^= bit_mod;

      eval = evaluateMovePlayer(moveSequence, depth, alpha, beta);

      Pos.chips[to]--;
      Pos.chips[from]++;
      Pos.player_present ^= bit_mod;
    }
    return eval;
  }
}