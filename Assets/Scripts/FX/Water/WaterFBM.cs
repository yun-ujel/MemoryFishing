using UnityEngine;

public class WaterFBM : MonoBehaviour
{
    [SerializeField] private Material material;

    [Header("Wave Count")]
    [SerializeField] private int vertexWaveCount = 8;
    [SerializeField] private int fragmentWaveCount = 40;

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


    private void Start()
    {
        material.SetInt("_VertexWaveCount", vertexWaveCount);
        material.SetInt("_FragmentWaveCount", fragmentWaveCount);

        material.SetFloat("_Seed", seed);
        material.SetFloat("_SeedIteration", seedIteration);

        material.SetFloat("_Frequency", frequency);
        material.SetFloat("_FrequencyMultiplier", frequencyMultiplier);

        material.SetFloat("_Amplitude", amplitude);
        material.SetFloat("_AmplitudeMultiplier", amplitudeMultiplier);

        material.SetFloat("_Speed", speed);
        material.SetFloat("_SpeedMultiplier", speedMultiplier);
    }
}
