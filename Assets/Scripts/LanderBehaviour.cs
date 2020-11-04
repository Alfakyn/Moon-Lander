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
    private const int FUEL_DEPLETE_RATE = 10;

    private const float AMOUNT_TO_ROTATE = 15.0f;
    private float z_rotation;

    private bool has_collided;

    public Animator butt_fire_animator;

    // Start is called before the first frame update
    void Start()
    {
        gravity_scale = rigidbody2d.gravityScale;
        initializeLander();
        fuel = 1000;

        z_rotation = 0.0f;
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
                    butt_fire_animator.SetBool("Using Butt Fire", false);
                }
                checkHorizontalDrag();
                speed_x = rigidbody2d.velocity.x;
                speed_y = rigidbody2d.velocity.y;
                break;
            case GameBehaviour.GameState.Standby:
                butt_fire_animator.SetBool("Using Butt Fire", false);
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
            butt_fire_animator.SetBool("Using Butt Fire", true);
        }
        else
        {
            butt_fire_animator.SetBool("Using Butt Fire", false);
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
                        display_behaviour.addScore(BASE_SCORE * collision.gameObject.GetComponent<WinColliderBehaviour>().getMultiplier());
                        Debug.Log("You win");
                        GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
                    }
                    else
                    {
                        Debug.Log("You lose. Landed with too much speed");
                        if (fuel <= 0.0f)
                        {
                            Debug.Log("Game Over, you ran out of fuel");
                            GameBehaviour.changeGameState(GameBehaviour.GameState.Finish);
                        }
                        else
                        {
                            fuel -= 200.0f;
                            if (fuel < 0.0f)
                            {
                                fuel = 0.0f;
                            }
                            GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
                        }
                    }
                }
                else
                {
                    //Make the lander explode
                    Debug.Log("You lose. Landed with rotation");
                    if (fuel <= 0.0f)
                    {
                        Debug.Log("Game Over, you ran out of fuel");
                        GameBehaviour.changeGameState(GameBehaviour.GameState.Finish);
                    }
                    else
                    {
                        fuel -= 200.0f;
                        if (fuel < 0.0f)
                        {
                            fuel = 0.0f;
                        }
                        GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
                    }
                }
            }
            if (collision.gameObject.tag.Equals("MoonSurface"))
            {
                //Make the lander explode
                Debug.Log("You lose. Collision on a wall");
                if (fuel <= 0.0f)
                {
                    Debug.Log("Game Over, you ran out of fuel");
                    GameBehaviour.changeGameState(GameBehaviour.GameState.Finish);
                }
                else
                {
                    fuel -= 200.0f;
                    if (fuel < 0.0f)
                    {
                        fuel = 0.0f;
                    }
                    GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
                }
            }
            has_collided = true;
        }
    }
}
