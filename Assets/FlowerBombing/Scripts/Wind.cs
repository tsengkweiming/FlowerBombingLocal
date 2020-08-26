using nobnak.Gist.MathAlgorithms.Distribution;
using UnityEngine;



	public class Wind : MonoBehaviour
	{
		public const string PROP_NORMAL_MAP = "_NormalTex";
		public const string PROP_WORLD_TO_UV_MATRIX = "_WorldToUVMatrix";

		public const float ROUND_IN_DEG = 360f;

		[SerializeField]
		[Range(0f, 1f)]
		float tiltPower = 1f;
		[SerializeField]
		protected Texture noiseTex;

		#region Unity
		void Update()
		{
			SetGlobalWorldToUVMatrix(Camera.main);
		}
		#endregion

		public virtual void SetNoiseMap(Texture noiseTex)
		{
			this.noiseTex = noiseTex;
			Debug.Log("SetNoiseMap");
			Shader.SetGlobalTexture(PROP_NORMAL_MAP, noiseTex);
		}
		//public override void Add(Plant p)
		//{
		//	p.Tilt = InitialTilt();
		//	p.currNormal = Quaternion.identity;
		//}

		protected Quaternion InitialTilt()
		{
			var gaussian = Gaussian.BoxMuller();
			var tilt = Quaternion.FromToRotation(Vector3.up, Vector3.back)
				* Quaternion.Euler(tiltPower * gaussian.x, Random.Range(0f, ROUND_IN_DEG), tiltPower * gaussian.y);
			return tilt;
		}

		protected static void SetGlobalWorldToUVMatrix(Camera cam)
		{
			var mat = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(0.5f, 0.5f, 1f))
				* GL.GetGPUProjectionMatrix(cam.projectionMatrix, cam.targetTexture != null)
				* cam.worldToCameraMatrix;
			Shader.SetGlobalMatrix(PROP_WORLD_TO_UV_MATRIX, mat);
		}
	}

	public partial class PlantFlower
	{
		public const string PROP_TILT = "_InitialTilt";

		public static readonly int ID_TILT = Shader.PropertyToID(PROP_TILT);

		[HideInInspector]
		public Quaternion tilt;
		[HideInInspector]
		public Quaternion currNormal;

		public Quaternion Tilt
		{
			set
			{
				tilt = value;
				//Block.SetVector(ID_TILT, new Vector4(value.x, value.y, value.z, value.w));
			}
		}
	}
