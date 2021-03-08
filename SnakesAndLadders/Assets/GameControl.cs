using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using Firebase;
using Firebase.Database;
using System.Threading;


public class GameControl : MonoBehaviour {

    private static GameObject whoWinsTextShadow, player1MoveText, player2MoveText;

    private static GameObject player1, player2;
    public static float turnTime;
    public static int diceSideThrown = 0;
    public static int player1StartWaypoint = 0;
    public static int player2StartWaypoint = 0;
    public static bool gameOver = false;
    //public static int[] isSnake = {5, 6, 7, 8, 9, 10, 11, 12};
    public static List<int> isSnake = new List<int>() { 13, 20, 26, 34, 42, 44, 52, 54, 64, 80, 83, 86, 88, 90, 95, 99, 3, 12, 24, 58, 67, 22, 51}; // siffra -1
    public static List<int> snakeTo = new List<int>() { 6, 1, 4, 27, 37, 22, 10, 45, 42, 58, 64, 73, 47, 71, 86, 62, 17, 31, 44, 83, 75, 39, 69};   // siffra
    public static DatabaseReference reference;

    // Use this for initialization
    void Start () {
        diceSideThrown = 0;
        player1StartWaypoint = 0;
        player2StartWaypoint = 0;
        gameOver = false;
        Dice.whosTurn = 1;
        turnTime = Time.time;
        whoWinsTextShadow = GameObject.Find("WhoWinsText");
        player1MoveText = GameObject.Find("Player1MoveText");
        player2MoveText = GameObject.Find("Player2MoveText");

        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        player1.GetComponent<FollowThePath>().moveAllowed = false;
        player2.GetComponent<FollowThePath>().moveAllowed = false;

        whoWinsTextShadow.gameObject.SetActive(false);
        player1MoveText.gameObject.SetActive(true);
        player2MoveText.gameObject.SetActive(false);

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        saveGameState(1, true);
    }


    public static void saveGameState(int turn, bool DiceValue=false, int ifSnake=0, int ifLadder=0)
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["/GameState/Date"] = DateTime.Now.ToString("yyyy-MM-dd");
        childUpdates["/GameState/UniversalTime"] = DateTime.Now.ToString("HH:mm:ss:fff");
        childUpdates["/GameState/EntireGameTime"] = Time.time - StartGameBottom.startTime;
        childUpdates["/GameState/TurnTime"] = Time.time - turnTime;
        childUpdates["/GameState/WhosTurn"] = turn; // 1 == Human, 0 == Furhat
        if (DiceValue)
        {
            childUpdates["/GameState/DiceValue"] = diceSideThrown;
        }
        childUpdates["/GameState/Human"] = player1StartWaypoint;
        childUpdates["/GameState/Furhat"] = player2StartWaypoint;
        childUpdates["/GameState/ifSnake"] = ifSnake;
        childUpdates["/GameState/ifLadder"] = ifLadder;

