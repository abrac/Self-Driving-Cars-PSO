# Overview

> Please note: the project is archived. Documentation is lacking. File an issue if you need support.

This project uses a Particle Swarm Optimisation (PSO) to train the computer to control a vehicle (acceleration and steering) and navigate a race track as quick as possible. The project requires [Unity 3D](https://unity.com/) coupled with a C# ide.

## Status
Core functionality is complete. Performance needs to be optimised.

# Project Concept

## Topic Summary

Optimization of an artificial intelligence (AI) driven vehicle in a virtual driving simulator with the objective of minimizing time taken per lap of both racetracks used in training and unseen racetracks.

## More detail

This project uses a PSO to optimize a vehicle controlled by a neural network (NN). A series of racetracks will be presented to the vehicle. The output of the optimization will be to have an AI that can navigate the racetracks without crashing, and as fast as possible. 

Another output of this project **was** to have a game where a vehicle can be controlled by a human and compete with other vehicles controlled by the optimized AI. It should be almost impossible for the human driver to beat the AI in the training tracks, and the AI should pose a reasonable challenge on *unseen* tracks. This output was not realised.

## Hypothesis

Initially all vehicles will be assigned random weights to ensure that the initial population is a uniform representation of the entire search space. The genetic algorithm should achieve an absolute measure of fitness for the path the vehicle should travel.

Thereby, we can expect that over the generations, the cars will learn to:

1. Navigate the training track without crashing and be able to (at least partially) carry this auto-navigation ability over to unseen tracks.
2. To find the racing line of the training track that makes the vehicle complete the track in the shortest possible time.

## Methodology

The project will most likely comprise the following objectives/milestones:

- [x] Set up a virtual driving simulator/environment using Unity.
- [x] Experimentally figure out what spatial and environmental information regarding the vehicle will be useful as inputs into the driving controller&#39;s neural network, and how to capture them from the environment.
- [x] Determine the best structure of the neural network for driving the cars (such as activation functions and outputs).
- [x] Set up a genetic algorithm to find the optimal weights for the neural network:
  - [x] Experimentally determine the best fitness function with which to evaluate the *fitness* of different neural networks' performance. Possible factors to consider are: how far the individual made it around the track before crashing (further is better), then the time taken to cover that distance (shorter is better). The evaluation of an individual stops when the individual crashes (or possibly finishes a lap).
- [x] Determine how to evolve the population as efficiently as possible – seeing as it will be an actual race in a 3D environment – it may take a long time per generation – especially once the evolution progresses.
- [ ] Depending on how quickly the team progresses through the initial stages of setup, and how effective/solid the evolutionary outcome is, there is a lot of flexibility regarding further exploration and experimentation on this topic (e.g. through a game)
 
![](_HiddenFiles/CarImage.png)
 
Above: possible type of inputs to be fed into the neural network. Outputs could then translate to which parts of the car controller (such as accelerate, turn left, turn right, or brake) should be activated, deactivated, or remain unchanged from previous state.
