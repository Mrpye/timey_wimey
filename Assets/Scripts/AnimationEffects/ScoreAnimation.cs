using System.Collections;
using UnityEngine;

public class ScoreAnimation : MonoBehaviour {
    [SerializeField] private ShadowText score;
    [SerializeField] private ShadowText time_bonus;
    [SerializeField] private ShadowText no_time_crystal_used_bonus;
    [SerializeField] private ShadowText no_hit_bonus;
    [SerializeField] private ShadowText mega_bonus;
    [SerializeField] private GameObject continue_button;
    [SerializeField] private MenuOptions menu;
    [SerializeField] private AudioSource audio;


    [SerializeField] private float animation_pause = 0.05f;
    [SerializeField] private int point_time_bonus = 10;
    [SerializeField] private int points_no_time_crystal_used_bonus = 500;
    [SerializeField] private int points_no_hit_bonus = 500;
    [SerializeField] private int points_mega_bonus = 1000;

    private PersistentManagerScript score_manager;
    private int ui_no_time_crystal_bonus = 0;
    private int ui_no_hit_bonus = 0;
    private int ui_points_no_hit_bonus = 0;
    private int ui_mega_bonus = 0;
    private bool can_continue;
    private void Start() {

        score_manager = GameObject.Find("PersistentScoreManager").GetComponent<PersistentManagerScript>();

       // score_manager.score = 0;
       // score_manager.remaining_time = 15*10;
        ui_no_time_crystal_bonus = 0;
        ui_points_no_hit_bonus = 0;
        can_continue = false;
        continue_button.SetActive(false);
        //no_time_crystal_used_bonus.gameObject.SetActive(false);
        //no_hit_bonus.gameObject.SetActive(false);
        //mega_bonus.gameObject.SetActive(false);

        if (score_manager.no_used_life_bonus && score_manager.no_hit_bonus) {
            ui_mega_bonus = points_mega_bonus;
            mega_bonus.gameObject.SetActive(true);
        }
        if (score_manager.no_used_life_bonus) {
            ui_no_time_crystal_bonus = points_no_time_crystal_used_bonus;
        }
        if (score_manager.no_hit_bonus) {
            ui_points_no_hit_bonus = points_no_hit_bonus;
        }

        
        UpdateDisplay();
        StartCoroutine(InitalPause());
    }


    private void Update() {
        if(can_continue && Input.GetButtonDown("Jump")) {
            menu.LoadNextLevel();
        }
    }
    private IEnumerator InitalPause() {
        
        yield return new WaitForSeconds(4);
        StartCoroutine(AnimateTimeBous());

    }

    private void UpdateDisplay() {

        score.UpdateText("Score: " + score_manager.score.ToString());
        time_bonus.UpdateText("Remaining Time: " + score_manager.remaining_time.ToString());
        no_time_crystal_used_bonus.UpdateText("No Time Crytals Used: " + ui_no_time_crystal_bonus.ToString());
        no_hit_bonus.UpdateText("No Hit: " + ui_points_no_hit_bonus.ToString());
        mega_bonus.UpdateText("Mega Bonus: " + ui_mega_bonus.ToString());
        
    }

    private IEnumerator AnimateTimeBous() {
        time_bonus.Hilight();
        audio.Play();
        do {
            yield return new WaitForSeconds(animation_pause);
            if (score_manager.remaining_time< 10) {
                score_manager.score += point_time_bonus* score_manager.remaining_time;
                score_manager.remaining_time -= score_manager.remaining_time; ;
            }else{
                score_manager.score += point_time_bonus *10;
                score_manager.remaining_time -= 10;
            }
            if (score_manager.remaining_time < 0) {
                score_manager.remaining_time =0;
            }
            UpdateDisplay();
        } while (score_manager.remaining_time > 0);
        audio.Stop();
        time_bonus.NormalColor();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AnimateNoHitBonus());
    }
    private IEnumerator AnimateNoHitBonus() {
        no_hit_bonus.Hilight();
        audio.Play();
        do {
            yield return new WaitForSeconds(animation_pause);
            ui_points_no_hit_bonus -= 100;

            if (ui_points_no_hit_bonus > 0) {
                score_manager.score += 100;
            } else {
                ui_points_no_hit_bonus = 0;
            }
            
            UpdateDisplay();
        } while (ui_points_no_hit_bonus > 0);
        audio.Stop();
        no_hit_bonus.NormalColor();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AnimateNoTimeCrystalBous());
    }
    private IEnumerator AnimateNoTimeCrystalBous() {
        no_time_crystal_used_bonus.Hilight();
        audio.Play();
        do {
            yield return new WaitForSeconds(animation_pause);
            ui_no_time_crystal_bonus -= 100;
         
            if (ui_no_time_crystal_bonus > 0) {
                score_manager.score += 100;
            } else {
                ui_no_time_crystal_bonus = 0;
            }

            UpdateDisplay();
        } while (ui_no_time_crystal_bonus > 0);
        audio.Stop();
        no_time_crystal_used_bonus.NormalColor();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(AnimateMegaBonus());

    }

    private IEnumerator AnimateMegaBonus() {
        mega_bonus.Hilight();
        audio.Play();
        do {
            yield return new WaitForSeconds(animation_pause);
            ui_mega_bonus -= 100;
            if (ui_mega_bonus > 0) {
                score_manager.score += 100;
            } else {
                ui_mega_bonus = 0;
            }
            UpdateDisplay();
        } while (ui_mega_bonus > 0);
        audio.Stop();
        mega_bonus.NormalColor();
        can_continue = true;
        continue_button.SetActive(true);
    }
}