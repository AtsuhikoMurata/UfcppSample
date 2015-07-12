using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// �����g�{����B
	/// </summary>
	public class HarmonicTone : Sound
	{
		int length;       // ���̒���
		double frequency; // ��ꉹ�̎��g��(�΂Ő��K��)
		PolarParameter[] parameters;

		/// <summary>
		/// ���̒����A��ꉹ�̎��g���A�{����̐U���E�ʑ����琶���B
		/// </summary>
		/// <param name="length">���̒���</param>
		/// <param name="freq">��ꉹ�̎��g��(���K���p���g��)</param>
		/// <param name="parameters">�{����̐U��(���j�A�l)�ƈʑ�(rad)</param>
		public HarmonicTone(int length, double freq, params PolarParameter[] parameters)
		{
			this.length = length;
			this.frequency = freq;
			this.parameters = parameters;
		}

		/// <summary>
		/// ���̒����A��ꉹ�̎��g���A�{����̐U�����琶���B
		/// </summary>
		/// <param name="length">���̒���</param>
		/// <param name="freq">��ꉹ�̎��g��(���K���p���g��)</param>
		/// <param name="amps">�{����̐U��(���j�A�l)</param>
		public HarmonicTone(int length, double freq, params double[] amps)
		{
			this.length = length;
			this.frequency = freq;
			this.parameters = new PolarParameter[amps.Length];
			for(int i=0; i<amps.Length; ++i)
			{
				this.parameters[i] = new PolarParameter(amps[i], 0);
			}
		}

		/// <summary>
		/// ���̒����A��ꉹ�̎��g���A�{����̐U���E�ʑ����琶���B
		/// </summary>
		/// <param name="length">���̒���</param>
		/// <param name="freq">��ꉹ�̎��g��(���K���p���g��)</param>
		/// <param name="amps">�{����̐U��(���j�A�l)</param>
		/// <param name="phase">�{����̈ʑ�</param>
		public HarmonicTone(int length, double freq, double[] amps, double[] phase)
		{
			this.length = length;
			this.frequency = freq;
			this.parameters = new PolarParameter[amps.Length];
			for(int i=0; i<amps.Length; ++i)
			{
				this.parameters[i] = new PolarParameter(amps[i], phase[i]);
			}
		}

		public override int Length
		{
			get{return this.length;}
		}

		public double this[int i]
		{
			get
			{
				double x = 0;
				for(int k=0; k<this.parameters.Length; ++k)
				{
					PolarParameter parameter = this.parameters[k];
					x += parameter.amplitude * Math.Sin(this.frequency * k * i + parameter.phase);
				}

				return x;
			}
		}

		public override double[] ToArray()
		{
			double[] array = new double[this.Length];
			for(int i=0; i<this.Length; ++i)
				array[i] = this[i];
			return array;
		}
	}
}
