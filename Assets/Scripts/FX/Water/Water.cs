using MemoryFishing.Utilities;
using UnityEditor;
using UnityEngine;

using static System.Runtime.InteropServices.Marshal;

namespace MemoryFishing.FX.Water
{
    public class Water : MonoBehaviour
    {
        public struct Wave
        {
            public Vector2 direction;
            public Vector2 origin;

            public float frequency;
            public float amplitude;
            public float phase;
            public float steepness;

            public Wave(float wavelength, float amplitude, float speed, float direction, float steepness, Vector2 origin)
            {
                frequency = 2.0f / wavelength;
                phase = speed * Mathf.Sqrt(9.8f * 2.0f * Mathf.PI / wavelength);

                this.amplitude = amplitude;
                this.steepness = steepness;
                this.origin = origin;
                this.direction = VectorUtils.DegreesToVector(direction);
                this.direction.Normalize();
            }
        }

        [SerializeField] private Material waterMaterial;

        [Header("Wave Settings")]
        [SerializeField] private int waveCount = 4;
        [SerializeField] private float planeLength = 20;

        [Space, SerializeField] private float medianWavelength = 1f;
        [SerializeField] private float wavelengthRange = 1f;

        [Space, SerializeField] private float medianDirection = 0f;
        [SerializeField] private float directionalRange = 30f;

        [Space, SerializeField] private float medianAmplitude = 1f;
        [SerializeField] private float steepness = 0f;

        [Space, SerializeField] private float medianSpeed = 1f;
        [SerializeField] private float speedRange = 0.1f;

        private ComputeBuffer waveBuffer;

        private void Start()
        {
            waveBuffer = new ComputeBuffer(waveCount, SizeOf(typeof(Wave)));

            GenerateWaves();
        }

        private void GenerateWaves()
        {
            float wavelengthMin = medianWavelength / (1.0f + wavelengthRange);
            float wavelengthMax = medianWavelength * (1.0f + wavelengthRange);
            float directionMin = medianDirection - directionalRange;
            float directionMax = medianDirection + directionalRange;
            float speedMin = Mathf.Max(0.01f, medianSpeed - speedRange);
            float speedMax = medianSpeed + speedRange;
            float ampOverLen = medianAmplitude / medianWavelength;

            float halfPlaneWidth = planeLength * 0.5f;
            Vector3 minPoint = transform.TransformPoint(new Vector3(-halfPlaneWidth, 0.0f, -halfPlaneWidth));
            Vector3 maxPoint = transform.TransformPoint(new Vector3(halfPlaneWidth, 0.0f, halfPlaneWidth));

            Wave[] waves = new Wave[waveCount];

            for (int i = 0; i < waves.Length; i++)
            {
                float wavelength = Random.Range(wavelengthMin, wavelengthMax);
                float direction = Random.Range(directionMin, directionMax);
                float amplitude = wavelength * ampOverLen;
                float speed = Random.Range(speedMin, speedMax);
                Vector2 origin = new(Random.Range(minPoint.x * 2, maxPoint.x * 2), Random.Range(minPoint.x * 2, maxPoint.x * 2));

                waves[i] = new Wave(wavelength, amplitude, speed, direction, steepness, origin);
            }

            waveBuffer.SetData(waves);
            waterMaterial.SetBuffer("_Waves", waveBuffer);
            waterMaterial.SetInt("_WaveCount", waveCount);
        }

        private void OnDisable()
        {
            waveBuffer.Release();
        }
    }
}