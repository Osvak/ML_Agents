using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    Rigidbody rBody;
    [SerializeField] private Material winMat;
    [SerializeField] private Material loseMat;
    [SerializeField] private Material hitMat;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    private List<GameObject> checkpointsDisabled = new List<GameObject>();
    private List<Vector3> spawns = new List<Vector3>();

    private int counter = 0;
    private int rand;

    private Collider cpCollider;

    private Vector3 pos1 = new Vector3(-4.6f, 0.5f, -10.84f);
    private Vector3 pos2 = new Vector3(11.57f, 0.5f, 11.5f);

    private Vector3 spawn1 = new Vector3(-11f, 0.5f, -11f);
    private Vector3 spawn2 = new Vector3(11f, 0.5f, -11f);
    private Vector3 spawn3 = new Vector3(-3f, 0.5f, 6f);


    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        spawns.Add(spawn1);
        spawns.Add(spawn2);
        spawns.Add(spawn3);
    }

    public Transform Target;

    //Collision collision;

    public override void OnEpisodeBegin()
    {
        rand = Random.Range(0, 2);
        counter = 0;

        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        //this.transform.localPosition = spawn1;
        this.transform.localPosition = spawns[rand];
        // Move the target to a new spot
        foreach (GameObject cp in checkpointsDisabled)
        {
            cp.SetActive(true);
        }
        
        //Target.localPosition = pos1;
        Target.localPosition = pos2;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 8;

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);
        // Rewards
        counter++;

        if (rBody.velocity.x < 0.2f && rBody.velocity.z < 0.2f)
        {
            SetReward(-0.1f);
        }

        if (counter >= 2000)
        {
            SetReward(-1.0f);
            floorMeshRenderer.material = loseMat;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if( collision.collider.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+1.0f);
            floorMeshRenderer.material = winMat;
            EndEpisode();
        }
        if( collision.collider.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-0.5f);
            floorMeshRenderer.material = hitMat;
        }
        if (collision.collider.TryGetComponent<Trap>(out Trap trap))
        {
            SetReward(-1.0f);
            floorMeshRenderer.material = loseMat;
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
        {
            SetReward(+0.1f);
            checkpointsDisabled.Add(other.gameObject);
            other.gameObject.SetActive(false);
            cpCollider = other;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-0.8f);
        }
    }
}