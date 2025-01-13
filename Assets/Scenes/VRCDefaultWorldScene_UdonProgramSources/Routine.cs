using UnityEngine;

public class Routine : BaseUdonCoroutine
{
    private int counter = 0;

    protected override bool Tick()
    {
        if (counter < 10)
        {
            Debug.Log($"Progress: {counter}/10");
            counter++;

            return false;
        }

        return true;
    }
}