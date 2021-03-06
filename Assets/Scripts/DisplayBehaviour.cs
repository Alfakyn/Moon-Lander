﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBehaviour : MonoBehaviour
{
    public LanderBehaviour lander_behaviour;

    public TextMeshProUGUI score_value_TMP, time_value_TMP, fuel_value_TMP;
    public TextMeshProUGUI altitude_value_TMP, speed_x_value_TMP, speed_y_value_TMP;
    public TextMeshProUGUI standby_message_TMP;

    private int score;
    private float time;

    public GameObject end_screen;

    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
        score = 0;

        score_value_TMP.text = "0000";

        end_screen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameBehaviour.game_state != GameBehaviour.GameState.Finish)
        {
            updateTime();
            updateSpeeds();
            updateFuel();
            updateAltitude();
        }
        else
        {
            end_screen.SetActive(true);
        }
    }

    void updateTime()
    {
        time += Time.deltaTime;

        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;

        if (seconds < 10)
        {
            time_value_TMP.text = minutes.ToString() + ":0" + seconds.ToString();
        }
        else
        {
            time_value_TMP.text = minutes.ToString() + ":" + seconds.ToString();
        }
    }

    void updateSpeeds()
    {
        speed_x_value_TMP.text = Mathf.Ceil(lander_behaviour.getSpeedX()).ToString();
        speed_y_value_TMP.text = Mathf.Ceil(lander_behaviour.getSpeedY()).ToString();
    }

    void updateScore()
    {
        int thousands = score / 1000;
        int hundreds = (score % 1000) / 100;
        int tens = (score % 100) / 10;
        int units = score % 10;

        score_value_TMP.text = thousands.ToString() + hundreds.ToString() + tens.ToString() + units.ToString();
    }

    public void addScore(int score_to_add)
    {
        score += score_to_add;
        updateScore();
    }

    void updateFuel()
    {
        int fuel = (int)lander_behaviour.getFuel();

        int thousands = fuel / 1000;
        int hundreds = (fuel % 1000) / 100;
        int tens = (fuel % 100) / 10;
        int units = fuel % 10;

        fuel_value_TMP.text = thousands.ToString() + hundreds.ToString() + tens.ToString() + units.ToString();
    }

    public void updateStandbyMessage(string message)
    {
        standby_message_TMP.text = message;
    }

    void updateAltitude()
    {
        int altitude = (int)lander_behaviour.getAltitude();

        int thousands = altitude / 1000;
        int hundreds = (altitude % 1000) / 100;
        int tens = (altitude % 100) / 10;
        int units = altitude % 10;

        altitude_value_TMP.text = thousands.ToString() + hundreds.ToString() + tens.ToString() + units.ToString();
    }
}
