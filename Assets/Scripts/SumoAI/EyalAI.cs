using UnityEngine;
using System.Collections;

public class EyalAI : SumoBaseAI
{
    // save info about other sumos


    // Update is called once per frame
    void Update()
    {
        //CalcObjective
        // => set objectve
        this.currObjective = AiObjective.push;
    }
}
