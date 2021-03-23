using UnityEngine;
using System.Collections;

public class Sumo : MonoBehaviour
{
    private Rigidbody rb;
    private Collider coll;
    private MeshRenderer mr;

    public bool actionLocked;

    [Header("Movement")]
    public float movementSpeed;

    [Header("Rotation")]
    public float rotationSpeed;

    [Header("Pushing")]
    public bool isPushing;
    public float pushCD;
    public float pushDuration;
    public float pushForce;

    [Header("Dodge")]
    public bool usedDodge;
    public float dodgeCD;
    public float dodgeDuration;
    public Material originalMat;
    public Material dodgeMat;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        mr = GetComponent<MeshRenderer>();
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
        mr.material = dodgeMat;

        // State changes
        usedDodge = true;
        StartCoroutine(ActionLockDodge(dodgeDuration));
        StartCoroutine(DodgeCooldown());
    }
    public void Push()
    {
        // Performing the push
        rb.AddForce(transform.forward * pushForce, ForceMode.Impulse);

        // State changes
        isPushing = true;
        StartCoroutine(ActionLockPush(pushDuration));
        StartCoroutine(PushCooldown());
    }

    IEnumerator ActionLockPush(float duration)
    {
        actionLocked = true;
        yield return new WaitForSeconds(duration);
        ActionUnlock();
    }

    IEnumerator ActionLockDodge(float duration)
    {
        actionLocked = true;
        yield return new WaitForSeconds(duration);
        mr.material = originalMat;
        coll.enabled = true;
        ActionUnlock();
    }

    void ActionUnlock()
    {
        actionLocked = false;
        rb.angularVelocity = Vector3.zero;
    }

    IEnumerator DodgeCooldown()
    {
        yield return new WaitForSeconds(dodgeCD);
        usedDodge = false;
    }

    IEnumerator PushCooldown()
    {
        yield return new WaitForSeconds(pushCD);
        isPushing = false;
    }
}
