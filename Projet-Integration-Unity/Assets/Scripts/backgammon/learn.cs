using UnityEngine;
using System.Numerics;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
unsafe public class learn : MonoBehaviour
{
  Learner AI;
  Bot bot;
  BoardState Pos;
  public float learnRate;
  public int games_played;
  public int plays_batch = 30;
  public float lambda;
  System.Random rng = new System.Random();
  System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
  void Start()
  {
    Camera.main.enabled = false;
    AI = new Learner();
    AI.LoadWeights("80_neurons.bin");

    Pos = new BoardState();
    bot = new Bot(Pos, AI, 0);
    
    AI.prev_output = AI.evaluatePosition(Pos); // init
    StartCoroutine(TrainingLoop());
  }
  IEnumerator TrainingLoop()
  {
    while(true){
      for(int i = 0; i < plays_batch; i++){
        play();
      }
      yield return null;
    }
  }

  unsafe void play()
  {
    learnRate = 0.01f / (1f + games_played * 0.00001f);
        uint bestMove = bot.bestMovePlayer(rng.Next(1, 7), rng.Next(1, 7), 0);
        for(; bestMove != 0; bestMove >>= 8)
        {
         uint move = bestMove & 0xff;
         Pos.makeMovePlayer(move);
       }
        Pos.playerTurn = false;
        AI.generateInputs(Pos);
        AI.learnForRegular(learnRate, lambda);
        if (Pos.hasPlayerWon())
        {
          AI.learnGameEnd(0, learnRate, lambda);
          games_played++;
          Pos.set();
          //Debug.Log(AI.getPreviousOutput()*AI.getPreviousOutput());
          AI.prev_output = AI.evaluatePosition(Pos); // init
          return;
        }

        bestMove = bot.bestMoveAI(rng.Next(1, 7), rng.Next(1, 7), 0);
        for(; bestMove != 0; bestMove >>= 8)
        {
         uint move = bestMove & 0xff;
         Pos.makeMoveAI(move);
        }
        Pos.playerTurn = true;
        AI.generateInputs(Pos);
        AI.learnForRegular(learnRate, lambda);
        if (Pos.hasAIWon())
        {
          AI.learnGameEnd(1, learnRate, lambda);
          games_played++;
          Pos.set();
          //Debug.Log((1-AI.getPreviousOutput())*(1-AI.getPreviousOutput()));
          AI.prev_output = AI.evaluatePosition(Pos); // init
          return;
        }
  }
  void Update()
  {
    
  }
  void OnApplicationQuit()
  {
      SaveWeights("new_80_neurons.bin");
  }
  void SaveWeights(string filename)
  {
      string path = Application.dataPath + "/" + filename;
      using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(path, System.IO.FileMode.Create)))
      {
          foreach (Layer layer in AI.layers)
          {
              for(int i = 0; i < layer.nodes_out * layer.nodes_in; i++)
                  writer.Write(layer.weights[i]);
              for(int i = 0; i < layer.nodes_out; i++)
                  writer.Write(layer.biases[i]);
          }
      }
      Debug.Log("Weights saved to: " + path);
  }
  void OnDestroy()
  {
      AI.Dispose();
  }
}