using System;
using SoundLibrary.Mathematics;

namespace SoundLibrary.SpectrumAnalysis
{
	/// <summary>
	/// ���g�������N���X�B
	/// </summary>
	public class Spectrum : ICloneable
	{
		double[] x; // �f�[�^�i�[�̈�
		Fft fft;

		#region �R���X�g���N�^�E�\�z�p���\�b�h

		/// <summary>
		/// �f�t�H���g�R���X�g���N�^
		/// </summary>
		public Spectrum() : this(null){}

		/// <summary>
		/// ���n�񒷂݂̂��w�肵�č\�z�B
		/// </summary>
		/// <param name="length">���n��</param>
		public Spectrum(int length) : this(new double[length]){}

		/// <summary>
		/// ���g���̈�f�[�^(���n��f�[�^�� Fft �N���X�ŕϊ���������)����쐬�B
		/// </summary>
		/// <param name="x">���g���̈�f�[�^</param>
		public Spectrum(double[] x)
		{
			this.x = x;
			this.fft = new Fft(x.Length);
		}

		/// <summary>
		/// ���ԗ̈�f�[�^����쐬�B
		/// </summary>
		/// <param name="X">���ԗ̈�f�[�^</param>
		/// <returns>�쐬���ꂽ���g������</returns>
		public static Spectrum FromTimeSequence(double[] x)
		{
			if(x == null) return null;
			return FromTimeSequence(x, x.Length, 0);
		}

		/// <summary>
		/// ���ԗ̈�f�[�^����쐬�B
		/// </summary>
		/// <param name="x">���ԗ̈�f�[�^</param>
		/// <param name="length">�f�[�^�̒���</param>
		/// <returns>�쐬���ꂽ���g������</returns>
		public static Spectrum FromTimeSequence(double[] x, int length)
		{
			return FromTimeSequence(x, length, 0);
		}

		/// <summary>
		/// ���ԗ̈�f�[�^����쐬�B
		/// </summary>
		/// <param name="x">���ԗ̈�f�[�^</param>
		/// <param name="length">�f�[�^�̒���</param>
		/// <param name="skip">�ǂݔ�΂�����</param>
		/// <returns>�쐬���ꂽ���g������</returns>
		public static Spectrum FromTimeSequence(double[] x, int length, int skip)
		{
			if(x == null) return null;

			int len = BitOperation.FloorPower2(length);

			double[] tmp = new double[len];
			for(int i=0; i<len; ++i) tmp[i] = x[skip + i];
			Fft fft = new Fft(len);
			fft.Transform(tmp);
			return new Spectrum(tmp);
		}

		/// <summary>
		/// �x�� ��[i - delay] �̎��g���������쐬����B
		/// </summary>
		/// <param name="delay">�x���T���v����</param>
		/// <returns>�쐬���ꂽ���g������</returns>
		public static Spectrum FromDelay(double delay, int length)
		{
			Spectrum s = new Spectrum(length);
			length /= 2;
			s.x[0] = 1;
			s.x[1] = Math.Cos(-Math.PI * delay);
			double dt = -Math.PI * delay / length;
			double t = dt;
			for(int i=1; i<length; ++i, t += dt)
			{
				s[i] = Complex.FromArg(t);
			}

			return s;
		}

		#endregion
		#region �l�̎擾�E�ݒ�

		/// <summary>
		/// ���g���̈�̃f�[�^���B
		/// </summary>
		public int Count
		{
			get{return x.Length/2;}
		}

		/// <summary>
		/// i �Ԗڂ̃f�[�^�𕡑f���l�ŕԂ��B
		/// </summary>
		public Complex this[int i]
		{
			set
			{
				if(i == 0) this.x[0] = value.Re;
				else if(i == x.Length/2) this.x[1] = value.Re;
				else
				{
					this.x[2*i]   = value.Re;
					this.x[2*i+1] = -value.Im;
				}
			}
			get
			{
				if(i == 0) return this.x[0];
				else if(i == x.Length/2) return this.x[1];
				return new Complex(this.x[2*i], -this.x[2*i+1]);
			}
		}

