using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private CharacterController cc;
    private AIController ai;
    private Transform startpos;
    public Transform ItemPos;
    public float moveSpeed = 5;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveRotation = Vector3.zero;
    public Item pickup = default(Item);
    private string itemName = default(string);
    public bool hasItem = false;

    public float hopTimer;
    public float hopSpeed;

    private int sweetCount, currentSweets;

    public GameObject body;

    public Text sweetsText;

    public Image aButton, xButton, yButton;

    public Image sweetIcon;
    public Transform butPos;
    void Start()
    {
        startpos = this.transform;
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        hopTimer = hopSpeed;

        sweetCount = 0;
        currentSweets = 1;

    }

    // Update is called once per frame
    void Update()
    {
        sweetsText.text = "X " + currentSweets.ToString();
        //rb.velocity = new Vector3(1, 0, 0);
        if (transform.position.y > 0.3)
        {
            transform.position = new Vector3(transform.position.x, -0.02683587f, transform.position.z);
        }
        hopTimer -= Time.deltaTime;
        if (hopTimer <= 0)
        {
            if (body.transform.position.y == 0)
            {
                body.transform.position = new Vector3(body.transform.position.x, 0.15f, body.transform.position.z);
                hopTimer = hopSpeed;
            }
            else
            {
                body.transform.position = new Vector3(body.transform.position.x, 0f, body.transform.position.z);
                hopTimer = hopSpeed;
            }
        }

        if (Input.GetAxis("Vertical") != 0 && (Input.GetAxis("Horizontal") != 0))
        {
            moveSpeed = 4;
        }
        else moveSpeed = 6f;

        if (Input.GetAxis("Vertical") > 0)
        {
            moveDirection = new Vector3(0, 0, moveSpeed * Time.deltaTime);

            moveRotation = new Vector3(Input.GetAxis("Horizontal") * 100, 0, Input.GetAxis("Vertical") * 100);
            moveDirection = Vector3.ClampMagnitude(moveDirection, moveSpeed);

            cc.Move(moveDirection);
            this.transform.rotation = Quaternion.LookRotation(moveRotation);
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            moveDirection = new Vector3(0, 0, moveSpeed * Time.deltaTime);

            moveRotation = new Vector3(Input.GetAxis("Horizontal") * 100, 0, Input.GetAxis("Vertical") * 100);
            moveDirection = Vector3.ClampMagnitude(moveDirection, moveSpeed);

            cc.Move(-moveDirection);
            this.transform.rotation = Quaternion.LookRotation(moveRotation);
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            moveDirection = new Vector3(moveSpeed * Time.deltaTime, 0, 0);

            moveRotation = new Vector3(Input.GetAxis("Horizontal") * 100, 0, Input.GetAxis("Vertical") * 100);
            moveDirection = Vector3.ClampMagnitude(moveDirection, moveSpeed);

            cc.Move(moveDirection);
            this.transform.rotation = Quaternion.LookRotation(moveRotation);

        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            moveDirection = new Vector3((moveSpeed * Time.deltaTime), 0, 0);

            moveRotation = new Vector3(Input.GetAxis("Horizontal") * 100, 0, Input.GetAxis("Vertical") * 100);
            moveDirection = Vector3.ClampMagnitude(moveDirection, moveSpeed);

            cc.Move(-moveDirection);
            this.transform.rotation = Quaternion.LookRotation(moveRotation);
        }

        // transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        if (Input.GetButton("XboxB"))
        {
            if (GetComponentInChildren<Item>() != null)
            {
                GetComponentInChildren<Item>().transform.SetParent(null);
                if (pickup != null)
                {
                    if (pickup.pickedUp == true)
                    {
                        xButton.gameObject.SetActive(false);
                        pickup.pickedUp = false;
                    }
                }
                hasItem = false;
            }

        }
        //if(hasItem == true)
        //{
        //    aButton.gameObject.SetActive(false);
        //}

    }
   
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == ("PickUp") && hasItem == false)
        {
            aButton.gameObject.SetActive(true);

            if (Input.GetButton("XboxA") && hasItem == false)
            {
                aButton.gameObject.SetActive(false);
                hasItem = true;
                pickup = other.gameObject.GetComponent<Item>();
                pickup.circle.SetActive(false);
                pickup.pickedUp = true;
                pickup.transform.parent = transform;
                pickup.transform.position = ItemPos.position;
                pickup.transform.rotation = ItemPos.rotation;
                Debug.Log("pick up");
                Debug.Log(pickup.name);
            }
        }
        if (other.gameObject.tag == "AI")
        {
            if (hasItem == true)
            {
                ai = other.GetComponent<AIController>();
                if (ai.state == AIController.States.Want)
                {
                    aButton.gameObject.SetActive(true);
                }
                if (ai.state == AIController.States.Crying && currentSweets != 0)
                {
                    yButton.gameObject.SetActive(true);
                }
            }
            if (Input.GetButtonDown("XboxA")&& hasItem)
            {

                ai = other.GetComponent<AIController>();
                if (ai.state == AIController.States.Want)
                {
                    aButton.gameObject.SetActive(false);
                    
                    bool correct = ai.ReceiveItem(pickup);
                    if (correct)
                    {
                        //Debug.Log("SweetTest");
                        hasItem = false;
                        currentSweets++;
                        Image icon = Instantiate(sweetIcon, FindObjectOfType<CanvasScaler>().gameObject.transform);
                        icon.GetComponent<ButtonPos>().buttonPos = butPos;
                       Destroy(icon, 3);
                    }
                    
                }
                
            }

            ai = other.GetComponent<AIController>();
            if (ai.state == AIController.States.Crying && currentSweets != 0)
            {
                yButton.gameObject.SetActive(true);
            }
            if (Input.GetButton("XboxY"))
            {
                ai = other.GetComponent<AIController>();
                if (ai.state == AIController.States.Crying && currentSweets != 0)
                {
                    yButton.gameObject.SetActive(false);
                    ai.ReceiveSweet();

                    currentSweets--;
                }
            }

        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == ("PickUp") && hasItem == false)
        {
            aButton.gameObject.SetActive(false);
        }
        if (other.gameObject.tag == "AI")
        {
            xButton.gameObject.SetActive(false);
            yButton.gameObject.SetActive(false);
        }
    }
}
