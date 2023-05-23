using UnityEngine;

public class Spider : MonoBehaviour
{
    public float spiderSpeed;
    private float m_RandomizedSpeed;
    private float m_NextActionTime = -1f;
    private Vector3 m_TargetPosition;

    private void FixedUpdate()
    {
        if (spiderSpeed > 0f)
        {
            Crawl();
        }
    }

    private void Crawl()
    {
        if (Time.fixedTime >= m_NextActionTime)
        {
            m_RandomizedSpeed = spiderSpeed * Random.Range(.5f, 1.5f);
            var position = transform.position;
            m_TargetPosition = new Vector3(
                Random.Range(5f * -2f, 5f * 2f),
                position.y,
                Random.Range(5f * -2f, 5f * 2f));
            transform.rotation = Quaternion.LookRotation(m_TargetPosition - position, Vector3.up);

            float timeToGetThere = Vector3.Distance(position, m_TargetPosition) / m_RandomizedSpeed;
            m_NextActionTime = Time.fixedTime + timeToGetThere;
        }
        else
        {
            Vector3 moveVector = transform.forward * (m_RandomizedSpeed * Time.fixedDeltaTime);
            if (moveVector.magnitude <= Vector3.Distance(transform.position, m_TargetPosition))
            {
                transform.position += moveVector;
            }
            else
            {
                transform.position = m_TargetPosition;
                m_NextActionTime = Time.fixedTime;
            }
        }
    }
}