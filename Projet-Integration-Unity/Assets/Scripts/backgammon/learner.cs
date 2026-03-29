using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

[BurstCompile]
public static unsafe class BurstForward
{
    [BurstCompile]
    public static void CalcOutputs(float* inputs,float* weights,float* biases,float* outputs,int nodes_in,int nodes_out)
    {
        for (int i = 0; i < nodes_out; i++)
        {
            float sum = biases[i];
            for (int j = 0; j < nodes_in; j++)
                sum += inputs[j] * weights[j * nodes_out + i];
            outputs[i] = 1.0f / (1.0f + math.exp(-sum));
        }
    }

    [BurstCompile]
    public static void updateGradients(float* inputs, float* weightGradient,float* nodeValues, float* biasGradient,int nodes_in,int nodes_out) {

        for (int i = 0; i < nodes_out; i++) {
            for (int j = 0; j < nodes_in; j++) {
                weightGradient[j * nodes_out + i] += inputs[j] * nodeValues[i];
            }
            biasGradient[i] += nodeValues[i];
        }
    }
    [BurstCompile]
    public static void calculateHiddenLayerNodeValues(float* nextWeights, float* nextNodeValues, int nextNodesOut, float* outputs, float* nodeValues, int nodes_out) {
        for (int i = 0; i < nodes_out; i++) {
            float value = 0;
            for (int j = 0; j < nextNodesOut; j++) value += nextWeights[i * nextNodesOut + j] * nextNodeValues[j];
            nodeValues[i] = value * outputs[i] * (1.0f - outputs[i]);
        }
    }
    [BurstCompile]
    public static void applyGradient(float learnRate, float* weights, float* weightGradient, float* biasGradient, float* biases, int nodes_in, int nodes_out) {
        for (int j = 0; j < nodes_in; j++) {
            for (int i = 0; i < nodes_out; i++) {
                int index = j * nodes_out + i;
                weights[index] -= learnRate * weightGradient[index];
            }
        }
        for (int i = 0; i < nodes_out; i++) {
            biases[i] -= learnRate * biasGradient[i];
        }
    }
}
unsafe class Layer : System.IDisposable {
    public int nodes_in;
    public int nodes_out;
    public float* weights;
    public float* biases;
    public float* weightGradient;
    public float* biasGradient;
    public float* nodeValues;

    public void Dispose()
    {
      UnsafeUtility.Free(weights, Allocator.Persistent);
      UnsafeUtility.Free(biases, Allocator.Persistent);
      UnsafeUtility.Free(weightGradient, Allocator.Persistent);
      UnsafeUtility.Free(biasGradient, Allocator.Persistent);
      UnsafeUtility.Free(nodeValues, Allocator.Persistent);
    }
    float deriveCost(float Out, float expectedOut) {
        return 2 * (Out - expectedOut);
    }
    float deriveOutput(float Out) {
      return Out * (1.0f- Out);
    }
    public Layer(int in_num, int out_num) {
        nodes_in = in_num;
        nodes_out = out_num;

        weights = (float*)UnsafeUtility.Malloc(nodes_in * nodes_out * sizeof(float), 16, Allocator.Persistent);
        weightGradient = (float*)UnsafeUtility.Malloc(nodes_in * nodes_out * sizeof(float), 16, Allocator.Persistent);
        biases = (float*)UnsafeUtility.Malloc(nodes_out * sizeof(float), 16, Allocator.Persistent);
        biasGradient = (float*)UnsafeUtility.Malloc(nodes_out * sizeof(float), 16, Allocator.Persistent);
        nodeValues = (float*)UnsafeUtility.Malloc(nodes_out * sizeof(float), 16, Allocator.Persistent);

        UnsafeUtility.MemClear(biases, nodes_out * sizeof(float));
        UnsafeUtility.MemClear(biasGradient, nodes_out * sizeof(float));
        UnsafeUtility.MemClear(nodeValues, nodes_out * sizeof(float));
        UnsafeUtility.MemClear(weightGradient, nodes_in * nodes_out * sizeof(float));
        
        System.Random rng = new System.Random();
        float scale = Mathf.Sqrt(6.0f/ (nodes_in + nodes_out)); // uniform Xavier
        for (int i = 0; i < nodes_in * nodes_out; i++)
          weights[i] = ((float)rng.NextDouble() * 2.0f- 1.0f) * scale;
        for (int i = 0; i < nodes_out; i++)
          biases[i] = 0;
    }

