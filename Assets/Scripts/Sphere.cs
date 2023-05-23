using UnityEngine;

namespace DefaultNamespace
{
    public class Sphere : MonoBehaviour
    {
        public float speed = 5f;
        public float amplitude = 2f;
        public float frequency = 2f;
        public Transform target;

        private Vector3 initialPosition;
        private float startTime;

        private void Start()
        {
            initialPosition = transform.position;
            startTime = Time.time;
        }

        private void Update()
        {
            float time = Time.time - startTime;
            float newX = initialPosition.x + Mathf.Sin(time * frequency) * amplitude;
            float newZ = initialPosition.z + Mathf.Cos(time * frequency) * amplitude;
            float newY = initialPosition.y + Mathf.PingPong(time * speed, amplitude * 2f);

            transform.position = new Vector3(newX, newY, newZ);

            if (target != null)
            {
                Vector3 targetDirection = target.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * speed);
            }
        }
        
    }
}