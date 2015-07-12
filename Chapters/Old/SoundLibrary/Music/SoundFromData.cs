using System;

using SoundLibrary.Data;

namespace SoundLibrary.Music
{
	/// <summary>
	/// IDataGenerator ���� Sound �����B
	/// </summary>
	public class SoundFromData : Sound
	{
		IDataGenerator generator;
		int length;

		/// <summary>
		/// IDataGenerator �Ɖ��̒������w�肵�ď������B
		/// </summary>
		/// <param name="generator">�f�[�^�����N���X</param>
		/// <param name="length">���̒���</param>
		public SoundFromData(IDataGenerator generator, int length)
		{
			this.generator = generator;
			this.length = length;
		}

		public override int Length
		{
			get
			{
				return this.length;
			}
		}

		public override double[] ToArray()
		{
			double[] x = new double[this.length];

			for(int i=0; i<this.length; ++i)
				x[i] = this.generator.Next();

			return x;
		}
	}
}
