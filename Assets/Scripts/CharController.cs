using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour
{
    public GameObject armL;
    public GameObject armR;
    public GameObject hips;
    public GameObject spine;
    public GameObject chest;
    CamController cam;
    Rigidbody rb, armLRB, armRRB;
    Vector3 hipPos;
    Vector3 spinePos;
    Vector3 chestPos;
    public float armTorque;
    public float speed;
    public float jumpForce;
    Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CamController>();
        rb = hips.GetComponent<Rigidbody>();
        //hipPos = hips.transform.position;
        //spinePos = spine.transform.position;
        //chestPos = chest.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            direction = cam.transform.right;
            direction -= Vector3.up * direction.y;
            rb.AddForce(speed * direction * Time.deltaTime);
            //armLRB.AddTorque(armTorque);
            //armRRB.AddTo
        }
        else if (Input.GetAxisRaw("Horizontal") < 0) {
            direction = -cam.transform.right;
            direction -= Vector3.up * direction.y;
            rb.AddForce(speed * direction * Time.deltaTime);
        }
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            direction = cam.transform.forward;
            direction -= Vector3.up * direction.y;
            rb.AddForce(speed * direction * Time.deltaTime);
        }
        else if (Input.GetAxisRaw("Vertical") < 0) {
            direction = -(cam.transform.forward);
            direction -= Vector3.up * direction.y;
            rb.AddForce(speed * direction * Time.deltaTime);
        }
        if (Input.GetButtonDown("Jump")) {
            rb.AddForce(Vector3.up*jumpForce);
        }
    }
}
