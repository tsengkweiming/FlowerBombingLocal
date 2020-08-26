using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunting.FluidableColorTexture {

	public enum ColorSwizzle {
		PassThrough = (1 << 12) | (2 << 8) | (4 << 4) | 8,

		RedOnly = (1 << 12) | 8,
		GreenOnly = (2 << 8) | 8,
		BlueOnly = (4 << 4) | 8,

		RedAsWhite = (1 << 12) | (1 << 8) | (1 << 4) | 8,
		GreenAsWhite = (2 << 12) | (2 << 8) | (2 << 4) | 8,
		BlueAsWhite = (4 << 12) | (4 << 8) | (4 << 4) | 8,
		AlphaAsWhite = (8 << 12) | (8 << 8) | (8 << 4) | 8,
	}

	public static class ColorSwizzleExtension {

		public static Vector4 PackedVector(this ColorSwizzle swizzle) {
			var x = ((int)swizzle >> 12) & 15;
			var y = ((int)swizzle >> 8) & 15;
			var z = ((int)swizzle >> 4) & 15;
			var w = ((int)swizzle) & 15;
			return new Vector4(x, y, z, w);
		}
	}
}
