using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M = MathNet.Numerics.LinearAlgebra.Matrix<float>;
using V = MathNet.Numerics.LinearAlgebra.Vector<float>;
using Normal = MathNet.Numerics.Distributions.Normal;//need this to generate random numbers
using UnityEngine.Assertions;

public class NeuralNetwork {
    // a very simple neural network with 2 hidden layers and 1 output layer
    // use the most commonly used relu activation function
    // since we use neuroevolution we don't worry about gradients
    M layer1;
    int _input_dim;
    int _output_dim;

    public NeuralNetwork(int input_dim, int output_dim) {
        // use a random gaussian initialization of the network layers
        _input_dim = input_dim;
        _output_dim = output_dim;

        RandomlyInitLayers();
    }

    public NeuralNetwork Clone() {
        NeuralNetwork newClone = new NeuralNetwork(_input_dim, _output_dim);
        newClone.setLayers(getLayersCopy());
        return newClone;
    }

    public void RandomlyInitLayers() {
        layer1 = M.Build.Random(_input_dim + 1, _output_dim, new Normal(0, 0.01));
    }

    public List<M> getLayersCopy() {
        // get a copy of the neural net weights
        List<M> listOfLayers = new List<M>();
        listOfLayers.Add(M.Build.DenseOfMatrix(layer1));
        return listOfLayers;
    }

    public void setLayers(List<M> listOfLayers) {
        // set the neural net weights
        layer1 = M.Build.DenseOfMatrix(listOfLayers[0]);
    }

    public M getTestingState(int dim) {
        //for testing purposes
        return M.Build.Random(1, dim, new Normal(0, 0.01));
    }

    //public void Relu(M matrix) {
    //    matrix = matrix.CoerceZero(x => x < 0);
    //}

    private void Relu(ref M matrix) {
        // relu activation function
        // note you need to pass by reference
        // not setting this to public as it's used within the network
        matrix.CoerceZero(x => x < 0);
    }

    public void Mutate(float mutateStd=0.01f) {
        //apply a gaussian noise to the all the weights in the layers
        layer1 += M.Build.Random(_input_dim+1, _output_dim, new Normal(0, mutateStd));
    }

    public List<M> GetMutatedLayersCopy(float mutateStd = 0.01f) {
        //apply a gaussian noise to the all the weights in the layers
        List<M> listOfLayers = new List<M>();

        M newLayer1 = layer1 + M.Build.Random(_input_dim + 1, _output_dim, new Normal(0, mutateStd));
        listOfLayers.Add(newLayer1);
        return listOfLayers;
    }

    private M getBias() {
        // used to get bias term, for better nn training
        return M.Build.DenseOfArray(new[,] { { 1.0f } });
    }

    public M Forward(M x) {
        //x is the game state, in the form of 1xD matrix
        //given a game state x, forward through the neural net
        M bias = getBias();
        M x_b = x.Append(bias);
        M activation = x_b * layer1;
        return activation;
    }

    public void NNDebug() {
        //M trialVec = M.Build.Random(1, 4, new Normal(0, 0.1));
        //M m1 = M.Build.DenseOfArray(new[,] {{ 1.0f}});
        //M z = trialVec.Append(m1);
        //Debug.Log(trialVec);
        //Debug.Log(m1);
        //Debug.Log(z);
        M sampleState = getTestingState(4);
        M resutls = Forward(sampleState);
        Debug.Log(resutls);
    }
}







/*
 public class NeuralNetwork {
    // a very simple neural network with 2 hidden layers and 1 output layer
    // use the most commonly used relu activation function
    // since we use neuroevolution we don't worry about gradients
    M layer1;
    M layer2;
    int _input_dim;
    int _hidden_dim;
    int _output_dim;

    public NeuralNetwork(int input_dim, int hidden_dim, int output_dim) {
        // use a random gaussian initialization of the network layers
        _input_dim = input_dim;
        _hidden_dim = hidden_dim;
        _output_dim = output_dim;

        RandomlyInitLayers();
    }

    public void RandomlyInitLayers() {
        layer1 = M.Build.Random(_input_dim + 1, _hidden_dim, new Normal(0, 0.01));
        layer2 = M.Build.Random(_hidden_dim + 1, _output_dim, new Normal(0, 0.01));
    }

    public List<M> getLayersCopy() {
        // get a copy of the neural net weights
        List<M> listOfLayers = new List<M>();
        listOfLayers.Add(M.Build.DenseOfMatrix(layer1));
        listOfLayers.Add(M.Build.DenseOfMatrix(layer2));
        return listOfLayers;
    }

    public void setLayers(List<M> listOfLayers) {
        // set the neural net weights
        Assert.IsTrue(listOfLayers.Count==2);
        layer1 = M.Build.DenseOfMatrix(listOfLayers[0]);
        layer2 = M.Build.DenseOfMatrix(listOfLayers[1]);
    }

    public M getTestingState(int dim) {
        //for testing purposes
        return M.Build.Random(1, dim, new Normal(0, 0.01));
    }

    //public void Relu(M matrix) {
    //    matrix = matrix.CoerceZero(x => x < 0);
    //}

    private void Relu(ref M matrix) {
        // relu activation function
        // note you need to pass by reference
        // not setting this to public as it's used within the network
        matrix.CoerceZero(x => x < 0);
    }

    public void Mutate(float mutateStd=0.01f) {
        //apply a gaussian noise to the all the weights in the layers
        layer1 += M.Build.Random(_input_dim+1, _hidden_dim, new Normal(0, mutateStd));
        layer2 += M.Build.Random(_hidden_dim+1, _output_dim, new Normal(0, mutateStd));
    }

    public List<M> GetMutatedLayersCopy(float mutateStd = 0.01f) {
        //apply a gaussian noise to the all the weights in the layers
        List<M> listOfLayers = new List<M>();

        M newLayer1 = layer1 + M.Build.Random(_input_dim + 1, _hidden_dim, new Normal(0, mutateStd));
        M newLayer2 = layer2 + M.Build.Random(_hidden_dim + 1, _output_dim, new Normal(0, mutateStd));
        listOfLayers.Add(newLayer1);
        listOfLayers.Add(newLayer2);
        return listOfLayers;
    }

    private M getBias() {
        // used to get bias term, for better nn training
        return M.Build.DenseOfArray(new[,] { { 1.0f } });
    }

    public M Forward(M x) {
        //x is the game state, in the form of 1xD matrix
        //given a game state x, forward through the neural net
        M bias = getBias();
        M x_b = x.Append(bias);
        M activation = x_b * layer1;
        Relu(ref activation);

        activation = activation.Append(bias);
        activation = activation * layer2;
        return activation;
    }

    public void NNDebug() {
        //M trialVec = M.Build.Random(1, 4, new Normal(0, 0.1));
        //M m1 = M.Build.DenseOfArray(new[,] {{ 1.0f}});
        //M z = trialVec.Append(m1);
        //Debug.Log(trialVec);
        //Debug.Log(m1);
        //Debug.Log(z);
        M sampleState = getTestingState(4);
        M resutls = Forward(sampleState);
        Debug.Log(resutls);
    }
}
     */
