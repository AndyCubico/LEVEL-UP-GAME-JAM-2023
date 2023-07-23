using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] public FloatSO doors;

    // Update is called once per frame
    void Update()
    {
        if (doors.Value == 3.0f)
        { 
            StartCoroutine(Open());
        }
    }

    IEnumerator Open()
    {
        yield return new WaitForSeconds(0.5f);
        DeleteDoors();
    }

    private void DeleteDoors()
    {

    }
}
