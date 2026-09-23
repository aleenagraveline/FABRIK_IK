using UnityEngine;

public class FABRIK : MonoBehaviour
{
    [SerializeField] private Transform[] joints;
    [SerializeField] private Transform target;

    private float[] boneLengths;
    private Vector3[] positions;

    // These store the animated pose at the beginning of each frame
    private Vector3[] animatedPositions;
    private Quaternion[] animatedRotations;

    void Start()
    {
        boneLengths = new float[joints.Length - 1];
        positions = new Vector3[joints.Length];

        animatedPositions = new Vector3[joints.Length];
        animatedRotations = new Quaternion[joints.Length];

        for (int i = 0; i < joints.Length - 1; i++)
        {
            boneLengths[i] = Vector3.Distance(
                joints[i].position,
                joints[i + 1].position
            );
        }
    }

    void LateUpdate()
    {
        Solve();
    }

    void Solve()
    {
        // save current animated pose
        for (int i = 0; i < joints.Length; i++)
        {
            animatedPositions[i] = joints[i].position;
            animatedRotations[i] = joints[i].rotation;

            positions[i] = joints[i].position;
        }

        float totalLength = 0f;

        // calculate total length of arm
        for (int i = 0; i < boneLengths.Length; i++)
        {
            totalLength += boneLengths[i];
        }

        // calculate distance from shoulder to target
        float targetDistance = Vector3.Distance(
            positions[0],
            target.position
        );

        if (targetDistance > totalLength) // unreachable target
        {
            for (int i = 0; i < positions.Length - 1; i++)
            {
                positions[i + 1] = positions[i] +
                    (target.position - positions[i]).normalized *
                    boneLengths[i];
            }
        }
        else // reachable target
        {
            for (int iteration = 0; iteration < 10; iteration++)
            {
                // Set hand to target
                positions[positions.Length - 1] = target.position;

                // Backward pass
                for (int i = positions.Length - 2; i >= 0; i--)
                {
                    Vector3 direction =
                        (positions[i] - positions[i + 1]).normalized;

                    positions[i] =
                        positions[i + 1] +
                        direction * boneLengths[i];
                }

                // Keep shoulder at its animated position
                positions[0] = animatedPositions[0];

                // Forward pass
                for (int i = 0; i < positions.Length - 1; i++)
                {
                    Vector3 direction =
                        (positions[i + 1] - positions[i]).normalized;

                    positions[i + 1] =
                        positions[i] +
                        direction * boneLengths[i];
                }
            }
        }

        // apply rotations
        for (int i = 0; i < joints.Length - 1; i++)
        {
            // Direction of this bone in the animated pose
            Vector3 animatedDirection =
                (animatedPositions[i + 1] - animatedPositions[i]).normalized;

            // Direction FABRIK wants this bone to point
            Vector3 desiredDirection =
                (positions[i + 1] - positions[i]).normalized;

            // Rotation needed to move the animated direction
            // to the FABRIK direction
            Quaternion ikRotation =
                Quaternion.FromToRotation(
                    animatedDirection,
                    desiredDirection
                );

            // Apply IK adjustment on top of the animation
            joints[i].rotation =
                ikRotation * animatedRotations[i];
        }
    }
}