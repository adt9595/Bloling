using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public AudioSource ballSound;
    public AudioClip ballRoll;
    public float positionSpeed;
    public float maxForce;
    public float maxTorque;

    private Rigidbody rb;
    private GameManager gameManager;
    private CamController cam;
    private bool throwing = false;
    private bool thrown = false;
    private float minThrowForce = 8.5f;
    private float throwForce = 0;
    private float throwTorque = 0;
    private bool touchingFloor = false;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ballSound = GetComponent<AudioSource>();
        cam = FindObjectOfType<CamController>();
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 80;
        rb.isKinematic = true;
    }

    public bool isThrown() {
        return thrown;
    }

    public bool isThrowing()
    {
        return throwing;
    }

    public void setThrown(bool _thrown) {
        thrown = _thrown;
    }

    public void setThrowing(bool _throwing) {
        throwing = _throwing;
    }

    public void setTouchingFloor(bool _touchingFloor) {
        touchingFloor = _touchingFloor;
    }

    private void reset()
    {
        
    }

    private void FixedUpdate()
    {
        if (!thrown)// && !throwing)
        {
            float horizontalMove = Input.GetAxis("Horizontal");
            if (horizontalMove > 0.05 || horizontalMove < -0.05)
            {
                //Debug.Log("Should be moving");
                transform.position += Vector3.back * Mathf.Sign(horizontalMove) * positionSpeed * Time.fixedDeltaTime;
            }
        }
    }

    private void Update()
    {
        if (Input.GetButton("Launch") && !thrown) {
            throwBall(15, 0);
        }
        if (Input.GetButton("Aim") && !thrown)
        {
            if (Input.GetAxis("Mouse Y") > 0)
            {
                throwing = true;
                if (Input.GetAxis("Mouse Y") > throwForce)
                {
                    throwForce = Mathf.Min(Input.GetAxis("Mouse Y"), 4f);
                }
                if (Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(throwTorque))
                {
                    throwTorque = Input.GetAxis("Mouse X");
                }
            }
            if (Input.GetAxis("Mouse Y") == 0 && throwing && throwForce > 3f)
            {
                throwing = false;
                gameManager.setCountingTimeout(true);
                throwBall(Mathf.Max(throwForce * maxForce, 13.5f), throwTorque * maxTorque);
            }
        }
    }

    void throwBall(float force, float torque) {
        Debug.Log(force + ", " + torque);
        cam.disableArrow();
        rb.isKinematic = false;
        Debug.Log(cam.transform.forward.x + ", " + cam.transform.forward.z);
        Vector3 direction = new Vector3(cam.transform.forward.x,0,cam.transform.forward.z).normalized;

        rb.velocity = direction * force;
        rb.AddTorque(-direction * torque);

        //rb.velocity = Vector3.right * force;
        //rb.AddTorque(-Vector3.right * torque);

        throwForce = 0;
        throwTorque = 0;
        thrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pin"))
        {
            if (!gameManager.isCountingScore())
            {
                gameManager.setCountingScore(true);
            }
            ballSound.Stop();
        }
        else if (collision.gameObject.CompareTag("Floor"))
        {
            if (!touchingFloor)
            {
                touchingFloor = true;
                ballSound.volume = Random.Range(0.9f, 1.1f);
                ballSound.pitch = Random.Range(0.8f, 1.1f);
                ballSound.PlayOneShot(ballRoll);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            //touchingFloor = false;
            ballSound.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AudioTrigger"))
        {
            ballSound.Stop();
        }
    }


}
