using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// ���Ȃ萶���p�p�����[�^�B
	/// </summary>
	public class TremoloParameter
	{
		public double rate;  // ���Ȃ�̎��g��(���K���p���g��)�B
		public double depth; // ���Ȃ�̐[��(0�`1)�B
		public int    delay; // ���Ȃ肪������n�߂鎞��(�X�e�b�v��)�B

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="rate">���Ȃ�̎��g��(���K���p���g��)</param>
		/// <param name="depth">���Ȃ�̐[��(0�`1)</param>
		/// <param name="delay">���Ȃ肪������n�߂鎞��(�X�e�b�v��)</param>
		public TremoloParameter(double rate, double depth, int delay)
		{
			this.rate = rate;
			this.depth = depth;
			this.delay = delay;
		}
	}

	/// <summary>
	/// ���ƂȂ� Sound �ɂ��Ȃ���|���� Sound �𐶐�����B
	/// </summary>
	public class Tremolo : Sound
	{
		TremoloParameter parameter;
		Sound sound;

		/// <summary>
		/// ���Ȃ�̃p�����[�^�ƌ��ƂȂ� Sound ���w�肵�Đ����B
		/// </summary>
		/// <param name="parameter">���Ȃ�̃p�����[�^</param>
		/// <param name="sound">���ƂȂ鉹</param>
		public Tremolo(TremoloParameter parameter, Sound sound)
		{
			if(sound.Length < parameter.delay)
				throw new ArgumentException("�����Z����");

			this.parameter = parameter;
			this.sound = sound;
		}

		/// <summary>
		/// ���Ȃ�p�����[�^�ƌ��ƂȂ� Sound ���w�肵�Đ����B
		/// </summary>
		/// <param name="rate">���Ȃ�̎��g��(���K���p���g��)</param>
		/// <param name="depth">���Ȃ�̐[��(0�`1)</param>
		/// <param name="delay">���Ȃ肪������n�߂鎞��(�X�e�b�v��)</param>
		/// <param name="sound">���ƂȂ鉹</param>
		public Tremolo(double rate, double depth, int delay, Sound sound)
			: this(new TremoloParameter(rate, depth, delay), sound)
		{
		}


		public override int Length
		{
			get
			{
				return this.sound.Length;
			}
		}

		public override double[] ToArray()
		{
			double[] x = this.sound.ToArray();

			int delay = this.parameter.delay;
			double rate = this.parameter.rate;
			double depth = this.parameter.depth;

			for(int i=delay+1; i<x.Length; ++i)
			{
				x[i] *= 1 + depth * Math.Sin(rate * (i - delay));
			}

			return x;
		}

	}
}
