using System;

namespace SpectrumAnalysis
{
	/// <summary>
	/// ���f���N���X�B
	/// </summary>
	public struct Complex
	{
		double re; // ����
		double im; // ����

		/// <summary>
		/// �������w�肵�č\�z�B
		/// </summary>
		/// <param name="re">����</param>
		public Complex(double re) : this(re, 0){}

		/// <summary>
		/// �����A�������w�肵�č\�z�B
		/// </summary>
		/// <param name="re">����</param>
		/// <param name="im">����</param>
		public Complex(double re, double im)
		{
			this.re = re;
			this.im = im;
		}

		/// <summary>
		/// �R�s�[�R���X�g���N�^�B
		/// </summary>
		/// <param name="z">�R�s�[��</param>
		public Complex(Complex z)
		{
			this.re = z.re;
			this.im = z.im;
		}

		/// <summary>
		/// double �� Complex �̈Öق̕��ϊ��B
		/// </summary>
		/// <param name="x">double �l</param>
		/// <returns>x �� Complex ����������</returns>
		public static implicit operator Complex(double x)
		{
			return new Complex(x);
		}

		/// <summary>
		/// ����
		/// </summary>
		public double Re
		{
			set{this.re = value;}
			get{return this.re;}
		}

		/// <summary>
		/// ����
		/// </summary>
		public double Im
		{
			set{this.im = value;}
			get{return this.im;}
		}

		/// <summary>
		/// �p���[(��Βl�̓��)�̃��j�A�l�B
		/// </summary>
		public double LinearPower
		{
			get{return this.re * this.re + this.im * this.im;}
		}

		/// <summary>
		/// �p���[(��Βl�̓��)�� dB �l�B
		/// </summary>
		public double Power
		{
			get{return 10 * Math.Log10(this.LinearPower);}
		}

		/// <summary>
		/// ��Βl
		/// </summary>
		public double Abs
		{
			get{return Math.Sqrt(this.LinearPower);}
		}

		/// <summary>
		/// �Ίp
		/// </summary>
		public double Arg
		{
			get{return Math.Atan2(this.im, this.re);}
		}

		/// <summary>
		/// ���𕡑f���B
		/// </summary>
		/// <returns>this �̋���</returns>
		public Complex Conjugate()
		{
			return new Complex(this.re, -this.im);
		}

		/// <summary>
		/// �t���B
		/// </summary>
		/// <returns>this �̋t��</returns>
		public Complex Invert()
		{
			double pow = this.LinearPower;
			return new Complex(this.re/pow, -this.im/pow);
		}

		/// <summary>
		/// �P��+�B
		/// </summary>
		/// <param name="a">�I�y�����h</param>
		/// <returns>+a</returns>
		public static Complex operator+ (Complex a)
		{
			return new Complex(a);
		}

		/// <summary>
		/// �P��-�B
		/// </summary>
		/// <param name="a">�I�y�����h</param>
		/// <returns>-a</returns>
		public static Complex operator- (Complex a)
		{
			return new Complex(-a.re, -a.im);
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>���Z����</returns>
		public static Complex operator+ (Complex a, Complex b)
		{
			double re = a.re + b.re;
			double im = a.im + b.im;
			return new Complex(re, im);
		}
		public static Complex operator+ (double a, Complex b)
		{
			double re = a + b.re;
			double im = b.im;
			return new Complex(re, im);
		}
		public static Complex operator+ (Complex a, double b)
		{
			double re = a.re + b;
			double im = a.im;
			return new Complex(re, im);
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>���Z����</returns>
		public static Complex operator- (Complex a, Complex b)
		{
			double re = a.re - b.re;
			double im = a.im - b.im;
			return new Complex(re, im);
		}
		public static Complex operator- (double a, Complex b)
		{
			double re = a - b.re;
			double im = -b.im;
			return new Complex(re, im);
		}
		public static Complex operator- (Complex a, double b)
		{
			double re = a.re - b;
			double im = a.im;
			return new Complex(re, im);
		}

		/// <summary>
		/// ��Z�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>��Z����</returns>
		public static Complex operator* (Complex a, Complex b)
		{
			double re = a.re * b.re - a.im * b.im;
			double im = a.im * b.re + a.re * b.im;
			return new Complex(re, im);
		}
		public static Complex operator* (double a, Complex b)
		{
			double re = a * b.re;
			double im = a * b.im;
			return new Complex(re, im);
		}
		public static Complex operator* (Complex a, double b)
		{
			double re = a.re * b;
			double im = a.im * b;
			return new Complex(re, im);
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>���Z����</returns>
		public static Complex operator/ (Complex a, Complex b)
		{
			return a * b.Invert();
		}
		public static Complex operator/ (double a, Complex b)
		{
			return a * b.Invert();
		}
		public static Complex operator/ (Complex a, double b)
		{
			double re = a.re / b;
			double im = a.im / b;
			return new Complex(re, im);
		}
	}//class Complex
}
