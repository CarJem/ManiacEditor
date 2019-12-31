Water
=======
**Bio:** 

Sonic’s Greatest Weakness and the Most Deadly Hazard. Comes in Many Different Variations. Some of which have functions that are specifically stage locked.

Attributes
-----------

**Type:** 

Determines the Type of Water that is being used.

    * 0 - Initial Water Height Position Marker; 
	Uses CPZ/HCZ/AIZ Style Water; Starting Height Based on Entity Position (not the values it has)
	
    * 1 - Bounded Water Box; GHZ Style Water
    * 2 - Water Bubbler
    * 3 - Bounding Boxes for Adjusting Water Height.
    * 4 - Water Bubbler
    * 5 - ???
    * 6 - Water Splash
    * 7 - Bubble
    * 8 - CountDown Bubble


**NumDuds:** 

???

**Size:**  

Used for type 1, for the water box itself, and type 3 for the area in which the player can activate the water height adjustment.

**Height:** 

Used in type 3 for setting the water height. In a CPZ/Normal setup, the water level can go up and down between the x and y values of this attribute. In a HCZ however, it will not bob and stay at the level defined by either x or y.

**Speed:** 

For Type 2, Must be higher than 0 for the water to able to change height, the larger it is, the faster the water will move into position.

**Button Tag:** 

Used in HCZ often, with a type 2, this will make the object un-intractable unless the button tag is triggered with a “Button” with the same “tag”.

**R, G, and B:** 

For type 1 only, sets the color of the water, with RGB being all 0 being black, and RGB being all 255 being transparent.

**Priority:** 

Determines the layer the water is Interactable with; The outcome of the value depends on Layer Setup of your scene. 

**Destroy on Trigger:** 

Removes the object once the “trigger” from the “Button” to the “tag” we have set has been sent.
NOTES: CPZ/HCZ/AIZ Style Water CAN NOT be given a custom water pallet outside of those stage setups. They are hard locked to those stage setup entities. Attempting to use these in zones without water palettes will cause the water to appear transparent (AKA No palette adjustment).