        reference.UpdateChildrenAsync(childUpdates);
    }

    // Update is called once per frame
    public void Update()
    {
        if (Dice.whosTurn == -1 && !player1.GetComponent<FollowThePath>().moveAllowed)
        {
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(true);
        }
        else if (Dice.whosTurn == 1 && !player2.GetComponent<FollowThePath>().moveAllowed)
        {
            player1MoveText.gameObject.SetActive(true);
            player2MoveText.gameObject.SetActive(false);
        }
        // NORMAL MOVE CONDITIONS
        // BOB
        if (player1.GetComponent<FollowThePath>().waypointIndex > 
            player1StartWaypoint + diceSideThrown)
        {
            
            if (isSnake.Contains(player1.GetComponent<FollowThePath>().waypointIndex))
            {
                int delta = snakeTo[isSnake.IndexOf(player1.GetComponent<FollowThePath>().waypointIndex)] - (isSnake[isSnake.IndexOf(player1.GetComponent<FollowThePath>().waypointIndex)] - 1);
                if (delta < 0)
                {
                    player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
                    saveGameState(1, false, delta, 0);
                }
                else
                {
                    player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
                    saveGameState(1, false, 0, delta);
                }
                Thread.Sleep(500);
                player1StartWaypoint = snakeTo[isSnake.IndexOf(player1.GetComponent<FollowThePath>().waypointIndex)];
                player1.GetComponent<FollowThePath>().waypointIndex = snakeTo[isSnake.IndexOf(player1.GetComponent<FollowThePath>().waypointIndex)];
                diceSideThrown = 0;
            }
            else
            {
               
                player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
                if (player1.GetComponent<FollowThePath>().moveAllowed)
                {
                    saveGameState(1);
                    turnTime = Time.time;
                }
                player1.GetComponent<FollowThePath>().moveAllowed = false;
                //player1MoveText.gameObject.SetActive(false);
                //player2MoveText.gameObject.SetActive(true);
                
            }
        }
        

        // FURHAT
        if (player2.GetComponent<FollowThePath>().waypointIndex >
            player2StartWaypoint + diceSideThrown)
        {
            if (isSnake.Contains(player2.GetComponent<FollowThePath>().waypointIndex))
            {
                int delta = snakeTo[isSnake.IndexOf(player2.GetComponent<FollowThePath>().waypointIndex)] - (isSnake[isSnake.IndexOf(player2.GetComponent<FollowThePath>().waypointIndex)] - 1);
                if (delta < 0)
                {
                    player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
                    saveGameState(0, false, delta, 0);
                }
                else
                {
                    player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
                    saveGameState(0, false, 0, delta);
                }
                Thread.Sleep(500);
                player2StartWaypoint = snakeTo[isSnake.IndexOf(player2.GetComponent<FollowThePath>().waypointIndex)];
                player2.GetComponent<FollowThePath>().waypointIndex = snakeTo[isSnake.IndexOf(player2.GetComponent<FollowThePath>().waypointIndex)];
                diceSideThrown = 0;
            }
            else
            {   
                player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
                if (player2.GetComponent<FollowThePath>().moveAllowed)
                {
                    saveGameState(0);
                    turnTime = Time.time;
                }
                player2.GetComponent<FollowThePath>().moveAllowed = false;
                //player2MoveText.gameObject.SetActive(false);
                //player1MoveText.gameObject.SetActive(true);

            }
        }

        // WIN CONDITIONS
        if (player1.GetComponent<FollowThePath>().waypointIndex == 
            player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "YOU WIN!";
            if (gameOver == false)
            {
                saveGameState(1);
            }
            gameOver = true;

        }

        if (player2.GetComponent<FollowThePath>().waypointIndex ==
            player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            whoWinsTextShadow.GetComponent<Text>().text = "Furhat Wins";
            if (gameOver == false)
            {
                saveGameState(0);
            }
            gameOver = true;

        }
    }

    public static void MovePlayer(int playerToMove)
    {
        switch (playerToMove) { 
            case 1:
                player1.GetComponent<FollowThePath>().moveAllowed = true;
                break;

            case 2:
                player2.GetComponent<FollowThePath>().moveAllowed = true;
                break;
        }
    }

    public static void save()
    {

        string strFilePath = @"C:\Users\ilian\Unity_Projects\saved_data\Data.csv";
        string strSeperator = ",";
        StringBuilder sbOutput = new StringBuilder();

        
        sbOutput.AppendLine(string.Join(strSeperator, Time.time.ToString("f6"), player1StartWaypoint, player2StartWaypoint, (-1)*Dice.whosTurn, diceSideThrown));
        //Time.time.ToString("f6")
        
        // Create and write the csv file
        //File.WriteAllText(strFilePath, sbOutput.ToString());

        // To append more lines to the csv file
        File.AppendAllText(strFilePath, sbOutput.ToString());
    }

}
