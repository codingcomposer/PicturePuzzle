# PicturePuzzle

This is a jigsaw-ish puzzle game.
From an any given input sprite, it generates the seperate sprites according to the set column / row counts and shuffles them.
To test the game, Go to the 'Play' scene and hit the play button.
The pieces at the right position are shown as the normal brightness, while the pieces at wrong positions are shown brightly.
The 'right' pieces are immovable, and you can only move 'wrong' pieces. Select two wrong pieces to swap them.
If one got correct, its color will get normal. Once every piece is at the right position, the game shows the clear message.
To make the sprite colors get brighter, I've modified the original sprites-default shader to be an additive-shader.
