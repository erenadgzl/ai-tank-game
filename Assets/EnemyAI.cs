using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AForge.Fuzzy;

public class EnemyAI : MonoBehaviour
{

    NavMeshAgent agent;
    public Transform[] wayPoints;
    Vector3[] wayPointsPos=new Vector3[3];
    int currentPoint = 0;
    Transform player;
    Animator fsm;
    float maxCheckDistance = 10;
    public Transform rayOrigin;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < wayPoints.Length; i++)
        {
            wayPointsPos[i] = wayPoints[i].position;
        }

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        fsm = GetComponent<Animator>();

        agent.SetDestination(wayPointsPos[currentPoint]);
        StartCoroutine("CheckPlayer");

        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
         
    }
    IEnumerator CheckPlayer()
    {
        CheckVisibility();
        CheckDistance();

        yield return new WaitForSeconds(0.1f);
        yield return CheckPlayer();
    }

    private void CheckVisibility()
    {
        Vector3 dir = player.position - rayOrigin.position;
        dir = dir.normalized;



        Debug.DrawRay(rayOrigin.position, dir * maxCheckDistance, Color.red);

        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin.position, dir, out hitInfo))
        {
            if (hitInfo.transform.tag == "Player")
            {
                fsm.SetBool("isVisible", true);
            }
            else
            {
                fsm.SetBool("isVisible", false);
            }
        }
        else
        {
            fsm.SetBool("isVisible", false);

        }

    }

    private void CheckDistance()
    {
        float d = Vector3.Distance(player.position, transform.position);
        fsm.SetFloat("distance", d);


        float distancefromWaypoint = Vector3.Distance(transform.position, wayPointsPos[currentPoint]);
        fsm.SetFloat("distanceFromWaypoint", distancefromWaypoint);
    }

    public void SetNextWayPoint()
    {
        if (currentPoint == 0)
            currentPoint = 1;
        else if (currentPoint == 1)
            currentPoint = 2;
        else
            currentPoint = 0;

        agent.SetDestination(wayPointsPos[currentPoint]);
    }
    public void SetLookRotation()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.2f);
    }
    public void Shoot()
    {
        GetComponent<ShootBehaviour>().Shoot(5f);
    }
    public void Chase()
    {
        
        agent.SetDestination(player.position);
        Evaluate();
    }
    public void StopAgent()
    {
        agent.isStopped=true;
    }
    public void StartAgent()
    {
        agent.isStopped = false;
    }
    public void Patrol()
    {
        agent.SetDestination(wayPointsPos[currentPoint]);
    }
    
    float distance, speed;
    FuzzySet fsNear, fsMed, fsFar;
    LinguisticVariable lvDistance;
    FuzzySet fsSlow, fsMedium, fsFast;
    LinguisticVariable lvSpeed;

    Database database;
    InferenceSystem infSystem;

    private void Initialize()
    {
        SetDistanceFuzzySet();
        SetSpeedFuzzySet();
        AddToDatabase();

    }

    private void SetDistanceFuzzySet()
    {
        fsNear = new FuzzySet("Near", new TrapezoidalFunction(20, 40, TrapezoidalFunction.EdgeType.Right));
        fsMed = new FuzzySet("Med", new TrapezoidalFunction(20, 40, 50, 70));
        fsFar = new FuzzySet("Far", new TrapezoidalFunction(50, 70, TrapezoidalFunction.EdgeType.Left));

        lvDistance = new LinguisticVariable("Distance", 0, 100);

        lvDistance.AddLabel(fsNear);
        lvDistance.AddLabel(fsMed);
        lvDistance.AddLabel(fsFar);
    }

    private void SetSpeedFuzzySet()
    {
        fsSlow = new FuzzySet("Slow", new TrapezoidalFunction(30, 50, TrapezoidalFunction.EdgeType.Right));
        fsMedium = new FuzzySet("Medium", new TrapezoidalFunction(30, 45, 80, 100));
        fsFast = new FuzzySet("Fast", new TrapezoidalFunction(80, 100, TrapezoidalFunction.EdgeType.Left));

        lvSpeed = new LinguisticVariable("Speed", 0, 120);

        lvSpeed.AddLabel(fsSlow);
        lvSpeed.AddLabel(fsMedium);
        lvSpeed.AddLabel(fsFast);


    }

    private void AddToDatabase()
    {
        database = new Database();
        database.AddVariable(lvDistance);
        database.AddVariable(lvSpeed);

        infSystem = new InferenceSystem(database, new CentroidDefuzzifier(120));
        infSystem.NewRule("Rule 1", "IF Distance IS Far THEN Speed IS Slow");
        infSystem.NewRule("Rule 2", "IF Distance IS Med THEN Speed IS Medium");
        infSystem.NewRule("Rule 3", "IF Distance IS Near THEN Speed IS Fast");
    }

    public void SetSpeed(float a)
    {
        agent.speed = a;
    }
    
    private void Evaluate()
    {
        distance = Vector3.Distance(transform.position, player.position);
        Vector3 dir = (player.position - transform.position);
        distance = dir.magnitude;
        dir.Normalize();
        distance = distance * 5;
        

        infSystem.SetInput("Distance", distance);
        speed = infSystem.Evaluate("Speed");
        speed = speed / 24+5;
        //transform.position += dir * speed * Time.deltaTime*0.01f;
        agent.speed = speed;
        
         Debug.Log(speed);


    }

}
