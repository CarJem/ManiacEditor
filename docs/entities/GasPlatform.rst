
GasPlatform
=======

**Bio:**

This entity is found in Oil Ocean Zone (OOZ), and acts very differently from the other types of platforms. Exclusively used in OOZ Officially, this platform will “pop-up” depending on it’s settings

Attributes
----------

**Type:**

Sets how the GasPlatform with behave.

	* 0 - Pop Up on set interval; Interval set by the Interval and IntervalOffset attributes.
	* 1 - Pop Up with a added Spring-Like Effect
	* 2 (and Beyond) - Nothing; Solid Dumb Platform

**ChildCount:**

Depending on it’s value, this attribute causes objects to move with the platform. For Example, if you set childCount to 2, and the Entity Slot is 287, Entities with a Slot of 288, and 289 will move with the platform.

**Interval:**

Handles how long between the gas platform coming to a stop, and poping up next time.

**IntervalOffset:**

Sets the Time in the Interval you want to start at when the stage loads
