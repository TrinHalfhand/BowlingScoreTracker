/******************************
 		Summary
******************************/
This is a bowl score tracking API designed to keep tabs on each stage of 10 frame game. This can be 1 to 2 rolls per frame with a special exception of a potential 3rd roll in 10th frame. Upon a successful call to the functions either a message or json response will be returned. This will maintain the game for the session it is called within allowing for an future interfaceto expand the options to another layer and track multiple people's games.  

/****************************
 		Functions
****************************/
Start(): POST
- Success returns a 201 Created
- If a game is already in session a 409 Conflict will return
- When a game is not started or has completed (all 10 frames have played) a new game can begin.

Roll(roll): POST
- Posts one roll each time called placing it into a frame and calculating the current score.
- Success returns a 202 Accepted
- Failed validation returns a 400 Bad Request 
- Validates: 
	> roll between 0 and 10
	> total of first two rolls in the frame is between 0 and 10
	> game is active

ScoreByFrame(): GET
- Returns a collection of frames
- Success returns a JSON response
- Failed returns a 400 Bad Request 
- Response will contain:
	> Frames
		> Rolls: group of one to three rolls
		> ScoreTotal: score up to this frame
		> FrameNumber: bowling frame number
		> BonusType: None (blank), Strike, or Spare

TotalScore(): GET
- Returns the final current frame score or complete game score
- Success returns a JSON response
- Failed returns a 400 Bad Request 
- Response will contain:
	> TotalScore: Final frame score up to this point 

/******************************
	Future Options
******************************/
- Update a frame roll or rolls 
- Clear a frame but not the whole game
- Reset the entire game
- Added levels to track:
    > Each person's game
	> Alley number
	> Bowling team
	> Handycap
	> Special Rolls (turkey, perfect game)

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

Game Terminology
Spare: A spare is when the player knocks down all 10 pins in two tries. 
Strike: A strike is when the player knocks down all 10 pins on his first try.

