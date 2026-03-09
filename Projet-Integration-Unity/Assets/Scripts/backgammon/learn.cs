using UnityEngine;
using Unity.Mathematics;
using System.Numerics;
public class learn : MonoBehaviour
{
  Learner AI;
  Bot bot;
  BoardState Pos;
  public double learnRate;
  public int games_played;
  public int plays_batch = 30;
  void Start()
  {
    learnRate = 3;
    games_played = 0;
    AI = new Learner();


    Pos = new BoardState();
    bot = new Bot(Pos, AI);
    
    AI.evaluatePosition(Pos); // init
    double[][] temp = AI.curr_in_out;
    AI.curr_in_out = AI.prev_in_out;
    AI.prev_in_out = temp;
  }
  void Update()
  {
    for(int i = 0; i < plays_batch; i++){
      bot.makeForDicePlayer(UnityEngine.Random.Range(1, 7), UnityEngine.Random.Range(1, 7));
      AI.generateInputs(Pos);
      if (Pos.hasPlayerWon())
      {
        AI.learnGameEnd(0, learnRate);
        games_played++;
        Pos.set();
        AI.evaluatePosition(Pos); // init
        double[][] temp = AI.curr_in_out;
        AI.curr_in_out = AI.prev_in_out;
        AI.prev_in_out = temp;
      }
      else AI.learnForRegular();

      bot.makeForDiceAI(UnityEngine.Random.Range(1, 7), UnityEngine.Random.Range(1, 7));
      AI.generateInputs(Pos);
      if (Pos.hasAIWon())
      {
        AI.learnGameEnd(1, learnRate);
        games_played++;
        Pos.set();
        AI.evaluatePosition(Pos); // init
        double[][] temp = AI.curr_in_out;
        AI.curr_in_out = AI.prev_in_out;
        AI.prev_in_out = temp;
      }
      else AI.learnForRegular();
    }
  }
  void OnApplicationQuit()
  {
      SaveWeights("new_weights.bin");
  }
  void SaveWeights(string filename)
  {
      string path = Application.persistentDataPath + "/" + filename;
      using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(path, System.IO.FileMode.Create)))
      {
          foreach (Layer layer in AI.layers)
          {
              foreach (double w in layer.weights)
                  writer.Write(w);
              foreach (double b in layer.biases)
                  writer.Write(b);
          }
      }
      Debug.Log("Weights saved to: " + path);
  }

  void LoadWeights(string filename)
  {
      string path = Application.persistentDataPath + "/" + filename;
      if (!System.IO.File.Exists(path)) return;
      using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(path, System.IO.FileMode.Open)))
      {
          foreach (Layer layer in AI.layers)
          {
              for (int i = 0; i < layer.weights.Length; i++)
                  layer.weights[i] = reader.ReadDouble();
              for (int i = 0; i < layer.biases.Length; i++)
                  layer.biases[i] = reader.ReadDouble();
          }
      }
  }
}