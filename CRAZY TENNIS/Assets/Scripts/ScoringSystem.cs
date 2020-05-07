// Keeps track of and displays the player's score

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringSystem : MonoBehaviour
{
    /// <summary> The textbox that will display the score </summary>
    public Text scoreTextbox;

    /// <summary> Reference to the crowd, used to make them cheer when certain scoring events occur </summary>
    public CrowdBehaviour crowd;
    
    /// <summary> The score to be displayed </summary>
    private int score = 0;

    // Point values: Constants specifying how much to add to the score given certain events
    private static readonly int
        BALL_NEAR_HIT = 1,  // The player almost gets hit by the ball
        BALL_HIT      = 1,  // The player hits the ball
        OPPONENT_HIT  = 1,  // The player hits an opponent
        PHASE_CLEAR   = 1,  // The player beats one of the opponent's phases
        OPPONENT_BEAT = 1;  // The player beats the opponent
    /// <summary> The maximum amount that the score can increase by in one go. 
    /// Used to gauge how much the crowd should cheer at each action </summary>
    private static readonly int MAX_POINT_VALUE = Mathf.Max(BALL_NEAR_HIT, BALL_HIT, OPPONENT_HIT, PHASE_CLEAR, OPPONENT_BEAT);
    /// <summary> How long the displayed score should be </summary>
    private static readonly int DIGITS = 6;

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
        crowd.Cheer((float)BALL_HIT / MAX_POINT_VALUE);
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
