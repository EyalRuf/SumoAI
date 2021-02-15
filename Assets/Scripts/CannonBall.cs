using UnityEngine;

/// <summary>
/// The projectile that is fired.
/// Currently never leaves the scene once added
/// </summary>
public class CannonBall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Makes the bullet fly
    /// </summary>
    void FixedUpdate()
    {
        transform.Translate(new Vector3(0f, 0f, 500 * Time.fixedDeltaTime), Space.Self);
    }
}
