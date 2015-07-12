using System;

namespace Filter
{
	/// <summary>
	/// ���֐��p�� delegate
	/// </summary>
	public delegate double WindowFunction(double x);

	/// <summary>
	/// FIR �t�B���^�֌W�̋��ʊ֐��Q
	/// </summary>
	class FirCommon
	{
		/// <summary>
		/// ���`�ʑ����[�p�X/�o���h�p�X/�n�C�p�X�t�B���^�̃t�B���^�^�C�v�B
		/// </summary>
		public enum FirFilterType
		{
			LPF, BPF, HPF
		}

		/// <summary>
		/// ���`�ʑ��o���h�p�X�t�B���^�̌W�����v�Z����B
		/// �E�������݂̂��v�Z�B
		/// </summary>
		/// <param name="type">�t�B���^�^�C�v</param>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="w">�Ւf���g��(BPF �̏ꍇ�͎Ւf�ш敝�AHPF �̏ꍇ�� ��-�Ւf���g��)</param>
		/// <param name="w0">(BPF �̂�) ���S���g��</param>
		/// <param name="window">���֐�</param>
		/// <returns>�W���̉E���������v�Z��������</returns>
		public static void CalcLinearBPFCoefficient(FirFilterType type, double[] coef, double w, double w0, WindowFunction window)
		{
			int n = coef.Length;
			double sum;

			sum = coef[0] = window(0) * w;
			for(int i=1; i<n; ++i)
			{
				double tmp = window(i) * Math.Sin(w * i) / i;
				sum += tmp * 2;

				if(type == FirFilterType.LPF)
				{
					coef[i] = tmp;
				}
				else if(type == FirFilterType.HPF)
				{
					if(i%2 != 0)
						coef[i] = -tmp;
					else
						coef[i] = tmp;
				}
				else
				{
					coef[i] = 2 * Math.Cos(w0 * i) * tmp;
				}
			}

			for(int i=0; i<n; ++i)
			{
				coef[i] /= sum;
			}
		}//GetLinearBPFCoefficient

		/// <summary>
		/// ���֐��p�B���1��Ԃ��֐��B
		/// </summary>
		public static double Constant1(double x)
		{
			return 1;
		}
	}//class FirCommon

	/// <summary>
	/// FIR �t�B���^�N���X�B
	/// </summary>
	public class FirFilter : IFilter
	{
		protected double[] coef; // �W���z��
		protected CircularBuffer buff; // �x���o�b�t�@

		/// <summary>
		/// �f�t�H���g�R���X�g���N�^
		/// </summary>
		public FirFilter() : this(null){}

		/// <summary>
		/// �^�b�v�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="taps">�^�b�v��</param>
		public FirFilter(int taps) : this(new double[taps]){}

		/// <summary>
		/// �W�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="coef">�W�����i�[�����z��B</param>
		public FirFilter(double[] coef)
		{
			this.Coefficient = coef;
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// N: �t�B���^���� (= this.coef.Length - 1)
		/// x: ����
		/// y: �o��
		/// c[i]: �W���z��
		/// d[i]: i+1 �T���v���O�� x �̒l
		/// �Ƃ���ƁA
		/// y = x*c[N] + ��_{i=0}^{N-1} d[i]*c[N-1-i]
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			int N = this.coef.Length - 1;
			double y = this.coef[N] * x;
			for(int i=0; i<N-1; ++i)
			{
				y += this.buff[i] * this.coef[i];
			}
			this.buff.PushBack(x);
			return y;
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
			for(int i=0; i<this.buff.Length; ++i)
			{
				this.buff[i] = 0;
			}
		}

		/// <summary>
		/// �W���̎擾/�ݒ�
		/// </summary>
		public double[] Coefficient
		{
			set
			{
				this.coef = value;
				if(value == null)
					this.buff = null;
				else
					this.buff = new CircularBuffer(coef.Length - 1);
			}
			get
			{
				return this.coef;
			}
		}
	}//class FirFilter

	/// <summary>
	/// ���`�ʑ� FIR �t�B���^�N���X�B
	/// �W�������Ώۂł��邱�Ƃ𗘗p���Čv�Z��/�������ʍ팸�B
	/// ��^�b�v�o�[�W����
	/// (�^�b�v�� 2n + 1 �ŁAcoef[n-i] == coef[n+i] foreach i)
	/// </summary>
	public class OddLinearFir : IFilter
	{
		protected double[] coef;
		protected CircularBuffer buff;

		/// <summary>
		/// �f�t�H���g�R���X�g���N�^
		/// </summary>
		public OddLinearFir() : this(null){}

