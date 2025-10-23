using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int stageNum = 2;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
            SceneManager.LoadScene(stageNum);
        stageNum++;
    }
}
