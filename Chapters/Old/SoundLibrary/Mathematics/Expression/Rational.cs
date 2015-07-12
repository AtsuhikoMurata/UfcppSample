using System;

using Ellip = SoundLibrary.Mathematics.Elliptic;

namespace SoundLibrary.Mathematics.Expression
{
	using CoefType   = System.Double;
	using DomainType = System.Double;
	using ValueType  = System.Double;

	/// <summary>
	/// �L�����B
	/// </summary>
	public class Rational
	{
		#region �t�B�[���h

		Polynomial num;
		Polynomial denom;

		#endregion
		#region ������

		/// <summary>
		/// 0�����ŏ������B
		/// </summary>
		public Rational() : this(0) {}

		/// <summary>
		/// ���q�������̎������w�肵�ď������B
		/// </summary>
		/// <param name="nOrder">���q�������̎���</param>
		public Rational(int nOrder) : this(nOrder, 0) {}

		/// <summary>
		/// �������w�肵�ď������B
		/// </summary>
		/// <param name="nOrder">���q�������̎���</param>
		/// <param name="dOrder">���ꑽ�����̎���</param>
		public Rational(int nOrder, int dOrder) : this(new Polynomial(nOrder), new Polynomial(dOrder)) {}

		/// <summary>
		/// ���q/���ꑽ�������w�肵�ď������B
		/// </summary>
		/// <param name="num">���q������</param>
		/// <param name="denom">���ꑽ����</param>
		public Rational(Polynomial num, Polynomial denom)
		{
			this.num   = num;
			this.denom = denom;
		}

		#endregion
		#region �l�̌v�Z

		/// <summary>
		/// f(x) ���v�Z�B
		/// </summary>
		/// <param name="x">x</param>
		/// <returns>f(x)</returns>
		public ValueType Value(DomainType x)
		{
			return this.num.Value(x) / this.denom.Value(x);
		}

		#endregion
		#region ���q/���ꑽ�����̎擾

		/// <summary>
		/// ���q�������B
		/// </summary>
		public Polynomial Numerator
		{
			get{return this.num;}
			set{this.num = value;}
		}

		/// <summary>
		/// ���ꑽ�����B
		/// </summary>
		public Polynomial Denominator
		{
			get{return this.denom;}
			set{this.denom = value;}
		}

		#endregion
		#region operator

		/// <summary>
		/// �P��+�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <returns>+f(x)</returns>
		public static Rational operator+ (Rational f)
		{
			return new Rational(+f.num, +f.denom);
		}

		/// <summary>
		/// �P��-�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <returns>-f(x)</returns>
		public static Rational operator- (Rational f)
		{
			return new Rational(-f.num, +f.denom);
		}

		/// <summary>
		/// ���������m�̉��Z�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="g">g(x)</param>
		/// <returns>f(x) + g(x)</returns>
		public static Rational operator+ (Rational f, Rational g)
		{
			Polynomial num = f.num * g.denom + f.denom * g.denom;
			Polynomial denom = f.denom * g.denom;

			return new Rational(num, denom);
		}

		/// <summary>
		/// ���������m�̌��Z�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="g">g(x)</param>
		/// <returns>f(x) - g(x)</returns>
		public static Rational operator- (Rational f, Rational g)
		{
			Polynomial num = f.num * g.denom - f.denom * g.denom;
			Polynomial denom = f.denom * g.denom;

			return new Rational(num, denom);
		}

		/// <summary>
		/// ���������m�̏�Z�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="g">g(x)</param>
		/// <returns>f(x) * g(x)</returns>
		public static Rational operator* (Rational f, Rational g)
		{
			Polynomial num = f.num * g.num;
			Polynomial denom = f.denom * g.denom;

			return new Rational(num, denom);
		}

		/// <summary>
		/// ���������m�̏��Z�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="g">g(x)</param>
		/// <returns>f(x) / g(x)</returns>
		public static Rational operator/ (Rational f, Rational g)
		{
			Polynomial num = f.num * g.denom;
			Polynomial denom = f.denom * g.num;

			return new Rational(num, denom);
		}

		#endregion
		#region ����ȑ��������擾

		#region �`�F�r�V�F�t�L����

		/// <summary>
		/// �`�F�r�V�F�t�L����(elliptic rational)���v�Z����B
		/// </summary>
		/// <param name="n">����</param>
		/// <param name="l">x > 1 �ɂ�����ɏ��l</param>
		/// <returns>���� n �̃`�F�r�V�F�t�L����</returns>
		public static Rational Elliptic(int n, double l)
		{
			double m1 = 1 / (l * l);
			double m1p = 1 - m1;

			double Kk1  = Ellip.K(m1);
			double Kk1p = Ellip.K(m1p);

			double m    = Ellip.InverseQ(Math.Exp(-Math.PI * Kk1p / (n * Kk1)));
			double Kk   = Ellip.K(m);

			Polynomial num   = Polynomial.X(0, 1);
			Polynomial denom = Polynomial.X(0, 1);

			Rational r = new Rational(Polynomial.X(0, 1), Polynomial.X(0, 1));
			double g = 1;

			for(int i=n-1; i>0; i-=2)
			{
				double u = Kk * (double)i / n;
				double sn = Ellip.Sn(u, m);
				double w = sn * sn;

				g *= (m*w - 1) / (1/w - 1);
				num   *= Polynomial.X(2,	1/w) - 1;
				denom *= Polynomial.X(2, m*w) - 1;
			}

			if((n & 1) == 1)
			{
				num *= Polynomial.X(1);
			}

			num *= g;

			return new Rational(num, denom);
		}

		#endregion

		#endregion
	}
}
