using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//is it time for selecting a player class?
	public bool ClassSelectState;

	public int WinningScore;

	private int numberOfPlayers = 4; //replace 4 with a variable pulled from elsewhere later

	public GUIText CenterAnnouncementText;
	public GUIText PlayerScoreText_1;
	public GUIText PlayerScoreText_2;
	public GUIText PlayerScoreText_3;
	public GUIText PlayerScoreText_4;
	
	public int[] PlayerScores;

	public bool[] PlayersAlive;
	
	public GameObject Player_1;
	public GameObject Player_2;
	public GameObject Player_3;
	public GameObject Player_4;

	public Mesh[] PlayerClasses;



	// Use this for initialization
	void Start () {
		WinningScore = 10;

		PlayerScores = new int[numberOfPlayers]; //create array of player scores
		SetScoreText();

		PlayersAlive = new bool[numberOfPlayers]; //create array of whether players are alive

		PlayerClasses = new Mesh[numberOfPlayers];//create array of player meshes as classes, setup references to meshes.
		UpdatePlayerClassArray();

		SetAllPlayersAlive(); //set all players to be alive.

		CenterAnnouncementText.text = ""; //clear announcement text
		StartCoroutine(classSelectCountDown ());// start class select countdown, Coroutines are are for modelling behaviour over several frames


	}
	
	// Update is called once per frame
	void Update () {
	

	
	}

	public void setWhoDied(OuyaPlayer playerNumber)
	{
		int i = (int)playerNumber - 1; //figure out index number in Player Scores Array by subtracting one from the index number of Ouya Player enum
		PlayersAlive[i] = false; //player with index number i is dead.

		for ( i = 0; i < numberOfPlayers; i++) //
		{
			//Debug.Log ( "Player " + i + " is alive? " + PlayersAlive[i] + ".");
		}

		countDownConditionCheck(); //check if conditions are met for another countdown, this happens only when someone dies, and after a coundown finishes.
	}
	public void countDownConditionCheck()
	{
		//Debug.Log ("Countdown Condition Check.");

		if(checkWinningScoreReached())
		{
			Debug.Log ("win condition reached.");
			return;
		}
		Debug.Log("win condition not reached, continuing with countdown condition check");

		int numberPlayersAlive = 0;
		for ( int i = 0; i < numberOfPlayers; i++) //increment numberPlayersAlive for every player that's alive
		{

			if (PlayersAlive[i] == true)
			{
				numberPlayersAlive++;
			}
		}

		if (numberPlayersAlive <= 1) //start class selection countdown if there's only one left.
		{
			StartCoroutine(classSelectCountDown());
			Debug.Log ("Trying to start countdown.");
		}

		//Debug.Log ("Number of players alive: " + numberPlayersAlive);

		//check if players are the same class

		UpdatePlayerClassArray(); //update player class array so so the following has latest info.

		for (int i = 0; i < numberOfPlayers; i++)
		{
			//Debug.Log ("i: " + i);
			for (int j = i; j < numberOfPlayers; j++) //check for same player classes
			{
				bool twoPlayersSame = PlayerClasses[i].ToString().Equals(PlayerClasses[j].ToString());
				//Debug.Log("Player " + i + " and Player " + j + " are the same.");
				//Debug.Log (" i: " + i + " j: " + j);
				if (twoPlayersSame == true 
				    && i != j 
				    && numberPlayersAlive == 2 
				    && PlayersAlive[i] == true 
				    && PlayersAlive[j] == true)//found a matching pair, not matched to itself, both alive and two players left
				{

					StartCoroutine(classSelectCountDown());//start count down
				}

				if (twoPlayersSame == true 
				    && i != j 
				    && numberPlayersAlive > 2 
				    && PlayersAlive[i] == true 
				    && PlayersAlive[j] == true) //what happens if they match but there's more than two alive.
				{

					for (int k = j; k < numberOfPlayers; k++) 
					{
						//Debug.Log (" j: " + j + " k: " + k);
						bool thirdPlayerSame = PlayerClasses[j].ToString().Equals(PlayerClasses[k].ToString()); //check for more matching if more than 2 alive.

						if (thirdPlayerSame == true 
						    && k != j 
						    && numberPlayersAlive == 3 
						    && PlayersAlive[i] == true 
						    && PlayersAlive[j] == true
						    && PlayersAlive[k] == true //if there's three alive and they're all the same
						    )
						{
							StartCoroutine(classSelectCountDown());//start count down
						}

						if (thirdPlayerSame == true 
						    && k != j 
						    && numberPlayersAlive > 3 
						    && PlayersAlive[i] == true 
						    && PlayersAlive[j] == true
						    && PlayersAlive[k] == true) //what happens if three match but there's more than three alive!!!
						{
							for (int l = k; l < numberOfPlayers; l++)
							{
								bool fourthPlayerSame = PlayerClasses[l].ToString().Equals(PlayerClasses[k].ToString());

								//Debug.Log("number of players: " + numberOfPlayers);

								if (fourthPlayerSame == true
								    && k != l
								    && numberPlayersAlive == 4
								    && PlayersAlive[i] == true 
								    && PlayersAlive[j] == true
								    && PlayersAlive[k] == true 
								    && PlayersAlive[l] == true) //all four are the same class, start another countdown!
								{
									//Debug.Log("i:" + i + "j:" + j + "k:" + k + " l: " + l);
									//Debug.Log("4 of same");
									StartCoroutine(sameFourClassesAnnouncment());//start count down
								}

							}

						}

					}
				}
			}
		}
		

	}
	public void addToPlayerScore(OuyaPlayer playerNumber, int additionalScore) //add the player score when given player number and how much to add
	{
		//Debug.Log ("this is the index value of enum passed into add player score" + (int)playerNumber);
		int i = (int)playerNumber - 1; //figure out index number in Player Scores Array by subtracting one from the index number of Ouya Player enum
		PlayerScores[i] = PlayerScores[i] + additionalScore; //add additional score to existing score
		SetScoreText();
	}

	IEnumerator classSelectCountDown()
	{
		//Debug.Log ("Countdown script starting.");
		yield return new WaitForSeconds(1.0f);

		ClassSelectState = true; //turn on class selection
		SetAllPlayersAlive();
		CenterAnnouncementText.text = "Rock!";
		yield return new WaitForSeconds(1.0f);
		CenterAnnouncementText.text = "Paper!";
		yield return new WaitForSeconds(1.0f);
		CenterAnnouncementText.text = "Scissors!";
		yield return new WaitForSeconds(1.0f);
		CenterAnnouncementText.text = "Go!";
		yield return new WaitForSeconds(1.0f);
		CenterAnnouncementText.text = "";

		ClassSelectState = false; //turn off class selection
		yield return new WaitForSeconds(1.0f);
		countDownConditionCheck(); //check if conditions are met for another countdown, this happens only when someone dies, and after a coundown finishes.
	}

	IEnumerator sameFourClassesAnnouncment()
	{
		if (PlayerClasses[0].ToString() == "Rock Instance (UnityEngine.Mesh)")
		{
			CenterAnnouncementText.text = "You all chose rock!";
		}
		if (PlayerClasses[0].ToString() == "Paper Instance (UnityEngine.Mesh)")
		{
			CenterAnnouncementText.text = "You all chose paper!";
		}
		if (PlayerClasses[0].ToString() == "Scissors Instance (UnityEngine.Mesh)")
		{
			CenterAnnouncementText.text = "You all chose scissors!";
		}
		yield return new WaitForSeconds(1.0f);
		CenterAnnouncementText.text = "";
		yield return new WaitForSeconds(1.0f);
		StartCoroutine(classSelectCountDown()); //start count down after telling what happened.
	}

	void SetScoreText()
	{
		PlayerScoreText_1.text = "Player 1 Score:" + PlayerScores[0].ToString();
		PlayerScoreText_2.text = "Player 2 Score:" + PlayerScores[1].ToString();
		PlayerScoreText_3.text = "Player 3 Score:" + PlayerScores[2].ToString();
		PlayerScoreText_4.text = "Player 4 Score:" + PlayerScores[3].ToString();
	}

	void UpdatePlayerClassArray()
	{
		PlayerClasses[0] = Player_1.GetComponent<MeshFilter> ().mesh;
		PlayerClasses[1] = Player_2.GetComponent<MeshFilter> ().mesh;
		PlayerClasses[2] = Player_3.GetComponent<MeshFilter> ().mesh;
		PlayerClasses[3] = Player_4.GetComponent<MeshFilter> ().mesh;
	}

	void SetAllPlayersAlive()
	{
		for ( int i = 0; i < numberOfPlayers; i++) //set all players to be alive initially.
		{
			PlayersAlive[i] = true;//Debug.Log ( "Player " + i + " is alive?" + PlayersAlive[i] + ".");
			Player_1.SetActive(true);
			Player_2.SetActive(true);
			Player_3.SetActive(true);
			Player_4.SetActive(true);
		}
	}

	void SetAllPlayersDead()
	{
		for ( int i = 0; i < numberOfPlayers; i++) //set all players to be alive initially.
		{
			PlayersAlive[i] = false;//Debug.Log ( "Player " + i + " is alive?" + PlayersAlive[i] + ".");
			Player_1.SetActive(false);
			Player_2.SetActive(false);
			Player_3.SetActive(false);
			Player_4.SetActive(false);
		}
	}

	bool checkWinningScoreReached()
	{
		for ( int i = 0; i < numberOfPlayers; i++)
		{
			if (PlayerScores[i] >= WinningScore)
			{
				SetAllPlayersDead(); //disable all players
				StartCoroutine(setWinnerText(i)); //announce winner
				return true;
			}
		}

		return false; //nobody won yet carry on.
	}

	IEnumerator setWinnerText(int i)
	{
		CenterAnnouncementText.text = "Player " + (i + 1) + " wins!"; //set win text
		yield return new WaitForSeconds(4.0f);
		CenterAnnouncementText.text = "";
	}

}
