using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public void Init(VoidVoidDelegate _openDoorCallback, VoidVoidDelegate _closeDoorCallback)
    {
        openDoorCallback = _openDoorCallback;
        closeDoorCallback = _closeDoorCallback;

        StartCoroutine("CheckIsUnitEnter");
    }

    private IEnumerator CheckIsUnitEnter()
    {
        while (true)
        {
            if(Physics.CheckSphere(transform.position, 2f, friendlyUnitLayer))
            {
                if(!isOpen)
                {
                    openDoorCallback?.Invoke();
                    isOpen = true;
                }
            }
            else
            {
                if (isOpen)
                {
                    closeDoorCallback?.Invoke();
                    isOpen = false;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    [SerializeField]
    private LayerMask friendlyUnitLayer;

    private VoidVoidDelegate openDoorCallback = null;
    private VoidVoidDelegate closeDoorCallback = null;

    private bool isOpen = false;
}
