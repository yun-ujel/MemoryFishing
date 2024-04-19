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

        [Header("Physics")]
        [SerializeField] private Rigidbody body;

        [Space]

        [SerializeField] private Vector3[] samplePoints;
        [SerializeField] private float height = 1;
        [SerializeField] private float volume = 1;

        [Space]

        [SerializeField] private float minimumDrag = 0f;
        [SerializeField] private float minimumAngularDrag = 0.05f;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            for (int i = 0; i < samplePoints.Length; i++)
            {
                Debug.DrawRay(transform.position + (transform.rotation * samplePoints[i]), Vector3.up, Color.yellow);
            }
        }
#endif

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

        private void FixedUpdate()
        {
            SimulateBuoyancy();
        }

        private void SimulateBuoyancy()
        {
            float density = body.mass / volume;

            float submergedVolume = 0.0f;
            float unitForce = (1.0f - density) / samplePoints.Length;

            for (int i = 0; i < samplePoints.Length; i++)
            {
                Vector3 worldPos = body.position + (body.rotation * samplePoints[i]);

                float waterLevel = GetHeightAtPosition(worldPos);
                float depth = waterLevel - worldPos.y + height;
                float submergedFactor = Mathf.Clamp01(depth / height);

                submergedVolume += submergedFactor;

                float displacement = Mathf.Max(0.0f, depth);
                Vector3 force = displacement * unitForce * -Physics.gravity;

                body.AddForceAtPosition(force, worldPos);
                Debug.Log($"Add Force {force} at position {worldPos}");
            }

            body.drag = Mathf.Lerp(minimumDrag, 1.0f, submergedVolume);
            body.angularDrag = Mathf.Lerp(minimumAngularDrag, 1.0f, submergedVolume);
        }
    }
}