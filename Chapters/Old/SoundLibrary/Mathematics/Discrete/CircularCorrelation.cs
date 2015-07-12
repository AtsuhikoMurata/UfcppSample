using System;
using System.Collections;

namespace SoundLibrary.Mathematics.Discrete
{
	using Type = System.Double;

	/// <summary>
	/// �z�� x �Ɣz�� y �̏z���ݑ��֒l�����߂�B
	/// </summary>
	public class CircularCorrelation : Function
	{
		Type[] x;
		Type[] y;

		public CircularCorrelation(Type[] x, Type[] y)
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
				return this.x.Length;
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

				int i=0;
				for(int j=n; j<len; ++i, ++j)
				{
					val += x[i] * y[j];
				}
				for(int j=0; i<len; ++i, ++j)
				{
					val += x[i] * y[j];
				}

				return val;
			}
		}
	}
}
