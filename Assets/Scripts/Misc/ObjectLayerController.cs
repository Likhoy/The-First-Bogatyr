using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLayerController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            transform.position -= new Vector3(0, 0, 2);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
            transform.position += new Vector3(0, 0, 2);
    }
}
