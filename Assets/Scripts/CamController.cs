using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public Transform target;
    public Transform pinCentre;
    public Transform aimArrow;
    public Vector3 offset;
    public float smoothSpeed;
    public float rotationSmoothSpeed;
    public float rotateSpeed;
    
    private Ball ball;
    private Vector3 initialForward;
    private Quaternion initialRotation;
    private Vector3 aimArrowPosition;
    private Quaternion aimArrowRotation;
    private bool rotating = false;
    private bool moving = true;

    private void Start()
    {
        initialForward = (target.position - transform.position).normalized;
        initialRotation = transform.rotation;
        aimArrowPosition = aimArrow.position;
        aimArrowRotation = aimArrow.rotation;
        resetPosition();
        ball = FindObjectOfType<Ball>();
    }

    public bool isMoving() {
        return moving;
    }

    public void setMoving(bool _moving) {
        moving = _moving;
    }

    public void setTarget(Transform _target) {
        target = _target;
    }

    public void resetPosition() {
        transform.position = new Vector3(target.position.x, 0, 0) + offset;
        transform.forward = initialForward;
        transform.rotation = initialRotation;
        aimArrow.gameObject.SetActive(true);
        aimArrow.position = aimArrowPosition;
        aimArrow.rotation = aimArrowRotation;
    }

    private void FixedUpdate()
    {
        if (!ball.isThrown()) {
            float horizontalMove = Input.GetAxis("Horizontal");
            if (horizontalMove > 0.05 || horizontalMove < -0.05)
            {
                Vector3 moveOffset = Vector3.back * Mathf.Sign(horizontalMove) * ball.positionSpeed * Time.fixedDeltaTime;
                //Debug.Log("Should be moving");
                transform.position += moveOffset;
                aimArrow.position += moveOffset;

            }
        }
        else if (moving && ball.isThrown())
        {
            Vector3 desiredPos = new Vector3(target.position.x, 0, 0) + offset;
            Quaternion desiredRot = initialRotation;
            transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotationSmoothSpeed);
        }
    }

    private void Update()
    {
        if (Input.GetButton("Rotate") && !ball.isThrown() && Mathf.Abs(Input.GetAxis("Mouse X")) > 0.02f) {
            float rotationDelta = Mathf.Sign(Input.GetAxis("Mouse X")) * rotateSpeed * Time.deltaTime;
            transform.RotateAround(ball.transform.position, Vector3.up,rotationDelta);
            aimArrow.RotateAround(ball.transform.position, Vector3.up, rotationDelta);
        }
    }

    public void disableArrow() {
        aimArrow.gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        setTarget(pinCentre);
        moving = false;
    }
}
