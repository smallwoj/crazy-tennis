// Keeps track of and displays the player's score

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringSystem : MonoBehaviour
{
    public Text scoreTextbox;   // The textbox that will display the score

    private int score = 0;      // The score to be displayed

    // Constants specifying how much to add to the score given certain events
    private static readonly int
        BALL_NEAR_HIT = 1,  // The player almost gets hit by the ball
        BALL_HIT = 1,       // The player hits the ball
        OPPONENT_HIT = 1,   // The player hits an opponent
        PHASE_CLEAR = 1,    // The player beats one of the opponent's phases
        OPPONENT_BEAT = 1;  // The player beats the opponent
    private static readonly int DIGITS = 6; // How long the displayed score
                                            // should be

    // Start is called when the... object is created?
    // You'd think I'd remember the usual comment
    public void Start()
    {
        scoreTextbox.text = score.ToString().PadLeft(DIGITS, '0');
    }

    // Scoring methods! As of right now, OpponentHit is the only one that's
    // called anywhere else in the code.
    // To call one of these from any other class, use the following line:
    // GameObject.FindGameObjectWithTag("Score").GetComponent<ScoringSystem>().whatevermethodyouwantedtocall();
    public void BallNearHit()
    {
        score += BALL_NEAR_HIT;
        scoreTextbox.text = score.ToString().PadLeft(DIGITS, '0');
    }
    public void BallHit()
    {
        score += BALL_HIT;
        scoreTextbox.text = score.ToString().PadLeft(DIGITS, '0');
    }
    public void OpponentHit()
    {
        score += OPPONENT_HIT;
        scoreTextbox.text = score.ToString().PadLeft(DIGITS, '0');
    }
    public void PhaseClear()
    {
        score += PHASE_CLEAR;
        scoreTextbox.text = score.ToString().PadLeft(DIGITS, '0');
    }
    public void OpponentBeat()
    {
        score += OPPONENT_BEAT;
        scoreTextbox.text = score.ToString().PadLeft(DIGITS, '0');
    }
}
