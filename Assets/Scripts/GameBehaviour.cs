using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public enum GameState { Running, Standby };
    public static GameState game_state;
    public static float standby_timer;

    private void Start()
    {
        standby_timer = 0.0f;
    }

    private void Update()
    {
        switch (game_state)
        {
            case GameState.Running:
                break;
            case GameState.Standby:
                if (standby_timer >= 5.0f)
                {
                    changeGameState(GameState.Running);
                }
                standby_timer += Time.deltaTime;
                break;
        }
    }

    public static void changeGameState(GameState next_state)
    {
        LanderBehaviour lander_behaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<LanderBehaviour>();
        
        switch (next_state)
        {
            case GameState.Running:
                lander_behaviour.rigidbody2d.WakeUp();
                lander_behaviour.initializeLander();
                GameObject[] win_colliders = GameObject.FindGameObjectsWithTag("WinCollider");
                foreach(GameObject win_collider in win_colliders)
                {
                    win_collider.GetComponent<WinColliderBehaviour>().initializeMultipliers();
                }                
                break;
            case GameState.Standby:
                lander_behaviour.rigidbody2d.Sleep();
                standby_timer = 0.0f;
                break;
        }

        game_state = next_state;
    }
}
