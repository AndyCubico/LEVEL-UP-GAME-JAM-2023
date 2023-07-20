using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EOS_COLLISION
{
    ON_COLLISION_NONE,
    ON_COLLISION_ENTER,
    ON_COLLISION_STAY,
    ON_COLLISION_EXIT
}

public class EoS_CollisionsManager : MonoBehaviour
{
    public EOS_COLLISION coll_state;

    // Start is called before the first frame update
    void Start()
    {
        coll_state = EOS_COLLISION.ON_COLLISION_NONE;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            coll_state = EOS_COLLISION.ON_COLLISION_ENTER;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            coll_state = EOS_COLLISION.ON_COLLISION_STAY;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Weapon")
        {
            return;
        }
            if (collision.tag == "Player")
        {
            coll_state = EOS_COLLISION.ON_COLLISION_EXIT;
        }
    }
}
