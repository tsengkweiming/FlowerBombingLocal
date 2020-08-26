using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPUDuneSystem {

	public class GPUDuneBehaviour : MonoBehaviour {
		[Range(0,4)]
		public int lod = 1;

		[SerializeField]
		protected TextureEvents OnCreate;
		[SerializeField]
		protected TextureEvents OnUpdate;
		[SerializeField]
		protected GPUDune dune;
		[SerializeField]
		protected Camera targetCam;

		#region Unity
		private void OnEnable() {
			dune.Init();
			dune.OnCreate += (d) => OnCreate.Invoke(dune.Target);
		}
		private void OnDisable() {
			dune.Dispose();
		}
		private void Update() {
			var size = new Vector2Int(
				(targetCam != null ? targetCam.pixelWidth : Screen.width),
				(targetCam != null ? targetCam.pixelHeight : Screen.height));
			dune.Lod = lod;
			dune.Size = size;
			dune.Update();
			OnUpdate.Invoke(dune.Target);
		}
		#endregion

		public Camera CurrentCamera {
			get { return targetCam; }
			set {
				targetCam = value;
			}
		}
		public GPUDune Dune {
			get { return dune; }
		}
	}
}
