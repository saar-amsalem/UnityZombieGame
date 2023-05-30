using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setVisibility : MonoBehaviour
{
    public GameObject self;
    // Update is called once per frame
    void start() {
        self.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("here");
            self.SetActive(!self.activeSelf);
            Debug.Log(self.activeSelf);
        }
    }
}
