using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// �������B
	/// ������ Sound �������B
	/// </summary>
	public class CompoundSound : Sound
	{
		Sound[] sounds;

		/// <summary>
		/// ������ Sound ���琶���B
		/// </summary>
		/// <param name="sounds">���������\�����鉹�B</param>
		/// <exception cref="ArgumentException">sounds �̒�����������Ă��Ȃ��Ƃ��ɔ������܂��B</exception>
		public CompoundSound(params Sound[] sounds)
		{
			int len = sounds[0].Length;
			foreach(Sound sound in sounds)
			{
				if(len != sound.Length)
					throw new ArgumentException("sounds �̒�����������Ă��܂���B");
			}

			this.sounds = sounds;
		}

		/// <summary>
		/// ���a�������B
		/// </summary>
		/// <param name="length">���̒���</param>
		/// <param name="freq">��ԉ��̉��̎��g��(���K���p���g��)</param>
		/// <param name="amp">�U��(���j�A�l)</param>
		/// <returns></returns>
		public static Sound MajorChord(int length, double freq, double amp)
		{
			return new CompoundSound(
				new PureTone(length, freq, amp / 3),
				new PureTone(length, freq * PureTemperament.MAJOR3, amp / 3),
				new PureTone(length, freq * PureTemperament.MINOR3, amp / 3)
				);
		}

		/// <summary>
		/// �P�a�������B
		/// </summary>
		/// <param name="length">���̒���</param>
		/// <param name="freq">��ԉ��̉��̎��g��(���K���p���g��)</param>
		/// <param name="amp">�U��(���j�A�l)</param>
		/// <returns></returns>
		public static Sound MinorChord(int length, double freq, double amp)
		{
			return new CompoundSound(
				new PureTone(length, freq, amp / 3),
				new PureTone(length, freq * PureTemperament.MINOR3, amp / 3),
				new PureTone(length, freq * PureTemperament.MAJOR3, amp / 3)
				);
		}

		public override int Length
		{
			get
			{
				return this.sounds[0].Length;
			}
		}

		public override double[] ToArray()
		{
			double[] x = new double[this.Length];
			foreach(Sound sound in this.sounds)
			{
				double[] y = sound.ToArray();
				for(int i=0; i<x.Length; ++i)
					x[i] += y[i];
			}
			return x;
		}

	}//class CompoundSound
}
