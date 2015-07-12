using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// Amplifier �̊T�v�̐����ł��B
	/// </summary>
	public class Amplifier : Sound
	{
		double level;
		Sound sound;

		public Amplifier(double level, Sound sound)
		{
			this.level = level;
			this.sound = sound;
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
			for(int i=0; i<x.Length; ++i) x[i] = this.level * x[i];
			return x;
		}
	}
}
