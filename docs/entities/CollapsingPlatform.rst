Collapsing Platform
=======
**Bio:** 

Turns a section of a FG layer into a collapsible platform. 

Attributes:
-------------
**Size**:

The area of the collapsible platform, measured in 16x16 squares

**Respawn**:

Defines whether the platform will respawn after going off screen. Set as True/False

**TargetLayer**:

Defines which layer the platform will affect. Experiment with different values to get the correct layer setting. 0=FG Low, 1=FG High, 2 etc

**Type**:

	* 0 – Collapses from left to right
	* 1 – Collapses from right to left
	* 2 - Collapses from centre outwards
	* 3 - Collapses from from left if player is left of the center, or right if player is right of the center
	* 4 - Collapses from from left if player is left of the center, or right if player is right of the center, or center if player is in the center area

**Delay**:

Time delay before platform collapses. 60 = 1 second, 90 = 1.5 seconds, you do the math.

**EventOnly**:

Determines if it collapses upon a stage event

**MightyOnly**:

Determines if its only Collapsable by mighty


**NOTES**: 

When used in Maniac, the area of the white rectangle defines the collapsible platform.
Setting type to 3 or over seems to loop through the different types.
