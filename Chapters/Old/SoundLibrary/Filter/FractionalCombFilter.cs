using System;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// �����x���R���t�B���^�B
	/// </summary>
	[Obsolete("SoundLibrary.Filter.Misc.CombFilter ���g���Ă��������B")]
	public class FractionalCombFilter : IFilter
	{
		double direct;
		double effect;
		double feedback;
		Delay.FractionalDelay delay;

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="direct">�_�C���N�g�Q�C��</param>
		/// <param name="effect">�G�t�F�N�g�Q�C��</param>
		/// <param name="feedback">�t�B�[�h�o�b�N�Q�C��</param>
		/// <param name="buffer">�o�b�t�@�T�C�Y</param>
		public FractionalCombFilter(double direct, double effect, double feedback, double buffer)
		{
			--buffer;
			if(buffer < 0) buffer = 0;

			this.direct = direct;
			this.effect = effect;
			this.feedback = feedback;
			this.delay = new Delay.FractionalDelay(buffer);
			this.Clear();
		}

		public FractionalCombFilter(double direct, double effect, double feedback, double buffer, int firLength)
		{
			--buffer;
			if(buffer < 0) buffer = 0;

			this.direct = direct;
			this.effect = effect;
			this.feedback = feedback;
			this.delay = new Delay.FractionalDelay(buffer, firLength);
			this.Clear();
		}

		/// <summary>
		/// �p�����[�^��ݒ肷��B
		/// </summary>
		/// <param name="direct">�_�C���N�g�Q�C��</param>
		/// <param name="effect">�G�t�F�N�g�Q�C��</param>
		/// <param name="feedback">�t�B�[�h�o�b�N�Q�C��</param>
		/// <param name="buffer">�x������</param>
		public void SetParameter(double direct, double effect, double feedback, double buffer)
		{
			this.DirectGain   = direct;
			this.EffectGain   = effect;
			this.FeedbackGain = feedback;
			this.DelayBuffer  = buffer;
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
		/// �o�b�t�@�T�C�Y
		/// </summary>
		public double DelayBuffer
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
			return new FractionalCombFilter(this.direct, this.effect, this.feedback, this.DelayBuffer);
		}
	}//class FractionalCombFilter
}
