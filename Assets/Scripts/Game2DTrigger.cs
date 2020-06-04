using UnityEngine;

/// <summary>
/// This class is the trigger manager
/// </summary>
public class Game2DTrigger : MonoBehaviour {
    [SerializeField] public LevelManager level_manager;
    [SerializeField] public AudioClip impact;
    [SerializeField] public bool death = false;
    [SerializeField] public GameEvent game_event;
    private AudioSource audioSource;

    private void Start() {
        audioSource = level_manager.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        //Used for when fall off into water or lava
        if (death == true && collision.tag.ToUpper() == "PLAYER") {
            if (level_manager != null) {
                if (impact != null) {
                    audioSource.PlayOneShot(impact, 1F);
                }
                level_manager.Player_Died();
            }
        }

        if (collision.tag.ToUpper() == "PLAYER" && gameObject.tag.ToUpper() == "HISTORY_PLAYER") {
            //Colision with a history player
            if (level_manager != null) {
                if (level_manager.invincible == false) {
                    if (audioSource != null) {
                        audioSource.PlayOneShot(impact, 1F);
                    }
                }
                level_manager.Collision_With_History_Player();
            }
        } else if (collision.tag.ToUpper() == "PLAYER" && gameObject.tag.ToUpper() == "GOAL") {
            //Reached our end goal
            if (level_manager != null) {
                if (audioSource != null) { audioSource.PlayOneShot(impact, 0.7F); }
                level_manager.EndGoal(collision.gameObject);
            }
        } else if (collision.tag.ToUpper() == "PLAYER" && gameObject.tag.ToUpper() == "SPAWN_ITEM") {
            //Collected a spawn item
            if (level_manager != null) {
                level_manager.ItemCollected(gameObject);
                if (audioSource != null) { audioSource.PlayOneShot(impact, 0.7F); }
            }
        } else if (collision.tag.ToUpper() == "PLAYER" && gameObject.tag.ToUpper() == "INCUR_FIX") {
            //Picked up a large Crystal
            if (level_manager != null) {
                level_manager.Time_Crystal_Collected(gameObject);
                if (audioSource != null) { audioSource.PlayOneShot(impact, 0.7F); }
            }
        } else if (collision.tag.ToUpper() == "PLAYER" && gameObject.tag.ToUpper() == "TRANSPORT") {
            //Used for transporting  left side to righ side of screen
            if (level_manager != null) {
                Target target = gameObject.GetComponent<Target>();
                if (target != null) {
                    if (game_event != null) {
                        game_event.Raise();
                    }
                    level_manager.TransportTo(collision.gameObject, target);
                }
            }
        }
    }
}