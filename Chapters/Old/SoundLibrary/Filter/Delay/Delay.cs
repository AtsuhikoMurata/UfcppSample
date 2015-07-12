using System;

namespace SoundLibrary.Filter.Delay
{
	/// <summary>
	/// �x����
	/// </summary>
	public class Delay : IDelay
	{
		CircularBuffer buf;

		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="taps">�x���^�b�v��</param>
		public Delay(int taps)
		{
			if(taps <= 0)
				this.buf = null;
			else
				this.buf = new CircularBuffer(taps);

			this.Clear();
		}

		/// <summary>
		/// �x���^�b�v��
		/// </summary>
		public int Taps
		{
			get
			{
				if(this.buf == null) return 0;
				return this.buf.Length;
			}
			set
			{
				if(this.buf == null)
					this.buf = new CircularBuffer(value);
				else
					this.buf.Resize(value);
			}
		}

		#region IFilter �����o

		/// <summary>
		/// �t�B���^�����O���s���B
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			if(this.buf == null)
				return x;

			double tmp = this.buf.Top;
			this.buf.PushBack(x);
			return tmp;
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
			if(this.buf == null)
				return;

			for(int i=0; i<this.buf.Length; ++i)
			{
				this.buf[i] = 0;
			}
		}

		#endregion
		#region IClonable �����o

		public object Clone()
		{
			return new Delay(this.buf.Length);
		}

		#endregion
		#region IDelay �����o

		public double DelayTime
		{
			get
			{
				return this.Taps;
			}
			set
			{
				this.Taps = (int)value;
			}
		}

		public double GetValue()
		{
			if(this.buf == null)
				return 0;

			return this.buf.Top;
		}

		public void Push(double x)
		{
			this.buf.PushBack(x);
		}

		public double GetBufferValue(int n)
		{
			return this.buf[n-1];
		}

		#endregion
	}//class Delay
}
