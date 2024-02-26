using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBehavior : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;
    private Rigidbody m_Rigidbody;
    private Vector3 wanderTarget;
    private bool isGrounded;

    public Vector3 targetDirection;

    [SerializeField] private int speed;
    [SerializeField] private int jumpForce;
    [SerializeField] private float rotationSpeed;


    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        wanderTarget = transform.position + new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Chase()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    public void Wander()
    {
        
    }
    public void Attack() 
    { 
        
    }
    public void Multiply() 
    { 
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
        m_Rigidbody.velocity = Vector3.zero;
        
        if (collision.gameObject.tag == "Ground")
            m_Rigidbody.AddForce(Vector3.up * jumpForce);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
            Chase();
    }
}
