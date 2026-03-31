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
  void Start()
  {
    Camera.main.enabled = false;
    games_played = 0;
    AI = new Learner();

    Pos = new BoardState();
    Pos.playerTurn = true;
    bot = new Bot(Pos, AI, 0);
    
    AI.evaluatePosition(Pos); // init
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
    AI.Swap();
    bot.makeForDicePlayer(rng.Next(1, 7), rng.Next(1, 7), 0);
    Pos.playerTurn = false;
    if (Pos.hasPlayerWon())
    {
      AI.learnFor(0, learnRate);
      games_played++;
      Pos.set();
      Debug.Log((AI.previousOutput())*(AI.previousOutput()));
      AI.evaluatePosition(Pos); // init
      return;
    }
    else {
      AI.evaluatePosition(Pos);
      AI.learnFor(AI.currentOutput(), learnRate);
    }

    AI.Swap();
    bot.makeForDiceAI(rng.Next(1, 7), rng.Next(1, 7), 0);
    Pos.playerTurn = true;
    if (Pos.hasAIWon())
    {
      AI.learnFor(1, learnRate);
      games_played++;
      Pos.set();
      Debug.Log((1-AI.previousOutput())*(1-AI.previousOutput()));
      AI.evaluatePosition(Pos); // init
      return;
    }
    else {
      AI.evaluatePosition(Pos);
      AI.learnFor(AI.currentOutput(), learnRate);
    }
  }
  void Update()
  {
    
  }
  void OnApplicationQuit()
  {
      SaveWeights("burger.bin");
  }
  void SaveWeights(string filename)
  {
      string path = Application.persistentDataPath + "/" + filename;
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