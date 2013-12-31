using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//is it time for selecting a player class?
	public bool ClassSelectState;



	private int numberOfPlayers = 4; //replace 4 with a variable pulled from elsewhere later

	public GUIText PlayerScoreText_1;
	public GUIText PlayerScoreText_2;
	public GUIText PlayerScoreText_3;
	public GUIText PlayerScoreText_4;
	
	public int[] PlayerScores;

	public bool[] PlayersAlive;

	public enum PlayerClass {Rock = 0, Paper = 1, Scissor = 2};
	//this is where i left off



	// Use this for initialization
	void Start () {
		PlayerScores = new int[numberOfPlayers]; //create array of player scores
		SetScoreText();

		PlayersAlive = new bool[numberOfPlayers]; //create array of whether players are alive
		for ( int i = 0; i < numberOfPlayers; i++) //set all players to be alive initially.
		{
			PlayersAlive[i] = true;//Debug.Log ( "Player " + i + " is alive?" + PlayersAlive[i] + ".");
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	playersLeftCheck();
	}

	public void setWhoDied(OuyaPlayer playerNumber)
	{
		int i = (int)playerNumber - 1; //figure out index number in Player Scores Array by subtracting one from the index number of Ouya Player enum
		PlayersAlive[i] = false; //player with index number i is dead.
		/*
		for ( i = 0; i < numberOfPlayers; i++) //
		{
			Debug.Log ( "Player " + i + " is alive?" + PlayersAlive[i] + ".");
		}
		*/
	}
	public void playersLeftCheck()
	{
		int numberPlayersAlive = 0;
		for ( int i = 0; i < numberOfPlayers; i++) //increment numberPlayersAlive for every player that's alive
		{

			if (PlayersAlive[i] == true)
			{
				numberPlayersAlive++;
			}
		}
		if (numberPlayersAlive <= 1) //start class selection countdown if 
		{
			classSelectCountDown();
		}
	}

	void classSelectCountDown()
	{
	}


	public void addToPlayerScore(OuyaPlayer playerNumber, int additionalScore) //add the player score when given player number and how much to add
	{
		//Debug.Log ("this is the index value of enum passed into add player score" + (int)playerNumber);
		int i = (int)playerNumber - 1; //figure out index number in Player Scores Array by subtracting one from the index number of Ouya Player enum
		PlayerScores[i] = PlayerScores[i] + additionalScore; //add additional score to existing score
		SetScoreText();
	}

	void SetScoreText()
	{
		PlayerScoreText_1.text = "Player 1 Score:" + PlayerScores[0].ToString();
		PlayerScoreText_2.text = "Player 2 Score:" + PlayerScores[1].ToString();
		PlayerScoreText_3.text = "Player 3 Score:" + PlayerScores[2].ToString();
		PlayerScoreText_4.text = "Player 4 Score:" + PlayerScores[3].ToString();
	}
}
