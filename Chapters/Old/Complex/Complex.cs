using System;

namespace MyMath
{
	/// <summary>
	/// ���f���^�̒��ۊ��N���X
	/// </summary>
	public abstract class Complex : ICloneable
	{
		#region ICloneable �����o

		public abstract object Clone();
		public abstract override bool Equals(object o);
		public abstract bool Equals(double x);
		public abstract override int GetHashCode();

		#endregion
		#region abstract �v���p�e�B

		/// <summary>
		/// ����
		/// </summary>
		public abstract double Re{set; get;}

		/// <summary>
		/// ����
		/// </summary>
		public abstract double Im{set; get;}

		/// <summary>
		/// ��Βl
		/// </summary>
		public abstract double Abs{set; get;}

		/// <summary>
		/// �Ίp
		/// </summary>
		public abstract double Arg{set; get;}

		#endregion
		#region abstract ���\�b�h

		/// <summary>
		/// ��Βl�̓���Ԃ�
		/// </summary>
		public abstract double Norm();

		/// <summary>
		/// this �𕄍����]���������̂�Ԃ�
		/// </summary>
		public abstract Complex Negate();

		/// <summary>
		/// this �̋t����Ԃ�
		/// </summary>
		public abstract Complex Invert();

		/// <summary>
		/// this �̋��𕡑f����Ԃ�
		/// </summary>
		public abstract Complex Conjugate();

		/// <summary>
		/// this �� z ���������l��Ԃ�
		/// (this�̒l�͕ω������Ȃ�)
		/// </summary>
		public abstract Complex Add(Complex z);
		public abstract Complex Add(double x);

		/// <summary>
		/// this ���� z ���������l��Ԃ�
		/// (this�̒l�͕ω������Ȃ�)
		/// </summary>
		public abstract Complex Sub(Complex z);
		public abstract Complex Sub(double x);

		/// <summary>
		/// this �� z ���|�����l��Ԃ�
		/// (this�̒l�͕ω������Ȃ�)
		/// </summary>
		public abstract Complex Mul(Complex z);
		public abstract Complex Mul(double x);

		/// <summary>
		/// this �� z �Ŋ������l��Ԃ�
		/// (this�̒l�͕ω������Ȃ�)
		/// </summary>
		public abstract Complex Div(Complex z);
		public abstract Complex Div(double x);

		#endregion
		#region object �����o

		/// <summary>
		/// ������
		/// </summary>
		public override string ToString()
		{
			if(this.Im > 0)
				return this.Re + "+i" + this.Im;
			if(this.Im < 0)
				return this.Re + "-i" + (-this.Im);
			return this.Re.ToString();
		}

		#endregion
		#region operator

		#region �P�����Z�q

		/// <summary>
		/// �P��+
		/// </summary>
		static public Complex operator+ (Complex z)
		{
			return (Complex)z.Clone();
		}

		/// <summary>
		/// �P��-
		/// </summary>
		static public Complex operator- (Complex z)
		{
			return z.Negate();
		}

		/// <summary>
		/// �������Z�q
		/// </summary>
		static public Complex operator~ (Complex z)
		{
			return z.Conjugate();
		}

		#endregion
		#region 2�����Z�q

		/// <summary>
		/// ���Z
		/// </summary>
		/// <returns>z+w</returns>
		static public Complex operator+ (Complex z, Complex w)
		{
			return z.Add(w);
		}
		static public Complex operator+ (Complex z, double x)
		{
			return z.Add(x);
		}
		static public Complex operator+ (double x, Complex z)
		{
			return z.Add(x);
		}

		/// <summary>
		/// ���Z
		/// </summary>
		/// <returns>z-w</returns>
		static public Complex operator- (Complex z, Complex w)
		{
			return z.Sub(w);
		}
		static public Complex operator- (Complex z, double x)
		{
			return z.Sub(x);
		}
		static public Complex operator- (double x, Complex z)
		{
			return z.Negate().Add(x);
		}

		/// <summary>
		/// ��Z
		/// </summary>
		/// <returns>z*w</returns>
		static public Complex operator* (Complex z, Complex w)
		{
			return z.Mul(w);
		}
		static public Complex operator* (Complex z, double x)
		{
			return z.Mul(x);
		}
		static public Complex operator* (double x, Complex z)
		{
			return z.Mul(x);
		}

		/// <summary>
		/// ���Z
		/// </summary>
		/// <returns>z/w</returns>
		static public Complex operator/ (Complex z, Complex w)
		{
			return z.Div(w);
		}
		static public Complex operator/ (Complex z, double x)
		{
			return z.Div(x);
		}
		static public Complex operator/ (double x, Complex z)
		{
			return z.Invert().Mul(x);
		}

		#endregion
		#region ��r���Z�q

		/// <summary>
		/// z �� w �̒l�̔�r
		/// </summary>
		static public bool operator== (Complex z, Complex w)
		{
			return z.Equals(w);
		}
		static public bool operator== (Complex z, double x)
		{
			return z.Equals(x);
		}
		static public bool operator== (double x, Complex z)
		{
			return z.Equals(x);
		}

		/// <summary>
		/// z �� w �̒l�̔�r
		/// </summary>
		static public bool operator!= (Complex z, Complex w)
		{
			return !z.Equals(w);
		}
		static public bool operator!= (Complex z, double x)
		{
			return !z.Equals(x);
		}
		static public bool operator!= (double x, Complex z)
		{
			return !z.Equals(x);
		}

		#endregion

		#endregion
		#region �ÓI���\�b�h

		static protected double Norm_(double x, double y)
		{
			return x*x + y*y;
		}

		static protected double Abs_(double x, double y)
		{
			return Math.Sqrt(Norm_(x, y));
		}

		static protected double Arg_(double x, double y)
		{
			return Math.Atan2(y, x);
		}

		#endregion
	}//class Complex
}