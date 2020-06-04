using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> sprites;
    [SerializeField] Color ColorA;
    [SerializeField] Color ColorB;
    [SerializeField] Color ColorC;
    [SerializeField]  float fideintimeA = 120;
    [SerializeField] float fideintimeB = 240;
    public void Go() {
        
        StartCoroutine(ColorAtoB());
    }
    

    IEnumerator ColorAtoB() {
        for (float t = 0.01f; t < fideintimeA; t += 0.1f) {
           Color tmp_color = Color.Lerp(ColorA, ColorB, t / fideintimeA);
            foreach (SpriteRenderer sr in sprites) {
                sr.color = tmp_color;
            }
            yield return null;
        }
        StartCoroutine(ColorBtoC());

    }
    IEnumerator ColorBtoC() {
        for (float t = 0.01f; t < fideintimeB; t += 0.1f) {
            Color tmp_color = Color.Lerp(ColorB, ColorC, t / fideintimeB);
            foreach (SpriteRenderer sr in sprites) {
                sr.color = tmp_color;
            }
            yield return null;
        }
        

    }

}
