# Project Sunlight

###Shirley Huang - Gameplay Programming Patterns Final Project

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

Code Structure
--
1. GameManager
 - Handles the basic status of all levels
1. EvolutionManager
 - Handles the evolution of all living agents.
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
