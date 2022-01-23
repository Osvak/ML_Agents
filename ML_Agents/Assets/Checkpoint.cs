using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Checkpoints checkpoints;
    private void OnTriggerEnter(Collider other)
    {
        //if(other.TryGetComponent<RollerAgent>(out RollerAgent agent))
        //{
        //    checkpoints.cpPass(this);
        //}
    }

    public void SetCheckpoints(Checkpoints checkpoints)
    {
        this.checkpoints = checkpoints;
    }
}
