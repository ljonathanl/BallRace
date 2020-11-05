using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointController : MonoBehaviour
{

    public BallController ballController;

    public RaceController raceController;

    public TextMesh text;
    public TextMesh textHub;

    public Renderer checkPointRenderer;
    public Renderer checkPointHubRenderer;



    void Update() {
        if (ballController) {
            text.transform.rotation = Quaternion.Euler(0, ballController.rotation, 0);
            textHub.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ballController && other.gameObject == ballController.gameObject) {
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
