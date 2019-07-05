using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {


    private string ItemName = default(string);
    private Collider col;

    public bool pickedUp = false;

    public Image image;

    public GameObject circle;
    public GameObject target;

    public Transform respawnPoint;

    // Use this for initialization
    void Awake () {
        ItemName = this.gameObject.name;
        col = this.GetComponent<Collider>();

        col.enabled = true;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        transform.name = transform.name.Replace("(Clone)", "").Trim();
    }
	
	// Update is called once per frame
	void Update () {
	   if(target != null)
        {
            if(target.GetComponent<AIController>().state == AIController.States.Crying)
            {
                target = null;
            }
        }
	}
    public void RemoveCol()
    {
        col.enabled = false;
    }
}
