using UnityEngine;

public class TankMover : Mover
{
    private Rigidbody rb; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    public override void Move(Vector3 direction, float speed)
    {
        Vector3 moveVector = direction.normalized * speed * Time.deltaTime; 
        rb.MovePosition(rb.position + moveVector);
    }
    public override void Rotate(float speed)
    {
        transform.Rotate(0, speed * Time.deltaTime, 0); 
    }

}
