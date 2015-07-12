using System;

namespace SoundLibrary.Filter.Misc
{
	/// <summary>
	/// ������
	/// </summary>
	public class Amplifier : IFilter
	{
		double gain;

		public Amplifier(double gain)
		{
			this.gain = gain;
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			return this.gain * x;
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
		}

		public object Clone()
		{
			return new Amplifier(this.gain);
		}

		/// <summary>
		/// ������
		/// </summary>
		public double Gain
		{
			get{return this.gain;}
			set{this.gain = value;}
		}
	}//class Amplifier
}
