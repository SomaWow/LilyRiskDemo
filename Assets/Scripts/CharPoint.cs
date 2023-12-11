using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharPoint : Point
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            ladder = collision.gameObject.name;
        }

        if (collision.gameObject.tag == "GroundBlock")
        {
            ground = collision.gameObject.name;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            ladder = null;
        }

        if (collision.gameObject.tag == "GroundBlock")
        {
            ground = null;
        }
    }
}