		/// <summary>
		/// ���n���Ԃ��B
		/// </summary>
		public double[] TimeSequence
		{
			get
			{
				double[] x = (double[])this.x.Clone();
				this.fft.Invert(x);
				for(int i=0; i<x.Length; ++i) x[i] *= 2.0/x.Length;
				return x;
			}
		}

		/// <summary>
		/// ���n���Ԃ��B
		/// ���炩���ߎ��n��f�[�^�̊i�[���p�ӂ��Ă����B
		/// </summary>
		/// <param name="x">�f�[�^�i�[��</param>
		public void GetTimeSequence(double[] x)
		{
			double a = 2.0/x.Length;
			for(int i=0; i<x.Length; ++i) x[i] = a * this.x[i];
			this.fft.Invert(x);
		}

		/// <summary>
		/// ���ԗ̈�̃f�[�^���B
		/// </summary>
		public int TimeLength
		{
			get{return x.Length;}
		}

		#endregion
		#region ���Z�q�E�ϊ����\�b�h

		/// <summary>
		/// �P���{�B���̂܂܂̒l��Ԃ��B
		/// </summary>
		/// <param name="a">�I�y�����h</param>
		/// <returns>+a</returns>
		public static Spectrum operator+ (Spectrum a)
		{
			return a.Clone();
		}

		/// <summary>
		/// �P���|�B
		/// </summary>
		/// <param name="a">�I�y�����h</param>
		/// <returns>-a</returns>
		public static Spectrum operator- (Spectrum a)
		{
			Spectrum c = a.Clone();
			for(int i=0; i<=c.Count; ++i)
				c[i] = -a[i];
			return c;
		}

		/// <summary>
		/// �X�y�N�g�����m�̘a A(��) + B(��)�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>�a</returns>
		public static Spectrum operator+ (Spectrum a, Spectrum b)
		{
			Spectrum c = a.Clone();
			for(int i=0; i<=c.Count; ++i)
				c[i] += b[i];
			return c;
		}

		/// <summary>
		/// �X�y�N�g�����m�̍� A(��) - B(��)�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>��</returns>
		public static Spectrum operator- (Spectrum a, Spectrum b)
		{
			Spectrum c = a.Clone();
			for(int i=0; i<=c.Count; ++i)
				c[i] -= b[i];
			return c;
		}

		/// <summary>
		/// �X�y�N�g�����m�̐� A(��) * B(��)�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>��</returns>
		public static Spectrum operator* (Spectrum a, Spectrum b)
		{
			Spectrum c = a.Clone();
			for(int i=0; i<=c.Count; ++i)
				c[i] *= b[i];
			return c;
		}

		/// <summary>
		/// �X�y�N�g���~�����̐� A(��) * x�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="x">�E�I�y�����h</param>
		/// <returns>��</returns>
		public static Spectrum operator* (Spectrum a, double x)
		{
			Spectrum c = a.Clone();
			for(int i=0; i<=c.Count; ++i)
				c[i] *= x;
			return c;
		}

		/// <summary>
		/// �X�y�N�g���~�����̐� x * A(��)�B
		/// </summary>
		/// <param name="x">���I�y�����h</param>
		/// <param name="a">�E�I�y�����h</param>
		/// <returns>��</returns>
		public static Spectrum operator* (double x, Spectrum a)
		{
			return a * x;
		}

		/// <summary>
		/// �X�y�N�g�����m�̏� A(��) / B(��)�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>��</returns>
		public static Spectrum operator/ (Spectrum a, Spectrum b)
		{
			Spectrum c = a.Clone();
			for(int i=0; i<=c.Count; ++i)
				c[i] /= b[i];
			return c;
		}

		public Spectrum Invert()
		{
			Spectrum c = this.Clone();
			for(int i=0; i<=c.Count; ++i)
				c[i] = this[i].Invert();
			return c;
		}

		#endregion
		#region �����E����

		/// <summary>
		/// ������z�񉻂��Ď擾�B
		/// </summary>
		/// <returns></returns>
		public double[] GetRe()
		{
			double[] tmp = new double[this.x.Length / 2];
			for(int i=0; i<tmp.Length; ++i) tmp[i] = this[i].Re;
			return tmp;
		}

