# Modular-combat
2D Modular Combat & AI System
This project implements a highly modular, component-based architecture for a 2D action game. By decoupling health, combat, and AI logic, each system can be swapped or tuned independently.

### 1. Core Systems
Health & Vitality (Health.cs)
The backbone of the interaction system. It is a universal component that can be attached to players and enemies alike.

Decoupled Damage: Uses a TakeDamage method that handles health reduction, death triggers, and damage mitigation (blocking).

Event-Driven: Utilizes OnDeath and OnHealthChanged delegates. This allows other scripts (like the UI or Quest systems) to "listen" for death without the Health script needing to know who they are.

Built-in Feedback: Automatically handles "Hurt" and "Death" animation triggers and spawns damage popups.

Damage Feedback (DamagePopup.cs)
A specialized UI/World-space script that provides immediate visual feedback.

Modular Setup: The Setup() method allows any object to pass a numerical value, which the script then animates (rising and fading) independently of the source of the damage.

### 2. AI & Enemy Intelligence
The AI is designed in layers, allowing for different "brains" to control the same physical enemy components.

Navigation & Combat Logic (EnemyBrain.cs, EnemyAI.cs, EnemyFollow.cs)
These scripts represent different AI profiles:

Modular Movement: They interface with a Rigidbody2D and an EnemyAnimationController to move and animate, but the "decision-making" (when to chase vs. when to idle) is contained within these specific scripts.

Detection Systems: Use Gizmos and radius-based logic to detect the player, making it easy to create "Stealth" enemies or "Aggressive" enemies simply by swapping the script or adjusting Inspector values.

### 3. Progression & Buff Systems
The Attack Booster (AttackBooster.cs)
Demonstrates how to extend gameplay via the Observer Pattern.

On-Death Triggers: It subscribes to the Health.OnDeath event. When an enemy dies, this script calculates a "drop" chance for an Attack Buff or a Full Heal.

Direct Modification: It modifies TagCombat.attackDamage directly, showing how easy it is to create permanent or temporary power-up loops.

Global Health Management (PlayerHealthManager.cs)
A management-level script that handles player scaling.

Growth Mechanics: It listens for any enemy death in the scene and rewards the player by increasing both current health and maxHealth cap.

Scalability: This allows for "RPG-lite" mechanics where the player grows stronger as the level progresses.

### 4. Supporting Systems
Camera System (SimpleCameraFollow.cs): A smoothed Lerp-based camera that follows a target. It uses LateUpdate to ensure movement is flicker-free, independent of the player's movement logic.

### Why This is Modular
Interchangeability: You can put Health.cs on a breakable crate, and it will work perfectly without needing an "Enemy" script.

Ease of Tuning: To make a "Boss," you don't write a new script; you simply take an existing enemy and increase the variables in the Health and EnemyBrain components.

Low Dependency: The DamagePopup doesn't care if a sword, a spell, or a falling hazard caused the damage; it just needs a number to display.

Extensibility: Want a "Vampire" buff? Simply create a new script that subscribes to Health.OnDeath (just like AttackBooster does) and adds health to the player.
