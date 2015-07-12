using System;

//! todo
/*
 * ���A�P��-�A��ݍ��� (operator)
 * �����E�ϕ�
 * http://www5.airnet.ne.jp/tomy/cpro/sslib9.htm
 * �ŏ����ߎ�
 * Chebyshev �ߎ�
 * 
 * �L��������낤�B
 * Fourier Series �Ƃ����B
 */

namespace SoundLibrary.Mathematics.Expression
{
	using CoefType   = System.Double;
	using DomainType = System.Double;
	using ValueType  = System.Double;

	/// <summary>
	/// �������B
	/// </summary>
	public class Polynomial : ICloneable
	{
		#region �t�B�[���h

		/// <summary>
		/// coef[n] �c n���̌W���B
		/// </summary>
		CoefType[] coef;

		#endregion
		#region ������

		public Polynomial() : this(0) {}

		/// <summary>
		/// �������w�肵�ď������B
		/// </summary>
		/// <param name="order">�������̎���</param>
		public Polynomial(int order) : this(new CoefType[order + 1]) {}

		/// <summary>
		/// �W���z����w�肵�ď������B
		/// </summary>
		/// <param name="coef">�W���z��</param>
		public Polynomial(params CoefType[] coef)
		{
			this.coef = coef;
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
			int n=coef.Length-1;
			ValueType y = this.coef[n];

			while(n > 0)
			{
				y *= x;
				--n;
				y += this.coef[n];
			}

			return y;
		}

		#endregion
		#region �W���̎擾

		public CoefType[] Coef
		{
			get{return this.coef;}
		}

		#endregion
		#region operator

		/// <summary>
		/// �P��+�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <returns>+f(x)</returns>
		public static Polynomial operator+ (Polynomial f)
		{
			return f.Clone();
		}

		/// <summary>
		/// ���������m�̉��Z�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="g">g(x)</param>
		/// <returns>f(x) + g(x)</returns>
		public static Polynomial operator+ (Polynomial f, Polynomial g)
		{
			CoefType[] a, b, c;
			Select(f.coef, g.coef, out a, out b);
			c = new CoefType[a.Length];

			int n = 0;
			for(; n<b.Length; ++n) c[n] = a[n] + b[n];
			for(; n<a.Length; ++n) c[n] = a[n];

			return new Polynomial(c);
		}

		/// <summary>
		/// ���������m�̌��Z�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="g">g(x)</param>
		/// <returns>f(x) - g(x)</returns>
		public static Polynomial operator- (Polynomial f, Polynomial g)
		{
			CoefType[] c;

			if(f.coef.Length > g.coef.Length)
			{
				c = new CoefType[f.coef.Length];

				int n = 0;
				for(; n<g.coef.Length; ++n) c[n] = f.coef[n] - g.coef[n];
				for(; n<f.coef.Length; ++n) c[n] = f.coef[n];
			}
			else
			{
				c = new CoefType[g.coef.Length];

				int n = 0;
				for(; n<f.coef.Length; ++n) c[n] = f.coef[n] - g.coef[n];
				for(; n<g.coef.Length; ++n) c[n] = -g.coef[n];
			}

			return new Polynomial(c);
		}

		/// <summary>
		/// -f
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <returns>-f(x)</returns>
		public static Polynomial operator-(Polynomial f)
		{
			CoefType[] c;
			c = new CoefType[f.coef.Length];
			for(int n=0; n<f.coef.Length; ++n) c[n] = -f.coef[n];
			return new Polynomial(c);
		}

		/// <summary>
		/// ���������m�̏�Z�B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="g">g(x)</param>
		/// <returns>f(x) �~ g(x)</returns>
		public static Polynomial operator* (Polynomial f, Polynomial g)
		{
			CoefType[] c = Convolute(f.coef, g.coef);
			return new Polynomial(c);
		}

		/// <summary>
		/// ���������W���́B
		/// </summary>
		/// <param name="f">f(x)</param>
		/// <param name="a">a</param>
		/// <returns>f(x) �� a</returns>
		public static Polynomial operator/ (Polynomial f, CoefType a)
		{
			CoefType[] c = (CoefType[])f.coef.Clone();
			for(int n=0; n<c.Length; ++n) c[n] /= a;
			return new Polynomial(c);
		}

		/// <summary>
		/// �W�����������̃L���X�g�B
		/// </summary>
		/// <param name="a">�W��</param>
		/// <returns>Polynominal</returns>
		public static implicit operator Polynomial (CoefType a)
		{
			return new Polynomial(a);
		}

