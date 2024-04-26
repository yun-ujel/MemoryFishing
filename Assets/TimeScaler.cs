using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    [SerializeField] private float timeScale = 1f;

    void Update()
    {
        Time.timeScale = timeScale;
    }
}
