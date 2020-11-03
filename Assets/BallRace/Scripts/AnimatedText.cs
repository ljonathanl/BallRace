using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedText : MonoBehaviour
{


    public string text = "*-*-*-*-*-*-*-*";
    private string textWithSpaces;
    private Text textLabel;
    public float animationFramerate = 0.5f;
    private int count;



    // Start is called before the first frame update
    void Start()
    {
        textLabel = GetComponent<Text>();
        StartCoroutine(Animate());
    }

    public void ChangeText(string value) {
        count = 0;
        text = value;
        textWithSpaces = ("              " + text + "              ").Replace(" ", ".");
    }

    IEnumerator Animate() {
        while(true) {
            yield return new WaitForSeconds(animationFramerate);
            textLabel.text = textWithSpaces.Substring(count, Mathf.Min(14, textWithSpaces.Length - count));
            count = (count + 1) % (textWithSpaces.Length - 14); 
        }
    }
}
