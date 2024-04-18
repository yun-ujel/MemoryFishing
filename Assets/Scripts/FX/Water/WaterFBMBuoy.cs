using UnityEngine;

namespace MemoryFishing.FX.Water
{
    public class WaterFBMBuoy : MonoBehaviour
    {
        [SerializeField] private int waveCount = 8;

        [Header("Direction Randomization")]
        [SerializeField] private float seed = 0;
        [SerializeField] private float seedIteration = 1253.2131f;

        [Header("Waves")]
        [SerializeField] private float frequency = 1.0f;
        [SerializeField] private float frequencyMultiplier = 1.18f;

        [Space]

        [SerializeField] private float amplitude = 1.0f;
        [SerializeField] private float amplitudeMultiplier = 0.82f;

        [Space]

        [SerializeField] private float speed = 1f;
        [SerializeField] private float speedMultiplier = 1.07f;

        [Header("References")]
        [SerializeField] private Rigidbody body;

        private float GetHeightAtPosition(Vector3 position)
        {
            float seed = this.seed;
            float frequency = this.frequency;
            float amplitude = this.amplitude;
            float speed = this.speed;

            float h = 0f;
            float amplitudeSum = 0f;

            for (int i = 0; i < waveCount; i++)
            {
                Vector2 direction = new Vector2(Mathf.Cos(seed), Mathf.Sin(seed)).normalized;

                float x = Vector2.Dot(direction, new(position.x, position.z)) * frequency * Time.time * speed;
                float wave = Mathf.Exp(Mathf.Sin(x) - 1);

                h += wave;
                amplitudeSum += amplitude;

                frequency *= frequencyMultiplier;
                amplitude *= amplitudeMultiplier;
                speed *= speedMultiplier;
                seed += seedIteration;
            }

            return h / amplitudeSum;
        }

        private Vector3 GetNormalAtPosition(Vector3 position)
        {
            float seed = this.seed;
            float frequency = this.frequency;
            float amplitude = this.amplitude;
            float speed = this.speed;

            Vector2 n = Vector2.zero;
            for (int i = 0; i < waveCount; i++)
            {
                Vector2 direction = new Vector2(Mathf.Cos(seed), Mathf.Sin(seed)).normalized;

                float x = Vector2.Dot(direction, new(position.x, position.z)) * frequency * Time.time * speed;
                float wave = amplitude * Mathf.Exp(Mathf.Sin(x) - 1);
                Vector2 dw = frequency * direction * (wave * Mathf.Cos(x));

                n += dw;

                frequency *= frequencyMultiplier;
                amplitude *= amplitudeMultiplier;
                speed *= speedMultiplier;
                seed += seedIteration;
            }

            return new Vector3(-n.x, 1.0f, -n.y).normalized;
        }

        private void Update()
        {
            Vector3 position = transform.position;
            position.y = GetHeightAtPosition(position);

            transform.position = position;
        }
    }
}