using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySceneManager : MonoBehaviour {

    Transform player;

    void Awake () {
        player = transform.Find("Player");
    }


    public void ChangePlayerPos () {
        if (Input.touchCount == 0) {
            player.position = Input.mousePosition;
        } else {
            for (var i = 0 ; i < Input.touchCount ; ++i) {
                Touch touch = Input.GetTouch(i);
                Debug.Log(touch.position);
                player.position = touch.position;
            }
        }

    }
}
