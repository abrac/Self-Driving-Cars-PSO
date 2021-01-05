# Overview

> Please note: the project is archived. Documentation of the project can be found in `documentation.pdf`. File an issue if you need support.

This project uses a Particle Swarm Optimisation (PSO) to train the computer to control a vehicle (acceleration and steering) and navigate a race track as quick as possible. The project is written in C# and requires [Unity 3D](https://unity.com/).

## Video
[![Video of project](https://i.imgur.com/yWIlvLO.png)](https://www.youtube.com/watch?v=vOLzxJX3GNc)

# Project Concept

## Topic Summary

Optimization of an artificial intelligence (AI) driven vehicle in a virtual driving simulator with the objective of minimizing time taken per lap of both racetracks used in training and unseen racetracks.

## More detail

This project uses a PSO to optimize a vehicle controlled by a neural network (NN). A series of racetracks will be presented to the vehicle. The output of the optimization will be to have an AI that can navigate the racetracks without crashing, and as fast as possible. 

It is envisaged that this project will be useful to game developers, who want AI-controlled vehicles. It should also be useful for the development of self-driving cars, or for finding the *racing-line* in motorsport. 

## Hypothesis

Initially all vehicles are assigned random weights to ensure that the initial population is a uniform representation of the entire search space. The evolutionary algorithm has a measure of fitness for the path a vehicle travels. This measure is based on the distance travelled before crashing and the time taken to complete a lap.

Thereby, over successive generations, the cars learn to:

1. Navigate the training track without crashing and be able to (at least partially) carry this auto-navigation ability over to unseen tracks.
2. To find the racing line of the training track that makes the vehicle complete the track in the shortest possible time.

![](_HiddenFiles/CarImage.png)
 
Above: Inputs to the neural network are from a set of "feelers" which report the distance to the nearest obstacle in various directions. Outputs  translate to which parts of the car controller (such as accelerate, turn left, turn right, or brake) should be activated, deactivated, or remain unchanged from the previous state.

## Status
Core functionality is complete. Performance of the simulation needs to be optimised.
