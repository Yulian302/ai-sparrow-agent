using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class SparrowArea : MonoBehaviour
{
    public SparrowAgent sparrowAgent;
    public GameObject sparrowBaby;
    public Spider spider;
    public TextMeshPro cumulativeRewardText;
    private ObjectSpawner m_Spawner;
    [HideInInspector] public float spiderSpeed;
    [HideInInspector] public float feedRadius = 1f;
    private List<GameObject> m_SpiderList;

    public void ResetArea()
    {
        RemoveAllSpiders();
        PlaceSparrow();
        PlaceBabySparrow();
        SpawnSpiders(30, .5f);
    }

    public void RemoveSpecificSpider(GameObject spiderObject)
    {
        m_SpiderList.Remove(spiderObject);
        Destroy(spiderObject);
    }

    public ObjectSpawner GETObjectSpawner => m_Spawner;
    public int SpiderRemaining => m_SpiderList.Count;

    private void RemoveAllSpiders()
    {
        if (m_SpiderList != null)
        {
            foreach (var t in m_SpiderList.Where(t => t != null))
            {
                Destroy(t);
            }
        }

        m_SpiderList = new List<GameObject>();
    }

    private void PlaceSparrow()
    {
        var rigidBody = sparrowAgent.GetComponent<Rigidbody>();
        rigidBody.velocity = rigidBody.angularVelocity = Vector3.zero;
        sparrowAgent.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        sparrowAgent.transform.position = m_Spawner.GETRandomPosition();
    }

    private void PlaceBabySparrow()
    {
        var rigidBody = sparrowBaby.GetComponent<Rigidbody>();
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        sparrowBaby.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    private void SpawnSpiders(int count, float speed)
    {
        for (var i = 0; i < count; i++)
        {
            var spiderObject = Instantiate(spider.gameObject, transform, true);
            var pos = m_Spawner.GETRandomPosition();
            spiderObject.transform.position = pos;
            spiderObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            m_SpiderList.Add(spiderObject);
            spiderObject.GetComponent<Spider>().spiderSpeed = speed;
        }
    }
    
    public void Start()
    {
        m_Spawner = FindObjectOfType<ObjectSpawner>();
    }

    public void Update()
    {
        cumulativeRewardText.text = sparrowAgent.GetCumulativeReward().ToString("0.00");
    }
}