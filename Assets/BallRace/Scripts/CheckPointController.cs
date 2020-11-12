using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{

    public Model.Ball ball = Model.Game.instance.ball;

    public RaceController raceController;

    public TextMesh text;
    public TextMesh textHub;

    public Renderer checkPointRenderer;
    public Renderer checkPointHubRenderer;



    void Update() {
        if (ball != null) {
            text.transform.rotation = Quaternion.Euler(0, ball.rotation, 0);
            textHub.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ball != null && other.gameObject.GetComponent<BallController>()) {
            raceController.OnCheckPoint(this);
        }
    }

    public void ChangeText(string value) {
        text.text = value;
        textHub.text = value;
    }

    public void ChangeColor(Color color) {
        color.a = 0.8f;
        checkPointHubRenderer.material.color = color;
        checkPointRenderer.material.color = color;
    }
}
