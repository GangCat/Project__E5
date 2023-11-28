using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueImage : MonoBehaviour
{
    public void Deqeueu()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