		#endregion
		#region ����ȑ��������擾

		#region x �ׂ̂���

		/// <summary>
		/// x �� n ���Ԃ��B
		/// </summary>
		/// <param name="n">�w��</param>
		/// <returns>x �� n ��</returns>
		public static Polynomial X(int n)
		{
			return Polynomial.X(n, 1);
		}

		/// <summary>
		/// a x^n ��Ԃ��B
		/// </summary>
		/// <param name="n">�w��</param>
		/// <param name="a">�W��</param>
		/// <returns>x �� n ��</returns>
		public static Polynomial X(int n, CoefType a)
		{
			CoefType[] c = new CoefType[n + 1];
			for(int i=0; i<n; ++i) c[i] = 0;
			c[n] = a;

			return new Polynomial(c);
		}

		#endregion
		#region �`�F�r�V�F�t������

		/// <summary>
		/// �`�F�r�V�F�t���������v�Z����B
		/// </summary>
		/// <param name="n">����</param>
		/// <returns>���� n �̃`�F�r�V�F�t������</returns>
		public static Polynomial Chebyshev(int n)
		{
			if(n == 0)
				return Polynomial.X(0, 1);
			else if(n == 1)
				return Polynomial.X(1, 1);
			
			return Polynomial.X(1, 2) * Polynomial.Chebyshev(n - 1) - Polynomial.Chebyshev(n - 2);
		}

		#endregion
		#region ���O�����W�����

		public static Polynomial Lagrange(DomainType[] x, DomainType[] y)
		{
			if(x.Length != y.Length)
				throw new System.ArgumentException("x �� y �̎����͓������Ȃ���΂����܂���B");

			int len = x.Length;
			Polynomial p = (Polynomial)(CoefType)0.0;
			Polynomial X = Polynomial.X(1);

			for(int i=0; i<len; ++i)
			{
				Polynomial q = (Polynomial)(CoefType)y[i];

				for(int j=0; j<len; ++j)
				{
					if(i == j) continue;

					Polynomial temp = (X - (CoefType)x[j]);
					temp /= (x[i] - x[j]);
					q *= (X - (CoefType)x[j]) / (x[i] - (CoefType)x[j]);
				}
				p += q;
			}

			return p;
		}

		#endregion

		#endregion
		#region static �֐�

		/// <summary>
		/// x �� y �̂����A�������̔z��� a �ɁA�Z������ b �Ɋi�[�B
		/// </summary>
		static void Select(CoefType[] x, CoefType[] y, out CoefType[] a, out CoefType[] b)
		{
			if(x.Length > y.Length)
			{
				a = x;
				b = y;
			}
			else
			{
				a = y;
				b = x;
			}
		}

		/// <summary>
		/// �z��̏􍞂ݐς��v�Z����B
		/// </summary>
		/// <param name="x">�z��1</param>
		/// <param name="y">�z��2</param>
		/// <returns>x * y</returns>
		static CoefType[] Convolute(CoefType[] x, CoefType[] y)
		{
			CoefType[] a, b, c;

			Select(x, y, out a, out b);

			c = new CoefType[a.Length + b.Length - 1];
			for(int k=0; k<c.Length; ++k) c[k] = 0;

			int i=0;
			for(; i<b.Length; ++i)
				for(int k=0, l=i; k<=i; ++k, --l)
					c[i] += a[k] * b[l];
			for(; i<a.Length; ++i)
				for(int k=i, l=0; l<b.Length; --k, ++l)
					c[i] += a[k] * b[l];
			for(; i<c.Length; ++i)
				for(int l=b.Length-1, k=i-l; k<a.Length; ++k, --l)
					c[i] += a[k] * b[l];

			return c;
		}

		#endregion
		#region object

		public override bool Equals(object obj)
		{
			Polynomial p = obj as Polynomial;

			if(p == null)
			{
				return false;
			}

			if(this.coef.Length != p.coef.Length)
				return false;

			for(int i=0; i<this.coef.Length; ++i)
			{
				if(this.coef[i] != p.coef[i])
					return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int code = 2376498;

			for(int i=0; i<this.coef.Length; ++i)
			{
				code <<= 2;
				code ^= this.coef[i].GetHashCode();
			}

			return (int)code;
		}

		#endregion
		#region ICloneable �����o

		public Polynomial Clone()
		{
			return new Polynomial((CoefType[])this.coef.Clone());
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
	}
}
