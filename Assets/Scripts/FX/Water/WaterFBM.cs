using UnityEngine;

using static System.Runtime.InteropServices.Marshal;
using static MemoryFishing.Utilities.VectorUtils;

public class WaterFBM : MonoBehaviour
{
    public struct Wave
    {
        public Vector2 direction;
        public Vector2 origin;

        public Wave(float direction, Vector2 origin)
        {
            this.direction = DegreesToVector(direction).normalized;
            this.origin = origin;
        }
    }

    [Space, SerializeField] private Material waterMaterial;

    [Header("Wave Settings")]
    [SerializeField] private int waveCount = 32;

    [Space, SerializeField] private int randomSeed = 0;

    [Space, SerializeField] private float medianDirection = 0f;
    [SerializeField] private float directionalRange = 30f;

    [Space, SerializeField] private float planeLength;
    
    private ComputeBuffer waveBuffer;
    private void Start()
    {
        GenerateWaves();
    }

    public void GenerateWaves()
    {
        waveBuffer = new ComputeBuffer(waveCount, SizeOf(typeof(Wave)));
        Random.InitState(randomSeed);

        float halfLength = planeLength / 2f;
        Vector3 size = new Vector3(halfLength, 0.0f, halfLength);

        Vector3 minPoint = transform.TransformPoint(-size);
        Vector3 maxPoint = transform.TransformPoint(size);

        float directionMin = medianDirection - directionalRange;
        float directionMax = medianDirection + directionalRange;

        Wave[] waves = new Wave[waveCount];

        for (int i = 0; i < waves.Length; i++)
        {
            float x = Random.Range(minPoint.x * 2, maxPoint.x * 2);
            float y = Random.Range(minPoint.x * 2, maxPoint.x * 2);

            Vector2 origin = new(x, y);

            float direction = Random.Range(directionMin, directionMax);

            waves[i] = new Wave(direction, origin);
        }

        waveBuffer.SetData(waves);
        waterMaterial.SetBuffer("_Waves", waveBuffer);
    }
}
