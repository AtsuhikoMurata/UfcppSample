using System;
using SoundLibrary.Filter.Delay;

namespace SoundLibrary.Music
{
	/// <summary>
	/// �r�u���[�g�����p�p�����[�^�B
	/// </summary>
	public class VibratoParameter
	{
		public double rate;  // �r�u���[�g���[�g(���K���p���g��)�B
		public int    depth; // �r�u���[�g�f�v�X(�X�e�b�v��)�B
		public int    delay; // �r�u���[�g�f�B���C(�X�e�b�v��)�B

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="rate">�r�u���[�g���[�g(���K���p���g��)</param>
		/// <param name="depth">�r�u���[�g�f�v�X(�X�e�b�v��)</param>
		/// <param name="delay">�r�u���[�g�f�B���C(�X�e�b�v��)</param>
		public VibratoParameter(double rate, int depth, int delay)
		{
			this.rate = rate;
			this.depth = depth;
			this.delay = delay;
		}
	}

	/// <summary>
	/// ���ƂȂ� Sound �Ƀr�u���[�g�Ȑ����|���� Sound �𐶐�����B
	/// </summary>
	public class Vibrato : Sound
	{
		VibratoParameter parameter;
		Sound sound;
		FractionalDelay delay;

		/// <summary>
		/// �r�u���[�g�p�����[�^�ƌ��ƂȂ� Sound ���w�肵�Đ����B
		/// </summary>
		/// <param name="parameter">�r�u���[�g�p�����[�^</param>
		/// <param name="sound">���ƂȂ鉹</param>
		public Vibrato(VibratoParameter parameter, Sound sound)
		{
			if(sound.Length < parameter.delay)
				throw new ArgumentException("�����Z����");

			this.parameter = parameter;
			this.sound = sound;

			this.delay = new FractionalDelay(2 * parameter.depth);
			this.delay.DelayTime = parameter.depth;
		}

		/// <summary>
		/// �r�u���[�g�p�����[�^�ƌ��ƂȂ� Sound ���w�肵�Đ����B
		/// </summary>
		/// <param name="rate">�r�u���[�g���[�g(���K���p���g��)</param>
		/// <param name="depth">�r�u���[�g�f�v�X(0�`1)</param>
		/// <param name="delay">�r�u���[�g�f�B���C(�X�e�b�v��)</param>
		/// <param name="sound">���ƂȂ鉹</param>
		public Vibrato(double rate, int depth, int delay, Sound sound)
			: this(new VibratoParameter(rate, depth, delay), sound)
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
			int depth = this.parameter.depth;

			int i=0, j=delay;
			for(; i<delay; ++i) this.delay.Push(x[i]);
			for(; i<delay + depth; ++i) this.delay.Push(x[i]);
			for(; i<x.Length; ++j, ++i)
			{
				this.delay.DelayTime = depth * (1 + Math.Sin(rate * (i - delay)));
				x[j] = this.delay.GetValue(x[i]);
			}
			for(; j<x.Length; ++j) x[j] = this.delay.GetValue(0);

			return x;
		}

	}
}
