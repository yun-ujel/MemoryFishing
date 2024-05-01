using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaterFBM))]
public class WaterFBMEditor : Editor
{
    private WaterFBM water;

    private void OnEnable()
    {
        water = (WaterFBM)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!EditorApplication.isPlaying)
        {
            return;
        }

        EditorGUILayout.Space(8);

        if (GUILayout.Button("Update"))
        {
            water.GenerateWaves();
        }
    }
}
