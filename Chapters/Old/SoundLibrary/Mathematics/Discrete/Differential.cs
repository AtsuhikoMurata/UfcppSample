using System;

namespace SoundLibrary.Mathematics.Discrete
{
	using Type = System.Double;

	/// <summary>
	/// ���֐��B
	/// </summary>
	public class Differential : Function
	{
		Function primitive;

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="primitive">���n�֐�</param>
		public Differential(Function primitive)
		{
			this.primitive = primitive;
		}

		public override int Begin
		{
			get
			{
				return this.primitive.Begin;
			}
		}

		public override int End
		{
			get
			{
				return this.primitive.End;
			}
		}

		public override int Length
		{
			get
			{
				return this.primitive.Length;
			}
		}

		public override Type this[int n]
		{
			get
			{
				if(n == this.primitive.Begin    ) return ForwardDifference(this.primitive, n);
				if(n == this.primitive.Begin + 1) return Diffrential3(this.primitive, n);
				if(n == this.primitive.End - 2  ) return Diffrential3(this.primitive, n);
				if(n == this.primitive.End - 1  ) return BackwardDifference(this.primitive, n);
				return Diffrential5(this.primitive, n);
			}
		}

		/// <summary>
		/// �O�i�����ߎ��Ŕ����B
		/// </summary>
		/// <param name="f">�����Ώ�</param>
		/// <param name="i">�ʒu</param>
		/// <returns>��������</returns>
		public static Type ForwardDifference(Function f, int i)
		{
			return f[i+1] - f[i];
		}

		/// <summary>
		/// ��ލ����ߎ��Ŕ����B
		/// </summary>
		/// <param name="f">�����Ώ�</param>
		/// <param name="i">�ʒu</param>
		/// <returns>��������</returns>
		public static Type BackwardDifference(Function f, int i)
		{
			return f[i] - f[i-1];
		}

		/// <summary>
		/// 3�_�ߎ��Ŕ����B
		/// </summary>
		/// <param name="f">�����Ώ�</param>
		/// <param name="i">�ʒu</param>
		/// <returns>��������</returns>
		public static Type Diffrential3(Function f, int i)
		{
			return (f[i+1] - f[i-1]) / 2;
		}

		/// <summary>
		/// 5�_�ߎ��Ŕ����B
		/// </summary>
		/// <param name="f">�����Ώ�</param>
		/// <param name="i">�ʒu</param>
		/// <returns>��������</returns>
		public static Type Diffrential5(Function f, int i)
		{
			return (-f[i+2] + 8 * f[i+1] - 8 * f[i-1] + f[i-2]) / 12;
		}

		/// <summary>
		/// �z����֐��Ɍ����ĂĔ����B
		/// </summary>
		/// <param name="x">�����Ώ�</param>
		/// <returns>��������</returns>
		public static Type[] Derive(Type[] x)
		{
			Type[] y = new Type[x.Length];
			Differential dx = new Differential(Function.FromArray(x));
			for(int i=0; i<y.Length; ++i)
			{
				y[i] = dx[i];
			}
			return y;
		}
	}
}