    public unsafe void calcOutputs(float* inputs, float* outputs)
    {
      BurstForward.CalcOutputs(inputs, weights, biases, outputs, nodes_in, nodes_out);
    }

    public void calculateOutputNodeValues(float expectedOut, float output) { // the output size will always be 1 (represents the probability of winning)
      nodeValues[0] = deriveCost(output, expectedOut) * deriveOutput(output);
    }

    public unsafe void updateGradients(float* inputs) {
      BurstForward.updateGradients(inputs, weightGradient, nodeValues, biasGradient, nodes_in, nodes_out);
    }

    public void calculateHiddenLayerNodeValues(Layer nextLayer, float* outputs) {
        BurstForward.calculateHiddenLayerNodeValues(nextLayer.weights, nextLayer.nodeValues, nextLayer.nodes_out, outputs, nodeValues, nodes_out);
    }

    public void applyGradient(float learnRate) {
        BurstForward.applyGradient(learnRate, weights, weightGradient, biasGradient, biases, nodes_in, nodes_out);
    }

    public void clearGradients() {
        UnsafeUtility.MemClear(biasGradient,   nodes_out * sizeof(float));
        UnsafeUtility.MemClear(weightGradient, nodes_in * nodes_out * sizeof(float));
    }
};

/*
Reseau neuronnal fait de la meme maniere que TD-Gammon:
A backgammon board consists of twenty-four board positions, and each of these positions is represented by eight input neurons. 
This board representation comprises the first 192 inputs to the network. 
For each position, four inputs represent the number of white pieces and four represent the number of black pieces. 
This representation is simple -- the first neuron is on if there is one piece on the board at the given position. 
The second and first are both on if there are two pieces, and the first three are on if there are three checkers. 
The fourth neuron represents one-half the number of checkers beyond three that are present on the board position.

The next two inputs represent the player who is moving; the first is on for a move by white and the second is on for a move by black. 
Pieces on the bar are the next two, each input being one-half the number of checkers on the bar for the corresponding player. 
Last, the number of pieces already borne off by each player is represented directly in the final two inputs. 
These are the only inputs whose values are ever expected to exceed unity with any regularity.

 

These 198 input units are fully connected to a hidden layer of 50 units, and this hidden layer is in turn connected to the single output neuron. 
Each hidden layer neuron, and the output layer neuron, also have bias inputs whose values are held at unity. 
*/
unsafe class Learner : System.IDisposable{ // we have to manually set the inputs via the curr_in_out parameter
    //public System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
    public int num_layers;
    public Layer[] layers;
    public int data_num;
    public float*[] curr_in_out;
    public float*[] prev_in_out;
    public Learner() {
        int[] layerSizes = {198, 30, 1};
        data_num = 0;
        num_layers = layerSizes.Length - 1;
        layers = new Layer[num_layers];
        curr_in_out = new float*[layerSizes.Length];
        prev_in_out = new float*[layerSizes.Length];
        for (int i = 0; i < num_layers; i++) layers[i] = new Layer(layerSizes[i], layerSizes[i + 1]);
        for (int i = 0; i < layerSizes.Length; i++) {
          curr_in_out[i] = (float*)UnsafeUtility.Malloc(layerSizes[i] * sizeof(float), 16, Allocator.Persistent);
          prev_in_out[i] = (float*)UnsafeUtility.Malloc(layerSizes[i] * sizeof(float), 16, Allocator.Persistent);
          UnsafeUtility.MemClear(curr_in_out[i], layerSizes[i] * sizeof(float));
          UnsafeUtility.MemClear(prev_in_out[i], layerSizes[i] * sizeof(float));
        }
    }
    
    public void Dispose()
    {
      for(int i = 0; i < num_layers + 1; i++)
      {
        UnsafeUtility.Free(curr_in_out[i], Allocator.Persistent);
        UnsafeUtility.Free(prev_in_out[i], Allocator.Persistent);
      }
      for(int i = 0; i < num_layers; i++) layers[i].Dispose();
    }