		/// <summary>
		/// ������z�񉻂��Ď擾�B
		/// </summary>
		/// <returns></returns>
		public double[] GetIm()
		{
			double[] tmp = new double[this.x.Length / 2];
			for(int i=0; i<tmp.Length; ++i) tmp[i] = this[i].Im;
			return tmp;
		}

		#endregion
		#region �U������

		/// <summary>
		/// �p���[�X�y�N�g��[dB]���擾�B
		/// </summary>
		public double[] GetPower()
		{
			double[] tmp = new double[this.x.Length / 2];
			for(int i=0; i<tmp.Length; ++i) tmp[i] = this[i].Power;
			return tmp;
		}

		/// <summary>
		/// �U���������擾�B
		/// </summary>
		public double[] GetAmplitude()
		{
			double[] tmp = new double[this.x.Length / 2];
			for(int i=0; i<tmp.Length; ++i) tmp[i] = this[i].Abs;
			return tmp;
		}

		#endregion
		#region �ʑ�����

		/// <summary>
		/// �ʑ��������擾�B
		/// </summary>
		public double[] GetPhase()
		{
			double[] tmp = new double[this.x.Length / 2];
			tmp[0] = 0;
			for(int i=1; i<tmp.Length; ++i) tmp[i] = this[i].Arg;
			return tmp;
		}

		/// <summary>
		/// �ʑ�����(�A�����b�v��������)���擾�B
		/// </summary>
		public double[] GetUnwrapPhase()
		{
			double[] tmp = this.GetPhase();
			Spectrum.Unwrap(tmp);
			return tmp;
		}

		/// <summary>
		/// �ʑ�����(�A�����b�v��������)���擾�B
		/// </summary>
		/// <param name="skip">�ŏ� skip �T���v���̓A�����b�v���Ȃ�</param>
		public double[] GetUnwrapPhase(int skip)
		{
			double[] tmp = this.GetPhase();
			Spectrum.Unwrap(tmp, skip);
			return tmp;
		}

		/// <summary>
		/// �ʑ��̃A�����b�s���O���s���B
		/// </summary>
		/// <param name="phase">�ʑ��̓������z��</param>
		public static void Unwrap(double[] phase)
		{
			double tmp = 0;
			double prev = phase[0];

			for(int i=1; i<phase.Length; ++i)
			{
				double dif = phase[i] - prev;
				prev = phase[i];
				if(dif < -Math.PI) tmp += 2 * Math.PI;
				if(dif >  Math.PI) tmp -= 2 * Math.PI;
				phase[i] += tmp;
			}
		}

		/// <summary>
		/// �ʑ��̃A�����b�s���O���s���B
		/// </summary>
		/// <param name="phase">�ʑ��̓������z��</param>
		/// <param name="start">�A�����b�s���O�̊J�n�n�_</param>
		public static void Unwrap(double[] phase, int start)
		{
			double tmp = 0;
			double prev = phase[start];

			for(int i=start+1; i<phase.Length; ++i)
			{
				double dif = phase[i] - prev;
				prev = phase[i];
				if(dif < -Math.PI) tmp += 2 * Math.PI;
				if(dif >  Math.PI) tmp -= 2 * Math.PI;
				phase[i] += tmp;
			}
		}

		#endregion
		#region �ŏ��ʑ��E�I�[���p�X�ʑ�

		/// <summary>
		/// �ŏ��ʑ������߂�B
		/// </summary>
		public double[] GetMinimumPhase()
		{
			int N = this.x.Length / 2;
			double[] tmp = new double[2*N];
			for(int i=0; i<N; ++i)
			{
				tmp[2*i]         = Math.Log(this[i].LinearPower);
				tmp[2*i+1]       = 0;
			}

			CFft cfft = new CFft(2*N);
			cfft.Invert(tmp);

			tmp[0] /= 2; tmp[1] /= 2;
			tmp[N] /= 2; tmp[N+1] /= 2;
			for(int i=N+1; i<2*N; ++i)
				tmp[i] = 0;

			cfft.Transform(tmp);

			double[] y = new double[N];
			for(int i=0; i<N; ++i)
			{
				y[i] = -tmp[2*i+1] / N;
			}

			return y;
		}//GetMinimumPhase

