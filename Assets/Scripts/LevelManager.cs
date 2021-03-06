﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//***********************************************************************
//This is used to handle level and creation of player and player history
//***********************************************************************
public class LevelManager : MonoBehaviour {

    [Serializable]
    public struct LevelData {
        public int start;
        public int pickup;
        public int exit;

        public LevelData(int start, int pickup, int exit) {
            this.start = start;
            this.pickup = pickup;
            this.exit = exit;
        }
    }

    #region Fields

    [Header("Debug")]
    [SerializeField] private bool cheat;

    [Header("Level Data")]
    [SerializeField] private List<LevelData> level_data = new List<LevelData>();

    [SerializeField] private int current_level_no;

    [SerializeField] private int spawn_wait;
    [SerializeField] public DayNight day_night;

    [SerializeField] public int change_at;

    [Header("Prefabs")]
    [SerializeField] private GameObject history_player_prefab;

    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject pickup_prefab;
    [SerializeField] private GameObject history_portal_prefab;
    [SerializeField] private GameObject player_portal_prefab;
    [SerializeField] private GameObject exit_portal_prefab;
    [SerializeField] private GameObject incursion_fix_prefab;
    [SerializeField] private GameObject ingame_menu;

    [Header("UI")]
    [SerializeField] private Slider incursion_meter;

    [SerializeField] private Text txtscore;

    // [SerializeField] private Image whitescreen;
    [SerializeField] private Transition transition;

    [SerializeField] private List<Image> item_pickedup;
    [SerializeField] private List<Image> item_lives;
    [SerializeField] private Text txtTimer;

    [Header("Point")]
    [SerializeField] private int pickup_points = 10;

    [SerializeField] private int exit_points = 50;

    [Header("Invulnerability")]
    [SerializeField] private int inv_seconds = 2;

    [SerializeField] private int time_freeze_seconds = 5;
    public Boolean invincible = false;

    [Header("Incursion")]
    [SerializeField] private int incursions;

    [SerializeField] private int max_incursions;
    [SerializeField] private int Incursion_Fix_at = 5;
    [SerializeField] private int Incursion_Fix_at_location = 5;
    [NonSerialized] private bool incursion_fix = false;
    [SerializeField] private Animator incursion_animator;

    [Header("Timer")]
    [SerializeField] private int start_time = 30;

    private int current_time;
    private Coroutine timer_obj;
    public bool time_frozen;

    [Header("Audio")]
    [SerializeField] public AudioClip time_freeze_audio;

    [SerializeField] public AudioClip crystal_used_audio;
    private AudioSource audioSource;

    private int items_collected;
    public int current_level;
    private GameObject exit_portal;
    private GameObject current_spawn_item;
    private GameObject incursion_fix_item;
    private bool day_night_flag = false;
    private GameObject player;
    private PersistentManagerScript score;
    private SpawnPointList spawn_points;
    [NonSerialized] public List<GameObject> history_players = new List<GameObject>();
    [NonSerialized] public List<FileStream> fs = new List<FileStream>();
    [NonSerialized] public List<int> session_count = new List<int>();

    #endregion Fields

    #region System Events

