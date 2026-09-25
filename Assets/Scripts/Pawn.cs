using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    //move speed variable 
    public float moveSpeed;

    //turn speed variable
    public float turnSpeed;

    public Mover mover;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        mover = GetComponent<Mover>();  
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public abstract void moveForward();

    public abstract void moveBackward();

    public abstract void rotateClockwise();

    public abstract void rotateCounterClockwise();

}
