using System;
using Unity.Mathematics;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class SparrowAgent : Agent
{
    static double tolerance = 1e-6;
    public float moveSpeed = 5f;
    [HideInInspector] public float turnSpeed = 10f;
    private float flyForce = 0.5f;
    public GameObject heartPrefab;
    public GameObject regurgitatedSpiderPrefab;
    private SparrowArea m_SparrowArea;
    private Rigidbody m_Rigidbody;
    private GameObject m_Baby;
    private bool m_IsFullStomach;
    private Animator m_Animation;

    public override void Initialize()
    {
        base.Initialize();
        m_SparrowArea = GetComponentInParent<SparrowArea>();
        m_Baby = m_SparrowArea.sparrowBaby;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animation = GetComponent<Animator>();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = actions.DiscreteActions[0];
        float turnAmount = 0f;
        var maxHeight = 30f;
        var turning = actions.DiscreteActions[1];
        var flying = actions.DiscreteActions[2];
        switch (turning)
        {
            case 1:
                turnAmount = -1f;
                break;
            case 2:
                turnAmount = 1f;
                break;
        }

        if (flying == 1)
        {
            m_Rigidbody.AddForce(transform.up * flyForce, ForceMode.VelocityChange);
            if (m_Rigidbody.velocity.magnitude > 6f)
            {
                m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * 6f;
            }

            m_Animation.Play("Fly");
        }

        if (m_Rigidbody.transform.position.y > maxHeight)
        {
            var heightDifference = m_Rigidbody.transform.position.y - maxHeight;
            m_Rigidbody.AddForce(-transform.up * heightDifference, ForceMode.VelocityChange);
            if (m_Rigidbody.velocity.y > 0f)
            {
                var velocity = m_Rigidbody.velocity;
                velocity = new Vector3(velocity.x, 0f, velocity.z);
                m_Rigidbody.velocity = velocity;
            }
        }

        var transform1 = transform;
        m_Rigidbody.MovePosition(transform1.position +
                                 transform1.forward * (forwardAmount * moveSpeed * Time.fixedDeltaTime));
        transform.Rotate(transform.up * (turnAmount * turnSpeed * Time.fixedDeltaTime));
        if (Math.Abs(math.round(m_Rigidbody.transform.position.y) - 22f) < tolerance ||
            m_Rigidbody.transform.CompareTag("stone"))
        {
            m_Animation.Play("Idle_A");
        }

        if (MaxStep > 0)
        {
            AddReward(-1f / MaxStep);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var forwardAction = 0;
        var turnAction = 0;
        var flyAction = 0;
        if (Input.GetKey(KeyCode.W))
        {
            forwardAction = 1;
            if (Math.Abs(math.round(m_Rigidbody.transform.position.y) - 22f) < tolerance) m_Animation.Play("Walk");
        }

        if (Input.GetKey(KeyCode.A))
        {
            turnAction = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            turnAction = 2;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            flyAction = 1;
        }

        actionsOut.DiscreteActions.Array[0] = forwardAction;
        actionsOut.DiscreteActions.Array[1] = turnAction;
        actionsOut.DiscreteActions.Array[2] = flyAction;
    }

    public override void OnEpisodeBegin()
    {
        m_IsFullStomach = false;
        m_SparrowArea.ResetArea();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // is agent hungry
        sensor.AddObservation(m_IsFullStomach);
        //1
        // distance to baby (scalar)
        var babyPosition = m_Baby.transform.position;
        var agentPosition = transform.position;
        sensor.AddObservation(Vector3.Distance(babyPosition, agentPosition));
        //3
        // baby position
        sensor.AddObservation(babyPosition);
        //3
        // direction to baby 
        sensor.AddObservation(babyPosition - agentPosition);
        //3
        // vector3 of space in front of agent
        sensor.AddObservation(transform.forward);
        //3
        // distance to closest spider
        var spiders = GameObject.FindGameObjectsWithTag("spider");
        if (spiders.Length > 0)
        {
            float nearestSpiderDistance = Mathf.Infinity;
            foreach (var spider in spiders)
            {
                float distanceToSpider = Vector3.Distance(agentPosition, spider.transform.position);
                if (distanceToSpider < nearestSpiderDistance)
                {
                    nearestSpiderDistance = distanceToSpider;
                }
            }

            sensor.AddObservation(nearestSpiderDistance);
        }
        else
        {
            sensor.AddObservation(-1f);
        }
        //1
        //9+3+3 observations
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("spider"))
        {
            EatSpider(other.gameObject);
            Debug.Log("Spider eaten!");
        }

        if (other.transform.CompareTag("baby"))
        {
            RegurgitateSpider();
        }
    }

    private void EatSpider(GameObject spiderObject)
    {
        if (m_IsFullStomach) return;
        m_IsFullStomach = true;
        m_SparrowArea.RemoveSpecificSpider(spiderObject);
        AddReward(0.2f);
    }

    private void RegurgitateSpider()
    {
        if (!m_IsFullStomach) return;
        m_IsFullStomach = false;
        //spawning spider
        var parent = transform.parent;
        var regSpider = Instantiate(regurgitatedSpiderPrefab, parent, true);
        var position = m_Baby.transform.position;
        regSpider.transform.position = position + new Vector3(0, 0.25f, 0);
        Destroy(regSpider, 1f);
        //spawning heart
        var heart = Instantiate(heartPrefab, parent, true);
        heart.transform.position = position + Vector3.up;
        Destroy(heart, 4f);
        AddReward(1f);
        if (m_SparrowArea.SpiderRemaining > 0) return;
        Destroy(heart);
        Destroy(regSpider);
        EndEpisode();
    }
}