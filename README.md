unity game project Balgos from 2026 (active development). top down game where you shoot monsters, main gimmick is that you cant see your surroundings,
instead the player and other gameobjects produce noises which are simulated as they travel through space. this noise is then displayed to the player 
allowing them to "see through sound"

requirements :
  - OS that can run unity editor.
  - unity installed on machine to run the editor.
  - a unity version to run the project in.

install instructions :
    - download repository and unzip.
    - open unity hub and click add then add project from disk.
    - navigate and select unzipped repository.
    - you will be prompted for a version to open it in, selecting missing version or latest LTS version will work best.
    - wait for editor to load and enjoy.

gameplay notes :

the player can move(WASD), look around(mouse), echolocate(RMB)(generate a noise) and fire their weapon(LMB). once the weapon is fired it
needs to be reloaded which is done by activating the lever on the side of the weapon(Q) reloading(E) and then activating the lever again(Q)

sound grid / audio visual system : 
  - the crux of the game idea and mechanics is in its sound grid, a grid is overlayed across the map which holds a set of positions throughout
    the game world, each position has a set of values on where it is, how much noise is at that spot and the values that dictate how noise
    spreads throughout the game world.

  - at regular intervals every active node in the grid is iterated and sound decays and spreads according to the values at each given position,
    through unities job system this expensive task can be run efficiently on most machines, even low spec ones.

  - objects interact with the grid in 3 main ways, they either write a decibel value to a position creating a noise, they read a position on the
    grid and check for noise (typically as a trigger to a further action) or they write a medium to the grid.

mediums : 
  - in order to simulate and display walls and other objects to the player mediums are deployed, these are objects with a collider and a preset
    list of values which are written to the grid, through this a shape can be imprinted onto the grid where sound decays faster or spreads slower
    creating visible variation and allowing the player to see that something is present.

  - mediums can be paired with a second script which routinely triggers an update to the grid allowing moving objects like players and enemies to
    appear as they move through space.

player controller : 
  - the player is visible via a active medium which allows the player to see where they are, the gun is also visible allowing the player to
    recognise if the lever is activated or if they are in need of a reload as the weapon will reflect these states with a lever protruding
    from the side of the weapon in the former case and the back of the gun extending to show the latter.
  
  - to assist in legibility the player produces both inside and outside of the grid to accompany the actions they take such as walking or reloading.
  - there are future intentions to overhaul reloading to fit desired game feel as development continues.

enemies : 
  - enemies are being worked on, they will navigate their enviroment, detect sounds and attack the enemy either by charging head first or trying to sneak        around and flank the player.
  - currently the enemies will most likely kill the player instantly as displaying health through the sound grid would be difficult but this may change
    as gameplay evolves.

please direct all inquiries, questions and problems to ruled.dev@gmail.com
