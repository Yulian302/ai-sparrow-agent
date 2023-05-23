/*using Unity.MLAgents;
using UnityEngine;

public class SparrowAcademy : MonoBehaviour
{
    public GameObject sparrowAgentPrefab;
    public GameObject sparrowBabyPrefab;
    public int numAgents;
    public int numEpisodes;
    public float timeBetweenEpisodes;
    private SparrowArea m_SparrowArea;
    private float m_ElapsedTime;
    private int m_CurrentEpisode;

    private void Awake()
    {
        m_SparrowArea = GetComponent<SparrowArea>();
    }

    private void Start()
    {
        for (int i = 0; i < numAgents; i++)
        {
            GameObject sparrowAgentObj = Instantiate(sparrowAgentPrefab, m_SparrowArea.GETObjectSpawner.getRandomPosition(), Quaternion.identity);
            sparrowAgentObj.transform.parent = transform;
        }
        ResetScene();
    }

    private void Update()
    {
        m_ElapsedTime += Time.deltaTime;
        if (m_ElapsedTime > timeBetweenEpisodes)
        {
            m_ElapsedTime = 0f;
            ResetScene();
        }
    }

    private void ResetScene()
    {
        foreach (Transform agent in transform)
        {
            agent.position = m_SparrowArea.GETObjectSpawner.getRandomPosition();
            agent.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            agent.GetComponent<SparrowAgent>().EndEpisode();
        }

        m_CurrentEpisode++;
        if (m_CurrentEpisode > numEpisodes)
        {
            EndLearning();
        }
    }

    private void EndLearning()
    {
        foreach (Transform agent in transform)
        {
            agent.GetComponent<SparrowAgent>().enabled = false;
        }
        enabled = false;
    }
}*/

