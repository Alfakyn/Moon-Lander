using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public enum GameState { Running, Standby, Finish};
    public static GameState game_state;
    public static float standby_timer;

    public static GameObject standby_message;
    public static LanderBehaviour lander_behaviour;

    private void Start()
    {
        standby_timer = 0.0f;
        standby_message = GameObject.FindWithTag("Standby Message");
        standby_message.SetActive(false);
        lander_behaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<LanderBehaviour>();
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
        //LanderBehaviour lander_behaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<LanderBehaviour>();
        //GameObject standby_message = GameObject.FindWithTag("Standby Message");
        
        switch (next_state)
        {
            case GameState.Running:
                lander_behaviour.lander_animator.SetTrigger("LanderToDefault");
                standby_message.SetActive(false);
                lander_behaviour.rigidbody2d.WakeUp();
                lander_behaviour.initializeLander();
                GameObject[] win_colliders = GameObject.FindGameObjectsWithTag("WinCollider");
                foreach(GameObject win_collider in win_colliders)
                {
                    win_collider.GetComponent<WinColliderBehaviour>().initializeMultipliers();
                }                
                break;
            case GameState.Standby:
                standby_message.SetActive(true);
                lander_behaviour.rigidbody2d.Sleep();
                standby_timer = 0.0f;
                break;
            case GameState.Finish:
                standby_message.SetActive(false);
                lander_behaviour.rigidbody2d.Sleep();
                break;
        }
        game_state = next_state;
    }
}
