using UnityEngine;
using Unity.Mathematics;
class Layer {
    public int nodes_in;
    public int nodes_out;
    public double[] weights;
    public double[] biases;
    public double[] weightGradient;
    public double[] biasGradient;
    public double[] nodeValues;
    double NodeCost(double Out, double expectedOut) {
        double error = expectedOut - Out;
        return error * error;
    }
    double deriveCost(double Out, double expectedOut) {
        return 2 * (Out - expectedOut);
    }
    double deriveOutput(double Out) {
      return Out * (1.0 - Out);
    }
    double outputCalc(double val) {
        return 1.0 / (1.0 + System.Math.Exp(-val)); // sigmoid function
    }
    public Layer(int in_num, int out_num) {
        nodes_in = in_num;
        nodes_out = out_num;
        weights = new double[nodes_in * nodes_out];
        weightGradient = new double[nodes_in * nodes_out];
        biases = new double[nodes_out];
        biasGradient = new double[nodes_out];
        nodeValues = new double[nodes_out];
        System.Random rng = new System.Random();
        double scale = System.Math.Sqrt(6.0 / (nodes_in + nodes_out)); // uniform Xavier
        for (int i = 0; i < nodes_in * nodes_out; i++)
          weights[i] = (rng.NextDouble() * 2.0 - 1.0) * scale;
    }

    public void calcOutputs(double[] inputs, double[] outputs) {
        for (int i = 0; i < nodes_out; i++) {
            double Out = biases[i];
            for (int j = 0; j < nodes_in; j++) {
                Out += inputs[j] * weights[j * nodes_out + i];
            }
            outputs[i] = outputCalc(Out);
        }
    }

    public void calculateOutputNodeValues(double expectedOut, double output) { // the output size will always be 1 (represents the probability of winning)
      nodeValues[0] = deriveCost(output, expectedOut) * deriveOutput(output);
    }

    public void updateGradients(double[] inputs) {
        for (int i = 0; i < nodes_out; i++) {
            for (int j = 0; j < nodes_in; j++) {
                weightGradient[j * nodes_out + i] += inputs[j] * nodeValues[i];
            }
            biasGradient[i] += nodeValues[i];
        }
    }

    public void calculateHiddenLayerNodeValues(Layer nextLayer, double[] outputs) {
        for (int i = 0; i < nodes_out; i++) {
            double value = 0;
            for (int j = 0; j < nextLayer.nodes_out; j++) value += nextLayer.weights[i * nextLayer.nodes_out + j] * nextLayer.nodeValues[j];
            nodeValues[i] = value * deriveOutput(outputs[i]);
        }
    }

    public void applyGradient(double learnRate) {
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

    public void clearGradients() {
        for (int i = 0; i < nodes_in * nodes_out; i++) weightGradient[i] = 0;
        for (int i = 0; i < nodes_out; i++) biasGradient[i] = 0;
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
class Learner { // we have to manually set the inputs via the curr_in_out parameter
    public int num_layers;
    public Layer[] layers;
    public int data_num;
    public double[][] curr_in_out;
    public double[][] prev_in_out;
    public Learner() {
        int[] layerSizes = {198, 50, 1};
        data_num = 0;
        num_layers = layerSizes.Length - 1;
        layers = new Layer[num_layers];
        curr_in_out = new double[layerSizes.Length][];
        prev_in_out = new double[layerSizes.Length][];
        for (int i = 0; i < num_layers; i++) layers[i] = new Layer(layerSizes[i], layerSizes[i + 1]);
        for (int i = 0; i < layerSizes.Length; i++) {
          curr_in_out[i] = new double[layerSizes[i]];
          prev_in_out[i] = new double[layerSizes[i]];
        }
    }

    public void generateInputs(BoardState board)
    {
      double[] inputs = curr_in_out[0];
      double player_borneoff = 15;
      double ai_borneoff = 15;
      for(int i = 0; i < 24; i++)
      {
        double quantity = (double)board.chips[i + 1];
        if(quantity >= 1.0) { //pretty damn ugly
          player_borneoff -= quantity;
          inputs[i*4] = 1.0;
          if(quantity >= 2.0) {
            inputs[i*4+1] = 1.0;
            if(quantity >= 3.0) {
              inputs[i*4+2] = 1.0;
              inputs[i*4+3] = 0.5 * (quantity - 3);
            }
            else {
              inputs[i*4+2] = 0.0;
              inputs[i*4+3] = 0.0;
            }
          }
          else {
            inputs[i*4+1] = 0.0;
            inputs[i*4+2] = 0.0;
            inputs[i*4+3] = 0.0;
          }
        }
        else if(quantity <= -1.0) {
          ai_borneoff += quantity;
          inputs[i*4] = 1.0;
          if(quantity <= -2.0) {
            inputs[i*4+1] = 1.0;
            if(quantity <= -3.0) {
              inputs[i*4+2] = 1.0;
              inputs[i*4+3] = 0.5 * (3 - quantity);
            }
            else {
              inputs[i*4+2] = 0.0;
              inputs[i*4+3] = 0.0;
            }
          }
          else {
            inputs[i*4+1] = 0.0;
            inputs[i*4+2] = 0.0;
            inputs[i*4+3] = 0.0;
          }
        }
        else {
          inputs[i*4] = 0.0;
          inputs[i*4+1] = 0.0;
          inputs[i*4+2] = 0.0;
          inputs[i*4+3] = 0.0;
        }
      }
      inputs[25*4] = board.playerTurn ? 1 : 0;
      inputs[25*4+1] = board.playerTurn ? 0 : 1;
      inputs[25*4+2] = 0.5 * (double)(board.chips[0]);
      inputs[25*4+3] = 0.5 * (double)(-board.chips[25]);
      inputs[25*4+4] = player_borneoff;
      inputs[25*4+5] = ai_borneoff;
    }

    public double makeOutputs() {
      for (int i = 0; i < num_layers; i++) layers[i].calcOutputs(curr_in_out[i], curr_in_out[i + 1]);
      return curr_in_out[num_layers][0];
    }

    void UpdateAllGradients(double expectedOutput) { // the previous output evaluation must be as close to the current one as possible
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
      double[][] temp = curr_in_out;
      curr_in_out = prev_in_out;
      prev_in_out = temp;
    }
    public void learnGameEnd(double result, double learnRate)
    {
      makeOutputs();
      UpdateAllGradients(result);
      double[][] temp = curr_in_out;
      curr_in_out = prev_in_out;
      prev_in_out = temp;

      double scaledRate = learnRate / data_num; // the AI learns when the game end
      data_num = 0;
      for (int i = 0; i < num_layers; i++) {
        layers[i].applyGradient(scaledRate);
        layers[i].clearGradients();
      }
    }
    public double evaluatePosition(BoardState board)
    {
      generateInputs(board);
      return makeOutputs();
    }
};
