using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] public FloatSO doors;
    [SerializeField] GameObject[] DoorsObj;

    // Update is called once per frame
    void Update()
    {
        for( int i= 0; i <= doors.Value-1; i++)
        {
            DoorsObj[i].GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (doors.Value == DoorsObj.Length)
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
        for (int i = 0; i <= doors.Value - 1; i++)
        {
            Destroy(DoorsObj[i].GetComponentInParent<SpriteRenderer>().gameObject);
            Destroy(DoorsObj[i].GetComponentInParent<BoxCollider2D>().gameObject);
            Destroy(DoorsObj[i].GetComponentInParent<Rigidbody2D>().gameObject);
        }

    }
}
