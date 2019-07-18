/******************************
 	Summary
******************************/
This is a bowl score tracking API designed to keep tabs on each stage of 10 frame game. This can be 1 to 2 rolls per frame with a special exception of a potential 3rd roll in 10th frame. This will be an API and return very generic responses with standard API error messages from the 200 (success) and 400 (exception) groups. Body of the message if a data request will contain json with all of the model data needed contained in it's contents. 

Bowling Terminology
Spare: A spare is when the player knocks down all 10 pins in two tries. 
Strike: A strike is when the player knocks down all 10 pins on his first try. 
Turkey: Three strikes in a row

In review:
?	Should this application track multiple players? Future.
?	Should it track the person's name for the display output? Future.
?	Will this be session based or need to cache? Currently it is while the api is active.

/****************************
 	Functions
****************************/
Start(): POST only
- Success returns a 201 Created

In review:
?	Should this erase the scores tracked up to this point? Currently does.
?	Should include a validation to insure it does not erase mid game? Currently it does.
		If yes what error code would be ideal? Currently it uses a 409 Conflict

Roll(numberRolled): POST only
- Current design will take one number for each time called
- Success returns a 202 Accepted + status (last of the game?)
- Failed validation returns a 400 Bad Request 
         (roll > 10, roll < 0 or total of first two rolls > 10)

In review:
?	Should this take more numbers then one per call?

ScoreByFrame(): GET only
- Returns a collection of frames
- Model will have:
	1) Rolls in an array of one to three rolls that occurred during the frame
	2) Total = scoring up to this frame
	3) FrameNumber indicating what frame it's on
	4) BonusType = if the frame has a spare or strike in it which effects scoring.

In review:
?	Data Structure format; do you prefer something generic to allow other languages to use this function, is there a specific format you had in mind?
?	Would we ever want to know the score of a different frame? Another Player?
?	Should there be a special mention when a person has three strikes in the 10th frame (Turkey)?

TotalScore(): GET only
- Returns the final current frame score or complete game score

In review:
?	Do I need to include an identity of who's total score 
?	Should this just return the total score?
?	Should there be a special mention when a person have scored a perfect game (300)?

Other options
In review:
?	Will there be a need to update a frame roll(s)? 
?	Clear a frame but not the whole game?

/****************************
 	Scoring
****************************/
Main Rules
1.	Game consists of 10 frames 
2.	Each frame the player has two opportunities to knock down 10 pins. 
3.	Total => total number of pins knocked down, plus bonuses for strikes and spares.

Bonus Points
1.	Bonus for Spare => Add next roll to the frame total of the previous frame
2.	Bonus for Strike => Add next two rolls to the frame total of the previous frame 
	Examples: 
		Last two frames were both strikes bonus added is 20 to the current frame
		Previous frames is a spare bonus added is 10 to the current frame
 
10th Frame
In the tenth frame a player who rolls a spare or strike is allowed to roll the extra balls to complete the frame. However no more than three balls can be rolled in tenth frame.
- Up to 3 strikes, a spare and one more roll, or just two rolls could be bowled
- The bonus is applied from the previous frame(s)
	Examples:
		Three strikes = 30 points + prevous frame(s) bonus
		Spare + Strike = 20 points + prevous frame(s) bonus
		Spare + Final Roll = 10 points + Final Roll + prevous frame(s) bonus
		Two Rolls = Roll Total + prevous frame(s) bonus
