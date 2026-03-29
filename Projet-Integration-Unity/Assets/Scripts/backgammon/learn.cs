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
    learnRate = 3;
    games_played = 0;
    AI = new Learner();

    Pos = new BoardState();
    bot = new Bot(Pos, AI, 0);
    
    AI.evaluatePosition(Pos); // init
    float*[] temp = AI.curr_in_out;
    AI.curr_in_out = AI.prev_in_out;
    AI.prev_in_out = temp;
    StartCoroutine(TrainingLoop());
  }
  IEnumerator TrainingLoop()
  {
    while(true){
      for(int i = 0; i < plays_batch; i++){
        play();
      }
      yield return null;
      //Debug.Log((1000 * (double)games_played / (timer.ElapsedMilliseconds)).ToString());
    }
  }

  unsafe void play()
  {
        bot.makeForDicePlayer(rng.Next(1, 7), rng.Next(1, 7), 0);
        AI.generateInputs(Pos);
        if (Pos.hasPlayerWon())
        {
          AI.learnGameEnd(0, learnRate);
          games_played++;
          Pos.set();
          AI.evaluatePosition(Pos); // init
          float*[] temp = AI.curr_in_out;
          AI.curr_in_out = AI.prev_in_out;
          AI.prev_in_out = temp;
        }
        else AI.learnForRegular();

        bot.makeForDiceAI(rng.Next(1, 7), rng.Next(1, 7), 0);
        AI.generateInputs(Pos);
        if (Pos.hasAIWon())
        {
          AI.learnGameEnd(1, learnRate);
          games_played++;
          Pos.set();
          AI.evaluatePosition(Pos); // init
          float*[] temp = AI.curr_in_out;
          AI.curr_in_out = AI.prev_in_out;
          AI.prev_in_out = temp;
        }
        else AI.learnForRegular();
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