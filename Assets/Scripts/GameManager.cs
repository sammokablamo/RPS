using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//is it time for selecting a player class?
	public bool ClassSelectState;

	public GUIText PlayerScoreText_1;
	public GUIText PlayerScoreText_2;
	public GUIText PlayerScoreText_3;
	public GUIText PlayerScoreText_4;

	private int numberOfPlayers = 4; //replace 4 with a variable pulled from elsewhere later
	
	public int[] PlayerScores;


	// Use this for initialization
	void Start () {
		PlayerScores = new int[numberOfPlayers];
		SetScoreText();
	}
	
	// Update is called once per frame
	void Update () {

		Debug.Log ( "Player 1:" + PlayerScores[0]);
		Debug.Log ( "Player 2:" + PlayerScores[1]);
		Debug.Log ( "Player 3:" + PlayerScores[2]);
		Debug.Log ( "Player 4:" + PlayerScores[3]);

	}



	public void addToPlayerScore(OuyaPlayer playerNumber, int additionalScore) //add the player score when given player number and how much to add
	{
		if ( playerNumber == OuyaPlayer.P01 ) //this is how you compare enum values.
		{
			PlayerScores[0] = PlayerScores[0] + additionalScore;
			SetScoreText();
		}
		if ( playerNumber == OuyaPlayer.P02 )
		{
			PlayerScores[1] = PlayerScores[1] + additionalScore;
			SetScoreText();
		}
		if ( playerNumber == OuyaPlayer.P03 )
		{
			PlayerScores[2] = PlayerScores[2] + additionalScore;
			SetScoreText();
		}
		if ( playerNumber == OuyaPlayer.P04 )
		{
			PlayerScores[3] = PlayerScores[3] + additionalScore;
			SetScoreText();
		}

	}

	void SetScoreText()
	{
		PlayerScoreText_1.text = "Player 1 Score:" + PlayerScores[0].ToString();
		PlayerScoreText_2.text = "Player 2 Score:" + PlayerScores[1].ToString();
		PlayerScoreText_3.text = "Player 3 Score:" + PlayerScores[2].ToString();
		PlayerScoreText_4.text = "Player 4 Score:" + PlayerScores[3].ToString();
	}
}
