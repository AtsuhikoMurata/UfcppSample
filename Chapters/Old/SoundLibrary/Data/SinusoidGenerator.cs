using System;

namespace SoundLibrary.Data
{
	/// <summary>
	/// �����g����
	/// </summary>
	public class SinusoidGenerator : IDataGenerator
	{
		int i = 0;
		double w;
		double gain;
		double phase;

		public SinusoidGenerator() : this(0) {}
		public SinusoidGenerator(double phase) : this((ushort)short.MaxValue, phase) {}
		public SinusoidGenerator(double gain, double phase) : this(Math.PI / 24000 * 1000, gain, phase) {}

		/// <summary>
		/// ���g���A�U���A�����ʑ����w��B
		/// </summary>
		/// <param name="w">���g��(2�΂Ő��K��)</param>
		/// <param name="gain">�U��(���j�A�l)</param>
		/// <param name="phase">�����ʑ�(rad)</param>
		public SinusoidGenerator(double w, double gain, double phase)
		{
			this.w = w;
			this.gain = gain;
			this.phase = phase;
		}

		public double Next()
		{
			double x = this.gain * Math.Sin(this.w * this.i + phase);
			++this.i;
			return x;
		}

		public void Reset()
		{
			this.i = 0;
		}

		public object Clone()
		{
			return new SinusoidGenerator(this.w, this.gain, this.phase);
		}
	}
}//namespace Data