		/// <summary>
		/// �I�[���p�X�ʑ������߂�B
		/// </summary>
		public double[] GetAllpassPhase()
		{
			double[] p  = GetPhase();
			Spectrum.Unwrap(p);
			double[] mp = GetMinimumPhase();
			int n = p.Length;

			for(int i=0; i<n; ++i) p[i] -= mp[i];

			return p;
		}//GetAllpassPhase

		#endregion
		#region �ʑ��x���E�Q�x������

		/// <summary>
		/// �ʑ��x���������擾�B
		/// </summary>
		public static double[] GetPhaseDelay(double[] phase, double fs)
		{
			double df = fs / phase.Length;
			double dw = 2 * Math.PI * df;
			double[] tmp = new Double[phase.Length];

			tmp[0] = tmp[1] = -phase[1] / dw;
			for(int i=2; i<tmp.Length; ++i) tmp[i] = -phase[i] / (dw * i);
			return tmp;
		}

		public double[] GetPhaseDelay()
		{
			double[] phase = this.GetUnwrapPhase(this.Count / 100);

			double dw = 2 * Math.PI / phase.Length;
			double[] tmp = new Double[phase.Length];

			tmp[0] = tmp[1] = -phase[1] / dw;
			for(int i=2; i<tmp.Length; ++i) tmp[i] = -phase[i] / (dw * i);
			return tmp;
		}

		/// <summary>
		/// �Q�x���������擾�B
		/// F(��) = x + j y, ��=��F,
		/// gd = -d��/d�� = -(d/d��)Im[log F] = -Im[(d/d��)log F]
		/// = -Im[F'/F] = -Im[(x'+jy') / (x+jy) = (x'y - xy') / (x*x + y*y)
		/// </summary>
		public double[] GetGroupDelay()
		{
			double[] re = this.GetRe();
			double[] im = this.GetIm();
			double[] dre = SoundLibrary.Mathematics.Discrete.Differential.Derive(re);
			double[] dim = SoundLibrary.Mathematics.Discrete.Differential.Derive(im);
			double[] tmp = new Double[re.Length];
			double c = tmp.Length / Math.PI;

			for(int i=0; i<tmp.Length; ++i)
			{
				double d = re[i] * re[i] + im[i] * im[i];
				tmp[i] = im[i] * dre[i] - re[i] * dim[i];
				tmp[i] *= c / d;
			}
			return tmp;
		}

		/// <summary>
		/// �Q�x���������擾�B
		/// </summary>
		[System.Obsolete("GetGroupDelay �̑Ó������m���߂�ꎟ��폜���܂�")]
		public double[] GetGroupDelay0()
		{
			double[] tmp = this.GetUnwrapPhase(this.Count / 100);

			tmp = SoundLibrary.Mathematics.Discrete.Differential.Derive(tmp);
			double c = tmp.Length / Math.PI;

			for(int i=0; i<tmp.Length; ++i) tmp[i] *= -c;
			return tmp;
		}

		#endregion
		#region �x���֌W�A�񐄏����\�b�h(Obsolete �ɂ���Ȃ�A�폜����Ȃ肵����)

		/// <summary>
		/// ���σf�B���C(���I�[���p�X�ʑ��̕��ό��z�~-1)�����߂�B
		/// </summary>
		public double GetMeanDelay()
		{
			double[] p = this.GetUnwrapPhase(this.Count / 100);
			double[] mp = GetMinimumPhase();
			int n = p.Length;
			double delay = 0;

			for(int i=0; i<n; ++i) delay += p[i] - mp[i];
			delay *= -2.0 / (n * (n - 1));

			return delay;
		}

