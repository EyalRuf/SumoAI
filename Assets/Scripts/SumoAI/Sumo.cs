using UnityEngine;
using System.Collections;

public class Sumo : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;
    private Material color;

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
        color = GetComponent<Material>();
    }

    public void Movement()
    {
        rb.MovePosition(rb.position + (transform.forward * movementSpeed * Time.deltaTime));
    }

    public void Rotate(float rotationAngle)
    {
        //Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotationAngle, 0) * rotationSpeed * Time.deltaTime);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, new Vector3(0, rotationAngle, 0), rotationSpeed * Time.deltaTime, 0);
        transform.rotation = (Quaternion.LookRotation(newDirection));
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
        rb.AddForce(transform.forward * pushForce, ForceMode.Acceleration);

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
