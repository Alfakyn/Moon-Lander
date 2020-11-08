using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderBehaviour : MonoBehaviour
{
    public Rigidbody2D rigidbody2d;
    private float gravity_scale;
    private const int ACCELERATION = 3;
    private const float HORIZONTAL_DRAG_FACTOR = 0.001f;

    private float speed_x, speed_y;
    private float fuel;

    public DisplayBehaviour display_behaviour;
    private const int BASE_SCORE = 50;
    private const int WORSE_SCORE = 15;
    private const float INITIAL_FUEL = 1000;
    private const float FUEL_DEPLETE_RATE = 10.0f;
    private const float FUEL_LOST_ON_EXPLOSION = 200.0f;

    private const float AMOUNT_TO_ROTATE = 15.0f;
    private float z_rotation;

    private bool has_collided;

    public AudioSource audioSource;
    private float audio__source_volume = 0.3f;

    //public Animator butt_fire_animator;
    public Animator lander_animator;

    // Start is called before the first frame update
    void Start()
    {
        gravity_scale = rigidbody2d.gravityScale;
        initializeLander();
        fuel = INITIAL_FUEL;

        z_rotation = 0.0f;

        audioSource.volume = 0.0f;
    }

    public void initializeLander()
    {
        int position_x = Random.Range(-960, 961);
        transform.position = new Vector2(position_x, 370);

        int direction_x = -(position_x / Mathf.Abs(position_x));
        rigidbody2d.velocity = new Vector2(82 * direction_x, 0);

        transform.rotation = Quaternion.Euler(Vector3.zero);

        has_collided = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameBehaviour.game_state)
        {
            case GameBehaviour.GameState.Running:
                checkTurnInput();
                convertZRotation();
                break;
            case GameBehaviour.GameState.Standby:
                break;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) == true || Input.GetKeyDown(KeyCode.W) == true)
        {
            audioSource.volume = audio__source_volume;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) == true || Input.GetKeyUp(KeyCode.W) == true)
        {
            lander_animator.SetBool("BoosterInput", false);
            audioSource.volume = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        switch (GameBehaviour.game_state)
        {
            case GameBehaviour.GameState.Running:
                if (fuel > 0.0f)
                {
                    checkBoosterInput();
                }
                else
                {
                    lander_animator.SetBool("BoosterInput", false);
                }
                checkHorizontalDrag();
                speed_x = rigidbody2d.velocity.x;
                speed_y = rigidbody2d.velocity.y;
                break;
            case GameBehaviour.GameState.Standby:
                break;
        }        
    }

    void checkHorizontalDrag()
    {
        Vector2 velocity = rigidbody2d.velocity;
        velocity.x *= (1.0f - HORIZONTAL_DRAG_FACTOR);
        rigidbody2d.velocity = velocity;
    }

    void checkBoosterInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) == true || Input.GetKey(KeyCode.W) == true)
        {
            rigidbody2d.velocity += new Vector2(transform.up.x, transform.up.y) * ACCELERATION * gravity_scale * Time.fixedDeltaTime;
            fuel -= FUEL_DEPLETE_RATE * Time.fixedDeltaTime;
            lander_animator.SetBool("BoosterInput", true);
        }
    }

    void checkTurnInput()
    {
        if ((Input.GetKeyDown(KeyCode.LeftArrow) == true || Input.GetKeyDown(KeyCode.A) == true) && z_rotation < 90.0f)
        {
            transform.Rotate(0, 0, AMOUNT_TO_ROTATE);
        }
        else if ((Input.GetKeyDown(KeyCode.RightArrow) == true || Input.GetKeyDown(KeyCode.D) == true) && z_rotation > -90.0f)
        {
            transform.Rotate(0, 0, -AMOUNT_TO_ROTATE);
        }
    }

    private void convertZRotation()
    {
        z_rotation = transform.eulerAngles.z;
        if(z_rotation > 180.0f)
        {
            z_rotation -= 360.0f;
        }
    }

    public float getSpeedX()
    {
        return speed_x;
    }
    public float getSpeedY()
    {
        return speed_y;
    }

    public float getFuel()
    {
        return fuel;
    }

    private void removeFuel()
    {
        float actual_fuel_lost;

        if (fuel < FUEL_LOST_ON_EXPLOSION)
        {
            actual_fuel_lost = fuel;
        }
        else
        {
            actual_fuel_lost = FUEL_LOST_ON_EXPLOSION;
        }

        fuel -= actual_fuel_lost;

        display_behaviour.updateStandbyMessage("Your lander exploded\n" + ((int)actual_fuel_lost).ToString() + " units of fuel lost");
        lander_animator.SetTrigger("LanderExplosion");
        GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (has_collided == false)
        {
            if (collision.gameObject.tag.Equals("WinCollider"))
            {
                if (z_rotation > -1.0f && z_rotation < 1.0f)
                {
                    if (Mathf.Abs(speed_x) <= 15.0f && Mathf.Abs(speed_y) <= 15.0f)
                    {
                        int score_to_add = BASE_SCORE * collision.gameObject.GetComponent<WinColliderBehaviour>().getMultiplier();
                        display_behaviour.addScore(score_to_add);
                        display_behaviour.updateStandbyMessage("Perfect landing!\n" + score_to_add.ToString() + " points");
                        lander_animator.SetTrigger("LanderToDefault");
                        GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
                    }
                    else if (Mathf.Abs(speed_x) <= 25.0f && Mathf.Abs(speed_y) <= 25.0f)
                    {
                        int score_to_add = WORSE_SCORE * collision.gameObject.GetComponent<WinColliderBehaviour>().getMultiplier();
                        display_behaviour.addScore(score_to_add);
                        display_behaviour.updateStandbyMessage("Bumpy landing\n" + score_to_add.ToString() + " points");
                        lander_animator.SetTrigger("LanderToDefault");
                        GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
                    }
                    else
                    {                        
                        if (fuel <= 0.0f)
                        {
                            lander_animator.SetTrigger("LanderExplosion");
                            GameBehaviour.changeGameState(GameBehaviour.GameState.Finish);
                        }
                        else
                        {
                            removeFuel();
                        }
                    }
                }
                else
                {                    
                    if (fuel <= 0.0f)
                    {
                        lander_animator.SetTrigger("LanderExplosion");
                        GameBehaviour.changeGameState(GameBehaviour.GameState.Finish);
                    }
                    else
                    {
                        removeFuel();
                    }
                }
            }
            if (collision.gameObject.tag.Equals("MoonSurface"))
            {
                if (fuel <= 0.0f)
                {
                    lander_animator.SetTrigger("LanderExplosion");
                    GameBehaviour.changeGameState(GameBehaviour.GameState.Finish);
                }
                else
                {
                    removeFuel();
                }
            }
            has_collided = true;
        }
    }
}
