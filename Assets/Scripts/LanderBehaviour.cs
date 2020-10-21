using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LanderBehaviour : MonoBehaviour
{
    public Rigidbody2D rigidbody2d;
    private float gravity_scale;
    private const int acceleration = 3;

    public float velocity_x, velocity_y;
    public float gravity;
    public float time;

    private enum Tilt { 
        n_ninety = -6 , n_seventyfive = -5, n_sixty = -4, n_fortyfive = -3, n_thirty = -2, n_fifteen = -1,
        zero = 0, 
        p_fifteen = 1, p_thirty = 2, p_fortyfive = 3, p_sixty = 4, p_seventyfive = 5, p_ninety = 6};
    private Tilt tilt;


    // Start is called before the first frame update
    void Start()
    {
        gravity_scale = rigidbody2d.gravityScale;
        rigidbody2d.velocity = new Vector2(82.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
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

    private void FixedUpdate()
    {
        checkInput();
        time += Time.fixedDeltaTime;
        gravity = Physics2D.gravity.y;
        velocity_x = rigidbody2d.velocity.x;
        velocity_y = rigidbody2d.velocity.y;
    }

    void checkInput()
    {
        if (Input.GetKey(KeyCode.UpArrow) == true)
        {
            rigidbody2d.velocity += new Vector2(transform.up.x, transform.up.y) * acceleration * gravity_scale * Time.fixedDeltaTime;
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
}
