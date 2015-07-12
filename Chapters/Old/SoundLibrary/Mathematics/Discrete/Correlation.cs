using System;
using System.Collections;

namespace SoundLibrary.Mathematics.Discrete
{
	using Type = System.Double;

	/// <summary>
	/// �z�� x �Ɣz�� y �̑��ݑ��֒l�����߂�B
	/// </summary>
	public class Correlation : Function
	{
		Type[] x;
		Type[] y;

		public Correlation(Type[] x, Type[] y)
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
				return -(this.x.Length - 1);
			}
		}

		public override int End
		{
			get
			{
				return this.x.Length;
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
		/// ���ݑ��֒l C[n] = ��_i x[i]y[i-n] �����߂�B
		/// </summary>
		/// <param name="n">C[n] �� n</param>
		/// <returns>���ݑ��֒l</returns>
		public override Type this[int n]
		{
			get
			{
				int len = this.x.Length;
				Type val = 0;
				if(n < 0)
				{
					for(int i=0, j=-n; j<len; ++i, ++j)
					{
						val += x[i] * y[j];
					}
				}
				else
				{
					for(int i=n, j=0; i<len; ++i, ++j)
					{
						val += x[i] * y[j];
					}
				}

				return val;
			}
		}
	}
}
