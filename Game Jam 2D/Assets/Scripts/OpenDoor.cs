using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] public FloatSO doors;
    private Animator ANIM;
    private void Start()
    {
        ANIM = GetComponent<Animator>();

    }
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
        ANIM.SetTrigger("OPEN");
        yield return new WaitForSeconds(0.5f);
        DeleteDoors();
    }

    private void DeleteDoors()
    {
        Destroy(GetComponent<BoxCollider2D>().gameObject);

    }
}
