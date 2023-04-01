using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Unity.MLAgents.Policies;


public class Move : Agent
{
    private Rigidbody rb;
    public float speed;
    public float rotateSpeed;
    public float raycastDistance;
    private bool hasHitObstacle = false;
    [SerializeField] private GameObject obstacle;
    [SerializeField] public GameObject targetObject;
    public bool trainingMode = false;
    public GameObject area;
    public float rewardMultiplier = 1f;
    public float obstacleReward = -0.1f;
    public float successReward = 1f;

    private void OnEnvironmentReset()
    {
        transform.localPosition = area.transform.position +
                                  new Vector3(UnityEngine.Random.Range(-10f, 10f), 0f,
                                      UnityEngine.Random.Range(-10f, 10f));
        transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        hasHitObstacle = false;
    }

    public override void Initialize()
    {
        BehaviorParameters behaviorParameters = GetComponent<BehaviorParameters>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        SetObstacle(obstacle);
        if (trainingMode)
        {
            Academy.Instance.OnEnvironmentReset += OnEnvironmentReset;
            behaviorParameters.BehaviorType = BehaviorType.Default;
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        hasHitObstacle = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rb.velocity.magnitude);
        sensor.AddObservation(transform.rotation.eulerAngles);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
        {
            sensor.AddObservation(hit.distance / raycastDistance);
        }
        else
        {
            sensor.AddObservation(1f);
        }

        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("asteroid");
        foreach (GameObject asteroid in asteroids)
        {
            Vector3 directionToAsteroid = asteroid.transform.position - transform.position;
            sensor.AddObservation(directionToAsteroid.magnitude / raycastDistance);
            sensor.AddObservation(Vector3.Dot(directionToAsteroid.normalized, transform.forward));
            sensor.AddObservation(asteroid.GetComponent<Rigidbody>().velocity.magnitude);
        }

        int observationCount = sensor.ObservationSize();
        for (int i = 0; i < (100 - observationCount); i++)
        {
            sensor.AddObservation(0f);
        }
    }

    private void EndGame(string message)
    {
        Time.timeScale = 0f;
        Debug.Log("Success!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("asteroid"))
        {
            Debug.Log("Hit obstacle");
            Debug.Log(successReward);
            AddReward(-0.1f);
            hasHitObstacle = true;

            // Reset the agent's position and velocity
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.localPosition = Vector3.zero;

            if (trainingMode)
            {
                EndEpisode();
                AddReward(-1f);
            }
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Finish entered!Well done!");
            EndGame("Congratulations, you won the game!");
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (hasHitObstacle)
        {
            AddReward(Time.deltaTime * rewardMultiplier * successReward);
        }

        Vector3 targetPosition = targetObject.transform.position;
        Vector3 direction = targetPosition - transform.position;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        float moveForward = actions.ContinuousActions[0];
        if (moveForward > 0)
        {
            rb.AddForce(transform.forward * speed * moveForward);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
        {
            if (hit.collider.gameObject == obstacle)
            {
                Debug.Log("Hit obstacle");
                AddReward(obstacleReward);
                hasHitObstacle = true;
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Vertical");
        continuousActions[1] = Input.GetAxisRaw("Horizontal");
        if (trainingMode)
        {
            continuousActions[0] = 0f;
            continuousActions[1] = 0f;
        }
    }

    public void SetObstacle(GameObject obstacleObject)
    {
        obstacle = obstacleObject;
    }
}