Ring
=======

**Bio:**

Where coins are for Mario, rings are for Sonic. These collectables are everywhere. They are essential for staying alive safely. They are also pretty complex given their abundance.

Attributes
------------

**Type:** 

Changes the Type of Ring

	* 0 - Normal (Common)
	* 1 - Large Rings (Wields 5 Rings)
	* 2 - Sparkling Rings (Wields 1 Ring; Twinkles instead of displaying a ring)

**PlaneFilter:**

Determines the FG Layer the Ring is On; The outcome of the value depends on Layer Setup of your scene. 


**Move Type:**

Determines if and how the the ring will move

	* 0 - Normal (No Movement)
	* 1 - Left/Right/Up/Down Movement
	* 2 - Circular Movement
	* 3 - Path Movement
	* 4 - Attract Movement (Used when electric shield attracts rings)
	* >= 5 - Normal

	Note: Movement also relies on the following attributes.

**Amplitude:**

Only works on rings with a movement type higher than 0, for type 1, allows you to set how far the ring will go up and down and or left and right. For type 2, make the circle that the ring will circumfurimate around larger or smaller. The object simply will not do much without this attribute being set to something other than 0.

**Speed:** 

At 0, rings will not move along their movement path defined by their movement type at all. It gets faster the more you increment the number.

**Angle:** 

Determines the Starting Position for the Ring when using Type 2 Movement. As seen in Studiopolis Act 2.
