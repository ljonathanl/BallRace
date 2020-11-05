using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.Rendering.Universal;

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


    public Toggle soundToggle;
    public Toggle vfxToggle;
    public Toggle hdToggle;

    public bool isGamePaused = false;


    void Start() {
        var isSoundEnabled = PlayerPrefs.GetFloat("Sound", 1) == 1;
        var isVFXEnabled = PlayerPrefs.GetFloat("VFX", 1) == 1;
        var isHDEnabled = PlayerPrefs.GetFloat("HD", 1) == 1;
        EnableSound(isSoundEnabled);
        soundToggle.isOn = isSoundEnabled;
        EnableVFX(isVFXEnabled);
        vfxToggle.isOn = isVFXEnabled;
        EnableHD(isHDEnabled);
        hdToggle.isOn = isHDEnabled;

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
            // ambientLight.color = color;
        }
        speedText.text = Mathf.Floor(ballController.velocity * 3.6f) + "KM/H";

        if (Input.GetKeyDown(KeyCode.Escape)) {
            isGamePaused = !isGamePaused;
            Time.timeScale = isGamePaused ? 0 : 1;
            AudioListener.pause = isGamePaused;
        }
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
        PlayerPrefs.SetFloat("VFX", value ? 1 : 0);
    }

    public void EnableHD(bool value) {
        var urp = (UniversalRenderPipelineAsset) GraphicsSettings.currentRenderPipeline;
        urp.renderScale = value ? 2 : 1;
        PlayerPrefs.SetFloat("HD", value ? 1 : 0);
    }


    private static List<T> FindObjectsOfTypeAll<T>()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
            .SelectMany(g => g.GetComponentsInChildren<T>(true))
            .ToList();
    }

    public void EnableSound(bool value) 
    {
        AudioListener.volume = value ? 1 : 0;
        PlayerPrefs.SetFloat("Sound", value ? 1 : 0);
    }
}
