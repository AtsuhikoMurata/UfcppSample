using System;

using SoundLibrary.SpectrumAnalysis;
using SoundLibrary.Wave;

namespace SoundLibrary.WaveAnalysis
{
	/// <summary>
	/// Wave �f�[�^�i�[�p�N���X�B
	/// �������l���āA���n��̂܂܃f�[�^��ێ����Ă��� WaveTime �ƁA
	/// ���g���̈�ɕϊ����ăf�[�^��ێ����Ă��� WaveFrequency �ƁA
	/// Middle/Side �`���Ńf�[�^��ێ����Ă��� WaveMS �ɕ�����B
	/// ���̃N���X�͒��ۊ��N���X�B
	/// </summary>
	public abstract class WaveData
	{
		FormatHeader header;

		#region �R���X�g���N�^

		public WaveData(){}

		public WaveData(FormatHeader header)
		{
			this.header = header;
		}

		#endregion
		#region Wave �w�b�_�E���n��f�[�^�̎擾

		/// <summary>
		/// Wave �w�b�_���擾�B
		/// </summary>
		public FormatHeader Header{get{return this.header;}}

		/// <summary>
		/// ���n�� L ch �M�����擾�B
		/// </summary>
		public abstract double[] TimeL{set; get;}

		/// <summary>
		/// ���n�� R ch �M�����擾�B
		/// </summary>
		public abstract double[] TimeR{set; get;}

		/// <summary>
		/// ���n��̒����B
		/// </summary>
		public virtual int TimeLength
		{
			get{return this.TimeL.Length;}
		}

		#endregion
		#region ���g�������̎擾�E�ݒ�

		/// <summary>
		/// ���g������ L ch �M�����擾�B
		/// </summary>
		public abstract Spectrum Left{set; get;}

		/// <summary>
		/// ���g������ R ch �M�����擾�B
		/// </summary>
		public abstract Spectrum Right{set; get;}

		/// <summary>
		/// ���g������ Middle (L + R) ch �M�����擾�B
		/// </summary>
		public virtual Spectrum Middle
		{
			get{return this.Left + this.Right;}
			set{this.SetMS(value, this.Side);}
		}

		/// <summary>
		/// ���g������ Side (L - R) ch �M�����擾�B
		/// </summary>
		public virtual Spectrum Side
		{
			get{return this.Left - this.Right;}
			set{this.SetMS(this.Middle, value);}
		}

		/// <summary>
		/// Left/Right ch �M����ݒ�B
		/// </summary>
		/// <param name="middle">M ch</param>
		/// <param name="side">S ch</param>
		public virtual void SetLR(Spectrum left, Spectrum right)
		{
			this.Left = left;
			this.Right = right;
		}

		/// <summary>
		/// Middle/Side ch �M����ݒ�B
		/// </summary>
		/// <param name="middle">M ch</param>
		/// <param name="side">S ch</param>
		public virtual void SetMS(Spectrum middle, Spectrum side)
		{
			this.Left  = 0.5 * (middle + side);
			this.Right = 0.5 * (middle - side);
		}

		/// <summary>
		/// ���g�������̒����B
		/// </summary>
		public virtual int Count
		{
			get{return this.Left.Count;}
		}

		#endregion
		#region �����`���̕ϊ�

		/// <summary>
		/// �����`�������n��ɕϊ��B
		/// </summary>
		/// <returns>�����`�������n��Ŏ��� WaveData</returns>
		public WaveTime ToTime()
		{
			return new WaveTime(this.header, this.TimeL, this.TimeR);
		}

		/// <summary>
		/// �����`�������g�������ɕϊ��B
		/// </summary>
		/// <returns>�����`�������g�������Ŏ��� WaveData</returns>
		public WaveFrequency ToSpectrum()
		{
			return new WaveFrequency(this.header, this.Left, this.Right);
		}

		/// <summary>
		/// �����`�������g������(Middle/Side)�ɕϊ��B
		/// </summary>
		/// <returns>�����`�������g�������Ŏ��� WaveData</returns>
		public WaveMS ToMS()
		{
			return new WaveMS(this.header, this.Middle, this.Side);
		}

		#endregion
		#region ���g�������̉�́E���������Ȃ�

		/// <summary>
		/// ���E�̎��ԍ����擾�B
		/// ��: Left ch �̕����x���B
		/// ��: Right ch �̕����x���B
		/// </summary>
		/// <returns>���E�̎��ԍ�</returns>
		public int GetDelay()
		{
			return SoundLibrary.Mathematics.Discrete.Function.Argmax(
				new SoundLibrary.Mathematics.Discrete.Correlation(this.TimeL, this.TimeR));
		}

