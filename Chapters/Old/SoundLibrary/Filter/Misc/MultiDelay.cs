using System;

namespace SoundLibrary.Filter.Misc
{
	/// <summary>
	/// �}���`�f�B���C�B
	/// </summary>
	public class MultiDelay : IFilter
	{
		public struct Tuple
		{
			public double gain;
			public int delay;

			public Tuple(double gain, int delay)
			{
				this.gain = gain;
				this.delay = delay;
			}
		}
		Tuple[] filters;
		CircularBuffer buf;

		public MultiDelay(params Tuple[] filters)
		{
			this.filters = filters;

			int maxDelay = int.MinValue;
			foreach(Tuple t in filters)
				if(t.delay > maxDelay) maxDelay = t.delay;
			++maxDelay;

			if(maxDelay > 0)
				this.buf = new CircularBuffer(maxDelay);

			this.Clear();
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			this.buf.PushBack(0);

			foreach(Tuple t in this.filters)
				this.buf[t.delay] += x * t.gain;
			return this.buf[0];
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
			for(int i=0; i<this.buf.Length; ++i)
				this.buf[i] = 0;
		}

		public object Clone()
		{
			return new MultiDelay((Tuple[])this.filters.Clone());
		}
	}
}
