using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class EndGame : MonoBehaviour {



    public List<AIController> children;
    public int StartAmount = 0;
    public int friends = 0;
    public int amountCry = 0;
    public int amountLeft = 0;
    Scene main;
    // Use this for initialization
    void Awake ()
    {
        main = SceneManager.GetActiveScene();
        children = new List<AIController>();
        children.AddRange(FindObjectsOfType<AIController>());
        StartAmount = children.Count;
        DontDestroyOnLoad(this.gameObject);
	}
	
    public void crybaby()
    {
        amountCry++;
    }
    public void leavers()
    {
        amountLeft++;
    }
	// Update is called once per frame
	void Update () {
        children.Clear();
        children.AddRange(FindObjectsOfType<AIController>());
        if (main == SceneManager.GetActiveScene())
        {
            friends = 0;
            foreach (AIController ai in children)
            {

                if (ai.state == AIController.States.Friend)
                {
                    friends++;
                }
            }
            if (children.Count <= 0)
            {

                SceneManager.LoadScene("GameOver");


            }
            if(friends == children.Count)
            {
                SceneManager.LoadScene("GameOver");
            }
        }


    }
}
