using System;

namespace SoundLibrary.Data
{
	/// <summary>
	/// �z���C�g�m�C�Y�����B
	/// (Box-Muller �@��p���Đ��K�����𐶐��B)
	/// </summary>
	public class WhiteNoiseGenerator : IDataGenerator
	{
		double mean;  // ����
		double sigma; // �W���΍�
		int seed;
		Random rand;

		/// <summary>
		/// ����0�A�W���΍�1�̃z���C�g�m�C�Y�𐶐��B
		/// </summary>
		public WhiteNoiseGenerator() : this(0, 1, 0){}

		/// <summary>
		/// ���ϒl�ƕW���΍����w�肵�ăz���C�g�m�C�Y�𐶐��B
		/// </summary>
		/// <param name="mean">���ϒl</param>
		/// <param name="sigma">�W���΍�</param>
		public WhiteNoiseGenerator(double mean, double sigma, int seed)
		{
			this.mean = mean;
			this.sigma = sigma;
			this.seed = seed;
			this.rand = new Random(seed);
		}

		public double Next()
		{
			double amp = Math.Sqrt(-2 * Math.Log(this.rand.NextDouble()));
			double phase = 2 * Math.PI * this.rand.NextDouble();
			return this.sigma * amp * Math.Sin(phase) + this.mean;
		}

		public void Reset()
		{
			this.rand = new Random(seed);
		}

		public object Clone()
		{
			return new WhiteNoiseGenerator(this.mean, this.sigma, this.seed);
		}
	}
}// namespace
