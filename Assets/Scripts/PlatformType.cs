using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformType : MonoBehaviour
{
    [SerializeField] private bool IsOn;
    [SerializeField] Game2DTrigger  game_trigger;
    private Animator animation;
    private void Start() {
        Animator animation = GetComponent<Animator>();
        animation.SetBool("red", !IsOn);
  

    }
    public void random() {
        Animator animation = GetComponent<Animator>();
        float res= Random.Range(0, 10);
        if (res > 5) {
            IsOn = true;
        } else {
            IsOn = false;
        }
        animation.SetBool("red", !IsOn);
    }
    private void OnValidate() {
        Animator animation = GetComponent<Animator>();
        animation.SetBool("red", !IsOn);
        
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (game_trigger != null) {
            game_trigger.death = !IsOn;
        }
    }
}
