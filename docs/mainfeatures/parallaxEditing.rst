Parallax Editing
=========================================

+ STEP 1: 
Make your background. Self Explanatory. You'll have to have your background set up and organized into Maniac editor before you can start editing parallax. Remember to make your background loop properly for the game, as in, when the screen scrolls all the way to the right end of your background, it can repeat itself again without looking cut off. We want it as clean as possible!

+ STEP 2: 
Open up the layer manager. You can find it under the scenes tab at the top. If you're saying "it's not there", you probably haven't downloaded the latest version, so go do that now. If you're saying "There's no Scenes option", when the hell was the last time you updated your Maniac editor? are you using a version from like, November 2017 or something?

+ STEP 3: 

Now it gets complicated. Alright, so select the background layer in the right tab of the new window that just popped up. On the right, you'll be seeing a bunch of scary looking numbers!!! Oh no!! Don't fret though, my friend, because I'll explain to you what these numbers are. First things first, disregard the stuff in the middle. That's a topic for another day, this tutorial merely covers basic parallax editing. The numbers on the right are what we're focusing on. Here's what each one means:

    * Behavior: 
    Effects vary from zone to zone. Here’s an example with Chemical Plant:
    
    https://giant.gfycat.com/ScentedSpanishDromaeosaur.webm
    
    * DrawOrder: 
    We don't know for sure what this does exactly, but we assume something like what layer it's on?
    
    * Relative Speed: 
    Finally, something interesting! Relative Speed determines how fast rows defined by a horizontal rule (more on that later) scroll when the screen moves. The higher the number, the faster it scrolls.
    
    * Constant Speed: 
    Same as the above, except it determines how fast rows defined by a horizontal rule move on their own. Useful for stuff like clouds or moving setpieces (like the trains in Studiopolis Act 1). Keep in mind this is not either/or, if you leave your Relative Speed value blank, it will just be a static image moving across the screen, which looks very not good.


+ STEP 4: 
Ok, but what's a "horizontal rule"? Simply put, horizontal rules determine what rows are affected by parallax and which values they use. Each one has their own set of values, as well as their own "horizontal mappings". These dictate Which rows have the relative and constant speed values entered. But how exactly do we figure out what a "row" is? Well, essentially, a row is a single, horizontal row of pixels in a layer. To put it in simpler terms, one 16x16 tile (the very same ones you used to create your background in maniac) is made up of 16 rows of pixels. I'll place an image at the bottom of the tutorial to provide a visual explanation for those of you still scratching your heads.
+ STEP 5: 
Where making this happen With that out of the way, let's go over how to assign parallax values to rows. In the Horizontal Mapping section there are two values: the Start Line and the Line Count. Don't be deceived by the Start Line value, it doesn't include the row you set it to itself, only the ones immediately after. So if for example, you wanted to assign a specific parallax value to, say, the first three rows in the layer, you would set the start line to zero, because setting it to 1 would make the game assign the values to rows 2, 3, and 4, rather than 1, 2, and 3. Then there's the Line Count. This tells the game how many lines after the start line are going to be given this specific value. While you can technically set this to any number you want, usually most backgrounds won't be set up for this kind of scrolling. If you're just starting out I recommend splitting up your rows into chunks of 16 (AKA one whole row of tiles) to make your life easier.(edited) 

STEP 6: 
Some other tips and tricks for cool kids such as yourself

- You'll typically want your rows to have a higher relative speed value the lower down they are on the layer. Think of it like looking out the window while you're in the backseat of the car. All the stuff close to you whizzes by while stuff in the distance moves much slower. It's the same basic idea with mania's backgrounds.
- Always make sure you do your math correctly! If you don't your background might become messed up or you might not even be able to save at all due to overflow.
- As long as you know what you're doing, don't be afraid to save your progress and then test it out in-game, to see how it looks. And remember, always make a backup. For God's sake, always make a backup!
- It turns out you can set relative and constant speeds to negative values, which will make the rows instead move in the opposite direction.
- I’m sure this is common knowledge, but please, don’t mess with the relative or constant scrollspeeds of foreground layers (specifically fg high and fg low). Setting the Relative speed to anything other than the game’s default 256 and the Constant Speed to anything higher than 0, shit gets pearshaped real fast.



