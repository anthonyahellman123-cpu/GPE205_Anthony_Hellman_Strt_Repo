
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public Pawn pawn;

    // Called when the controller starts.
    public virtual void Start()
    {

    }

    // Process controller input every frame.
    public virtual void Update()
    {
        ProcessInputs();
    }

    // Each controller decides how to control its pawn.
    public abstract void ProcessInputs();
}
