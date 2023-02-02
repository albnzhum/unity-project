using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    GameObject Capsule;
    public float moveSpeed = 0.1f; //скорость
    public float moveSpeed1 = 0.1f;

    private float vInput; //вертикальная ось
    private float hInput; //горизонтальная ось

    //private int counter = 0;

    /*public GameObject Capsule;
    private Vector3 offset;*/
    
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<BoxCollider>().isTrigger = true;
    }
    // Update is called once per frame
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Circle")
        {
            other.transform.position = new Vector3(Random.Range(0f, 28.5f), 0.25f, Random.Range(0f, 28.5f));
            counter++;
            Debug.Log(counter);

        }
    }*/

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision detected");
    }


    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //transform.position = new Vector2(transform.position.x +1,  transform.position.y);
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * moveSpeed1;
        transform.Translate(hInput, vInput, 0);

    }
}
