using MemoryFishing.Utilities;
using UnityEngine;

using static System.Runtime.InteropServices.Marshal;
using static MemoryFishing.Utilities.MeshUtils;

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
        [SerializeField] private MeshFilter meshFilter;

        [Header("Plane Settings")]
        [SerializeField] private float cellSize;
        [SerializeField] private int size;

        [Space, SerializeField] private float yPosition;

        [Header("Wave Settings")]
        [SerializeField] private int waveCount = 4;

        [Space, SerializeField] private float medianWavelength = 1f;
        [SerializeField] private float wavelengthRange = 1f;

        [Space, SerializeField] private float medianDirection = 0f;
        [SerializeField] private float directionalRange = 30f;

        [Space, SerializeField] private float medianAmplitude = 1f;
        [SerializeField] private float steepness = 0f;

        [Space, SerializeField] private float medianSpeed = 1f;
        [SerializeField] private float speedRange = 0.1f;

        private ComputeBuffer waveBuffer;

        private Mesh mesh;

        private Vector3[] verts;
        private Vector2[] uvs;
        private int[] tris;

        private void Start()
        {
            waveBuffer = new ComputeBuffer(waveCount, SizeOf(typeof(Wave)));

            GeneratePlane();
            GenerateWaves();
        }

        private void GeneratePlane()
        {
            mesh = new Mesh();

            Vector3 origin = 0.5f * cellSize * new Vector3(-size, 0, -size);
            origin.y = yPosition;
            Vector3 quadSize = Vector3.one.ExcludeYAxis() * cellSize;

            CreateEmptyMeshArrays(size * size, out verts, out uvs, out tris);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    int i = (x * size) + y;
                    Vector3 worldPos = GridToWorldPosition(x, y, origin);
                    GetUVs(x, y, out Vector2 min, out Vector2 max);

                    AddQuadFromMeshArrays(verts, uvs, tris, i, worldPos, Vector3.up, quadSize, min, max);
                }
            }

            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.triangles = tris;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;
        }

        private Vector3 GridToWorldPosition(int x, int y, Vector3 origin)
        {
            Vector3 bottomLeft = new Vector3(x, 0, y) * cellSize;
            Vector3 halfSize = new Vector3(1, 0, 1) * cellSize / 2f;

            return bottomLeft + origin + halfSize;
        }

        private void GetUVs(int x, int y, out Vector2 min, out Vector2 max)
        {
            Vector2 cell = Vector2.one / size;
            Vector2 pos = new(x, y);

            min = cell * pos;
            max = min + (cell * pos);
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

            float halfSize = size * 0.5f;
            Vector3 minPoint = transform.TransformPoint(new Vector3(-halfSize, 0.0f, -halfSize));
            Vector3 maxPoint = transform.TransformPoint(new Vector3(halfSize, 0.0f, halfSize));

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