		/// <summary>
		/// �^�b�v�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="n">�^�b�v�� �� 2n + 1</param>
		public OddLinearFir(int n) : this(new double[n+1]){}

		/// <summary>
		/// �W�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="coef">�W�����i�[�����z��B</param>
		public OddLinearFir(double[] coef)
		{
			this.Coefficient = coef;
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// n: �^�b�v���� 2n + 1 (n = this.coef.Length - 1)
		/// x: ����
		/// y: �o��
		/// c[i]: �W���z��
		/// d[i]: i+1 �T���v���O�� x �̒l
		/// �Ƃ���ƁA
		/// y = d[n]*c[0] + ��_{i=1}^{n} (d[n+i] + d[n-i])*c[i]
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			this.buff.PushBack(x);

			int n = this.coef.Length - 1;
			double y = this.coef[0] * this.buff[n];
			for(int i=1; i<=n; ++i)
			{
				y += this.coef[i] * (this.buff[n+i] + this.buff[n-i]);
			}
			return y;
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
			for(int i=0; i<this.buff.Length; ++i)
			{
				this.buff[i] = 0;
			}
		}

		/// <summary>
		/// �W���̎擾/�ݒ�
		/// </summary>
		public double[] Coefficient
		{
			set
			{
				this.coef = value;
				if(value == null)
					this.buff = null;
				else
					this.buff = new CircularBuffer(2*coef.Length - 1);
			}
			get
			{
				int n = this.coef.Length;
				double[] tmp = new double[this.coef.Length * 2 - 1];
				for(int i=0; i<n; ++i)
				{
					tmp[n-1 + i] = tmp [n-1 - i] = this.coef[i];
				}

				return tmp;
			}
		}
	}//class OddLinearFir

	/// <summary>
	/// ���[�p�X FIR �t�B���^�B
	/// </summary>
	public class LowPassFir : OddLinearFir
	{
		/// <summary>
		/// ���[�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="w">�Ւf���g��</param>
		public LowPassFir(int n, double w) : this(n, w, new WindowFunction(FirCommon.Constant1)){}
		
		/// <summary>
		/// ���[�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="w">�Ւf���g��</param>
		/// <param name="window">���֐�</param>
		public LowPassFir(int n, double w, WindowFunction window) : base(n)
		{
			FirCommon.CalcLinearBPFCoefficient(FirCommon.FirFilterType.LPF, this.coef, w, 0, window);
		}
	}//class LowPassFir

	/// <summary>
	/// �n�C�p�X FIR �t�B���^�B
	/// </summary>
	public class HighPassFir : OddLinearFir
	{
		/// <summary>
		/// �n�C�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="w">�Ւf���g��</param>
		public HighPassFir(int n, double w) : this(n, w, new WindowFunction(FirCommon.Constant1)){}
		
		/// <summary>
		/// �n�C�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="w">�Ւf���g��</param>
		/// <param name="window">���֐�</param>
		public HighPassFir(int n, double w, WindowFunction window) : base(n)
		{
			FirCommon.CalcLinearBPFCoefficient(FirCommon.FirFilterType.HPF, this.coef, Math.PI - w, 0, window);
		}
	}//class HighPassFir

	/// <summary>
	/// �o���h�p�X FIR �t�B���^�B
	/// </summary>
	public class BandPassFir : OddLinearFir
	{
		/// <summary>
		/// �o���h�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="wl">�������g��</param>
		/// <param name="wh">������g��</param>
		public BandPassFir(int n, double wl, double wh) : this(n, wl, wh, new WindowFunction(FirCommon.Constant1)){}
		
		/// <summary>
		/// �o���h�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="wl">�������g��</param>
		/// <param name="wh">������g��</param>
		/// <param name="window">���֐�</param>
		public BandPassFir(int n, double wl, double wh, WindowFunction window) : base(n)
		{
			FirCommon.CalcLinearBPFCoefficient(FirCommon.FirFilterType.BPF, this.coef, (wl - wh)/2, (wl + wh)/2, window);
		}
	}//class BandPassFir

	public class Delay : IFilter
	{
		CircularBuffer buf;

		/// <summary>
		/// �R���X�g���N�^
		/// </summary>
		/// <param name="taps">�x���^�b�v��</param>
		public Delay(int taps)
		{
			this.buf = new CircularBuffer(taps);
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			double tmp = this.buf.Top;
			this.buf.PushBack(x);
			return tmp;
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
			for(int i=0; i<this.buf.Length; ++i)
			{
				this.buf[i] = 0;
			}
		}

		public int Taps{get{return this.buf.Length;}}
	}//class Delay
}//namespace Filter
