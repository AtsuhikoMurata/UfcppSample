using System;

namespace SoundLibrary.Mathematics
{
	/// <summary>
	/// ���f���B
	/// </summary>
	public struct Complex
	{
		double re; // ����
		double im; // ����

		#region �R���X�g���N�^�E�\�z�p���\�b�h

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
		/// �Ίp���w�肵�Đ�Βl1�̕��f�����쐬����B
		/// </summary>
		/// <param name="arg">�Ίp</param>
		/// <returns>�w�肵���Ίp������Βl1�̕��f��</returns>
		public static Complex FromArg(double arg)
		{
			double re = Math.Cos(arg);
			double im = Math.Sin(arg);
			return new Complex(re, im);
		}

		/// <summary>
		/// ��΂ƕΊp���w�肵�ĕ��f�����쐬����B
		/// </summary>
		/// <param name="abs">��Βl</param>
		/// <param name="arg">�Ίp</param>
		/// <returns>�w�肵����Βl�ƕΊp�������f��</returns>
		public static Complex FromPolar(double abs, double arg)
		{
			double re = abs * Math.Cos(arg);
			double im = abs * Math.Sin(arg);
			return new Complex(re, im);
		}

		/// <summary>
		/// �p���[��dB�l�ƕΊp���w�肵�ĕ��f�����쐬����B
		/// </summary>
		/// <param name="power">�p���[��dB�l</param>
		/// <param name="arg">�Ίp</param>
		/// <returns>�w�肵����Βl�ƕΊp�������f��</returns>
		public static Complex FromPowerPolar(double power, double arg)
		{
			return Complex.FromPolar(Util.DBToLinear(power), arg);
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

		#endregion
		#region �����E����

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
		/// �����Ƌ�����ݒ肷��B
		/// </summary>
		/// <param name="re">����</param>
		/// <param name="im">����</param>
		public void SetCartesian(double re, double im)
		{
			this.re = re;
			this.im = im;
		}

		#endregion
		#region ��Βl�E�Ίp

		/// <summary>
		/// ��΂ƕΊp��ݒ肷��B
		/// </summary>
		/// <param name="abs">��Βl</param>
		/// <param name="arg">�Ίp</param>
		public void SetPolar(double abs, double arg)
		{
			this.re = abs * Math.Cos(arg);
			this.im = abs * Math.Sin(arg);
		}

		/// <summary>
		/// ���(�p���[��dB�l�Ŏw��)�ƕΊp��ݒ肷��B
		/// </summary>
		/// <param name="power">�p���[��dB�l</param>
		/// <param name="arg">�Ίp</param>
		public void SetPowerPolar(double power, double arg)
		{
			this.SetPolar(Util.DBToLinear(power), arg);
		}

		/// <summary>
		/// �Ίp��ݒ肷��(��Βl��1)�B
		/// </summary>
		/// <param name="arg">�Ίp</param>
		public void SetArg(double arg)
		{
			this.re = Math.Cos(arg);
			this.im = Math.Sin(arg);
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

		#endregion
		#region ���Z�q�E�ϊ����\�b�h

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

		/// <summary>
		/// ���l����B
		/// </summary>
		/// <param name="a">���I�y�����h</param>
		/// <param name="b">�E�I�y�����h</param>
		/// <returns>���茋��</returns>
		public static bool operator== (Complex a, Complex b)
		{
			return a.re == b.re && a.im == b.im;
		}
		public static bool operator== (Complex a, double b)
		{
			return a.re == b && a.im == 0;
		}
		public static bool operator== (double a, Complex b)
		{
			return a == b.re && 0 == b.im;
		}

		public static bool operator!= (Complex a, Complex b)
		{
			return a.re != b.re || a.im != b.im;
		}
		public static bool operator!= (Complex a, double b)
		{
			return a.re != b || a.im != 0;
		}
		public static bool operator!= (double a, Complex b)
		{
			return a != b.re || 0 != b.im;
		}

		#endregion
		#region object

		public override bool Equals(object obj)
		{
			Complex c = (Complex)obj;
			return this.re.Equals(c.re) && this.im.Equals(c.im);
		}

		public override int GetHashCode()
		{
			return this.re.GetHashCode() ^ this.im.GetHashCode();
		}

		#endregion
		#region string ������
		public override string ToString()
		{
			return string.Format("({0}, {1})", this.re, this.Im);
		}
		#endregion
	}//class Complex
}
