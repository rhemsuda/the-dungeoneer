# the-dungeoneer
Procedural generated dungeon crawler. Explore dungeons, fight monsters, and collect keys to make your way to the final boss.

### How to run
1. Open Build folder
2. Click `dungeoneer.exe` to play

### Controls
WASD - Movement controls\n
SHIFT - Run\n
1 - Basic attack\n
2 - Jump attack (High critical rate)\n
3 - Spin attack (Area of Effect)\n
P - Use potion\n
F - Interact\n

### Background
To strengthen my knowledge of procedural design and game development, I challenged myself to make a clone of an aspect of a popular game I used to play called Runescape. The Dungeoneer was modelled after Runescapes Dungeoneering skill, which allows the user to traverse through randomly generated dungeons with random keys and doors. I took this a little further and focused on supporting different sized rooms that can be linked together like puzzle pieces. The project took longer than expected, and there were lots of constraints and problems that came up that delayed development and caused a slightly late arrival. Below I will explain some of the constraints I had to overcome, and some of the areas I learned the most from this assignment.

### Procedural Dungeon Creation
The first hurdle was to create a system that would allow multiple pieces to be seamlessly put together, and allow perfect flow from one end of the dungeon to another with no knowledge that the individual rooms were put together. There were a couple of design decisions I had to make in order to support this. My doors are a uniform size of 4x4 units, created in blender. Each wall has both a wall with an opening, and a solid wall. When building, the rooms are aligned by their connectors using some matrix math, and then when placed, the solid wall is removed leaving the room with the opening. A big part of getting this to work was in actually designing the assets in Blender, but coming up with the algorithm to rotate and place the rooms so the connectors meet up was equally as challenging.

### Procedural Placement of Keys and Doors
By far the most difficult part of this assignment was the key and door placement. The goal here was to be able to place one key with a corresponding door (by color) and have them always able to reach the end. The problem is that we do not have an even flow from one end to the other, and any number of rooms can branch into any number of other rooms, so ensuring all keys are accessible is a challenge. After many different attempts I ended up running an algorithm which checks for each key placement, whether it can still reach the start room and can still reach the player, and will recursively check the same thing when it runs into a room with another door. This was very tricky as I needed to ensure I didn’t go back through the same door when checking. I ended up treating the rooms as nodes and implementing a breadth-first search on them with some checks to ensure we don’t go through rooms from the previously called function. (See the implementation at DungeonCreator:PlaceKeysAndDoors)

### Enemy AI
The game was pretty boring without some monsters to kill, so my next challenge was to create a basic combat system where enemies are spawned at a point, roam around that point until a player comes, chases the player and attacks. I implemented an FSM to handle switching between AI states, which was all tied into the navigation agent running the movement of the enemy. I had some good opportunities to work with the Unity Animation Event system, which allowed me to create a magic enemy that realistically throws magic balls, which was something I hadn’t done before. Overall the Enemy AI was more of a fun process, and I was surprised at how well it clicked. I am interested to see how I can expand how I’ve written so far to create more in-depth AI systems.

### Dynamic Navigation Mesh Linking
Something I hadn’t thought about until I tried to implement Enemy AI and my procedural scene. This is what caused me all of my problems at the end of the assignment, and why I can really appreciate integration and regression testing. Because the NavMesh in Unity is natively a “build-time” only operation done through baking, there was no direct support for what I needed to do. Since the rooms are each added and combined into one single mesh, this process would need to be done dynamically, so I needed to start doing some research. After a bit of digging I found the NavMeshComponents Library written by someone on the Unity team and provided as an API to solve this exact problem. Once I had gotten the scripts implemented, there was a lot of figuring out to do because it isn’t very well documented. Most of it was fiddling with things in the editor to make sure that the links were in the right place when new rooms were placed, and that they were only enabled when valid passageways were available (doors open not closed). Once this worked, everything started to come together and having monsters spawned in different rooms of my dungeon was the best feeling ever.
