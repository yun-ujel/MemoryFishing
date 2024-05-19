using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class BloomPassSettings
{
    [field: Header("Rendering"), SerializeField] public RenderPassEvent RenderPassEvent { get; set; }
    [field: SerializeField] public string[] PassTags { get; set; } = new string[] { "Bloom" };

    [field: Header("Bloom"), SerializeField, Range(1, 16)] public int Downsamples { get; set; } = 1;
    [field: SerializeField, Range(0.01f, 2.0f)] public float DownsampleDelta { get; set; } = 1;
    [field: SerializeField, Range(0.01f, 2.0f)] public float UpsampleDelta { get; set; } = 1;

    [field: Space, SerializeField, Range(0.01f, 10.0f)] public float Intensity { get; set; } = 1;
}