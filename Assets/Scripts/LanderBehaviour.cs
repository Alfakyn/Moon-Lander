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

    private enum Tilt { 
        n_ninety = -6 , n_seventyfive = -5, n_sixty = -4, n_fortyfive = -3, n_thirty = -2, n_fifteen = -1,
        zero = 0, 
        p_fifteen = 1, p_thirty = 2, p_fortyfive = 3, p_sixty = 4, p_seventyfive = 5, p_ninety = 6};
    private Tilt tilt;


    // Start is called before the first frame update
    void Start()
    {
        gravity_scale = rigidbody2d.gravityScale;
        initializeLander();
        fuel = 1000;
    }

    public void initializeLander()
    {
        int position_x = Random.Range(-960, 961);
        transform.position = new Vector2(position_x, 370);

        int direction_x = -(position_x / Mathf.Abs(position_x));
        rigidbody2d.velocity = new Vector2(82 * direction_x, 0);

        transform.rotation = Quaternion.Euler(Vector3.zero);
        tilt = Tilt.zero;
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameBehaviour.game_state)
        {
            case GameBehaviour.GameState.Running:
                checkTurnInput();
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
                checkBoosterInput();
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
        if (Input.GetKey(KeyCode.UpArrow) == true)
        {
            rigidbody2d.velocity += new Vector2(transform.up.x, transform.up.y) * ACCELERATION * gravity_scale * Time.fixedDeltaTime;
            fuel -= FUEL_DEPLETE_RATE * Time.fixedDeltaTime;
        }
    }

    void checkTurnInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) == true)
        {
            if ((int)tilt < 6)
            {
                ++tilt;
                updateRotation();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) == true)
        {
            if ((int)tilt > -6)
            {
                --tilt;
                updateRotation();
            }
        }
    }

    private void updateRotation()
    {
        switch (tilt)
        {
            case Tilt.n_ninety:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
                break;
            case Tilt.n_seventyfive:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, -75.0f);
                break;
            case Tilt.n_sixty:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, -60.0f);
                break;
            case Tilt.n_fortyfive:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);
                break;
            case Tilt.n_thirty:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, -30.0f);
                break;
            case Tilt.n_fifteen:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, -15.0f);
                break;
            case Tilt.zero:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                break;
            case Tilt.p_fifteen:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 15.0f);
                break;
            case Tilt.p_thirty:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 30.0f);
                break;
            case Tilt.p_fortyfive:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                break;
            case Tilt.p_sixty:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 60.0f);
                break;
            case Tilt.p_seventyfive:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 75.0f);
                break;
            case Tilt.p_ninety:
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                break;
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
        if (collision.gameObject.tag.Equals("WinCollider"))
        {
            if(gameObject.transform.eulerAngles.z == 0)
            {
                if (speed_x <= 15 && speed_y <= 15)
                {
                    display_behaviour.addScore(BASE_SCORE * collision.gameObject.GetComponent<WinColliderBehaviour>().getMultiplier());
                    Debug.Log("You win");
                }
                else
                {
                    Debug.Log("You lose. Landed with too much speed");
                }
            }
            else
            {
                //Make the lander explode
                Debug.Log("You lose. Landed with rotation");
            }
        }
        if (collision.gameObject.tag.Equals("MoonSurface"))
        {
            //Make the lander explode
            Debug.Log("You lose. Collision on a wall");
        }

        GameBehaviour.changeGameState(GameBehaviour.GameState.Standby);
    }
}
