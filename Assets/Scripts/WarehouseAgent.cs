using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem; // ADDED: Required for the new Input System

public class WarehouseAgent : Agent
{
    // --- PUBLIC REFERENCES (Drag in Inspector) ---
    public GameManager gameManager;

    [Tooltip("The specific point where the agent will start each episode.")]
    public Transform agentStartPosition;
    [Tooltip("The delivery area the agent needs to return the parcel to.")]
    public Transform packingArea; 
    [Tooltip("How fast the agent moves forward. Adjust this value if the agent is too fast or slow.")]
    public float moveSpeed = 4f; // A stable, controllable speed
    [Tooltip("How fast the agent turns. Adjust this if turning is too sensitive.")]
    public float turnSpeed = 150f; // A stable turning speed

    // --- AGENT STATE (Private) ---
    private Rigidbody agentRb;
    private bool hasParcel;
    private GameObject carriedParcel;
    private Transform parcelTarget;

    // --- Action Variables ---
    private float m_ForwardInput;
    private float m_TurnInput;

    public override void Initialize()
    {
        agentRb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset agent state
        hasParcel = false;
        carriedParcel = null;
        m_ForwardInput = 0f;
        m_TurnInput = 0f;

        // Reset agent physics
        if (agentRb != null)
        {
            agentRb.linearVelocity = Vector3.zero;
            agentRb.angularVelocity = Vector3.zero;
        }

        // Reset agent position and rotation
        if (agentStartPosition != null)
        {
            transform.position = agentStartPosition.position;
            transform.rotation = agentStartPosition.rotation;
        }

        // Find the newly spawned parcel
        GameObject parcelObject = GameObject.FindGameObjectWithTag("Parcel");
        if (parcelObject != null)
        {
            parcelTarget = parcelObject.transform;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // The target is now conditional: if we have the parcel, the target is the packing area.
        // Otherwise, the target is the parcel.
        Transform currentTarget = hasParcel ? packingArea : parcelTarget;
        if (currentTarget == null)
        {
            sensor.AddObservation(new float[7]); // Return empty observations if no target
            return;
        }

        // Observe direction to target, relative to agent's own orientation
        Vector3 directionToTarget = transform.InverseTransformDirection((currentTarget.position - transform.position).normalized);
        sensor.AddObservation(directionToTarget); // 3 values
        sensor.AddObservation(hasParcel ? 1f : 0f); // 1 value (re-added hasParcel state)
        sensor.AddObservation(transform.InverseTransformDirection(agentRb.linearVelocity)); // 3 values
    }

    /// <summary>
    /// The agent's brain makes a decision here. We just store the inputs.
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions)
    {
        m_ForwardInput = actions.ContinuousActions[0];
        m_TurnInput = actions.ContinuousActions[1];
    }

    /// <summary>
    /// This method allows a human to control the agent with the keyboard for testing.
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        
        // Read input directly from the keyboard using the new Input System
        float forwardInput = 0f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) forwardInput = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) forwardInput = -1f;

        float turnInput = 0f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) turnInput = 1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) turnInput = -1f;

        continuousActionsOut[0] = forwardInput;
        continuousActionsOut[1] = turnInput;
    }

    /// <summary>
    /// All physics and reward logic is now in FixedUpdate for stability.
    /// </summary>
    void FixedUpdate()
    {
        // Apply physics-based rotation for smooth turning
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(Vector3.up * m_TurnInput * turnSpeed * Time.fixedDeltaTime);
        agentRb.MoveRotation(targetRotation);

        // Apply physics-based forward movement for smooth motion
        Vector3 targetVelocity = transform.forward * m_ForwardInput * moveSpeed;
        agentRb.linearVelocity = new Vector3(targetVelocity.x, agentRb.linearVelocity.y, targetVelocity.z);

        // --- Smart Reward System ---
        Transform currentTarget = hasParcel ? packingArea : parcelTarget;
        if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            
            // Reward for facing the target (encourages turning correctly)
            float facingReward = Mathf.Clamp(Vector3.Dot(transform.forward, directionToTarget), 0f, 1f);
            
            // Reward for moving towards the target (encourages forward movement)
            float velocityReward = Mathf.Clamp(Vector3.Dot(agentRb.linearVelocity.normalized, directionToTarget), 0f, 1f);

            // Combine and apply the rewards to guide the agent
            AddReward((facingReward * velocityReward) * 0.01f);
        }

        // Small penalty for existing, encourages the agent to be efficient
        AddReward(-1f / MaxStep);
    }

    /// <summary>
    /// Handles trigger events for collecting and delivering the parcel.
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // --- Logic for picking up the parcel ---
        if (!hasParcel && other.gameObject.CompareTag("Parcel"))
        {
            hasParcel = true;
            carriedParcel = other.gameObject;
            parcelTarget = null; // We are no longer seeking the parcel
            carriedParcel.transform.SetParent(this.transform);
            carriedParcel.transform.localPosition = new Vector3(0, 1.5f, 0); // Position it behind/above agent
            carriedParcel.GetComponent<Collider>().enabled = false;
            AddReward(1.0f); // Give a large reward for pickup
        }

        // --- Logic for delivering the parcel ---
        if (hasParcel && other.gameObject.CompareTag("PackingArea"))
        {
            Destroy(carriedParcel);
            AddReward(1.0f); // Give a large reward for delivery
            if (gameManager != null) gameManager.ResetEpisode(); // Tell GM to spawn the next parcel
            EndEpisode(); // End this episode
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Obstacle"))
        {
            // If we crash while carrying a parcel, we must destroy it to prevent a memory leak.
            if (hasParcel && carriedParcel != null)
            {
                Destroy(carriedParcel);
            }
            AddReward(-1.0f);
            EndEpisode();
        }
    }
}

