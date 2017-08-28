using UnityEngine;
using System.Collections;

public class ObjectRotation : MonoBehaviour
{
	public Transform target;
	public bool down = false;
	public float limit = 10.0f;
	
	private float _inertia = 0.0f;
	private float _prevX;
	private float _prevY;
	private Vector2 _delta = new Vector2(0.0f, 0.0f);
	
	void Awake()
	{
		if (target == null)
		{
			target = transform;
		}
	}
	
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			_delta.x = 0.0f;
			_delta.y = 0.0f;
			_prevX = Input.mousePosition.x;
			_prevY = Input.mousePosition.y;
			down = true;
		}
		
		if (Input.GetMouseButtonUp(0))
		{
			down = false;
			
			if (_delta.magnitude > 8.0f)
			{
				float v = Mathf.Clamp(_delta.sqrMagnitude, 0.0f, limit);
				_delta.Normalize();
				_delta *= v;
				_inertia = 1.0f;
			}
		}
		
		if (down)
		{
			_delta.x = _prevX - Input.mousePosition.x;
			_delta.y = _prevY - Input.mousePosition.y;
			_prevX = Input.mousePosition.x;
			_prevY = Input.mousePosition.y;
			Vector3 aular = new Vector3(-_delta.y, _delta.x, 0.0f);
			target.Rotate(aular, Space.World);
		}
		else if(_inertia >= 0.0f)
		{
			_inertia *= 0.1f;
			
			if (_inertia > 0.05f)
			{
				Vector3 aular = new Vector3(-_delta.y * _inertia, _delta.x * _inertia, 0.0f);
				target.Rotate(aular, Space.World);
			}
			else
			{
				_inertia = 0.0f;
			}
		}
	}
}