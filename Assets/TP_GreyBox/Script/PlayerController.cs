using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speed = 10.0f;

	private CameraRail rail = null;
	private float distance = 0;

	Rigidbody _rigidbody = null;
	protected bool IsActive { get; private set; }


	private static PlayerController _instance;
	public static PlayerController Instance { get { return _instance; } }
	public void Awake()
	{
		if(Instance != null) { Destroy(gameObject); }
		_instance = this;

		_rigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		if (rail == null || rail.GetPoints.Count == 0)
		{
			if (Camera.main)
			{
				Vector3 direction = Vector3.zero;
				direction += Input.GetAxisRaw("Horizontal") * new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
				direction += Input.GetAxisRaw("Vertical") * new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
				direction.Normalize();
				_rigidbody.velocity = direction * speed + Vector3.up * _rigidbody.velocity.y;
			}
		}
		else
        {
			distance += Input.GetAxis("Vertical") * Time.fixedDeltaTime * speed;

			if (distance >= 0 && distance < rail.GetLength) { transform.position = rail.GetPosition(distance); }
			else
            {
				Vector3 direction = Vector3.zero;
				if (distance < 0) { direction = rail.GetPoints[1].position + (rail.GetPoints[0].position - rail.GetPoints[1].position).normalized * -distance * 2; }
				else { direction = rail.GetPoints[rail.GetPoints.Count - 1].position; distance = rail.GetLength; }
				transform.position = direction;
            }
        }
	}

	public void SetRail(CameraRail rail)
    {
		this.rail = rail;
		if (this.rail)
		{
			this.rail.isLoop = false;
			this.rail.RecalculateLength();
			this.rail.RecalculatePoints();
			_rigidbody.useGravity = false;
		}
		else { _rigidbody.useGravity = true; }

		distance = 0;

	}
}
