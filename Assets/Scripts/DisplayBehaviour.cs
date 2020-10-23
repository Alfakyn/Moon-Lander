using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBehaviour : MonoBehaviour
{
    public LanderBehaviour lander_behaviour;

    public TextMeshProUGUI score_value, time_value, fuel_value;
    public TextMeshProUGUI altitude_value, speed_x_value, speed_y_value;

    private int score, fuel;
    private int speed_x, speed_y, altitude;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        time = 0.0f;
        score = 0;
        fuel = 0;

        speed_x = 0;
        speed_y = 0;
        altitude = 0;
    }

    // Update is called once per frame
    void Update()
    {
        updateTime();
        updateSpeeds();
    }

    void updateTime()
    {
        time += Time.deltaTime;

        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;

        if (seconds < 10)
        {
            time_value.text = minutes.ToString() + ":0" + seconds.ToString();
        }
        else
        {
            time_value.text = minutes.ToString() + ":" + seconds.ToString();
        }
    }

    void updateSpeeds()
    {
        speed_x_value.text = Mathf.Ceil(lander_behaviour.getSpeedX()).ToString();
        speed_y_value.text = Mathf.Ceil(lander_behaviour.getSpeedY()).ToString();
    }
}
