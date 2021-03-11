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
    public bool isPushing;
    public float pushCD;
    public float pushDuration;
    public float pushForce;

    [Header("Dodge")]
    public bool isDodging;
    public float dodgeCD;
    public float dodgeDuration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
    }

    public void Movement()
    {
        rb.MovePosition(rb.position + (transform.forward * movementSpeed * Time.deltaTime));
    }

    public void Rotate(float rotateTo)
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, rotateTo, 0), rotationSpeed * Time.deltaTime);
    }

    public void Dodge()
    {
        // Doing the dodge
        coll.enabled = false;

        // State changes
        isDodging = true;
        actionLocked = true;
        StartCoroutine(ActionLock(dodgeDuration));
        StartCoroutine(DodgeCooldown());
    }
    public void Push()
    {
        // Performing the push
        rb.AddForce(transform.forward * pushForce, ForceMode.Impulse);

        // State changes
        isPushing = true;
        actionLocked = true;
        StartCoroutine(ActionLock(pushDuration));
        StartCoroutine(PushCooldown());
    }

    IEnumerator ActionLock(float duration)
    {
        yield return new WaitForSeconds(duration);
        actionLocked = false;
    }

    IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCD);
        isDodging = false;
        coll.enabled = true;
    }

    IEnumerator PushCooldown()
    {
        yield return new WaitForSeconds(pushCD);
        isPushing = false;
    }
}