    private void Start() {
        CleanUp();
        day_night_flag = false;
        score = GameObject.Find("PersistentScoreManager").GetComponent<PersistentManagerScript>();
        score.current_level = current_level_no;
        transition.BeginingFadein();
        // SetWhiteToTransparent();
        spawn_points = GetComponent<SpawnPointList>();
        current_level = 0;
        StartCoroutine(Wait_And_Spawn_New_Player(0));
        //Spawn_New_Player(0);
        IncPickupScore(0);
        Spawn_Spawn_Pickup_Item(0);
        SetupIncursionMetere();
        ResetItemCollected();
        UI_DrawLives();
        incursion_fix = false;
        time_frozen = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnApplicationQuit() {
        CloseAllStreams();
    }

    #endregion System Events

    #region Lives

    public void UI_DrawLives() {
        for (int i = 0; i < this.item_lives.Count; i++) {
            if (i <= score.lives - 1) {
                item_lives[i].enabled = true;
            } else {
                item_lives[i].enabled = false;
            }
        }
    }

    public void DecLife() {
        score.SubLife();
        UI_DrawLives();
    }

    public void AddLife() {
        if (score.AddLife()) {
            //We got a full set of lifes so time will get frozen
            StartCoroutine(FreezeTime());
        }
        UI_DrawLives();
    }

    #endregion Lives

    #region TimeCrystal Functions

    public void DecItemCollected() {
        items_collected = current_level;
        for (int i = 0; i < item_pickedup.Count; i++) {
            if (i >= items_collected) {
                item_pickedup[i].enabled = false;
            } else {
                item_pickedup[i].enabled = true;
            }
        }
    }

    public void ResetItemCollected() {
        foreach (Image e in item_pickedup) {
            e.enabled = false;
        }
        items_collected = 0;
    }

    public void IncItemCollected() {
        item_pickedup[items_collected].enabled = true;
        items_collected++;
    }

    public void TimeCrystalUsed() {
        if (score.lives > 0) {
            if (time_frozen == false) {
                DecLife();
                if (incursions == 0) {
                    StartCoroutine(FreezeTime());
                } else {
                    DecIncursion();
                }
            } else {
                if (incursions > 0) {
                    DecLife();
                    DecIncursion();
                }
            }
        }
    }

    #endregion TimeCrystal Functions

    #region Incursions Functions

    private void DecIncursion() {
        incursions--;
        if (incursion_meter != null) {
            if (crystal_used_audio != null) { audioSource.PlayOneShot(crystal_used_audio, 0.7F); }
            if (incursions < 0) { incursions = 0; }
            incursion_meter.value = incursions;
        }
        StartCoroutine(MakeInvincible());
    }

    private void IncIncursion() {
        if (incursion_meter != null) {
            if (cheat == false) {
                score.no_hit_bonus = false;
                incursions++;
            }
            if (incursion_animator != null) {
                incursion_animator.Play("aberation", -1, 0f);
            }
            incursion_meter.value = incursions;
        }
        StartCoroutine(MakeInvincible());
    }

    private void ResetIncursion() {
        if (incursion_meter != null) {
            incursions = 0;
            incursion_meter.value = incursions;
        }
    }

    private void SetupIncursionMetere() {
        if (incursion_meter != null) {
            incursion_meter.maxValue = max_incursions;
            incursion_meter.value = incursions;
        }
    }

    #endregion Incursions Functions

    #region Timer Functions

    private void Start_Timer() {
        Stop_Timer(false);
        current_time = start_time;
        timer_obj = StartCoroutine(Timer());
    }

    private void Stop_Timer(bool addtime) {
        if (timer_obj != null) {
            StopCoroutine(timer_obj);
        }
        if (addtime == true) {
            score.AddRemainingTime(current_time);
        }
        txtTimer.text = "Time: " + start_time.ToString();
    }

    private IEnumerator Timer() {
        do {
            txtTimer.text = "Time: " + current_time.ToString();
            yield return new WaitForSeconds(1);
            current_time--;
        } while (current_time > 0);
        txtTimer.text = "Time: " + current_time.ToString();
        SpaceTimeDestroyed();
    }

    #endregion Timer Functions

    #region FileManager

    private void CleanUp() {
        if (current_spawn_item != null) { Destroy(current_spawn_item); }
        if (exit_portal != null) { Destroy(exit_portal); }
        if (current_spawn_item != null) { Destroy(current_spawn_item); }
        if (incursion_fix_item != null) { Destroy(incursion_fix_item); }
    }

    public void CloseAllStreams() {
        foreach (FileStream fs in this.fs) {
            if (fs != null) {
                fs.Close();
            }
        }
        this.fs.Clear();
        session_count.Clear();
    }

    public void CloseStream(int level) {
        if (session_count[level] <= 0) {
            session_count[level] = session_count[level] - 1;
            // Debug.Log("Closing FS " + level);
            this.fs[level].Close();
            this.fs[level] = null;
        }
    }

    public FileStream GetFileStream(int level, bool read) {
        FileStream fs = null;
        string path = Application.persistentDataPath + "/level" + level + ".json";
        bool need_to_add = true;
        if (this.fs.Count >= level + 1) {
            // Debug.Log("Retreaving FS " + level);
            fs = this.fs[level];
            need_to_add = false;
        }
        if (fs == null) {
            if (read == false) {
                //Debug.Log("Create FS " + level);
                fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            } else {
                //Debug.Log("open FS " + level);
                fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            if (need_to_add == true) {
                this.fs.Add(fs);
                session_count.Add(1);
            } else {
                this.fs[level] = fs;
                session_count[level] = session_count[level] + 1;
            }
        } else {
            session_count[level] = session_count[level] + 1;
            // Debug.Log("Retreaving FS " + level);
        }
        return fs;
    }

    #endregion FileManager

    #region Portal

    public GameObject Spawn_Entry_Portal(GameObject portal_prefab, int level = 0) {
        current_spawn_item = Instantiate(portal_prefab, spawn_points.spawn_point_list[level_data[level].start].transform.position, Quaternion.identity);
        return current_spawn_item;
    }

    public GameObject Spawn_Exit_Portal(int level = 0) {
        if (exit_portal != null) { Destroy(exit_portal); }
        exit_portal = Instantiate(exit_portal_prefab, spawn_points.spawn_point_list[level_data[level].exit].transform.position, Quaternion.identity);
        Animator spawn_portal_a = exit_portal.GetComponent<Animator>();
        spawn_portal_a.SetInteger("state", 1);
        Game2DTrigger trig = exit_portal.GetComponent<Game2DTrigger>();
        trig.level_manager = GetComponent<LevelManager>();
        return exit_portal;
    }

    public void CloseExitPortal() {
        if (exit_portal != null) { Destroy(exit_portal); exit_portal = null; }
    }

    #endregion Portal

    #region LevelMangment

    public void SpaceTimeDestroyed() {
        CleanUpAllElements();
        transition.EndLevelFadeOut("EndGame");
    }

    public void CleanUpAllElements() {
        //*********************
        //Destroy History Items
        //*********************
        StopAllCoroutines();
        foreach (GameObject go in history_players) {
            Destroy(go);
        }
        if (current_spawn_item != null) { Destroy(current_spawn_item); }
        if (exit_portal != null) { Destroy(exit_portal); }
        if (current_spawn_item != null) { Destroy(current_spawn_item); }
        if (incursion_fix_item != null) { Destroy(incursion_fix_item); }
        Destroy(player);
        CloseExitPortal();
        history_players.Clear();
        current_level = 0;
        ResetIncursion();
        CloseAllStreams();
        Spawn_Spawn_Pickup_Item(current_level);
        ResetItemCollected();
    }

    #endregion LevelMangment

    #region SpawnPoint Functions

    private void SetSpawnPointRed(int level) {
        SpriteRenderer sr = spawn_points.spawn_point_list[level_data[level].start].GetComponent<SpriteRenderer>();
        sr.color = Color.red;
    }

    public void SetSpawnPointGreen(int level) {
        SpriteRenderer sr = spawn_points.spawn_point_list[level_data[level].start].GetComponent<SpriteRenderer>();
        sr.color = Color.green;
    }

    public void SetGameObjectToPos(GameObject go, int level) {
        if (spawn_points == null) { spawn_points = GetComponent<SpawnPointList>(); }
        go.transform.position = spawn_points.spawn_point_list[level_data[level].start].transform.position;
    }

    public void Spawn_Spawn_Pickup_Item(int level = 0) {
        current_spawn_item = Instantiate(pickup_prefab, spawn_points.spawn_point_list[level_data[level].pickup].transform.position, Quaternion.identity);
        SpriteRenderer sr = current_spawn_item.GetComponent<SpriteRenderer>();
        sr.sprite = item_pickedup[level].sprite;
        sr.color = item_pickedup[level].color;
        Game2DTrigger trig = current_spawn_item.GetComponent<Game2DTrigger>();
        trig.level_manager = GetComponent<LevelManager>();
    }

    #endregion SpawnPoint Functions

    #region Make Invincible or Freez Time

    private IEnumerator MakeInvincible() {
        invincible = true;
        yield return new WaitForSeconds(inv_seconds);
        invincible = false;
    }

    private IEnumerator FreezeTime() {
        time_frozen = true;
        if (time_freeze_audio != null) { audioSource.PlayOneShot(time_freeze_audio, 0.7F); }
        yield return new WaitForSeconds(time_freeze_seconds);
        time_frozen = false;
    }

    #endregion Make Invincible or Freez Time

    #region Actions

    public void IncLevel() {
        current_level++;
    }

    private void IncPickupScore(int points) {
        if (txtscore != null) {
            score.score = score.score + points;
            txtscore.text = "Score: " + score.score.ToString();
        }
    }

    public void TransportTo(GameObject go, Target target) {
        if (target.xpos_only == true) {
            go.transform.position = new Vector3(target.target.position.x, go.transform.position.y, go.transform.position.z);
        } else {
            go.transform.position = new Vector3(target.target.transform.position.x, target.target.transform.position.y, go.transform.position.z);
        }
    }

    public void Time_Crystal_Collected(GameObject go) {
        AddLife();
        Destroy(go);
    }

    public void ItemCollected(GameObject go) {
        this.Spawn_Exit_Portal(current_level);
        IncPickupScore(pickup_points);
        IncItemCollected();
        if (items_collected >= Incursion_Fix_at && incursion_fix == false) {
            incursion_fix = true;
            StartCoroutine(Wait_Spawn_New_Incursion_Fix());
        }
        Destroy(go);
    }

    public void Collision_With_History_Player() {
        if (invincible == false) {
            IncIncursion();
            if (incursions >= max_incursions) {
                //*********************
                SpaceTimeDestroyed();
            }
        }
    }

    #endregion Actions

    #region Player And History Player

    private IEnumerator Wait_And_Spawn_New_History_Player(int level = 0) {
        this.SetSpawnPointRed(level);
        GameObject spawn_portal = Spawn_Entry_Portal(history_portal_prefab, level);//Spawn a portal
        yield return new WaitForSeconds(spawn_wait);
        this.SetSpawnPointGreen(level);
        if (spawn_portal != null) {
            Animator spawn_portal_a = spawn_portal.GetComponent<Animator>();
            if (spawn_portal_a != null) { spawn_portal_a.SetInteger("state", 1); }
        }
        yield return new WaitForSeconds(1);
        Spawn_New_History_Player(level);
        if (spawn_portal != null) { Destroy(spawn_portal); }
    }

    private IEnumerator Wait_And_Spawn_New_Player(int level = 0) {
        this.SetSpawnPointRed(level);
        GameObject spawn_portal = Spawn_Entry_Portal(player_portal_prefab, level);//Spawn a portal
        yield return new WaitForSeconds(spawn_wait);
        this.SetSpawnPointGreen(level);
        Animator spawn_portal_a = spawn_portal.GetComponent<Animator>();
        spawn_portal_a.SetInteger("state", 1);//Make it grow
        yield return new WaitForSeconds(1);
        Spawn_New_Player(level);
        Destroy(spawn_portal);
    }

    public void Spawn_New_Player(int level) {
        GameObject new_player = Instantiate(player_prefab, spawn_points.spawn_point_list[level_data[level].start].transform.position, Quaternion.identity);
        PlayerMovement pm = new_player.GetComponent<PlayerMovement>();
        player = new_player;
        pm.current_level = level;
        pm.is_history_player = false;
        pm.level_manager = GetComponent<LevelManager>();
        pm.fs = GetFileStream(level, false);
        StartCoroutine(MakeInvincible());
        current_time = start_time;
        Start_Timer();
    }

    public void Spawn_New_History_Player(int level = 0) {
        GameObject new_player = Instantiate(history_player_prefab, spawn_points.spawn_point_list[level_data[level].start].transform.position, Quaternion.identity);
        history_players.Add(new_player);
        PlayerMovement pm = new_player.GetComponent<PlayerMovement>();
        pm.current_level = level;
        pm.is_history_player = true;
        pm.level_manager = GetComponent<LevelManager>();
        Game2DTrigger trig = new_player.GetComponent<Game2DTrigger>();
        trig.level_manager = pm.level_manager;
        pm.fs = GetFileStream(level, true);
    }

    #endregion Player And History Player

    #region LargeTime Crystals

    private IEnumerator Wait_Spawn_New_Incursion_Fix() {
        GameObject spawn_portal = Instantiate(player_portal_prefab, spawn_points.spawn_point_list[Incursion_Fix_at_location].transform.position, Quaternion.identity);
        yield return new WaitForSeconds(spawn_wait);
        Animator spawn_portal_a = spawn_portal.GetComponent<Animator>();
        spawn_portal_a.SetInteger("state", 1);//Make it grow
        yield return new WaitForSeconds(1);
        Spawn_New_Incursion_Fix(Incursion_Fix_at_location);
        Destroy(spawn_portal);
    }

    public void Spawn_New_Incursion_Fix(int Incursion_Fix_at_location) {
        GameObject new_player = Instantiate(incursion_fix_prefab, spawn_points.spawn_point_list[Incursion_Fix_at_location].transform.position, Quaternion.identity);
        Game2DTrigger trig = new_player.GetComponent<Game2DTrigger>();
        trig.level_manager = this;
    }

    #endregion LargeTime Crystals

    #region Either Die or Reach Goal

    public void Player_Died() {
        DecLife();
        current_level--;
        if (current_level < 0) { current_level = 0; }
        if (score.lives < 0) {
            SpaceTimeDestroyed();
        } else {
            //*********************
            //Destroy History Items
            //*********************
            StopAllCoroutines();
            foreach (GameObject go in history_players) {
                Destroy(go);
            }
            CleanUp();

            Destroy(player);
            CloseExitPortal();
            history_players.Clear();
            time_frozen = false;
            ResetIncursion();
            CloseAllStreams();
            Spawn_Spawn_Pickup_Item(current_level);
            //ResetItemCollected();
            DecItemCollected();
            for (int i = 0; i < current_level; i++) {
                StartCoroutine(Wait_And_Spawn_New_History_Player(i));
            }
            StartCoroutine(Wait_And_Spawn_New_Player(current_level));
        }
    }

    public void EndGoal(GameObject go) {
        if (go != null) {
            PlayerMovement pm = go.GetComponent<PlayerMovement>();
            if (pm.is_history_player == false) {
                //******************
                //This is the player
                //******************
                Stop_Timer(true);
                CloseStream(current_level);
                Destroy(go);
                CloseExitPortal();
                IncLevel();
                time_frozen = false;

                if (cheat && current_level >= 1) {
                    CleanUpAllElements();
                    transition.EndLevelFadeOut("score");
                } else if (current_level >= level_data.Count) {
                    CleanUpAllElements();
                    transition.EndLevelFadeOut("score");
                } else {
                    StartCoroutine(Wait_And_Spawn_New_Player(current_level));
                    if (current_level > 0) {
                        StartCoroutine(Wait_And_Spawn_New_History_Player(0));
                    }
                    IncPickupScore(exit_points);
                    Spawn_Spawn_Pickup_Item(current_level);
                }
                if (day_night != null && change_at == current_level) {
                    if (day_night_flag == false) {
                        day_night_flag = true;
                        day_night.Go();
                    }
                }
            } else {
                int tmp_level = pm.current_level + 1;
                CloseStream(pm.current_level);
                this.history_players.Remove(player);
                Destroy(go);
                StartCoroutine(Wait_And_Spawn_New_History_Player(tmp_level));
            }
        }
    }

    #endregion Either Die or Reach Goal
}