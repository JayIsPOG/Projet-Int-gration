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
  System.Random rng = new System.Random();
  System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
  void Start()
  {
    Camera.main.enabled = false;
    games_played = 0;
    AI = new Learner();
    AI.LoadWeights("160_neurons.bin");

    Pos = new BoardState();
    bot = new Bot(Pos, AI, 0);
    
    AI.evaluatePosition(Pos); // init
    AI.Swap();
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
        uint bestMove = bot.bestMovePlayer(rng.Next(1, 7), rng.Next(1, 7), 0);
        for(; bestMove != 0; bestMove >>= 8)
        {
         uint move = bestMove & 0xff;
         Pos.makeMovePlayer(move);
       }
        Pos.playerTurn = false;
        AI.generateInputs(Pos);
        if (Pos.hasPlayerWon())
        {
          AI.learnGameEnd(0, learnRate);
          games_played++;
          Pos.set();
          //Debug.Log(AI.getPreviousOutput()*AI.getPreviousOutput());
          AI.evaluatePosition(Pos); // init
          AI.Swap();
          return;
        }
        else AI.learnForRegular(learnRate);

        bestMove = bot.bestMoveAI(rng.Next(1, 7), rng.Next(1, 7), 0);
        for(; bestMove != 0; bestMove >>= 8)
        {
         uint move = bestMove & 0xff;
         Pos.makeMoveAI(move);
        }
        Pos.playerTurn = true;
        AI.generateInputs(Pos);
        if (Pos.hasAIWon())
        {
          AI.learnGameEnd(1, learnRate);
          games_played++;
          Pos.set();
          //Debug.Log((1-AI.getPreviousOutput())*(1-AI.getPreviousOutput()));
          AI.evaluatePosition(Pos); // init
          AI.Swap();
          return;
        }
        else AI.learnForRegular(learnRate);
  }
  void Update()
  {
    
  }
  void OnApplicationQuit()
  {
      SaveWeights("160_neurons.bin");
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