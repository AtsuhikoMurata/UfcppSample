using System;

namespace SoundLibrary.Filter.Delay
{
	/// <summary>
	/// �����x���t�B���^�B
	/// </summary>
	public class FractionalDelay : IDelay
	{
		CircularBuffer buf;
		int integer;
		double fraction;
		double[] coef; // �����x���������邽�߂� FIR �W��

		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="time">�x���^�b�v��</param>
		public FractionalDelay(double time) : this(time, 4){}

		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="time">�x���^�b�v��</param>
		/// <param name="firLength">�����x��FIR�̎���</param>
		public FractionalDelay(double time, int firLength)
		{
			if(time < 0)
			{
				this.integer = 0;
				this.fraction = 0;
				this.buf = null;
				this.coef = null;
			}
			else
			{
				this.coef = new double[firLength];
				GetFractionalDelayCoef(this.Delay, this.Length, this.coef);
				this.DelayTime = time;
			}

			this.Clear();
		}


		/// <summary>
		/// �t�B���^�����O���s���B
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			if(this.buf == null)
				return x;

			int n = this.integer - this.Delay;
			int i = 0;
			if(n < 0)
			{
				i = -n;
				n = 0;
			}

			this.buf.PushFront(x);

			double y = 0;
			for(; i<this.Length; ++i, ++n)
				y += this.buf[n] * this.coef[i];
			return y;
		}

		/// <summary>
		/// �l���z�o�b�t�@�Ƀv�b�V������B
		/// </summary>
		/// <param name="x">�v�b�V������l</param>
		public void Push(double x)
		{
			if(this.buf == null)
				return;

			this.buf.PushFront(x);
		}

		/// <summary>
		/// �l�̎��o���B
		/// </summary>
		/// <returns>�t�B���^����</returns>
		public double GetValue()
		{
			if(this.buf == null)
				return 0;

			int n = this.integer - this.Delay;
			int i = 0;
			if(n < 0)
			{
				i = -n;
				n = 0;
			}

			double y = 0;
			for(; i<this.Length; ++i, ++n)
				y += this.buf[n] * this.coef[i];
			return y;
		}

		public double GetBufferValue(int n)
		{
			if(n > this.Length)
				return this.buf[n-1];
			else
				return 0.0;
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

		/// <summary>
		/// �x���^�b�v��
		/// </summary>
		public double DelayTime
		{
			get
			{
				return this.Integer + this.Fraction;
			}
			set
			{
				this.Integer = (int)value;
				this.Fraction = value - integer;
			}
		}

		/// <summary>
		/// �x���^�b�v���̐�������
		/// </summary>
		public int Integer
		{
			get
			{
				return this.integer;
			}
			set
			{
				this.integer = value;
				if(this.buf == null)
					this.buf = new CircularBuffer(this.BufferSize);
				else if(this.buf.Length < this.BufferSize)
					this.buf.Resize(this.BufferSize);
			}
		}

		/// <summary>
		/// �x���^�b�v���̏�������
		/// </summary>
		public double Fraction
		{
			get
			{
				return this.fraction;
			}
			set
			{
				if(this.coef == null)
				{
					this.fraction = value;
					this.coef = GetFractionalDelayCoef(this.fraction + this.Delay, this.Length);
				}
				if(this.fraction != value)
				{
					this.fraction = value;
					GetFractionalDelayCoef(this.fraction + this.Delay, this.Length, this.coef);
				}
			}
		}

		/// <summary>
		/// �����x��FIR�̃^�b�v���B
		/// </summary>
		public int Length
		{
			get{return this.coef.Length;}
			set
			{
				if(this.coef.Length < value)
				{
					this.coef = GetFractionalDelayCoef(this.fraction + this.Delay, this.Length);
				}

				if(this.buf.Length < this.BufferSize)
					this.buf.Resize(this.BufferSize);
			}
		}

		int Delay{get{return this.Length / 2 - 1;}}
		int BufferSize{get{return this.integer + this.Length - this.Delay;}}

		public object Clone()
		{
			return new FractionalDelay(this.DelayTime);
		}

		#region static ���\�b�h

		/// <summary>
		/// �f�B���C�l���番���x�� FIR �t�B���^�W�����v�Z����B
		/// </summary>
		/// <param name="delay">�f�B���C�l</param>
		/// <param name="length">FIR �t�B���^�̃^�b�v��</param>
		/// <returns>FIR �t�B���^�W��</returns>
		static double[] GetFractionalDelayCoef(double delay, int length)
		{
			double[] coef = new double[length];
			GetFractionalDelayCoef(delay, length, coef);
			return coef;
		}

		static void GetFractionalDelayCoef(double delay, int length, double[] coef)
		{
			//*
			SpectrumAnalysis.Spectrum tmp = SpectrumAnalysis.Spectrum.FromDelay(delay, length);
			tmp.GetTimeSequence(coef);
			//*/

			/*
			for(int i=0; i<length; ++i)
			{
				coef[i] = 1;
				for(int j=0; j<length; ++j)
				{
					if(i == j) continue;

					coef[i] *= (delay - j) / (i - j);
				}
			}
			//*/
		}

		#endregion
	}
}
