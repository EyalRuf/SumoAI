using UnityEngine;
using System.Collections;

public class Sumo : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;

    public bool actionLocked;

    [Header("Movement")]
    public float movementSpeed;
    //public Vector3 moveDestination;

    [Header("Rotation")]
    public float rotationSpeed;

    [Header("Pushing")]
    public bool usedPush;
    public float pushCD;
    public float pushDuration;
    public float pushForce;

    [Header("Dodge")]
    public bool usedDodge;
    public float dodgeCD;
    public float dodgeDuration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    public void MoveTowards(Vector3 destination)
    {
        float step = movementSpeed * Time.deltaTime;
        rb.position = Vector3.MoveTowards(rb.position, destination, step);
    }

    public void RotateTo(float angle)
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, angle, 0), rotationSpeed * Time.deltaTime);
    }

    public void Dodge()
    {
        // Doing the dodge
        coll.enabled = false;

        // State changes
        usedDodge = true;
        actionLocked = true;
        StartCoroutine(ActionLockDodge(dodgeDuration));
        StartCoroutine(DodgeCooldown());
    }
    public void Push()
    {
        // Performing the push
        rb.AddForce(transform.forward * pushForce, ForceMode.Impulse);

        // State changes
        usedPush = true;
        actionLocked = true;
        StartCoroutine(ActionLockPush(pushDuration));
        StartCoroutine(PushCooldown());
    }

    IEnumerator ActionLockPush(float duration)
    {
        yield return new WaitForSeconds(duration);
        actionLocked = false;
    }

    IEnumerator ActionLockDodge(float duration)
    {
        yield return new WaitForSeconds(duration);
        coll.enabled = true;
        actionLocked = false;
    }

    IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCD);
        usedDodge = false;
    }

    IEnumerator PushCooldown()
    {
        yield return new WaitForSeconds(pushCD);
        usedPush = false;
    }
}
