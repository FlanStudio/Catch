using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerLOL : MonoBehaviour
{
    public static GameManagerLOL singleton;
    public GameObject player1;
    public GameObject player2;
    public GameObject localPlayer;
    private GameObject player_chaser;

    public uint max_points = 10;

    public uint player1_points = 0;
    public uint player2_points = 0;

    public float time;
    public float max_time = 30;
    // Start is called before the first frame update
    private void Awake()
    {
        singleton = this;
    }
    void Start()
    {
        int random;
        random = Random.Range(0, 2);
        
        if(random == 0)
        {
            player_chaser = player1;
        }
        else
        {
            player_chaser = player2;
        }
        
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;
        
        if (player1_points >= 10 || player2_points > 10)
        {
            if(localPlayer == player1)
                Application.LoadLevel(2);
            else
                Application.LoadLevel(3);
        }

        if (time >= max_time)
        {
            time = 0;

            if(player_chaser == player1)
            {
                player2_points++;
            }
            else
            {
                player1_points++;
            }
        }
    }
    public static void PlayerCatched()
    {
        if (singleton.player_chaser == singleton.player1)
        {
            if(singleton.localPlayer == singleton.player1)
            {
                Application.LoadLevel(2); //For the moment
                singleton.player1_points++;
            }
            else
                Application.LoadLevel(3);

        }
        else
        {
            if(singleton.localPlayer == singleton.player2)
            {
                Application.LoadLevel(2); //For the moment
                singleton.player2_points++;
            }
            else
                Application.LoadLevel(3);

        }
    }
}
