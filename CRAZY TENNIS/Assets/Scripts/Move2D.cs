using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move2D : MonoBehaviour
{
    public float moveSpeed = 2f;
	private Vector3 movement = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if(Input.GetKey(KeyCode.LeftShift))
            movement /= 2;
    }

	void FixedUpdate()
	{
		GetComponent<Rigidbody2D>().velocity = movement * moveSpeed;
	}
}
