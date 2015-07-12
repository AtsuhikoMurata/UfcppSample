using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// �����������B
	/// </summary>
	public class PeriodicSound : Sound
	{
		int iteration;
		Sound sound;

		/// <summary>
		/// sound �� iteration ��J��Ԃ��������B
		/// </summary>
		/// <param name="iteration">������</param>
		/// <param name="sound">����</param>
		public PeriodicSound(int iteration, Sound sound)
		{
			this.iteration = iteration;
			this.sound = sound;
		}

		public override int Length
		{
			get
			{
				return this.sound.Length * this.iteration;
			}
		}

		public override double[] ToArray()
		{
			double[] w = this.sound.ToArray();
			double[] x = new double[this.Length];

			for(int i=0, j=0; i<iteration; ++i)
				for(int k=0; k<w.Length; ++k, ++j)
					x[j] = w[k];
			return x;
		}
	}
}
