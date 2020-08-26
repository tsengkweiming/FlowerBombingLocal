using Hunting.FluidableColorTexture;
using nobnak.Gist;
using nobnak.Gist.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GardenSystem {

	public class TextureVisualizer : MonoBehaviour {

		[SerializeField]
		protected Input input;
		[SerializeField]
		protected Events events;
		[SerializeField]
		protected Link link;

		protected Validator validator = new Validator();
		protected Texture packedMask = null;
		protected Texture camoPattern = null;

		#region unity
		protected void Awake() {
			validator.Validation += () => {
				switch (input.VisualizedTarget) {
					default:
						SetTexture(CamoPattern);
						SetSwizzle(ColorSwizzle.PassThrough.PackedVector());
						break;
					case VisualizedTarget.Mask_ALL:
						SetTexture(PackedMask);
						SetSwizzle(ColorSwizzle.PassThrough.PackedVector());
						break;
					case VisualizedTarget.Mask_Sensor:
						SetTexture(PackedMask);
						SetSwizzle(ColorSwizzle.RedAsWhite.PackedVector());
						break;
					case VisualizedTarget.Mask_Camo:
						SetTexture(PackedMask);
						SetSwizzle(ColorSwizzle.GreenAsWhite.PackedVector());
						break;
				}
			};

			input.Changed += () => validator.Invalidate();
		}
		protected void OnValidate() {
			validator.Invalidate();
		}
		protected void Update() {
			validator.Validate();
		}
		#endregion

		#region public
		public Texture PackedMask {
			set {
				if (packedMask != value) {
					packedMask = value;
					validator.Invalidate();
				}
			}
			protected get { return packedMask; }
		}
		public Texture CamoPattern {
			set {
				if (camoPattern != value) {
					camoPattern = value;
					validator.Invalidate();
				}
			}
			protected get { return camoPattern; }
		}
		public Input CurrentInput {
			get { return input; }
			set {
				validator.Invalidate();
				input = value;
			}
		}
		#endregion

		#region private
		protected void SetTexture(Texture tex) {
			link.material.Set(tex);
		}
		protected void SetSwizzle(Vector4 swizle) {
			link.material.Set(swizle);
		}
		#endregion

		#region classes
		[System.Serializable]
		public class Input {
			public event System.Action Changed;
			[SerializeField]
			protected VisualizedTarget visualized;

			public VisualizedTarget VisualizedTarget {
				get { return visualized; }
				set {
					if (visualized != value) {
						visualized = value;
						Notify();
					}
				}
			}
			private void Notify() {
				if (Changed != null)
					Changed();
			}
		}
		[System.Serializable]
		public class Events {
			public TextureEvents OnChange;
		}
		[System.Serializable]
		public class Link {
			public RendererMaterialListener material;
		}
		public enum VisualizedTarget {
			Camo = 0,
			Mask_ALL,
			Mask_Sensor,
			Mask_Camo
		}
		#endregion
	}
}
