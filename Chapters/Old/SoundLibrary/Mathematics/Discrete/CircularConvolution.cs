using System;
using System.Collections;

namespace SoundLibrary.Mathematics.Discrete
{
	using Type = System.Double;

	/// <summary>
	/// �z�� x �Ɣz�� y �̏z�􍞂݂����߂�B
	/// </summary>
	public class CircularConvolution : Function
	{
		Type[] x;
		Type[] y;

		public CircularConvolution(Type[] x, Type[] y)
		{
			if(x.Length != y.Length)
				throw new ArgumentException("x �� y �̒����͓������Ȃ���΂Ȃ�܂���B");

			this.x = x;
			this.y = y;
		}

		public override int Begin
		{
			get
			{
				return 0;
			}
		}

		public override int End
		{
			get
			{
				return this.Length;
			}
		}

		public override int Length
		{
			get
			{
				return this.x.Length;
			}
		}

		/// <summary>
		/// �z�􍞂� C[n] = ��_i x[i]y[n-i] �����߂�B
		/// </summary>
		/// <param name="n">C[n] �� n</param>
		/// <returns>�z�􍞂݌���</returns>
		public override Type this[int n]
		{
			get
			{
				int len = this.x.Length;
				Type val = 0;

				int i=0;
				for(int j=n-1; i<n; ++i, --j)
				{
					val += x[i] * y[j];
				}
				for(int j=len-1; i<len; ++i, --j)
				{
					val += x[i] * y[j];
				}

				return val;
			}
		}
	}
}
