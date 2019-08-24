Animals
=======
**Bio:**
Places one of the 8 animals into the area

Attributes:
------------
**Type**:	 

	* 0 – Flicky (Blue Bird)
	* 1 – Ricky (Chipmunk)
	* 2 – Pocky (Rabbit)
	* 3 – Pecky (Penguin)
	* 4 – Picky (Pig)
	* 5 – Cucky (Chicken)
	* 6 – Rocky (Seal)
	* 7 – Becky (Bear)
	* 8 – Locky (Bald Eagle)
	* 9 – Tocky (Turtle)
	* 10 – Wocky (Monkey)
	* 11 – Micky (Rat)

**Behaviour**:

Determines how the animal behaves in the level

	* 0 – Animal runs away like normal
	* 1 – Animal will follow the player around
	* 2 – Animal will move in random directions

**NOTE**: Do not set type over 11, the game will crash due to file I/O error!

**NOTE 2**: Animals have behaviour controlled by a static object file, such behaviours as speeds are in this file! other behaviours this file controls are not known
