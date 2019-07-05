using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AIController : MonoBehaviour
{
    
    public enum States { Idle, Want, Friend, Crying, Tantrum };
    public States state;
    private EndGame eg;
    bool isHappy = false;
    public AudioClip happy, upset, yum;
    AudioSource AS;
    NavMeshAgent agent;
    Transform destination;
    Vector3 randomDirection;

    public List<AIController> allAI;
    public List<AIController> nearbyAI;

    public Transform imagePos;
    public Transform itemPos;

    //IDLE STATE VARIABLES
    public float walkRadius;
    public float getPositionWait;
    public float getPositionTimer;
    float dist;
    public bool getPosition = true;
    public float randWalkWait;

    //WANT STATE VARIABLES
    public List<Item> items;
    public float wantedItemWait;
    public float wantedItemTimer;
    public Item wantedItem;
    Image wantedItemUI;
    public bool chosenItem = false;
    public bool receivingItem = false;

    //FRIEND STATE VARIABLES
    GameObject player;
    float distToPlayer;

    //CRYING STATE VARIABLES
    public float cryingRadius;
    public float cryingWait;
    public float cryingTimer;
    public bool crying = false;

    float happyTimer = 5f;

    Image go;
    public Image happyFace, cryFace, angryFace;

    float delay = 10f;
    public bool canCry = true;

    public GameObject cryParticle;
    public ParticleSystem cryP;
    public Animation anim;

    GameController controller;

    public bool cryDelay = false;
    public float cryInfluenceTimer;

    public Image sweetAnim;
   
    public CountDown cd;

    void Start()
    {

        state = States.Idle;
        eg = FindObjectOfType<EndGame>();
        AS = GetComponent<AudioSource>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        allAI = new List<AIController>();
        allAI.AddRange(FindObjectsOfType<AIController>());
        player = GameObject.FindGameObjectWithTag("Player");
        controller = FindObjectOfType<GameController>();
        //anim = GetComponent<Animation>();

        randWalkWait = Random.Range(0, 5);
        getPositionTimer = randWalkWait;
        wantedItemTimer = wantedItemWait;
        cryingTimer = cryingWait;
        //cryDelayTimer = cryDelayReset;
        cd = FindObjectOfType<CountDown>();
    }

    void Update()
    {
        if (cd.Timer > 30)
        {
            wantedItemWait -= Time.deltaTime / 12;
        }
        //if (cryDelay == true && crying == false && canCry == true)
        //{
        //    Debug.Log("CRYYDELAYY");
        //    cryDelayTimer -= Time.deltaTime;
        //    if (cryDelayTimer <= 0)
        //    {
        //        startCrying();
        //        cryDelayTimer = cryDelayReset;
        //    }
        //}
        //else
        //{
        //    cryDelayTimer = cryDelayReset;
        //}

        //items.AddRange(FindObjectsOfType<Item>());
        if (canCry == false)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                canCry = true;
                delay = 10f;
            }

        }

        if (state == States.Idle) //IDLE STATE LOGIC
        {
            crying = false;
            cryParticle.SetActive(false);
            //getPosition = true;
            if (getPosition == true)
            {
                agent.enabled = false;
                getPositionTimer -= Time.deltaTime;
                if (getPositionTimer <= 0)
                {
                    agent.enabled = true;
                    agent.destination = RandomNavmeshLocation(walkRadius);
                    //transform.LookAt(agent.destination);
                    getPosition = false;
                    randWalkWait = Random.Range(0, 5);
                    getPositionTimer = randWalkWait;
                }
            }

            dist = Vector3.Distance(transform.position, agent.destination);

            if (dist <= 1)
            {
                getPosition = true;
            }
        }

        if (state == States.Want)  //WANT STATE LOGIC
        {
            crying = false;
            //Debug.Log(gameObject + " : WANT");
            agent.enabled = false;
            if (chosenItem == false)
            {
                items.Clear();
                items.AddRange(FindObjectsOfType<Item>());
                wantedItem = items[Random.Range(0, items.Count)];
                chosenItem = true;
                wantedItem.target = this.gameObject;
                wantedItemTimer = wantedItemWait;
                //UI stuff here
                go = Instantiate(wantedItem.image);
                //go.transform.parent = FindObjectOfType<Canvas>().transform;    
                go.transform.SetParent(FindObjectOfType<CanvasScaler>().transform, false);
                go.GetComponent<BubbleBehaviour>().kid = this;
            }
            if (wantedItem == null)
            {
                Destroy(go.gameObject);
                startCrying();
            }
            if (!wantedItem.pickedUp)
            {
                wantedItem.circle.SetActive(true);
            }
            wantedItemTimer -= Time.deltaTime;
            if (wantedItemTimer <= 0)
            {
                wantedItem.circle.SetActive(false);
                Destroy(go.gameObject);
                startCrying();
                wantedItemTimer = wantedItemWait;
            }
        }

        if (state == States.Friend) //FRIEND STATE LOGIC
        {
            anim.Play();
            if(isHappy == false)
            {
                AS.PlayOneShot(happy);
                isHappy = true;
            }
            happyTimer -= Time.deltaTime;
            if (happyTimer <= 0 && go != null)
            {
                Destroy(go.gameObject);
            }

            crying = false;
            agent.enabled = true;
            agent.destination = player.transform.position;



            distToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distToPlayer <= 2)
            {
                // agent.enabled = false;
                agent.speed = 0;
            }
            else
            {
                // agent.enabled = true;
                agent.speed = 4;
            }
        }

        if (state == States.Crying) //CRYING STATE LOGIC
        {
            cryDelay = false;
            if (crying == false)
            {
                if (go != null)
                {
                    Destroy(go.gameObject);
                }
                go = Instantiate(cryFace);
                //go.transform.parent = FindObjectOfType<Canvas>().transform;
                go.transform.SetParent(FindObjectOfType<CanvasScaler>().transform, false);
                go.GetComponent<BubbleBehaviour>().kid = this;
            }
            crying = true;

            agent.enabled = false;
            cryingTimer -= Time.deltaTime;
            cryParticle.SetActive(true);

            foreach (AIController ai in allAI)
            {
                if (ai != this.gameObject)
                {
                    if (ai.state != States.Friend && ai.state != States.Tantrum && ai.state != States.Crying)
                    {
                        float dist = Vector3.Distance(transform.position, ai.transform.position);
                        if (dist <= cryingRadius)
                        {
                            if (ai.canCry)
                            {
                                //ai.state = States.Crying;
                                ai.cryInfluenceTimer += Time.deltaTime;
                                if (ai.cryInfluenceTimer > 1)
                                {
                                    ai.startCrying();
                                    cryInfluenceTimer = 0f;
                                }
                                //ai.cryDelay = true;
                               // StartCoroutine(CryCircleDelay(ai));
                            }
                        }
                        
                        
                    }
                }
            }

            if (cryingTimer <= 0)
            {
                cryingTimer = cryingWait;
                state = States.Tantrum;
                Destroy(go.gameObject);
                getPosition = true;
            }
            //Crying UI and visuals
            cryingRadius += Time.deltaTime / 10;
            cryP.startSpeed += Time.deltaTime / 5;
            //cryP.emission.rateOverTime += Time.deltaTime;

        }
        if (state == States.Tantrum) //TANTRUM STATE LOGIC
        {
            
            cryP.Stop();
            agent.enabled = true;
            agent.SetDestination(GameObject.FindGameObjectWithTag("Door").transform.position);
            if (go == null)
            {
                go = Instantiate(angryFace);
                //go.transform.parent = FindObjectOfType<Canvas>().transform;
                go.transform.SetParent(FindObjectOfType<CanvasScaler>().transform, false);
                go.GetComponent<BubbleBehaviour>().kid = this;
            }                                 
        }
    }
    IEnumerator CryCircleDelay(AIController ai)
    {
        if (ai.cryDelay == true)
        {
            yield return new WaitForSeconds(0.5f);
            if (ai.cryDelay == true)
            {
                ai.startCrying();
            }
        }
    }

    public void startCrying()
    {
        AS.PlayOneShot(upset);
        crying = true;
        state = States.Crying;
        go = Instantiate(cryFace);
        //go.transform.parent = FindObjectOfType<Canvas>().transform;
        go.transform.SetParent(FindObjectOfType<CanvasScaler>().transform, false);
        go.GetComponent<BubbleBehaviour>().kid = this;
        eg.crybaby();
        nearbyAI.Clear();

    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cryingRadius);
    }
    public bool ReceiveItem(Item received)
    {
        if (received.name == wantedItem.name)
        {

            Destroy(go.gameObject);
            // Debug.Log("unchild");
            state = States.Friend;
            go = Instantiate(happyFace);
            // go.transform.parent = FindObjectOfType<Canvas>().transform;
            go.transform.SetParent(FindObjectOfType<CanvasScaler>().transform, false);
            go.GetComponent<BubbleBehaviour>().kid = this;

            wantedItem.circle.SetActive(false);
            wantedItem.RemoveCol();
            wantedItem.transform.parent = null;
            wantedItem.transform.parent = transform;
            wantedItem.transform.position = itemPos.position;
            wantedItem.transform.rotation = itemPos.rotation;

            controller.itemToRespawn = wantedItem;
            //controller.RespawnItem();

            return true;
        }
        else
        {
            Destroy(go.gameObject);
            wantedItem.circle.SetActive(false);
            startCrying();
            return false;
        }
    }

    public void ReceiveSweet()
    {
        //Instantiate(sweetAnim, sweetAnim.transform.position, sweetAnim.transform.rotation);
        AS.PlayOneShot(yum);
        canCry = false;
        getPosition = true;
        chosenItem = false;
        wantedItemTimer = wantedItemWait;
        Debug.Log("sweets");
        state = States.Idle;
        Destroy(go.gameObject);
        cryP.Stop();
        cryingRadius = 1;
        cryP.startSpeed = 2;
        cryingTimer = cryingWait;

    }

    public void chooseBadState()
    {
        float rand = Random.Range(0, 2);
        if (rand == 0)
        {
            startCrying();
        }
        else
        {
            state = States.Tantrum;
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        //Debug.Log(finalPosition);
        return finalPosition;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (state == States.Tantrum)
        {
            if (col.CompareTag("Door"))
            {
                eg.leavers();
                Destroy(go.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}
