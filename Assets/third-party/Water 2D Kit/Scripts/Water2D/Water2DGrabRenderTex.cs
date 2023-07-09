using UnityEngine;

namespace RavingBots.Water2D
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	public class Water2DGrabRenderTex : MonoBehaviour
	{
		public Camera GrabCamera;

		private MeshRenderer _renderer;

		protected void Awake()
		{
			_renderer = GetComponent<MeshRenderer>();

			if (!GrabCamera || !GrabCamera.targetTexture)
				Debug.LogWarning("Attach the camera with the render texture");
		}
		
		private void UpdateGrabTransform()
		{
			if (!GrabCamera || !GrabCamera.targetTexture)
				return;

			var a = GrabCamera.ViewportToWorldPoint(new Vector3(0, 0, GrabCamera.nearClipPlane));
			var b = GrabCamera.ViewportToWorldPoint(new Vector3(0, 1, GrabCamera.nearClipPlane));

			var rt = GrabCamera.targetTexture;
			var m = _renderer.sharedMaterial;

			var h = (a - b).magnitude;
			var w = h*((float)rt.width/rt.height);

			var s = new Vector2(transform.lossyScale.x/w, transform.lossyScale.y/h);

			var c = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
			Vector2 o = c - a;
			o.x /= w;
			o.y /= h;

			m.SetTextureScale("_GrabTex", s);
			m.SetTextureOffset("_GrabTex", o);
		}

		protected void LateUpdate()
		{
			UpdateGrabTransform();
		}
	}
}
