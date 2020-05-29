
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuOptions : MonoBehaviour
{
    PersistentManagerScript score;

    [SerializeField] private Transition transition;

    [SerializeField] private Text version;
    private void Start() {
        if (version != null) {
            version.text = "Version: " + Application.version.ToString();
        }
    }
    private void OnValidate() {
        if (version != null) {
            version.text = "Version: " + Application.version.ToString();
        }
    }
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void ResetScore() {
        PersistentManagerScript score = GameObject.Find("PersistentScoreManager").GetComponent<PersistentManagerScript>();
        score.ResetAll();
    }

    public void MainMenu() {
        ResetScore();
        transition.EndLevelFadeOut("MainMenu");

    }

    public void LoadNextLevel() {
        transition.EndLevelFadeOut();
    }
    public void Instructions() {
        transition.EndLevelFadeOut("Instructions");
       
    }
    public void PlayAgain() {
        ResetScore();
        transition.EndLevelFadeOut("Zone1a");
    }
    public void Credits() {
        ResetScore();
        transition.EndLevelFadeOut("Credits");
    }
    public void LevelSelect() {
        ResetScore();
        transition.EndLevelFadeOut("LevelSelect");
    }
    public void Level1() {
        ResetScore();
        transition.EndLevelFadeOut("Zone1a");
    }
    public void Level2() {
        ResetScore();
        transition.EndLevelFadeOut("Zone1b");

    }
    public void Level3() {
        ResetScore();
        transition.EndLevelFadeOut("Zone1c");

    }
    public void Level4() {
        ResetScore();
        transition.EndLevelFadeOut("Zone1d");

    }
    public void Level5() {
        ResetScore();
        transition.EndLevelFadeOut("Zone1e");

    }
    public void QuitGame() {
        Application.Quit();
    }
}
