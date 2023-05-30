using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieController : MonoBehaviour
{
    public Transform cameraTransform;

    
    public bool dead = false;

    public float zombieSpeed = 3;
    Vector3 dest;

    private void Awake()
    {
        GetComponent<Animation>().Play("Z_Run_InPlace");
    }

    // Update is called once per frame
    void Update()
    {
        if(!dead) {
            if(Vector3.Distance(cameraTransform.position,transform.position) > 3f) {
                updateZombieMovement();
            }
        }
    }

    void updateZombieMovement(){
        dest = new Vector3(cameraTransform.position.x, 0f, cameraTransform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, dest, zombieSpeed*Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(cameraTransform.position - transform.position), zombieSpeed*Time.deltaTime);
    }

    

    

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ammo")
        {
            if(!dead){
                GetComponent<Animation>().Play("Z_FallingBack");
                dead = true;
            }
        }
    }
}
