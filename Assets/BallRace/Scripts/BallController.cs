using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public Model.Ball ball = Model.Game.instance.ball;

    Rigidbody rb;
    // public float speed = 1f;

    // public float bonusSpeed = 0;

    // public float turnSpeed = 1f;

    // public float jumpSpeed = 1f;


    // public float rotation = 0;

    public GameObject ballHub;


    // public float distanceToGround;

    // public float velocity;

    // public Color color;

    private Color lastColor;

    public AudioSource rollingAudio;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private Vector3 lastPosition = Vector3.zero;


    void Update() {
        if (ball.position != lastPosition) {
            transform.position = ball.position;
        } else {
            ball.position = transform.position;
        }
        lastPosition = ball.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // if (ball.position != lastPosition) {
        //     transform.position = ball.position;
        // } else {
        //     ball.position = transform.position;
        // }
        // lastPosition = ball.position;
        ball.velocity = rb.velocity.magnitude;
        ball.rotation = (ball.rotation + Input.GetAxis("Horizontal") * ball.turnSpeed) % 360; 
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
            ball.distanceToGround = hit.distance;

            var v3 = new Vector3(0, 0, Input.GetAxis("Vertical") * (ball.speed + ball.bonusSpeed));
            rb.AddForce(Quaternion.Euler(0, ball.rotation, 0) * v3);

            if (ball.distanceToGround < 1) {
                if (Input.GetButton("Jump")) {
                    rb.AddForce(Vector3.up * ball.jumpSpeed, ForceMode.Impulse);
                }
            }
        }
        if (ball.color != lastColor) {
            GetComponent<Renderer>().material.color = ball.color;
            ballHub.GetComponent<Renderer>().material.color = ball.color;
            lastColor = ball.color;
        }
    
 
        var scaledVelocity = Remap(Mathf.Clamp(ball.velocity, 0, 20), 0, 20, 0f, 1.5f);
 
        // set volume based on volume curve
        rollingAudio.volume = ball.distanceToGround < 1 ? 1 : 0;
 
        // set pitch based on pitch curve
        rollingAudio.pitch = scaledVelocity;
    }

    void LateUpdate()
    {

    }
 
 
    // https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    // void OnCollisionEnter() {
    //     fallAudio.pitch = Remap(Mathf.Clamp(velocity, 0, 20), 0, 20, 0f, 1.5f);
    //     fallAudio.Play();
    // }
}
