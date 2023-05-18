using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entrance : MonoBehaviour
{
    public GameObject UIobject;
    // Start is called before the first frame update
    void Start()
    {
        UIobject.SetActive(false);
    }
    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(other.tag == "Player")
        {
            UIobject.SetActive(true);
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerExit(Collider other)
    {
        UIobject.SetActive(false);
    }
}
