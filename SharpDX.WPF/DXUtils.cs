using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.WPF;
using System.Runtime.InteropServices;
using System.IO;
using System;

namespace SharpDX.WPF
{
	public static class DXUtils
	{
		#region GetOrThrow<T>()

		public static T GetOrThrow<T>(this T obj)
			where T : class, IDisposable
		{
			if (obj == null)
				throw new ObjectDisposedException(typeof(T).Name);
			return obj;
		} 

		#endregion

		#region Matrix: TransformNormal(), TransformCoord(), Multiply()

		public static Vector3 TransformNormal(this Matrix m, Vector3 v)
		{
			var v2 = Multiply(m, v.X, v.Y, v.Z, 0);
			return new Vector3(v2.X, v2.Y, v2.Z);
		}

		public static Vector3 TransformCoord(this Matrix m, Vector3 v)
		{
			var v2 = Multiply(m, v.X, v.Y, v.Z, 1);
			return new Vector3(v2.X, v2.Y, v2.Z);
		}

		public static Vector3 Multiply(this Matrix m, float x, float y, float z, float w)
		{
			return new Vector3(
				m.M11 * x + m.M12 * y + m.M13 * z + m.M14 * w
				, m.M21 * x + m.M22 * y + m.M23 * z + m.M24 * w
				, m.M31 * x + m.M32 * y + m.M33 * z + m.M34 * w
				);
		}

		#endregion

		#region DEG2RAD()

		public static float DEG2RAD(this float degrees)
		{
			return degrees * (float)Math.PI / 180.0f;
		}

		#endregion
	}
}
