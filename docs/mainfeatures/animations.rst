Animation Editing
=========================================

so you've opened the RSDK Animation Editor and you're wondering what all these weird values mean, well worry no more as they'll all get explained here!

Animation Values
------------

**Name:** The name of the animation

**Speed:** Determines the speed of the animation (the greater the value the faster the speed)

**Loop index:** Determines which frame the animation loops from

**Rotation Flags:** Determines the rotation style of the selected animation


Frame Values
------------

**Texture:** The spritesheet from which the selected frame is taken from

**X:** Determines where in the spritesheet the frame XPos starts at

**Y:** Determines where in the spritesheet the frame YPos starts at

**Width:** Determines how wide the frame is

**Height:** Determines how tall the frame is

**Pivot X:** Determines the X offset from the frame (Most sprites' pivot X is: negative width divided by 2)

**Pivot Y:** Determines the Y offset from the frame (Most sprites' pivot Y is: negative height divided by 2)

**Duration:** Determines how long the frame plays for

**Id:** (Decorations) Determines the layer of the selected frame.

Hitbox Values
------------

**Hitbox:** What hitbox you're editing the values of (each frame has it's own set of values per hitbox)

**Left:** Determines the Hitboxes X offset from the center of the sprite

**Right:** Determines the Hitboxes width from the center of the sprite

**Top:** Determines the Hitboxes Y offset from the center of the sprite

**Bottom:** Determines the Hitboxes height from the center of the sprite


Rotation Flag Types
------------

**Default Behavior:** no rotation

**Full Engine rotation:** Complete rotation (every 1 degree)

**45 degree rotation:** Rotates every 45 degrees

**Static rotation using extra frames:** rotates every 90 degrees (Older RSDK versions use this flag when using pre-drawn rotation frames)

**1/2 Engine rotation:** rotates every 180 degrees

**90 degree rotation:** weirdly rotates every 90 degrees (ex. for decorations with direction=0, animation will rotate up, down, right, left, down, up, left, right)

**135/255 degree rotation:** Static 135 rotation.(decorations: depends on direction: 0= 255 1=135)

**just 135 degree rotation:**Static 135 rotation.(decorations: depends on direction: 0= 255 1=135)
