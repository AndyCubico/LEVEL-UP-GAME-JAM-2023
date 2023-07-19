using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOverlay : MonoBehaviour
{
    [SerializeField] private GameObject toDisable;
    [SerializeField] private GameObject toEnable;

    // Start is called before the first frame update
    void Start()
    {
        toEnable.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Change()
    {
        toDisable.SetActive(false);
        toEnable.SetActive(true);
    }
}
