Player
=======
**Bio:** 

This entity is where the player will spawn when the scene is loaded. You can have multiple Player Entities in a scene, but depending on its attributes, if there is a confliction, it will always prioritize based on a higher Slot ID.

Attributes
-------------

**CharacterID:**

Determines who spawns at the object

* 0 - No one (Free Cam mode)
* 1 - Sonic
* 2 - Tails
* 3 - Sonic and Tails
* 4 - Knuckles
* 5 - Sonic and Knuckles
* 6 - Tails and Knuckles
* 7 - Team Sonic (Sonic,Tails and Knuckles)

NOTES: Sonic and Tails will simply be handled as Sonic if you used for say Sonic Only. Also, Mighty and Ray are Handled as Sonic, Meaning wherever Sonic can spawn, mighty and ray will and only will spawn if the CharacterID is set to Sonic.
