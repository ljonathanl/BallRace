using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using System.Linq;

public class Hub : MonoBehaviour
{

    public BallController ballController;

    public BallCamera ballCamera;

    public FlexibleColorPicker flexibleColorPicker;

    public Text speedText;

    public Text raceText;

    public Text leaderBoardText;

    public Text infoText;

    public Button colorButton;

    public Light ambientLight;

    public InputField nameText;

    public Color color;


    void Start() {
        // flexibleColorPicker.gameObject.SetActive(false);
        // colorButton.onClick.AddListener(() => {
        //     flexibleColorPicker.gameObject.SetActive(!flexibleColorPicker.gameObject.activeInHierarchy);
        // });
    }


    // Update is called once per frame
    void Update()
    {
        if (ballController.color != color) {
            ballController.color = color;
            // var buttonColors = colorButton.colors;
            // buttonColors.normalColor = buttonColors.pressedColor = buttonColors.selectedColor = buttonColors.highlightedColor = flexibleColorPicker.color;
            // colorButton.colors = buttonColors;
            ambientLight.color = color;
        }
        speedText.text = Mathf.Floor(ballController.velocity * 3.6f) + "KM/H";
    }


    public void EnableVFX(bool value) {
        var particles = FindObjectsOfTypeAll<ParticleSystem>();
        foreach(var particle in particles) {
            particle.gameObject.SetActive(value);
        }

        var visualEffects = FindObjectsOfTypeAll<VisualEffect>();
        foreach(var visualEffect in visualEffects) {
            visualEffect.gameObject.SetActive(value);
        }

        ballCamera.GetComponent<Volume>().enabled = value;
    }


    private static List<T> FindObjectsOfTypeAll<T>()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
            .SelectMany(g => g.GetComponentsInChildren<T>(true))
            .ToList();
    }
}
