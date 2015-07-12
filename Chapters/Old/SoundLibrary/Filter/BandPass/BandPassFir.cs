using System;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// ���֐��p�̃f���Q�[�g�B
	/// </summary>
	public delegate double WindowFunction(double i);

	/// <summary>
	/// ���֐��p�̃C���^�[�t�F�[�X�B
	/// </summary>
	public interface IWindow
	{
		/// <summary>
		/// ���֐��̒l���擾�B
		/// </summary>
		/// <param name="i">i �T���v���ڂ̒l���擾����</param>
		/// <returns>���֐��̒l</returns>
		double Get(double i);

		/// <summary>
		/// ���֐��̎�����ݒ肷��B
		/// </summary>
		int Order{get; set;}
	}

	/// <summary>
	/// ���`�ʑ����[�p�X/�o���h�p�X/�n�C�p�X�t�B���^�̃t�B���^�^�C�v�B
	/// </summary>
	public enum FirFilterType
	{
		LPF, BPF, HPF
	}

	/// <summary>
	/// ���֐��^�C�v�B
	/// </summary>
	public enum WindowType
	{
		Rectangular, // ��`��
		Hamming,     // �n�~���O��
		Hanning,     // �n�j���O��
		Blackman,    // �u���b�N�}����
		Keiser,      // �J�C�U�[��
	}

	/// <summary>
	/// FIR �t�B���^�֌W�̋��ʊ֐��Q
	/// </summary>
	public class FirCommon
	{
		public static IFirFilter GetLowPassFilter(int taps, double w, WindowType type)
		{
			return CalcLinearBPFCoefficient(FirFilterType.LPF, w, 0, taps, type);
		}

		public static IFirFilter GetBandPassFilter(int taps, double wl, double wh, WindowType type)
		{
			return CalcLinearBPFCoefficient(FirFilterType.BPF, (wl - wh)/2, (wl + wh)/2, taps, type);
		}

		public static IFirFilter GetHighPassFilter(int taps, double w, WindowType type)
		{
			return CalcLinearBPFCoefficient(FirFilterType.HPF, Math.PI - w, 0, taps, type);
		}

		public static IFirFilter CalcLinearBPFCoefficient(FirFilterType type, double w, double w0, int taps, WindowType window)
		{
			double[] coef;

			if(taps % 2 == 1)
			{
				coef = new double[(taps + 1) / 2];
				CalcOddLinearBPFCoefficient(type, coef, w, w0, GetWindow(window, taps));
				return new OddLinearFir(coef);
			}
			else
			{
				coef = new double[taps / 2];
				CalcEvenLinearBPFCoefficient(type, coef, w, w0, GetWindow(window, taps));
				return new EvenLinearFir(coef);
			}
		}

		/// <summary>
		/// ���`�ʑ��o���h�p�X�t�B���^(��^�b�v)�̌W�����v�Z����B
		/// �E�������݂̂��v�Z�B
		/// </summary>
		/// <param name="type">�t�B���^�^�C�v</param>
		/// <param name="coef">�W���̊i�[��</param>
		/// <param name="w">�Ւf���g��(BPF �̏ꍇ�͎Ւf�ш敝�AHPF �̏ꍇ�� ��-�Ւf���g��)</param>
		/// <param name="w0">(BPF �̂�) ���S���g��</param>
		/// <param name="window">���֐�</param>
		public static void CalcOddLinearBPFCoefficient(FirFilterType type, double[] coef, double w, double w0, WindowFunction window)
		{
			int n = coef.Length - 1;
			double sum;

			sum = coef[n] = window(0) * w;
			for(int i=1; i<=n; ++i)
			{
				double tmp = window(i) * Math.Sin(w * i) / i;
				sum += tmp * 2;

				if(type == FirFilterType.LPF)
				{
					coef[n - i] = tmp;
				}
				else if(type == FirFilterType.HPF)
				{
					if(i%2 != 0)
						coef[n - i] = -tmp;
					else
						coef[n - i] = tmp;
				}
				else
				{
					coef[n - i] = 2 * Math.Cos(w0 * i) * tmp;
				}
			}

			for(int i=0; i<=n; ++i)
			{
				coef[i] /= sum;
			}
		}//GetOddLinearBPFCoefficient


		/// <summary>
		/// ���`�ʑ��o���h�p�X�t�B���^(��^�b�v)�̌W�����v�Z����B
		/// �E�������݂̂��v�Z�B
		/// </summary>
		/// <param name="type">�t�B���^�^�C�v</param>
		/// <param name="coef">�W���̊i�[��</param>
		/// <param name="w">�Ւf���g��(BPF �̏ꍇ�͎Ւf�ш敝�AHPF �̏ꍇ�� ��-�Ւf���g��)</param>
		/// <param name="w0">(BPF �̂�) ���S���g��</param>
		/// <param name="window">���֐�</param>
		public static void CalcEvenLinearBPFCoefficient(FirFilterType type, double[] coef, double w, double w0, WindowFunction window)
		{
			int n = coef.Length - 1;
			double sum;

			sum = 0;
			for(int i=0; i<=n; ++i)
			{
				double x = i + 0.5;

				double tmp = window(x) * Math.Sin(w * x) / x;
				sum += tmp * 2;

				if(type == FirFilterType.LPF)
				{
					coef[n - i] = tmp;
				}
				else if(type == FirFilterType.HPF)
				{
					if(i%2 != 0)
						coef[n - i] = -tmp;
					else
						coef[n - i] = tmp;
				}
				else
				{
					coef[n - i] = 2 * Math.Cos(w0 * x) * tmp;
				}
			}

			for(int i=0; i<n; ++i)
			{
				coef[i] /= sum;
			}
		}//GetEvenLinearBPFCoefficient

		/// <summary>
		/// ���֐��p�B���1��Ԃ��֐��B
		/// </summary>
		public static double Constant1(double i)
		{
			return 1;
		}

		public static WindowFunction GetWindow(WindowType type, int order)
		{
			switch(type)
			{
				case WindowType.Hamming:
					return new WindowFunction(new Window.Hamming(order).Get);
				case WindowType.Hanning:
					return new WindowFunction(new Window.Hanning(order).Get);
				case WindowType.Blackman:
					return new WindowFunction(new Window.Blackman(order).Get);
				case WindowType.Keiser:
					return new WindowFunction(new Window.Keiser(order, 20).Get);
				default:
					return new WindowFunction(Constant1);
			}
		}
	}//class FirCommon

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
		public LowPassFir(int n, double w, IWindow window) : this(n, w, new WindowFunction(window.Get)){}

		/// <summary>
		/// ���[�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="w">�Ւf���g��</param>
		/// <param name="window">���֐�</param>
		public LowPassFir(int n, double w, WindowFunction window) : base(n)
		{
			this.SetParameter(w, window);
		}

		/// <summary>
		/// �p�����[�^��ݒ肷��B
		/// </summary>
		/// <param name="w">�Ւf���g��</param>
		/// <param name="window">���֐�</param>
		public void SetParameter(double w, WindowFunction window)
		{
			FirCommon.CalcOddLinearBPFCoefficient(FirFilterType.LPF, this.coef, w, 0, window);
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
		public HighPassFir(int n, double w, IWindow window) : this(n, w, new WindowFunction(window.Get)){}
		
		/// <summary>
		/// �n�C�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="w">�Ւf���g��</param>
		/// <param name="window">���֐�</param>
		public HighPassFir(int n, double w, WindowFunction window) : base(n)
		{
			this.SetParameter(w, window);
		}

		/// <summary>
		/// �p�����[�^��ݒ肷��B
		/// </summary>
		/// <param name="w">�Ւf���g��</param>
		/// <param name="window">���֐�</param>
		public void SetParameter(double w, WindowFunction window)
		{
			FirCommon.CalcOddLinearBPFCoefficient(FirFilterType.HPF, this.coef, Math.PI - w, 0, window);
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
		public BandPassFir(int n, double wl, double wh, IWindow window) : this(n, wl, wh, new WindowFunction(window.Get)){}

		/// <summary>
		/// �o���h�p�X�t�B���^���쐬
		/// </summary>
		/// <param name="n">�^�b�v����2n+1</param>
		/// <param name="wl">�������g��</param>
		/// <param name="wh">������g��</param>
		/// <param name="window">���֐�</param>
		public BandPassFir(int n, double wl, double wh, WindowFunction window) : base(n)
		{
			this.SetParameter(wl, wh, window);
		}

		/// <summary>
		/// �p�����[�^��ݒ肷��B
		/// </summary>
		/// <param name="wl">�������g��</param>
		/// <param name="wh">������g��</param>
		/// <param name="window">���֐�</param>
		public void SetParameter(double wl, double wh, WindowFunction window)
		{
			FirCommon.CalcOddLinearBPFCoefficient(FirFilterType.BPF, this.coef, (wl - wh)/2, (wl + wh)/2, window);
		}
	}//class BandPassFir

	namespace Window
	{
		/// <summary>
		/// �n�j���O��
		/// </summary>
		public class Hanning : IWindow
		{
			int order;
			public Hanning(int order){this.order = order;}

			public double Get(double i)
			{
				return 0.5 + 0.5 * Math.Cos(2 * Math.PI * i / (double)this.order) ;
			}

			public int Order
			{
				get{return this.order;}
				set{this.order = value;}
			}
		}

		/// <summary>
		/// �n�~���O��
		/// </summary>
		public class Hamming : IWindow
		{
			int order;
			public Hamming(int order){this.order = order;}

			public double Get(double i)
			{
				return 0.54 + 0.46 * Math.Cos(2 * Math.PI * i / (double)this.order) ;
			}

			public int Order
			{
				get{return this.order;}
				set{this.order = value;}
			}
		}

		/// <summary>
		/// �u���b�N�}����
		/// </summary>
		public class Blackman : IWindow
		{
			int order;
			public Blackman(int order){this.order = order;}

			public double Get(double i)
			{
				return 0.42 + 0.5 * Math.Cos(2 * Math.PI * i / (double)this.order) 
					+ 0.08 * Math.Cos(4 * Math.PI * i / (double)this.order);
			}

			public int Order
			{
				get{return this.order;}
				set{this.order = value;}
			}
		}

		public class Keiser : IWindow
		{
			/// <summary>
			/// 0����1��ό`�x�b�Z���֐�
			/// </summary>
			static double I0(double x)
			{
				double sum = 1.0;
				double xj = 1.0;

				for(int j=1; j<20 ; j++)
				{
					xj = 0.5*xj*x/j;
					sum = sum + Pow2(xj);
				}
				return sum;
			}

			static double Pow2(double x){return x * x;}

			int order2; // ����/2
			double alpha;

			public Keiser(int order, double attenuate)
			{
				this.order2 = order / 2;

				if(attenuate >= 50.0)
					this.alpha = 0.1102 * (attenuate - 8.7);
				else if(attenuate > 21.0)
					this.alpha = 0.5842 * Math.Pow(attenuate - 21.0 , 0.4)
						+ 0.07886 * (attenuate - 21.0);
				else
					this.alpha = 0.0;  //���`���ƂȂ�
			}

			public double Get(double i)
			{
				double fm = this.alpha * Math.Sqrt(1.0-Pow2((double)i / this.order2));
				return I0(fm) / I0(this.alpha);
			}

			public int Order
			{
				get{return this.order2 * 2;}
				set{this.order2 = value / 2;}
			}
		}

		/// <summary>
		/// ��`��
		/// </summary>
		public class Rectangular : IWindow
		{
			public double Get(double i)
			{
				return 1;
			}

			public int Order
			{
				get{return 0;}
				set{}
			}
		}
	}//namespace Window
}//namespace Filter
