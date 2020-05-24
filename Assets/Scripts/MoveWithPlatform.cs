﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag.ToUpper() == "PLAYER") {
            collision.collider.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag.ToUpper() == "PLAYER") {
            collision.collider.transform.SetParent(null);
        }
    }
}
