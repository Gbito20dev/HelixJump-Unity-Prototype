🌀 Helix Jump - Unity 6 Prototype

    🎮 PLAY THE PROTOTYPE HERE

📌 Project Overview

This is a fully functional, highly optimized prototype of the popular hyper-casual game Helix Jump. The primary goal of this project was not just to replicate the mechanics, but to demonstrate a production-ready software architecture.

Built with scalability and teamwork in mind, the codebase heavily relies on Object-Oriented Programming (OOP) principles, Data-Driven Design, and custom Editor tools to empower Game Designers. Furthermore, the visual polish was achieved strictly using a closed set of provided assets, showcasing resourcefulness and technical art skills.
⚙️ Core Technical Features & Architecture
🛠️ Designer-Friendly & Procedural Generation

    Data-Driven Design (ScriptableObjects): Level configurations (number of floors, platform types, ring distances) are decoupled from the code. Designers can create and balance infinite levels directly from the Inspector.

    Custom Editor Tools: Implemented custom context menus ([ContextMenu]) to generate, preview, and clean up levels directly within the Unity Editor without needing to enter Play Mode, drastically speeding up iteration times.

    Smart Spawning Algorithm: Power-ups (like the invincibility modifier) are spawned using probability algorithms rather than fixed positions, automatically inheriting the central pillar's physics and rotation.

🧩 Clean Architecture (OOP)

    Single Responsibility Principle: Scripts are highly modular. The Ball only handles its physics, while the Rings autonomously evaluate the player's height to trigger their destruction.

    Optimized Message Passing: Centralized core flow using the Singleton pattern (e.g., GameManager), allowing decoupled communication between objects without heavy GetComponent calls every frame.

    Smart Memory Management: Integrated a native save system (PlayerPrefs) that reloads the scenario dynamically and injects new LevelData without actually destroying and reloading the Unity Scene, eliminating loading bottlenecks.

🛡️ Robust Physics & Bug Prevention

    Edge Case Handling: Implemented mathematical safeguards (e.g., checking contact normals) to prevent the ball from getting stuck on geometric edges.

    Guard Clauses: The code includes strict safety checks that immediately halt execution if the ball attempts to apply physical forces during altered states (like becoming isKinematic on Game Over), keeping the console 100% free of runtime errors.

    Frictionless Collision: Configured optimal frictionless physics materials and high solver iterations to prevent high-speed clipping through platforms.

🎨 Game Feel & Audiovisual Polish (Closed Assets)

    Dynamic Audio System: The GameManager programmatically scales the audio pitch as the player chains combos. The ball features randomized micro-pitch variations on every bounce to prevent audio fatigue.

    Time-Independent Screen Shake: Engineered a camera shake system using unscaledDeltaTime, allowing violent visual feedback upon Game Over even when the engine's Time.timeScale is frozen.

    Pre-Collision Logic (Invincibility Mode): Instead of overtaxing the physics engine, the "Destroyer Mode" alters the logical threshold of the rings, destroying them milliseconds before the physical impact for a buttery-smooth, cinematic fall.

    Art Direction: Maximized visual quality using only the provided assets by implementing Normal Maps for simulated depth, Trail Renderers, Particle Systems, and dynamic Solid Color palette shifts.

🚀 How to Play

    Click the link above to play directly in your browser via WebGL.

    Click and drag horizontally to rotate the central pillar.

    Guide the bouncing ball through the gaps and avoid the colored danger zones!

    Chain multiple drops to activate the Invincibility modifier and smash through platforms!
