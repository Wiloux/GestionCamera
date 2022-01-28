using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed = 10.0f;

	public float maxVelocity = 2;

	Rigidbody _rigidbody = null;
	protected bool IsActive { get; private set; }

	public void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
    {
		Vector3 direction = Vector3.zero;
		direction += Input.GetAxisRaw("Horizontal") * Vector3.right;
		direction += Input.GetAxisRaw("Vertical") * Vector3.forward;
		direction.Normalize();

		if (_rigidbody && _rigidbody.velocity.magnitude < maxVelocity) { _rigidbody.velocity += direction * speed * Time.deltaTime; }
	}
}
