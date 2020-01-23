Breakable Wall
=======
**Bio:** 

Turns a section of a FG layer into a breakable wall. 

Attributes:
-------------

**Type**:

	* 0 – Breakable from the sides
	* 1 – Breaks from top
	* 2 - Breaks from the top in chunks, gives a score bonus
  * 2 - Breaks from the top in chunks, gives a score bonus
  * 4 - Breaks from bottom on contact (with bottom), breaks in chunks (two rows at a time) 
  * 5 - Breaks from bottom on contact (with bottom), breaks the entire selection of tiles

**OnlyKnux**:

Determines if its only brekable by knuckles

**OnlyMighty**:

Determines if its only brekable by mighty

**Priority**:

Defines which FG layer the platform will affect. 0=FG Low, 1=FG High.

**Size**:

The area of the breakable wall, measured in 16x16 squares

**NOTES**: 

When used in Maniac, the area of the white rectangle defines the collapsible platform.
Setting type to 3 or over seems to loop through the different types.
