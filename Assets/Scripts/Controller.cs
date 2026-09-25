using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public Pawn pawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }
    public abstract void ProcessInputs();
}