		/// <summary>
		/// �ʑ����畽�σf�B���C�����������������߂�B
		/// </summary>
		/// <param name="delay">�ʑ����畽�σf�B���C������������</param>
		public double[] GetPhase0(double delay)
		{
			double[] p = this.GetUnwrapPhase(this.Count / 100);
			int n = p.Length;

			for(int i=0; i<n; ++i) p[i] += delay * i;

			return p;
		}

		/// <summary>
		/// �ʑ����畽�σf�B���C�����������������߂�B
		/// </summary>
		public double[] GetPhase0()
		{
			double[] p = this.GetUnwrapPhase(this.Count / 100);
			double[] mp = GetMinimumPhase();
			int n = p.Length;
			double x = 0;

			for(int i=0; i<n; ++i) x += p[i] - mp[i];
			x *= -2.0 / (n * n);

			for(int i=0; i<n; ++i) p[i] += x * i;

			return p;
		}

		#endregion
		#region �`���̕ϊ�

		/// <summary>
		/// �U�������̕��ϒl��0dB�ɂȂ�悤�ɐ��K��
		/// </summary>
		public void Normalize()
		{
			double level = 0;
			for(int i=0; i<this.Count; ++i) level += Math.Log(this[i].LinearPower);
			level /= 2 * this.Count;
			level = Math.Exp(-level);
			for(int i=0; i<this.Count; ++i) this[i] *= level;
		}

		/// <summary>
		/// �ŏ��ʑ�������B
		/// </summary>
		public void ConvertToMinimumPhase()
		{
			double[] amp = this.GetAmplitude();
			double[] phase = this.GetMinimumPhase();
			int len = this.Count;

			for(int i=0; i<len; ++i)
			{
				this[i] = Complex.FromPolar(amp[i], phase[i]);
			}
		}

		/// <summary>
		/// �ŏ��ʑ����������g�������𐶐�����B
		/// </summary>
		/// <returns>�ŏ��ʑ����������g������</returns>
		public Spectrum GetMinimumPahsedSpectrum()
		{
			Spectrum s = this.Clone();
			s.ConvertToMinimumPhase();
			return s;
		}

		#endregion
		#region ICloneable �����o

		/// <summary>
		/// �C���X�^���X�̃R�s�[���쐬�B
		/// </summary>
		/// <returns></returns>
		public Spectrum Clone()
		{
			return new Spectrum((double[])this.x.Clone());
		}

		object System.ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
		#region static �֐�

		/// <summary>
		/// x ���q���x���g�ϊ�����B
		/// </summary>
		/// <param name="x">�ϊ���</param>
		/// <returns>�ϊ���</returns>
		public static double[] HilbertTransform(double[] x)
		{
			int N = x.Length;
			double[] tmp = new double[2*N];
			for(int i=0; i<N; ++i)
			{
				tmp[2*i]         = x[i];
				tmp[2*i+1]       = 0;
			}

			CFft cfft = new CFft(2*N);
			cfft.Invert(tmp);

			tmp[0] /= 2; tmp[1] /= 2;
			tmp[N] /= 2; tmp[N+1] /= 2;
			for(int i=N+1; i<2*N; ++i)
				tmp[i] = 0;

			cfft.Transform(tmp);

			double[] y = new double[x.Length];
			for(int i=0; i<N; ++i)
			{
				x[i] = tmp[2*i]   * 2 / N;
				y[i] = tmp[2*i+1] * 2 / N;
			}

			return y;
		}//HilbertTransform

		/// <summary>
		/// �f�[�^�̃X���[�W���O���s���B
		/// </summary>
		/// <param name="data"></param>
		public static void Smooth(double[] data)
		{
			double sum = data[0] + data[1] + data[2];
			data[0] = sum / 3;
			sum += data[3];
			data[1] = sum / 4;
			sum += data[4];
			data[1] = sum / 5;
			int i=3;
			for(; i<data.Length-2; ++i)
			{
				sum -= data[i-3];
				sum += data[i+2];
				data[i] = sum / 5;
			}
			sum -= data[i-3];
			data[i] = sum / 4;
			++i;
			sum -= data[i-3];
			data[i] = sum / 3;
		}

		#endregion
	}//class Spectrum
}//namespace SpectrumAnalysis
