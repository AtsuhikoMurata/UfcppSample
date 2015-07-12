using System;

namespace SoundLibrary.Mathematics
{
	/// <summary>
	/// ���v�����֐��Q���`�B
	/// </summary>
	public class Statistics
	{
		/// <summary>
		/// �z�� x �̘a�B
		/// </summary>
		public static double Sum(double[] x)
		{
			double sum = 0;
			for(int i=0; i<x.Length; ++i)
				sum += x[i];
			return sum;
		}

		/// <summary>
		/// �z�� x �̎���a�B
		/// </summary>
		public static double SquareSum(double[] x)
		{
			double sum = 0;
			for(int i=0; i<x.Length; ++i)
				sum += x[i] * x[i];
			return sum;
		}

		/// <summary>
		/// �z�� x �� y �̐Ϙa�B
		/// </summary>
		public static double Mac(double[] x, double[] y)
		{
			double mac = 0;
			for(int i=0; i<x.Length; ++i)
				mac += x[i] * y[i];
			return mac;
		}

		/// <summary>
		/// �z�� x �̕��ϒl�B
		/// </summary>
		public static double Average(double[] x)
		{
			return Sum(x) / x.Length;
		}

		/// <summary>
		/// �z�� x �̕��U�B
		/// </summary>
		static double Variance(double[] x)
		{
			double var = Average(x);
			var = var * var;
			var = SquareSum(x) / x.Length - var;
			return var;
		}

		/// <summary>
		/// �z�� x �� y �̋����U�B
		/// </summary>
		public static double Covariance(double[] x, double[] y)
		{
			double var = Mac(x, y) / x.Length - Average(x) * Average(y);
			return var;
		}
	}
}
