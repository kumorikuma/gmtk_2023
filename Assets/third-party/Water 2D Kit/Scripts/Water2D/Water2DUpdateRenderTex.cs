using UnityEngine;

namespace RavingBots.Water2D
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class Water2DUpdateRenderTex : MonoBehaviour
	{
		public float MaxGrabsPerSecond = -1;

		private float _lastGrabTime = -1;
		private Camera _grabCamera;
		private Camera _mainCamera;

		protected void Awake()
		{
			_grabCamera = GetComponent<Camera>();
			_mainCamera = transform.parent.GetComponentInParent<Camera>();
		}

		private void UpdateRenderTex()
		{
			if (Mathf.Approximately(MaxGrabsPerSecond, 0f))
			{
				_grabCamera.enabled = false;
				return;
			}

			_grabCamera.orthographicSize = _mainCamera.orthographicSize;
			_grabCamera.transform.rotation = Quaternion.identity;

			if (MaxGrabsPerSecond < 0)
				_grabCamera.enabled = true;
			else if (Time.time >= _lastGrabTime + 1f/MaxGrabsPerSecond)
			{
				_lastGrabTime = Time.time;
				_grabCamera.enabled = false;
				_grabCamera.Render();
			}
		}

		protected void LateUpdate()
		{
			UpdateRenderTex();
		}
	}
}
