using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAllValues : MonoBehaviour
{
    [SerializeField] private BoolSO isPlayerInLight;
    [SerializeField] private BoolSO isBossFight;
    [SerializeField] private FloatSO doorsOpen;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerInLight.Value = false;
        isBossFight.Value = false;
        doorsOpen.Value = 0;
    }
}
