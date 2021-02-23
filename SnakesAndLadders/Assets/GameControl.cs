﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {

    private static GameObject whoWinsTextShadow, player1MoveText, player2MoveText;

    private static GameObject player1, player2;

    public static int diceSideThrown = 0;
    public static int player1StartWaypoint = 0;
    public static int player2StartWaypoint = 0;
    //public static int[] isSnake = {5, 6, 7, 8, 9, 10, 11, 12};
    public static List<int> isSnake = new List<int>() { 5, 10, 15, 20, 25, 30, 4, 6};
    public static List<int> snakeTo = new List<int>() { 0, 1, 2, 3, 4, 6, 13, 22};
    public static bool gameOver = false;

    // Use this for initialization
    void Start () {

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
        // NORMAL MOVE CONDITIONS
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
                player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
                player1.GetComponent<FollowThePath>().moveAllowed = false;
                player1MoveText.gameObject.SetActive(false);
                player2MoveText.gameObject.SetActive(true);
            }

            


        }
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
                player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
                player2.GetComponent<FollowThePath>().moveAllowed = false;
                player2MoveText.gameObject.SetActive(false);
                player1MoveText.gameObject.SetActive(true);
            }
        }

        // WIN CONDITIONS
        if (player1.GetComponent<FollowThePath>().waypointIndex == 
            player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 1 Wins";
            gameOver = true;
        }

        if (player2.GetComponent<FollowThePath>().waypointIndex ==
            player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 2 Wins";
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
}
