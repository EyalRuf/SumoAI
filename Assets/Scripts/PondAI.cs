using System.Collections;

/// <summary>
/// An example AI.
/// You can modify this template to make your own AI
/// </summary>
public class PondAI : BaseAI
{
    public override IEnumerator RunAI() {
        while (true)
        {
            yield return Ahead(300);
            yield return TurnRight(180);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnScannedRobot(ScannedRobotEvent e)
    {
    }
}
