# GPP Final Project
Shirley Huang

---
Overview
--
This is a game that simulates a matrix of self-evolving agents doing different tasks in their rooms. 

In each level, the agents will take several generations to evolve where each generation takes 10 seconds. When the level is finished, the player can choose to proceed to the next level or stay and observe the agent.

There are two levels completed so far. 

1. Eat: the agent needs to eat as many food as possible.
2. Seek: the agent needs to stay as close as possible to its target.

**Patterns used in this project: Manager, Subclass, Service, Event**

Controls
--
W/A/S/D - Pan

Z/X - Zoom In/Out

Evolution design
--
The classic Genetic Algorithm is used in this game to enforce evolution of agents and emergence of behaviors.

How genetic algorithm in this game works:
Essentially, first generate a random population of agents, and then for each evolution generation we do the following steps:
1. Let the agents interact with their environments for a finite timesteps.
2. When time runs out, evaluate the agents for their fitness, an agent's fitness is basically its score rewarded by the environment it interacts with. E.g. in the eater task an agent's score is proportional to the number of food it eats before time runs out.
3. The several best ones in the population will be selected to survive to the next generation, other individuals are discarded. 
4. produce offsprings from the best individuals to refill the population.

The size of population, the number of elites in population and many other hyperparameters can affect the speed of evolution greatly. These numbers should be picked with caution. 

Designing the exact evolution process is tricky, since Unity is used as the game engine, the evolution is running in real-time, instead of in a fast-forwarding way as in most AI research. To make evolution in this game interesting and tractable within a limited amount of player's time, the following particular designs are crucial:
1. I considered the design of running a separate evolution process in the background of the main game with a separate thread and use that thread to update from time to time the game scene that the player observe, this can be an effective design, but it requires significant effort in remaking the whole game in pure C# and doing multi-threading, which can be difficult and time-consuming to do. Instead, we designed the grid of rooms system to make many agents run in parallel, and give the player the freedom to observe the whole population all at once. This helps evolution to proceed effectively.
2. In modern AI research, experiments show that evolution-based AI can take a dozen to a few milion or even more generations to develop meaningful behaviors, depending on the complexity of their tasks. In this game, which is apparently, a game instead of AI research experiments, the time to evolve is extremely limited, so the tasks are designed to be simple enough that agents can learn them relatively rapidly. And the model structure for the agents are all designed to be simple so that simpler but effective behaviors are evolved. 
3. For highly effective evolution, variance caused by the randomness of room generation and the stochastic movement of agents have to be dealt with. So in the grid of rooms system, we let the room assignment for each column to be the same and let each row of agents share the same weights. Since an agent is uniquely defined by a set of weights, this essentially means each row of rooms contain the same agent. An agent's fitness thus is naturally computed as the average fitness of all agent clones' on that row. In this way we also make sure that all agents have chance to experience the same environment states, in this way fitness comparison between agents is more meaningful and an agent's performance is more accurately measured. We experimented with these settings and found that they help accelerate the evolution process substantially. 

Code Structure
--
1. GameManager
 - Handles the basic status of all levels
1. EvolutionManager
 - Handles the evolution of all living agents.
 - First of all, the Evolution Manager is responsible for initializing all the rooms and agents for each task.
 - At the end of each generation, the Evolution Manager will evaluate all the agents, assign each one of them a score, and then select the elite ones, use them to reproduce and refill the population, and then start the new generation.
1. InputManager
 - Handles the user input
2. UIManager
 - Updates the UI
3. EventManager
 - Handles the events fires sends it to the listeners.
4. Services
 - A static class that holds references to all the managers
4. Room
 - A base class of the rooms where the task of the agents will be defined here.
 - Also handles the agent that is associated to that room
 - Subclasses: EaterRoom, SeekerRoom
5. Agent
 - A base class of the agents.
 - Subclasses: EaterRoom, SeekerRoom
