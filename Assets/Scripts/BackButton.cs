using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour {
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
