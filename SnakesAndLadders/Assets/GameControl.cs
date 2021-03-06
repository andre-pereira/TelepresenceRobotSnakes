﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;



public class GameControl : MonoBehaviour {

    private static GameObject whoWinsTextShadow, player1MoveText, player2MoveText;

    private static GameObject player1, player2;

    public static int diceSideThrown = 0;
    public static int player1StartWaypoint = 0;
    public static int player2StartWaypoint = 0;
    public static bool gameOver = false;
    //public static int[] isSnake = {5, 6, 7, 8, 9, 10, 11, 12};
    public static List<int> isSnake = new List<int>() { 13, 20, 26, 34, 42, 44, 52, 54, 64, 80, 83, 86, 88, 90, 95, 99, 3, 12, 24, 58, 67, 22, 51};
    public static List<int> snakeTo = new List<int>() { 6, 1, 4, 27, 37, 22, 10, 45, 42, 58, 64, 73, 47, 71, 86, 62, 17, 31, 44, 83, 75, 39, 69};
    

    // Use this for initialization
    void Start () {
        diceSideThrown = 0;
        player1StartWaypoint = 0;
        player2StartWaypoint = 0;
        gameOver = false;
        Dice.whosTurn = 1;
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
    }

    // Update is called once per frame
    void Update()
    {
        //save();
        // NORMAL MOVE CONDITIONS
        // BOB
        if (player1.GetComponent<FollowThePath>().waypointIndex > 
            player1StartWaypoint + diceSideThrown)
        {
            if (isSnake.Contains(player1.GetComponent<FollowThePath>().waypointIndex))
            {   
                player1StartWaypoint = snakeTo[isSnake.IndexOf(player1.GetComponent<FollowThePath>().waypointIndex)];
                player1.GetComponent<FollowThePath>().waypointIndex = snakeTo[isSnake.IndexOf(player1.GetComponent<FollowThePath>().waypointIndex)];
                diceSideThrown = 0;
            }
            else
            {
                player1.GetComponent<FollowThePath>().moveAllowed = false;
                player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
                player1MoveText.gameObject.SetActive(false);
                player2MoveText.gameObject.SetActive(true);


            }
        }

        // FURHAT
        if (player2.GetComponent<FollowThePath>().waypointIndex >
            player2StartWaypoint + diceSideThrown)
        {
            if (isSnake.Contains(player2.GetComponent<FollowThePath>().waypointIndex))
            {
                player2StartWaypoint = snakeTo[isSnake.IndexOf(player2.GetComponent<FollowThePath>().waypointIndex)];
                player2.GetComponent<FollowThePath>().waypointIndex = snakeTo[isSnake.IndexOf(player2.GetComponent<FollowThePath>().waypointIndex)];
                diceSideThrown = 0;
            }
            else
            {
                player2.GetComponent<FollowThePath>().moveAllowed = false;
                player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
                player2MoveText.gameObject.SetActive(false);
                player1MoveText.gameObject.SetActive(true);

            }
        }

        // WIN CONDITIONS
        if (player1.GetComponent<FollowThePath>().waypointIndex == 
            player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "YOU WIN!";
            gameOver = true;

        }

        if (player2.GetComponent<FollowThePath>().waypointIndex ==
            player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            whoWinsTextShadow.GetComponent<Text>().text = "Furhat Wins";
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