    public void generateInputs(BoardState board)
    {
      float* inputs = curr_in_out[0];
      float player_borneoff = 15;
      float ai_borneoff = 15;
      for(int i = 0; i < 24; i++)
      {
        float quantity = (float)board.chips[i + 1];
        if(quantity >= 1.0f) { //pretty damn ugly
          player_borneoff -= quantity;
          inputs[i*8] = 1.0f;
          if(quantity >= 2.0f) {
            inputs[i*4+1] = 1.0f;
            if(quantity >= 3.0f) {
              inputs[i*8+2] = 1.0f;
              inputs[i*8+3] = 0.5f * (quantity - 3);
            }
            else {
              inputs[i*8+2] = 0.0f;
              inputs[i*8+3] = 0.0f;
            }
          }
          else {
            inputs[i*8+1] = 0.0f;
            inputs[i*8+2] = 0.0f;
            inputs[i*8+3] = 0.0f;
          }
        }
        else if(quantity <= -1.0f) {
          ai_borneoff += quantity;
          inputs[i*8+4] = 1.0f;
          if(quantity <= -2.0f) {
            inputs[i*8+5] = 1.0f;
            if(quantity <= -3.0f) {
              inputs[i*8+6] = 1.0f;
              inputs[i*8+7] = 0.5f * (3 - quantity);
            }
            else {
              inputs[i*8+6] = 0.0f;
              inputs[i*8+7] = 0.0f;
            }
          }
          else {
            inputs[i*8+5] = 0.0f;
            inputs[i*8+6] = 0.0f;
            inputs[i*8+7] = 0.0f;
          }
        }
        else {
          inputs[i*8+4] = 0.0f;
          inputs[i*8+5] = 0.0f;
          inputs[i*8+6] = 0.0f;
          inputs[i*8+7] = 0.0f;
        }
      }
      inputs[24*8] = board.playerTurn ? 1f : 0f;
      inputs[24*8+1] = board.playerTurn ? 0f : 1f;
      inputs[24*8+2] = 0.5f * (float)(board.chips[0]);
      inputs[24*8+3] = 0.5f * (float)(-board.chips[25]);
      inputs[24*8+4] = player_borneoff;
      inputs[24*8+5] = ai_borneoff;
    }

    public float makeOutputs() {
      for (int i = 0; i < num_layers; i++) layers[i].calcOutputs(curr_in_out[i], curr_in_out[i + 1]);
      return curr_in_out[num_layers][0];
    }

    void UpdateAllGradients(float expectedOutput) { // the previous output evaluation must be as close to the current one as possible
      data_num++;
      Layer outLayer = layers[num_layers - 1];
      outLayer.calculateOutputNodeValues(expectedOutput, prev_in_out[num_layers][0]);
      outLayer.updateGradients(prev_in_out[num_layers - 1]);

      for (int h = num_layers - 2; h >= 0; h--) {
        Layer hiddenLayer = layers[h];
        hiddenLayer.calculateHiddenLayerNodeValues(layers[h + 1], prev_in_out[h + 1]);
        hiddenLayer.updateGradients(prev_in_out[h]);
      }
    }

    public void learnForRegular()
    {
      UpdateAllGradients(makeOutputs());
      float*[] temp = curr_in_out;
      curr_in_out = prev_in_out;
      prev_in_out = temp;
    }
    public void learnGameEnd(float result, float learnRate)
    {
      makeOutputs();
      UpdateAllGradients(result);
      float*[] temp = curr_in_out;
      curr_in_out = prev_in_out;
      prev_in_out = temp;

      float scaledRate = learnRate / data_num; // the AI learns when the game end
      data_num = 0;
      for (int i = 0; i < num_layers; i++) {
        layers[i].applyGradient(scaledRate);
        layers[i].clearGradients();
      }
    }
    public float evaluatePosition(BoardState board)
    {
      generateInputs(board);
      return makeOutputs();
    }
    public void LoadWeights(string filename)
  {
      string path = Application.persistentDataPath + "/" + filename;
      if (!System.IO.File.Exists(path)) return;
      using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(path, System.IO.FileMode.Open)))
      {
          foreach (Layer layer in layers)
          {
              for (int i = 0; i < layer.nodes_in * layer.nodes_out; i++)
                  layer.weights[i] = reader.ReadSingle();
              for (int i = 0; i < layer.nodes_out; i++)
                  layer.biases[i] = reader.ReadSingle();
          }
      }
  }
};
