using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// �Ɍ`���ŐU���ƈʑ���ێ�����\���́B
	/// </summary>
	public struct PolarParameter
	{
		public double amplitude;
		public double phase;

		public PolarParameter(double amp, double phase)
		{
			this.amplitude = amp;
			this.phase     = phase;
		}
	}

	/// <summary>
	/// �����g�P���B
	/// </summary>
	public class PureTone : Sound
	{
		int length;       // ���̒���
		double frequency; // ���g��(���K���p���g��)
		double amplitude; // �U��(���j�A�l)
		double phase;     // �����ʑ�(rad)

		/// <summary>
		/// �U���A���g�����w�肵�ď������B
		/// </summary>
		/// <param name="length">���̒���(�T���v����)</param>
		/// <param name="freq">���g��(���K���p���g��)</param>
		/// <param name="amp">�U��(���j�A�l)</param>
		public PureTone(int length, double freq, double amp)
		{
			this.length = length;
			this.frequency = freq;
			this.amplitude = amp;
			this.phase = 0;
		}

		/// <summary>
		/// �U���A���g���A�����ʑ����w�肵�ď������B
		/// </summary>
		/// <param name="length">���̒���(�T���v����)</param>
		/// <param name="freq">���g��(���K���p���g��)</param>
		/// <param name="amp">�U��(���j�A�l)</param>
		/// <param name="phase">�����ʑ�(rad)</param>
		public PureTone(int length, double freq, double amp, double phase)
		{
			this.length = length;
			this.frequency = freq;
			this.amplitude = amp;
			this.phase = phase;
		}

		/// <summary>
		/// �U���A���g���A�����ʑ����w�肵�ď������B
		/// </summary>
		/// <param name="length">���̒���(�T���v����)</param>
		/// <param name="freq">���g��(���K���p���g��)</param>
		/// <param name="parameter">�U���Ə����ʑ�</param>
		public PureTone(int length, double freq, PolarParameter parameter)
			: this(length, freq, parameter.amplitude, parameter.phase)
		{
		}

		public override int Length
		{
			get{return this.length;}
		}

		public double this[int i]
		{
			get
			{
				return this.amplitude * Math.Sin(this.frequency * i + this.phase);
			}
		}

		public override double[] ToArray()
		{
			double[] array = new double[this.Length];
			for(int i=0; i<this.Length; ++i)
				array[i] = this[i];
			return array;
		}

	}//class PureTone
}
