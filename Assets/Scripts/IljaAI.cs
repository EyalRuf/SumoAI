using System.Collections;
using UnityEngine;

/// <summary>
/// An example AI that is a bit more elaborate than <seealso cref="PondAI"/>.
/// </summary>
public class IljaAI : BaseAI
{
    public override IEnumerator RunAI() {
        for (int i = 0; i < 10; i++)
        {
            yield return Ahead(200);
            yield return FireFront(1);
            yield return TurnLookoutLeft(90);
            yield return TurnLeft(360);
            yield return FireLeft(1);
            yield return TurnLookoutRight(180);
            yield return Back(200);
            yield return FireRight(1);
            yield return TurnLookoutLeft(90);
            yield return TurnRight(90);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnScannedRobot(ScannedRobotEvent e)
    {
        Debug.Log("Ship detected: " + e.Name + " at distance: " + e.Distance);
    }
}
