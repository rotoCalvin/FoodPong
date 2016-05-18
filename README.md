# FoodPong

This project runs out of Unity and utilizes OpenCV and LeanTween

## Running the Activity

Press "Spacebar" on the keyboard and click the top-left, top-right, bottom-left, and bottom-right corners of the play screen.  This uses homograpy to shift the perspective of the screen, making it easier to find and set pixels.

Pressing "C" on the keyboard start the game without doing any camera calibration.

## Controls

These should be easy to lookup in the GameManager.cs file, but here are some basics.

### Keyboard Shortcuts

- "Z": Start calibration for a "greyscaleThreshold value"
- "X": Starts the activity in a mode where a user can walk up to the screen to start the game
- "C": Starts the game
- "O": Turns all modes off

- "Spacebar": toggles video feed
- "L": toggles Log visibility

- "N": lowers the value of the "greyscaleThreshold value"
- "M": raises the value of the "greyscaleThreshold value"

- "Q": Start the food dropping
- "A,B,P": Different Testing methods