		/// <summary>
		/// �x����^����B
		/// </summary>
		/// <param name="delay">�x������</param>
		/// <returns>�x����^������̃f�[�^</returns>
		public WaveData AddDelay(int delay)
		{
			WaveTime w = this.ToTime();
			double[] l = w.TimeL;
			double[] r = w.TimeR;

			int i = l.Length - 1;
			int n = i - delay;
			for(; n>=0; --i, --n)
			{
				l[i] = l[n];
				r[i] = r[n];
			}
			for(; i>=0; --i)
			{
				l[i] = 0;
				r[i] = 0;
			}
			return w;
		}

		/// <summary>
		/// �ŏ��ʑ�������B
		/// ���E�̐M���̒x�������t���B
		/// </summary>
		/// <param name="baseDelay">L/R �����ɂ�����x��</param>
		/// <returns>�ŏ��ʑ��������̃f�[�^</returns>
		public WaveData ConvertToMinimumPhase(int baseDelay)
		{
			int delay = this.GetDelay();

			WaveFrequency w = this.ToSpectrum();
			w.Left.ConvertToMinimumPhase();
			w.Right.ConvertToMinimumPhase();

			w.Left  *= Spectrum.FromDelay(baseDelay + delay/2, this.Left.TimeLength);
			w.Right *= Spectrum.FromDelay(baseDelay - delay/2, this.Right.TimeLength);

			return w;
		}

		/// <summary>
		/// �ŏ��ʑ�������B
		/// ���E�̐M���̒x�������t���B
		/// </summary>
		public WaveData ConvertToMinimumPhase()
		{
			return this.ConvertToMinimumPhase(32);
		}

		/// <summary>
		/// F = this<br />
		/// [Gl Gr]   [Fl Fr]^-1<br />
		/// [Gr Gl] = [Fr Fl]   <br />
		/// G �����߂�B
		/// </summary>
		/// <returns>G</returns>
		public WaveData Invert()
		{
			return new WaveMS(this.header, this.Middle.Invert(), this.Side.Invert());
		}

		/// <summary>
		/// Al = a.Left, Ar = a.Right<br />
		/// Bl = b.Left, Br = b.Right<br />
		/// Cl = c.Left, Cr = c.Right<br />
		/// [Cl]   [Bl Br][Al]   [Al Ar][Bl]<br />
		/// [Cr] = [Br Bl][Ar] = [Ar Al][Br]<br />
		/// 
		/// c �����߂�B
		/// c.Middle = a.Middle * b.Middle,
		/// c.Side = a.Side * b.Side
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h1</param>
		/// <returns>�v�Z����</returns>
		public static WaveData operator* (WaveData a, WaveData b)
		{
			return new WaveMS(a.header, a.Middle * b.Middle, a.Side * b.Side);
		}

		/// <summary>
		/// Al = a.Left, Ar = a.Right<br />
		/// Bl = b.Left, Br = b.Right<br />
		/// Cl = c.Left, Cr = c.Right<br />
		/// [Cl]   [Bl Br]^-1[Al]<br />
		/// [Cr] = [Br Bl]   [Ar]<br />
		/// 
		/// c �����߂�B
		/// c.Middle = a.Middle / b.Middle,
		/// c.Side = a.Side / b.Side
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h1</param>
		/// <returns>�v�Z����</returns>
		public static WaveData operator/ (WaveData a, WaveData b)
		{
			return new WaveMS(a.header, a.Middle / b.Middle, a.Side / b.Side);
		}

		/// <summary>
		/// Left/Right �� s ���|����B
		/// </summary>
		/// <param name="s">���g������</param>
		public virtual void Mul(Spectrum s)
		{
			this.Left *= s;
			this.Right *= s;
		}

		/// <summary>
		/// Left/Right �� s �Ŋ���B
		/// </summary>
		/// <param name="s">���g������</param>
		public virtual void Div(Spectrum s)
		{
			this.Left /= s;
			this.Right /= s;
		}

		/// <summary>
		/// b.Left = a.Left * s, b.Right = a.Right * s;
		/// </summary>
		/// <param name="a">��搔</param>
		/// <param name="s">�搔</param>
		/// <returns>��Z����</returns>
		public static WaveData operator* (WaveData a, Spectrum s)
		{
			WaveData b = a.ToSpectrum();
			b.Mul(s);
			return b;
		}

