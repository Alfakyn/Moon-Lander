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

    public AudioSource boosters_audio_source;
    private const float AUDIO_CLIPS_VOLUME = 0.3f;
    public AudioClip explosion_audio_clip;

    public Animator lander_animator;

    private float altitude;

    // Start is called before the first frame update
    void Start()
    {
        gravity_scale = rigidbody2d.gravityScale;
        initializeLander();
        fuel = INITIAL_FUEL;

        z_rotation = 0.0f;

        boosters_audio_source.volume = 0.0f;

        altitude = 0.0f;
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
            boosters_audio_source.volume = AUDIO_CLIPS_VOLUME;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow) == true || Input.GetKeyUp(KeyCode.W) == true || fuel<0.0f)
        {
            lander_animator.SetBool("BoosterInput", false);
            boosters_audio_source.volume = 0.0f;
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
                calculateAltitude();
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

    private void checkFuelOnCrash()
    {
        if (fuel <= 0.0f)
        {            
            GameBehaviour.changeGameState(GameBehaviour.GameState.Finish);
        }
        else
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
            GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
        }

        lander_animator.SetTrigger("LanderExplosion");
        AudioSource.PlayClipAtPoint(explosion_audio_clip, transform.position, AUDIO_CLIPS_VOLUME);
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
                        checkFuelOnCrash();
                    }
                }
                else
                {
                    checkFuelOnCrash();
                }
            }
            if (collision.gameObject.tag.Equals("MoonSurface"))
            {
                checkFuelOnCrash();
            }
            has_collided = true;
        }
    }

    private void calculateAltitude()
    {
        Vector2 starting_point = transform.position - 13 * transform.up;
        RaycastHit2D raycast_hit_2d = Physics2D.Raycast(starting_point, Vector2.down);
        altitude = raycast_hit_2d.distance;
    }

    public float getAltitude()
    {
        return altitude;
    }
}
