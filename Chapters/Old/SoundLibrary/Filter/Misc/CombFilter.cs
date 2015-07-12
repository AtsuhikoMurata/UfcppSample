using System;

namespace SoundLibrary.Filter.Misc
{
	using IDelay = SoundLibrary.Filter.Delay.IDelay;
	using Delay = SoundLibrary.Filter.Delay.Delay;
	using FractionalDelay = SoundLibrary.Filter.Delay.FractionalDelay;

	/// <summary>
	/// �R���t�B���^�B
	/// </summary>
	public class CombFilter : IFilter
	{
		double direct;
		double effect;
		double feedback;
		IDelay delay;

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="direct">�_�C���N�g�Q�C��</param>
		/// <param name="effect">�G�t�F�N�g�Q�C��</param>
		/// <param name="feedback">�t�B�[�h�o�b�N�Q�C��</param>
		/// <param name="delay">�f�B���C�^�C��</param>
		public CombFilter(double direct, double effect, double feedback, double delayTime)
			: this(direct, effect, feedback, delayTime, new FractionalDelay(delayTime))
		{
		}

		public CombFilter(double direct, double effect, double feedback, double delayTime, IDelay delay)
		{
			this.direct = direct;
			this.effect = effect;
			this.feedback = feedback;
			this.delay = delay;
			this.DelayTime = delayTime;
			this.Clear();
		}

		/// <summary>
		/// �p�����[�^��ݒ肷��B
		/// </summary>
		/// <param name="direct">�_�C���N�g�Q�C��</param>
		/// <param name="effect">�G�t�F�N�g�Q�C��</param>
		/// <param name="feedback">�t�B�[�h�o�b�N�Q�C��</param>
		/// <param name="delay">�x������</param>
		public void SetParameter(double direct, double effect, double feedback, double delay)
		{
			this.DirectGain   = direct;
			this.EffectGain   = effect;
			this.FeedbackGain = feedback;
			this.DelayTime    = delay;
		}

		/// <summary>
		/// �_�C���N�g�Q�C��
		/// </summary>
		public double DirectGain
		{
			get{return this.direct;}
			set{this.direct = value;}
		}

		/// <summary>
		/// �G�t�F�N�g�Q�C��
		/// </summary>
		public double EffectGain
		{
			get{return this.effect;}
			set{this.effect = value;}
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
			get{return this.delay.DelayTime;}
			set
			{
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
			double y = x * this.direct + t * this.effect;
			this.delay.Push(x + t * this.feedback);
			return y;
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
			return new CombFilter(this.direct, this.effect, this.feedback, this.DelayTime);
		}

		public double GetBufferValue(int n)
		{
			return this.delay.GetBufferValue(n);
		}
	}
}
