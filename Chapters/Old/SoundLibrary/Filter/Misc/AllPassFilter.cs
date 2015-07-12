using System;

namespace SoundLibrary.Filter.Misc
{
	using IDelay = SoundLibrary.Filter.Delay.IDelay;
	using Delay = SoundLibrary.Filter.Delay.Delay;
	using FractionalDelay = SoundLibrary.Filter.Delay.FractionalDelay;

	/// <summary>
		/// �I�[���p�X�t�B���^�B
		/// </summary>
	public class AllPassFilter : IFilter
	{
		double feedback;
		IDelay delay;

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="direct">�_�C���N�g�Q�C��</param>
		/// <param name="effect">�G�t�F�N�g�Q�C��</param>
		/// <param name="feedback">�t�B�[�h�o�b�N�Q�C��</param>
		/// <param name="delay">�f�B���C�^�C��</param>
		public AllPassFilter(double feedback, double delay)
		{
			--delay;
			if(delay < 0) delay = 0;

			this.feedback = feedback;
			this.delay = new FractionalDelay(delay);
			this.Clear();
		}

		public AllPassFilter(double feedback, double delay, int firLength)
		{
			--delay;
			if(delay < 0) delay = 0;

			this.feedback = feedback;
			this.delay = new FractionalDelay(delay, firLength);
			this.Clear();
		}

		/// <summary>
		/// �p�����[�^��ݒ肷��B
		/// </summary>
		/// <param name="feedback">�t�B�[�h�o�b�N�Q�C��</param>
		/// <param name="delay">�x������</param>
		public void SetParameter(double feedback, double delay)
		{
			this.FeedbackGain = feedback;
			this.DelayTime    = delay;
		}

		/// <summary>
		/// �t�B�[�h�o�b�N�Q�C��
		/// </summary>
		public double FeedbackGain
		{
			get{return this.feedback;}
			set{this.feedback = value;}
		}


		/// <summary>
		/// �f�B���C�^�C��
		/// </summary>
		public double DelayTime
		{
			get{return this.delay.DelayTime + 1;}
			set
			{
				--value;
				if(value < 0) value = 0;
				this.delay.DelayTime = value;
			}
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			double t = this.delay.GetValue();
			double y = x + t * this.feedback;
			this.delay.Push(y);
				
			return y * -this.feedback + t;
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
			this.delay.Clear();
		}

		public object Clone()
		{
			return new AllPassFilter(this.feedback, this.DelayTime);
		}

		public double GetBufferValue(int n)
		{
			return this.delay.GetBufferValue(n);
		}
	}//class AllPassFilter
}
