using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    Rigidbody rb;
    public float speed = 1f;

    public float bonusSpeed = 0;

    public float turnSpeed = 1f;

    public float jumpSpeed = 1f;


    public float rotation = 0;

    public GameObject ballHub;


    public float distanceToGround;

    public float velocity;

    public Color color;

    private Color lastColor;

    public AudioSource rollingAudio;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = rb.velocity.magnitude;
        rotation = (rotation + Input.GetAxis("Horizontal") * turnSpeed) % 360; 
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
            distanceToGround = hit.distance;

            var v3 = new Vector3(0, 0, Input.GetAxis("Vertical") * (speed + bonusSpeed));
            rb.AddForce(Quaternion.Euler(0, rotation, 0) * v3);

            if (distanceToGround < 1) {
                if (Input.GetButton("Jump")) {
                    rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                }
                
            }
        }
        if (color != lastColor) {
            GetComponent<Renderer>().material.color = color;
            ballHub.GetComponent<Renderer>().material.color = color;
            lastColor = color;
        }
    
 
        var scaledVelocity = Remap(Mathf.Clamp(velocity, 0, 20), 0, 20, 0f, 1.5f);
 
        // set volume based on volume curve
        rollingAudio.volume = distanceToGround < 1 ? 1 : 0;
 
        // set pitch based on pitch curve
        rollingAudio.pitch = scaledVelocity;
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
