using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SwordController : MonoBehaviour
{

    PlayerController playerController;
   
   
    void Start()
    {
        playerController = transform.root.GetComponent<PlayerController>();
    }

   

    void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag != "Weapon")
            return;
        if (other.tag == "Monster")
        {
            if (playerController.playerState == PlayerController.PLAYER_STATE.ATTACK)
                Debug.Log(other.name);
        }
    }
}
