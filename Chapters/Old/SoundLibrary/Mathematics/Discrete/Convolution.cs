using System;
using System.Collections;

namespace SoundLibrary.Mathematics.Discrete
{
	using Type = System.Double;

	/// <summary>
	/// �z�� x �Ɣz�� y �̏􍞂݂����߂�B
	/// </summary>
	public class Convolution : Function
	{
		Type[] x;
		Type[] y;

		public Convolution(Type[] x, Type[] y)
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
				return 2 * this.x.Length - 1;
			}
		}

		/// <summary>
		/// �􍞂� C[n] = ��_i x[i]y[n-i] �����߂�B
		/// </summary>
		/// <param name="n">C[n] �� n</param>
		/// <returns>�􍞂݌���</returns>
		public override Type this[int n]
		{
			get
			{
				int len = this.x.Length;
				Type val = 0;
				if(n < len)
				{
					for(int i=0, j=n; i<=n; ++i, --j)
					{
						val += x[i] * y[j];
					}
				}
				else
				{
					for(int i=n-(len-1), j=len-1; i<len; ++i, --j)
					{
						val += x[i] * y[j];
					}
				}

				return val;
			}
		}
	}
}
