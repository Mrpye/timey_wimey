using UnityEngine;
using UnityEngine.UI;

public class ShadowText : MonoBehaviour {
    [SerializeField] private string text;
    [SerializeField] private Color color;
    [SerializeField] private Color hilight_color;
    [SerializeField] private Text txt;
    [SerializeField]  private Text shaddow;

    public void Hilight() {
        txt.color = hilight_color;
    }

    public void NormalColor() {
        txt.color = color;
    }

    private void Start() {
      /*  Transform t = gameObject.transform.Find("text");
        if (t != null) {
            txt = t.GetComponent<Text>();
        }

        Transform t1 = gameObject.transform.Find("shaddow");
        if (t1 != null) {
            shaddow = t.GetComponent<Text>();
        }*/

         NormalColor();
    }

    public void UpdateText(string text) {
        this.text = text;
/*
        if (txt == null) {
            Transform t = gameObject.transform.Find("text");
            if (t != null) {
                txt = t.GetComponent<Text>();
            }
        }

        if (shaddow == null) {
            Transform t1 = gameObject.transform.Find("shaddow");
            if (t1 != null) {
                shaddow = t1.GetComponent<Text>();
            }
        }
        */
        txt.text = text;
        shaddow.text = text;
    }

    private void OnValidate() {
        txt = gameObject.transform.Find("text").GetComponent<Text>();
        shaddow = gameObject.transform.Find("shaddow").GetComponent<Text>();
        txt.text = text;
        shaddow.text = text;
        NormalColor();
    }
}