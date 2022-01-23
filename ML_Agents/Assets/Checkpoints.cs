using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    private void Awake()
    {
        Transform checkpointsTransofrm = transform.Find("Checkpoints");

        foreach (Transform cpTransform in checkpointsTransofrm)
        {
            Checkpoint checkpoint = cpTransform.GetComponent<Checkpoint>();
            checkpoint.SetCheckpoints(this);
        }
    }

    public void cpPass(Checkpoint checkpoint)
    {
        Debug.Log(checkpoint.transform.name);
    }
}
