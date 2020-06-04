using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Our singleton for storing scores and current levels
/// </summary>
public class PersistentManagerScript : MonoBehaviour
{
    public static PersistentManagerScript Instance { get; private set; }

    public int score;
    public int lives;
    public int remaining_time;
    public bool no_hit_bonus;
    public bool no_used_life_bonus;
    public int current_level;


    private void Awake() {
        if(Instance == null) {
            Instance = this;
            ResetAll();
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void AddRemainingTime(int T) {
        remaining_time = remaining_time+ T;
    }

    public bool AddLife() {
        lives++;
        if (lives > 3) {
            lives = 3;
            return true;
        }
        return false;
    }

    public void SubLife() {
        lives--;
        no_used_life_bonus = false;
        if (lives<0) {
         //   lives = 0;
        }
    }

    public void ResetLives() {
        lives = 3;
    }
    public void ResetScore() {
        score = 3;
    }

    public void ResetRemainingScore() {
        remaining_time = 3;
    }

    public void ResetNoHitBonus() {
        no_hit_bonus = true;
    }


    public void ResetAll() {
        current_level = 0;
        score = 0;
        lives = 3;
        remaining_time = 0;
        no_hit_bonus = true;
        no_used_life_bonus = true;
    }

}
