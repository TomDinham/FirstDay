using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public List<AIController> children;
    public AIController selectedChild;

    public List<Item> items;
    public Item itemToRespawn;

    public float selectWait;
    public float selectTimer;
    public bool countToSelect = true;
    public bool chooseChild = false;
	
	void Start () {

        children = new List<AIController>();
        children.AddRange(FindObjectsOfType<AIController>());
        selectTimer = 2;
	}
		
	void Update () {

        if (countToSelect == true)
        {
            selectTimer -= Time.deltaTime;

            if (selectTimer <= 0)
            {
                countToSelect = false;
                selectTimer = selectWait;
                chooseChild = true;              
            }
        }

        if(chooseChild == true)
        {
            if (selectChild())
            {
                chooseChild = false;
            }
        }

        if (itemToRespawn != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (itemToRespawn.gameObject.name == items[i].gameObject.name)
                {
                    Instantiate(items[i], items[i].respawnPoint.position, items[i].transform.rotation, null);
                    Destroy(itemToRespawn.GetComponent<Item>());
                }
            }
        }
    }

    public bool selectChild()
    {
        int rand;       
        rand = Random.Range(0, children.Count);

        if(children[rand].state != AIController.States.Idle)
        {
            return false;
        }
        else if(children[rand].state == AIController.States.Idle)
        {
            selectedChild = children[rand];
            selectedChild.state = AIController.States.Want;
            countToSelect = true;
            
        }
        return true;
    }
}
