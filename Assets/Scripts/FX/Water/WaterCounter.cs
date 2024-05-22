using UnityEngine;
using UnityEngine.InputSystem;

using MemoryFishing.Utilities;

public class WaterCounter : MonoBehaviour
{
    private PlayerInput playerInput;

    [SerializeField] private Material material;
    private int waveCount;

    private void Start()
    {
        playerInput = GeneralUtils.GetPlayerInput();
        playerInput.actions["Player/Cancel"].performed += OnPressButton;
    }

    private void OnPressButton(InputAction.CallbackContext ctx)
    {
        waveCount++;

        material.SetFloat("_VertexWaveCount", waveCount);
        material.SetFloat("_FragmentWaveCount", waveCount);
    }
}
