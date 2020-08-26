using nobnak.Gist;
using nobnak.Gist.Resizable;
using UnityEngine;

namespace GPUDuneSystem {

	[System.Serializable]
	public class GPUDune : System.IDisposable {
		public const string MATERIAL_DUNE = "Dune";

		public const string PROP_NOISE_FREQ = "_NoiseFreq";
		public const string PROP_NOISE_HEIGHT = "_NoiseHeight";
		public const string PROP_POS_SCALE = "_UvToPosScale";
		public const string PROP_POS_OFFSET = "_UvToPosOffset";

		public const float SEED_SIZE = 100f;

		public event System.Action<GPUDune> OnCreate;

		[SerializeField]
		protected float time = 0f;
		[SerializeField]
		protected int lod = 0;
		[SerializeField]
		protected Vector2Int size;
		[SerializeField]
		protected float noiseFieldSize = 1f;
		[SerializeField]
		protected float noiseTimeSpeed = 1f;
		[SerializeField]
		protected float noiseFrequency = 1f;
		[SerializeField]
		protected float noiseHeight = 1f;
		[SerializeField]
		protected Vector3 seed;

		protected Material dune;
		protected LODRenderTexture tex;
		protected Validator validator = new Validator();
		public static int stopMotion;

		public GPUDune() : this(RenderTextureFormat.ARGBHalf) { }
		public GPUDune(RenderTextureFormat format) {
			OnCreate = null;
			tex = new LODRenderTexture();
			var formatTex = tex.Format;
			formatTex.textureFormat = format;
			tex.Format = formatTex;

			validator.Reset();
			validator.Validation += () => {
				if (size.x < 2 || size.y < 2)
					return;

				tex.Lod = lod;
				tex.Size = size;
				
				Debug.LogFormat("GPUDune : Create texture size={0}", tex.LodSize);
			};
			validator.Validated += () => {
				NotifyOnCreate();
			};
			validator.SetCheckers(() => tex != null);
		}

		#region IDisposable
		public void Dispose() {
			ReleaseTexture();
		}
		#endregion

		#region Properties
		public Texture Target {
			get {
				validator.Validate();
				return tex.Texture;
			}
		}
		public float NoiseFieldSize {
			get { return noiseFieldSize; }
			set { noiseFieldSize = value; }
		}
		public float NoiseTimeSpeed {
			get { return noiseTimeSpeed; }
			set { noiseTimeSpeed = value; }
		}
		public float NoiseFrequency {
			get { return noiseFrequency; }
			set { noiseFrequency = value; }
		}
		public float NoiseHeight {
			get { return noiseHeight; }
			set { noiseHeight = value; }
		}
		public Vector2Int Size {
			get { return size; }
			set {
				if (size != value) {
					validator.Invalidate();
					size = value;
				}
			}
		}
		public int Width {
			get { return size.x; }
			set {
				if (size.x != value) {
					validator.Invalidate();
					size.x = value;
				}
			}
		}
		public int Height {
			get { return size.y; }
			set {
				if (size.y != value) {
					validator.Invalidate();
					size.y = value;
				}
			}
		}
		public int Lod {
			get { return lod; }
			set {
				if (lod != value) {
					validator.Invalidate();
					lod = value;
				}
			}
		}
		#endregion

		public void Init() {
			OnCreate = null;
			seed = SEED_SIZE * new Vector3(
				RandomInPlusMinusOne(),
				RandomInPlusMinusOne(),
				RandomInPlusMinusOne());
		}

		private static float RandomInPlusMinusOne() {
			return Random.Range(-1f, 1f);
		}

		public void Update() {
			if (validator.Validate()) {
				var dt = Time.deltaTime * noiseTimeSpeed * stopMotion;
				var aspect = (float)size.x / size.y;

				time += dt;

				DuneMaterial.SetFloat(PROP_NOISE_FREQ, noiseFrequency);
				DuneMaterial.SetFloat(PROP_NOISE_HEIGHT, noiseHeight);
				DuneMaterial.SetVector(PROP_POS_SCALE, new Vector4(noiseFieldSize * aspect, noiseFieldSize, 0f, 0f));
				DuneMaterial.SetVector(PROP_POS_OFFSET, new Vector4(seed.x, seed.y, time + seed.z, 0f));
				Graphics.Blit(null, tex.Texture, DuneMaterial);
			}
		}

		protected void NotifyOnCreate() {
			if (OnCreate != null)
				OnCreate(this);
		}

		protected Material DuneMaterial {
			get {
				if (dune == null)
					dune = Resources.Load<Material>(MATERIAL_DUNE);
				return dune;
			}
		}

		protected void ReleaseTexture() {
			if (tex != null) {
				tex.Dispose();
				tex = null;
			}
		}
	}
}