		/// <summary>
		/// b.Left = a.Left / s, b.Right = a.Right / s;
		/// </summary>
		/// <param name="a">�폜��</param>
		/// <param name="s">����</param>
		/// <returns>���Z����</returns>
		public static WaveData operator/ (WaveData a, Spectrum s)
		{
			WaveData b = a.ToSpectrum();
			b.Div(s);
			return b;
		}

		#endregion
		#region ������܂肢��Ȃ��C������B
		/// <summary>
		/// �f�[�^�̓������擾�B
		/// </summary>
		/// <param name="spectrum">�X�y�N�g��</param>
		/// <param name="type">�����̎��</param>
		/// <returns>����</returns>
		public static double[] GetData(Spectrum spectrum, Property type)
		{
			switch(type)
			{
				case Property.Amplitude:
				{
					double[] tmp = spectrum.GetPower();
					Spectrum.Smooth(tmp);
					return tmp;
				}
				case Property.Phase:
				{
					double[] tmp = spectrum.GetPhase();
					Spectrum.Unwrap(tmp);
					Spectrum.Smooth(tmp);
					return tmp;
				}
				case Property.MinimumPhase:
				{
					double[] tmp = spectrum.GetMinimumPhase();
					Spectrum.Smooth(tmp);
					return tmp;
				}
				case Property.AllPassPhase:
				{
					double[] tmp  = spectrum.GetPhase();
					double[] tmp2 = spectrum.GetMinimumPhase();
					for(int i=0; i<tmp.Length; ++i) tmp[i] += tmp2[i];
					Spectrum.Unwrap(tmp);
					Spectrum.Smooth(tmp);
					return tmp;
				}
				/*
				case Property.PhaseDelay:
				{
					double[] tmp = spectrum.GetPhase();
					Spectrum.Unwrap(tmp);
					Spectrum.Smooth(tmp);
					return Spectrum.GetPhaseDalay(tmp, 48000);
				}
				case Property.GroupDelay:
				{
					double[] tmp = spectrum.GetPhase();
					Spectrum.Unwrap(tmp);
					Spectrum.Smooth(tmp);
					return Spectrum.GetGroupDalay(tmp, 48000);
				}
				*/
				default:
					return spectrum.TimeSequence;
			}
		}//GetData

		/// <summary>
		/// �f�[�^�̓������擾�B
		/// </summary>
		/// <param name="channel">�擾�������`���l��</param>
		/// <param name="type">�擾����������</param>
		/// <returns>����</returns>
		public double[] GetData(Channel channel, Property type)
		{
			switch(channel)
			{
				case Channel.Left:   return GetData(this.Left, type);
				case Channel.Right:  return GetData(this.Right, type);
				case Channel.LR:     return GetData(this.Left / this.Right, type);
				case Channel.RL:     return GetData(this.Right / this.Left, type);
				case Channel.Middle: return GetData(this.Middle, type);
				case Channel.Side:   return GetData(this.Side, type);
				case Channel.MS:     return GetData(this.Middle / this.Side, type);
				case Channel.SM:     return GetData(this.Side / this.Middle, type);
				default: return null;
			}
		}//GetData

		/// <summary>
		/// �X�y�N�g���̎擾�B
		/// </summary>
		/// <param name="channel">�擾�������`���l��</param>
		/// <returns>�X�y�N�g��</returns>
		public Spectrum GetSpectrum(Channel channel)
		{
			switch(channel)
			{
				case Channel.Left:   return this.Left;
				case Channel.Right:  return this.Right;
				case Channel.LR:     return this.Left / this.Right;
				case Channel.RL:     return this.Right / this.Left;
				case Channel.Middle: return this.Middle;
				case Channel.Side:   return this.Side;
				case Channel.MS:     return this.Middle / this.Side;
				case Channel.SM:     return this.Side / this.Middle;
				default: return null;
			}
		}//GetSpectrum

		/// <summary>
		/// �`���l���̃^�C�v�B
		/// </summary>
		public enum Channel
		{
			Left,   // L �`���l��
			Right,  // R �`���l��
			LR,     // Left / Right
			RL,     // Right / Left
			Middle, // M �`���l��
			Side,   // S �`���l��
			MS,     // Middle / Side
			SM,     // Side / Middle
		}

		/// <summary>
		/// �����̃^�C�v�B
		/// </summary>
		public enum Property
		{
			Amplitude,    // �U������
			Phase,        // �ʑ�����
			PhaseDelay,   // �ʑ��x������
			GroupDelay,   // �Q�x������
			MinimumPhase, // �ŏ��ʑ�����
			AllPassPhase, // �I�[���p�X�ʑ�����
			TimeSequence, // ���n��f�[�^
		}
		#endregion
	}//class Wave
}//namespace WaveAnalysis
