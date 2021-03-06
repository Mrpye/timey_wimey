﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float min = 2f;
    public float max = 3f;
    public float speed = 3f;
    [SerializeField] GameObject child;

    // Use this for initialization
    void Start() {
      
       
        RectTransform rect = GetComponent<RectTransform>();


        //BoxCollider2D col = child.GetComponent<BoxCollider2D>();
        //float BoxWidth = col.bounds.size.x;
        //min = rect.offsetMin.x + (BoxWidth * 2);
        //max = rect.offsetMax.x- (BoxWidth*2);


    }
    // Update is called once per frame
    void Update() {

        if(child != null) {
            child.transform.position = new Vector3(Mathf.PingPong(Time.time * speed, max - min) + min, transform.position.y, transform.position.z);
        }
        

    }

}
