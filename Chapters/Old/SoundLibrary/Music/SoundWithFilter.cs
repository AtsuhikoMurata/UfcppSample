using System;

using SoundLibrary.Filter;

namespace SoundLibrary.Music
{
	/// <summary>
	/// �t�B���^���|�������𐶐�����B
	/// </summary>
	public class SoundWithFilter : Sound
	{
		Sound sound;
		IFilter filter;
		int delay; // filter �̒x������

		/// <summary>
		/// ���ƂȂ� Sound�A�t�B���^�A�t�B���^�̒x�����Ԃ��w�肵�Đ����B
		/// </summary>
		/// <param name="sound">���ƂȂ鉹</param>
		/// <param name="filter">�t�B���^</param>
		/// <param name="delay">filter �̒x������</param>
		public SoundWithFilter(Sound sound, IFilter filter, int delay)
		{
			this.sound = sound;
			this.filter = filter;
			this.delay = delay;
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

			int i=0;
			int j=0;
			for(; i<this.delay; ++i)
			{
				filter.GetValue(x[i]);
			}
			for(; i<x.Length; ++i, ++j)
			{
				x[j] = filter.GetValue(x[i]);
			}
			for(; j<x.Length; ++j)
			{
				x[j] = filter.GetValue(0);
			}

			return x;
		}
	}
}
