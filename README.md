# Overview

> Please note: the project is archived. Documentation of the project can be found in `documentation.pdf`. File an issue if you need support.

This project uses a Particle Swarm Optimisation (PSO) to train the computer to control a vehicle (acceleration and steering) and navigate a race track as quick as possible. The project is written in C# and requires [Unity 3D](https://unity.com/).

## Status
Core functionality is complete. Performance of the simulation needs to be optimised.

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

![](_HiddenFiles/CarImage.png)
 
Above: possible type of inputs to be fed into the neural network. Outputs could then translate to which parts of the car controller (such as accelerate, turn left, turn right, or brake) should be activated, deactivated, or remain unchanged from previous state.
