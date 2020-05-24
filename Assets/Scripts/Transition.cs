using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour {
    [SerializeField] private Animator animation;
    private string overide_next_screen = "";
    private PersistentManagerScript score_manager;
    private int current_level = 0;
    public bool no_in_transition=false;
    public bool no_out_transition = false;
    private void Start() {
        GameObject go = GameObject.Find("PersistentScoreManager");
        score_manager = go.GetComponent<PersistentManagerScript>();
        current_level = score_manager.current_level;
        if (no_in_transition == true) {
            animation.SetTrigger("Hide");
        } else {
            animation.SetTrigger("FadeOut");
        }
        
    }

    public void EndLevelFadeOut(string scene = "") {
        this.overide_next_screen = scene;
        if (no_out_transition == false) { animation.SetTrigger("FadeIn"); }
    }

    public void BeginingFadein() {
        animation.SetTrigger("FadeOut");
    }

    public void LoadNextLevel() {
        if (!String.IsNullOrEmpty(overide_next_screen)) {
            SceneManager.LoadScene(overide_next_screen);
        } else {
            switch (current_level) {
                case 0:
                    SceneManager.LoadScene("Zone1b");
                    break;

                case 1:
                    SceneManager.LoadScene("Zone1c");
                    break;

                case 2:
                    SceneManager.LoadScene("Zone1d");
                    break;
                case 3:
                    SceneManager.LoadScene("Complete");
                    break;

                default:
                    // SceneManager.LoadScene("Complete");
                    break;
            }
        }
    }
}