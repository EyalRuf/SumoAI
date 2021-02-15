using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The event data that is generated when another participant in the arena was 'seen'
/// </summary>
public class ScannedRobotEvent {
    public string Name;
    public float Distance; 
}

public class BaseAI
{
    public PirateShipController Ship = null;

    /// <summary>
    /// Another participant was 'seen'. Do something with the info stored in the even data
    /// </summary>
    /// <param name="e">The event data</param>
    public virtual void OnScannedRobot(ScannedRobotEvent e)
    {
        // 
    }

    /// <summary>
    /// Move this ship ahead by the given distance
    /// </summary>
    /// <param name="distance">The distance to move</param>
    /// <returns></returns>
    public IEnumerator Ahead(float distance) {
        yield return Ship.__Ahead(distance);
    }

    /// <summary>
    /// Move the ship backwards by the given distance
    /// </summary>
    /// <param name="distance">The distance to move</param>
    /// <returns></returns>
    public IEnumerator Back(float distance) {
        yield return Ship.__Back(distance);
    }

    /// <summary>
    /// Turn the sensor to the left by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator TurnLookoutLeft(float angle) {
        yield return Ship.__TurnLookoutLeft(angle);
    }

    /// <summary>
    /// Turn the sensor to the right by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator TurnLookoutRight(float angle) {
        yield return Ship.__TurnLookoutRight(angle);
    }

    /// <summary>
    /// Turns the ship left by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator TurnLeft(float angle) {
        yield return Ship.__TurnLeft(angle);
    }

    /// <summary>
    /// Turns the ship right by the given angle
    /// </summary>
    /// <param name="angle">The angle to rotate</param>
    /// <returns></returns>
    public IEnumerator TurnRight(float angle) {
        yield return Ship.__TurnRight(angle);
    }

    /// <summary>
    /// Fire from the forward pointing cannon
    /// </summary>
    /// <param name="power">???</param>
    /// <returns></returns>
    public IEnumerator FireFront(float power) {
        yield return Ship.__FireFront(power);
    }

    /// <summary>
    /// Fire from the left pointing cannon
    /// </summary>
    /// <param name="power">???</param>
    /// <returns></returns>
    public IEnumerator FireLeft(float power) {
        yield return Ship.__FireLeft(power);
    }

    /// <summary>
    /// fire from the right pointing cannon
    /// </summary>
    /// <param name="power">???</param>
    /// <returns></returns>
    public IEnumerator FireRight(float power) {
        yield return Ship.__FireRight(power);
    }

    public virtual IEnumerator RunAI() {
        yield return null;
    }
}
