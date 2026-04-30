
This project is a dynamic 2D ecosystem simulation developed in Unity using C#. Its main goal is not just to create an entertaining game, but to model complex behavior patterns in a limited-resource environment. Three types of populations interact in a closed space: carnivores, herbivores, and omnivores. Each has unique characteristics for survival, speed, and energy consumption.

Core Features
* **Dynamic Food Chain:** Complex interactions between different animal populations.
* **State-Machine AI:** Animals make independent, real-time decisions (searching for food, attacking, running from predators, and starving) based on their current stats.
* **Mathematical Balancing:** Calculated models for damage, speed, and hunger depletion rates to ensure a realistic natural selection process.
* **Optimized Performance:** The engine calculates collisions, movement, and logic for dozens of independent objects simultaneously without dropping the frame rate.

Tech Stack
* **Game Engine:** Unity 2D
* **Programming Language:** C# 
* **Architecture:** Component-Based Architecture (MonoBehaviour), Object-Oriented Programming (OOP), Polymorphism
