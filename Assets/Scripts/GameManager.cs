using UnityEngine;
using System.Collections;
using System; //adding this for sorting

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

	public GameObject[] PlayerGameObjectsArray;

	public GameObject[] SpawnPointsArray; //create an array for spawn points

	int numberPlayersAlive; //number of players alive

	//declare variables for dead player repositioning
	float[] SpawnPointDistances;
	float[] SortedSpawnPointDistances;
	public GameObject[] LivePlayerGameObjects;
	GameObject[] DeadPlayerGameObjects;
	Vector3 LivePlayerAverageLocation;

	public int[] PlayerPillCounts; //pill counting array

	public float baseMoveSpeed; //players will follow this base move speed
	public float maxMoveSpeed;//all players will hit this max move speed
	public float moveSpeedDissipationRate;//all players will hit this max move speed

	public float speedIncrementPerPill;
	public float pillRespawnTime;


	private GameObject[] liveRockPlayers;
	private GameObject[] livePaperPlayers;
	private GameObject[] liveScissorPlayers;

	//round timer variables
	public float roundTime;

	//audio references
	public AudioClip lowToneClip;
	public AudioClip highToneClip;
	private enum GlobalAudioFX {lowToneAudio, highToneAudio};




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

		//add all spawnpoints to spawn points array

		SpawnPointsArray = GameObject.FindGameObjectsWithTag("SpawnPoint"); //setup spawn points references

		PlayerGameObjectsArray = GameObject.FindGameObjectsWithTag("Player"); 
		     


	}
	
	// Update is called once per frame
	void Update () {
	

	
	}


	public void countDownConditionCheck()
	{
		//Debug.Log ("Countdown Condition Check.");

		if(checkWinningScoreReached())
		{
			Debug.Log ("win condition reached.");
			return;
		}
		//Debug.Log("win condition not reached, continuing with countdown condition check");

		checkNumberOfPlayersAlive();

		if (numberPlayersAlive <= 1) //start class selection countdown if there's only one left.
		{
			relocateDeadPlayers(); //relocate dead players to spawn points
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
					relocateDeadPlayers(); //relocate dead players to spawn points
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
							relocateDeadPlayers(); //relocate dead players to spawn points
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
									//don't relocate dead players to spawn points because there are none.
									StartCoroutine(sameFourClassesAnnouncment());//start count down
								}

							}

						}

					}
				}
			}
		}
		

	}

	void checkNumberOfPlayersAlive ()
	{
		numberPlayersAlive = 0;
		for (int i = 0; i < numberOfPlayers; i++)//increment numberPlayersAlive for every player that's alive
		{
			if (PlayersAlive [i] == true) {
				numberPlayersAlive++;
			}
		}
	}

	public void addToPlayerScore(OuyaPlayer playerNumber, int additionalScore, OuyaPlayer deadPlayerNumber) //add the player score when given player number and how much to add
	{
		//Debug.Log ("this is the index value of enum passed into add player score" + (int)playerNumber);
		int livePlayerIndex = (int)playerNumber - 1; //figure out index number in Player Scores Array by subtracting one from the index number of Ouya Player enum
		int deadPlayerIndex = (int)deadPlayerNumber - 1; 

		if(PlayersAlive[deadPlayerIndex] == true) //only score if player is still alive at time of scoring. this gates multiple collision triggers from scoring same dude more than once.
		{
			setWhoDied(deadPlayerIndex);//change player to dead
			
			PlayerScores[livePlayerIndex] = PlayerScores[livePlayerIndex] + additionalScore; //add additional score to existing score
			SetScoreText(); //update score HUD
		}
	}

	//overloaded method where it doesn't require dead player game object, this version has setWhoDied removed
	public void addToPlayerScore(OuyaPlayer playerNumber, int additionalScore) //add the player score when given player number and how much to add
	{
		//Debug.Log ("this is the index value of enum passed into add player score" + (int)playerNumber);
		int livePlayerIndex = (int)playerNumber - 1; //figure out index number in Player Scores Array by subtracting one from the index number of Ouya Player enum

		PlayerScores[livePlayerIndex] = PlayerScores[livePlayerIndex] + additionalScore; //add additional score to existing score
		SetScoreText(); //update score HUD
	}

	public void setWhoDied(int deadPlayerIndex)
	{
		PlayersAlive[deadPlayerIndex] = false; //player with index number i is dead.
		
		countDownConditionCheck(); //check if conditions are met for another countdown, this happens only when someone dies, and after a coundown finishes.
	}

	IEnumerator classSelectCountDown()
	{

		StopCoroutine("startRoundTimer");//interupt round timer.
		//Debug.Log ("Countdown script starting.");
		yield return new WaitForSeconds(.5f);



		ClassSelectState = true; //turn on class selection
		SetAllPlayersAlive();

		CenterAnnouncementText.text = "Rock!";
		AudioManagement(GlobalAudioFX.lowToneAudio);
		yield return new WaitForSeconds(1.0f);

		CenterAnnouncementText.text = "Paper!";
		AudioManagement(GlobalAudioFX.lowToneAudio);
		yield return new WaitForSeconds(1.0f);

		CenterAnnouncementText.text = "Scissors!";
		AudioManagement(GlobalAudioFX.lowToneAudio);
		yield return new WaitForSeconds(1.0f);

		CenterAnnouncementText.text = "Go!";
		AudioManagement(GlobalAudioFX.highToneAudio);
		yield return new WaitForSeconds(1.0f);

		CenterAnnouncementText.text = "";

		ClassSelectState = false; //turn off class selection
		yield return new WaitForSeconds(1.0f);
		countDownConditionCheck(); //check if conditions are met for another countdown, this happens only when someone dies, and after a coundown finishes.

		StartCoroutine(startRoundTimer());//start round timer

	}

	/*************************************************************************
	 * awesome function to average location of live players,                 *
	 * get their distance from spawn points, order spawn points by distance, *
	 * shuffle dead player list and assign them to farthest spawn points     *
	 * ***********************************************************************/

	void relocateDeadPlayers () 
	{
		//get distances between live players and spawn points
		//spawn dead players at greatest distances randomly 


		SpawnPointDistances = new float[SpawnPointsArray.Length]; //size of spawn point distances array equal to number of spawnpoints

		SortedSpawnPointDistances = new float[0];

		LivePlayerGameObjects = new GameObject[0];

		DeadPlayerGameObjects = new GameObject[0];


		LivePlayerAverageLocation = Vector3.zero; //temp value in LivePlayer Average location

		setLiveAndDeadPlayerArrays (); //delegates players to either live players array and dead players array

		for(int i = 0; i < DeadPlayerGameObjects.Length; i++)//zero out velocities of dead player objects
		{
			//Debug.Log ("initial velocity " + DeadPlayerGameObjects[i].GetComponent<Rigidbody>().velocity);
			DeadPlayerGameObjects[i].GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
			//Debug.Log ("changed velocity " + DeadPlayerGameObjects[i].GetComponent<Rigidbody>().velocity);
		}

		findAverageLocationOfLivePlayers ();//get an average of locations of live players

		setSpawnPointDistancesArray (); //set items in array of distances between each spawn point the average player location
		for(int i = 0; i < SpawnPointDistances.Length; i++) //transfer values from unsorted to sorted but keep them unlinked.
		{
			Array.Resize(ref SortedSpawnPointDistances, SortedSpawnPointDistances.Length + 1);
			SortedSpawnPointDistances[i] = SpawnPointDistances[i];
			//Debug.Log ("Sorted Spawn Point " + i + " is " + SortedSpawnPointDistances[i] + " from average liveplayer location ");
		}
		 //transfer unsorted distances to sorted.
		Array.Sort (SortedSpawnPointDistances);//sort array from lowest value to highest.
		Array.Reverse (SortedSpawnPointDistances);//reverse order so highest is first.

		shuffleDeadPlayersArray ();//shuffle dead player array so when they're matched up with spawn points no one gets consistent advantage.

		//Debug.Log("Randomized DeadPlayer list: " + DeadPlayerObjects[i]);
		//debug for loop
		for (int i = 0; i < DeadPlayerGameObjects.Length; i++)
		{
			//find index of spawn point in unordered array that matches distance in ordered array, farthest distance first.
			int spawnPointIndex = Array.IndexOf(SpawnPointDistances, SortedSpawnPointDistances[i]);
			//Debug.Log ("spawnPointIndex " + i + " is " + spawnPointIndex);
			//Debug.Log ( "SortedSpawnPointDistances[i]" + SortedSpawnPointDistances[i]);
			//Debug.Log ( "SpawnPointDistances[spawnPointIndex]" + SpawnPointDistances[spawnPointIndex]);
			//set dead player position
			DeadPlayerGameObjects[i].transform.position = SpawnPointsArray[spawnPointIndex].transform.position;
  		}
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
				StopCoroutine("startRoundTimer");//stop round timer
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

	public void setLiveAndDeadPlayerArrays ()
	{
		//empty out array
		Array.Resize (ref LivePlayerGameObjects, 0);
		Array.Resize (ref DeadPlayerGameObjects, 0);

		for (int i = 0; i < PlayerGameObjectsArray.Length; i++)//separate live and dead players into two different arrays.
		{
			if (PlayerGameObjectsArray [i].activeSelf == true) {
				Array.Resize (ref LivePlayerGameObjects, LivePlayerGameObjects.Length + 1);
				// resize array of live players by one.
				LivePlayerGameObjects [LivePlayerGameObjects.Length - 1] = PlayerGameObjectsArray [i];
				// if player is alive add to live players array
				//Debug.Log ("number of LivePlayerGameObjects: " + i);
				//Debug.Log ("LivePlayerGameObjects # " + i + " = " + LivePlayerGameObjects [LivePlayerGameObjects.Length - 1]);
			}
			if (PlayerGameObjectsArray [i].activeSelf == false) {
				Array.Resize (ref DeadPlayerGameObjects, DeadPlayerGameObjects.Length + 1);
				//resize array by one.
				DeadPlayerGameObjects [DeadPlayerGameObjects.Length - 1] = PlayerGameObjectsArray [i];
				// if player is dead add to dead players array
			}
		}
	}

	public Vector3 findAverageLocationOfLivePlayers () //this is used in both spawn location decision and camera follow script
	{
		for (int i = 0; i < LivePlayerGameObjects.Length; i++) {
			//Debug.Log ("LivePlayerAverageLocation: " + LivePlayerAverageLocation);
			LivePlayerAverageLocation += LivePlayerGameObjects [i].transform.position;
			//add them up
			//Debug.Log ("LivePlayerAverageLocation Sum: " + LivePlayerAverageLocation);
		}
		//then divide
		LivePlayerAverageLocation = new Vector3 (LivePlayerAverageLocation.x / LivePlayerGameObjects.Length, LivePlayerAverageLocation.y / LivePlayerGameObjects.Length, LivePlayerAverageLocation.z / LivePlayerGameObjects.Length);
		//Debug.Log ("LivePlayerAverageLocation after division: " + LivePlayerAverageLocation);
		return LivePlayerAverageLocation;
	}

	void setSpawnPointDistancesArray ()
	{
		for (int i = 0; i < SpawnPointsArray.Length; i++) {
			SpawnPointDistances [i] = Vector3.Distance (LivePlayerAverageLocation, SpawnPointsArray [i].transform.position);
			//get distance between live player average and spawn point i
			//Debug.Log ("Spawn Point " + i + " is " + SpawnPointDistances[i] + " from average liveplayer location " + i);
		}
	}

	void shuffleDeadPlayersArray ()
	{
		//shuffle dead player array so when they're matched up with spawn points no one gets consistent advantage.
		for (int t = 0; t < DeadPlayerGameObjects.Length; t++)// Knuth shuffle algorithm :: courtesy of Wikipedia :)
		{
			GameObject tmp = DeadPlayerGameObjects [t];
			//assign t to temp
			int r = UnityEngine.Random.Range (t, DeadPlayerGameObjects.Length);
			// randomly choose another index in range of 
			DeadPlayerGameObjects [t] = DeadPlayerGameObjects [r];
			// move original to new random location
			DeadPlayerGameObjects [r] = tmp;
			//move random to temp
		}
	}

	public void startPickupRespawnTimerProxy(GameObject Pickup)
	{
		//Debug.Log ("trying to start proxy method");
		StartCoroutine(startPickupRespawnTimer(Pickup));
	}

	public IEnumerator startPickupRespawnTimer(GameObject Pickup)
	{
		//Debug.Log ("started pickup respawn countdown");
		yield return new WaitForSeconds(pillRespawnTime);
		Pickup.gameObject.SetActive(true);
		//Debug.Log ("completed respawn countdown");

	}

	IEnumerator startRoundTimer()
	{
		Debug.Log ("Start Round timer");
		yield return new WaitForSeconds(roundTime);// wait for round to end

		UpdatePlayerClassArray(); //update classes

		determineSurvivorBonus(); //determine who survived the round despite having a pursuer.

		relocateDeadPlayers(); //put dead players at respawn location 
		if(!checkWinningScoreReached())//only start class selection if win condition is false, this is redundant but i'm sticking it in to be safe.
		{
			StartCoroutine(classSelectCountDown()); //start class selection
		}
	
		//Debug.Log ("finished round time and round restarted");
	}

	void determineSurvivorBonus()
	{
		setLiveAndDeadPlayerArrays (); //update live and dead player arrays
		//Array.Sort (LivePlayerGameObjects);//sort array from lowest value to highest.
		//Array.Reverse (SortedSpawnPointDistances);//reverse order so highest is first.

		Debug.Log ("Determining survivor bonuses");
		//empty out arrays
		Array.Resize (ref liveRockPlayers, 0);
		Array.Resize (ref livePaperPlayers, 0);
		Array.Resize (ref liveScissorPlayers, 0);

		for (int i = 0; i < LivePlayerGameObjects.Length; i++) //this goes through the live player objects array and seperates live player objects into arrays for each class
		{
			if ( LivePlayerGameObjects[i].GetComponent<MeshFilter> ().mesh.ToString() == "Rock Instance (UnityEngine.Mesh)")//is your mesh component a rock instance?
			{
				Array.Resize (ref liveRockPlayers, liveRockPlayers.Length + 1);//update array size
				// resize array of live players by one.
				liveRockPlayers [liveRockPlayers.Length - 1] = LivePlayerGameObjects [i];//copy in player gameobject
				Debug.Log ("Live rock players: " + liveRockPlayers [liveRockPlayers.Length - 1]);
			}
			if ( LivePlayerGameObjects[i].GetComponent<MeshFilter> ().mesh.ToString() == "Paper Instance (UnityEngine.Mesh)")
			{
				Array.Resize (ref livePaperPlayers, livePaperPlayers.Length + 1);
				// resize array of live players by one.
				livePaperPlayers [livePaperPlayers.Length - 1] = LivePlayerGameObjects [i];
				Debug.Log ("Live paper players: " + livePaperPlayers [livePaperPlayers.Length - 1]);
			}
			if ( LivePlayerGameObjects[i].GetComponent<MeshFilter> ().mesh.ToString() == "Scissors Instance (UnityEngine.Mesh)")
			{
				Array.Resize (ref liveScissorPlayers, liveScissorPlayers.Length + 1);
				// resize array of live players by one.
				liveScissorPlayers [liveScissorPlayers.Length - 1] = LivePlayerGameObjects [i];
				Debug.Log ("Live scissor players: " + liveScissorPlayers[liveScissorPlayers.Length - 1]);
			}
		}
		//Debug.Log ("Number of Rock survivors: " + liveRockPlayers.Length);
		//Debug.Log ("Number of Paper survivors: " + livePaperPlayers.Length);
		//Debug.Log ("Number of Scissor survivors: " + liveScissorPlayers.Length);

		//start making decisions of who are the survivors based on the size of each array
		if (liveRockPlayers.Length == 0 && livePaperPlayers.Length != 0 && liveScissorPlayers.Length != 0)
		{
			for(int i = 0; i < livePaperPlayers.Length; i++)
			{

				addToPlayerScore(livePaperPlayers[i].GetComponent<InputHandler>().player, 2 * liveScissorPlayers.Length); //add to surviving player's score , multiply score by number of predators

				//Debug.Log ("Trying to give paper bonus to: " + livePaperPlayers[i]);

			}
		}

		else if (livePaperPlayers.Length == 0 && liveRockPlayers.Length != 0 && liveScissorPlayers.Length != 0)
		{
			for(int i = 0; i < liveScissorPlayers.Length; i++)
			{
				
				addToPlayerScore(liveScissorPlayers[i].GetComponent<InputHandler>().player, 2 * liveRockPlayers.Length); //add to surviving player's score , multiply score by number of predators

				Debug.Log ("Trying to give Scissors bonus." + liveScissorPlayers[i]);

			}
		}
		else if (liveScissorPlayers.Length == 0 && livePaperPlayers.Length != 0 && liveRockPlayers.Length != 0)
		{
			for(int i = 0; i < liveRockPlayers.Length; i++)
			{
				
				addToPlayerScore(liveRockPlayers[i].GetComponent<InputHandler>().player, 2 * livePaperPlayers.Length); //add to surviving player's score , multiply score by number of predators
				Debug.Log ("Trying to give Rock bonus." + liveRockPlayers[i]);

			}
		}
	}

	void AudioManagement (GlobalAudioFX AudioToPlay)
	{
		
		// If the shout input has been pressed...
		if(AudioToPlay == GlobalAudioFX.lowToneAudio)
		{
			// ... play the shouting clip where we are.
			audio.PlayOneShot(lowToneClip, 1.0f);
			//Debug.Log("play rock audio");
		}
		else if (AudioToPlay == GlobalAudioFX.highToneAudio)
		{
			// ... play the shouting clip where we are.
			audio.PlayOneShot(highToneClip, 1.0f);
			//Debug.Log("play rock audio");
		}
		

	}
}
