using System;

namespace SoundLibrary.Mathematics.ThreeDimension
{
	/// <summary>
	/// �n�~���g���̎l�����B
	/// </summary>
	public struct Quaternion
	{
		#region �t�B�[���h

		/// <summary>
		/// �����B
		/// </summary>
		public double a;

		/// <summary>
		/// �����B
		/// </summary>
		public Vector  u;

		#endregion
		#region ������

		/// <summary>
		/// �����Ƌ����x�N�g�����w�肵�ď������B
		/// a + ii�Euu�Aii=(i,j,k)�Auu��(p,q,r)�B
		/// </summary>
		/// <param name="a">����</param>
		/// <param name="u">�����x�N�g��</param>
		public Quaternion(double a, Vector u)
		{
			this.a = a;
			this.u = u;
		}

		/// <summary>
		/// �����Ƌ����̗v�f���w�肵�ď������B
		/// a + i p + j q + k r�B
		/// </summary>
		/// <param name="a">����</param>
		/// <param name="p">������ i �v�f</param>
		/// <param name="q">������ j �v�f</param>
		/// <param name="r">������ k �v�f</param>
		public Quaternion(double a, double p, double q, double r)
			: this(a, new Vector(p, q, r))
		{
		}

		#endregion
		#region ���Z�q

		/// <summary>
		/// x + y�B
		/// </summary>
		/// <param name="x">x</param>
		/// <param name="y">y</param>
		/// <returns>x + y</returns>
		public static Quaternion operator+ (Quaternion x, Quaternion y)
		{
			return new Quaternion(x.a + y.a, x.u + y.u);
		}

		/// <summary>
		/// x - y�B
		/// </summary>
		/// <param name="x">x</param>
		/// <param name="y">y</param>
		/// <returns>x - y</returns>
		public static Quaternion operator- (Quaternion x, Quaternion y)
		{
			return new Quaternion(x.a - y.a, x.u - y.u);
		}

		/// <summary>
		/// -x�B
		/// </summary>
		/// <param name="x">x</param>
		/// <returns>-x</returns>
		public static Quaternion operator- (Quaternion x)
		{
			return new Quaternion(-x.a, -x.u);
		}

		/// <summary>
		/// x �~ y�B
		/// </summary>
		/// <param name="x">x</param>
		/// <param name="y">y</param>
		/// <returns>x �~ y</returns>
		public static Quaternion operator* (Quaternion x, Quaternion y)
		{
			double a = x.a * y.a - Vector.InnerProduct(x.u, y.u);
			Vector  u = x.a * y.u + y.a * x.u + Vector.OuterProduct(x.u, y.u);
			return new Quaternion(a, u);
		}

		/// <summary>
		/// x �� y�B
		/// </summary>
		/// <param name="x">x</param>
		/// <param name="y">y</param>
		/// <returns>x �� y</returns>
		public static Quaternion operator/ (Quaternion x, Quaternion y)
		{
			return x * y.Inverse();
		}

		/// <summary>
		/// ����p �~ x�B
		/// </summary>
		/// <param name="p">����p</param>
		/// <param name="x">x</param>
		/// <returns>p �~ x</returns>
		public static Quaternion operator* (double p, Quaternion x)
		{
			return new Quaternion(p * x.a, p * x.u);
		}

		/// <summary>
		/// x �~ ����p�B
		/// </summary>
		/// <param name="x">x</param>
		/// <param name="p">����p</param>
		/// <returns>p �~ x</returns>
		public static Quaternion operator* (Quaternion x, double p){return p * x;}

		/// <summary>
		/// x �� ����p�B
		/// </summary>
		/// <param name="x">x</param>
		/// <param name="p">����p</param>
		/// <returns>x �� p</returns>
		public static Quaternion operator/ (Quaternion x, double p){return (1/p) * x;}

		#endregion
		#region ���̑��̃��\�b�h

		/// <summary>
		/// ���m�����B
		/// </summary>
		public double Norm
		{
			get{return this.a * this.a + this.u.Norm;}
		}

		/// <summary>
		/// �����l���������߂�B
		/// </summary>
		/// <returns>�����l����</returns>
		public Quaternion Conjunction()
		{
			return new Quaternion(this.a, -this.u);
		}

		/// <summary>
		/// �t�������߂�B
		/// </summary>
		/// <returns>�t��</returns>
		public Quaternion Inverse()
		{
			return this.Conjunction() / this.Norm;
		}

		#endregion
		#region ��]����݂� static ���\�b�h

		/// <summary>
		/// �l�������g����3������ԏ�̉�]�B
		/// p �~ (0, x) �~ ~p ���v�Z����(~p �� p �̋���)�B
		/// </summary>
		/// <param name="p">��]��/�p��\���l����</param>
		/// <param name="x">��]���������_�̃x�N�g��</param>
		/// <returns>��]��̓_�̃x�N�g��</returns>
		public static Vector Rotate(Quaternion p, Vector x)
		{
			Vector y = (p.a * p.a - p.u.Norm) * x;
			y += 2 * (Vector.InnerProduct(p.u, x) * p.u + p.a * Vector.OuterProduct(p.u, x));
			return y;
		}

		/// <summary>
		/// �l�������g����3������ԏ�̉�]�B
		/// p �~ x �~ ~q ���v�Z����(~p �� p �̋���)�B
		/// </summary>
		/// <param name="p">p</param>
		/// <param name="x">x</param>
		/// <returns>p �~ x �~ ~q</returns>
		public static Quaternion Rotate(Quaternion p, Quaternion x)
		{
			return new Quaternion(x.a, Rotate(p, x.u));
		}

		/// <summary>
		/// �x�N�g��(axis)�����Ƃ��āA��(theta)��]���邽�߂̎l�������v�Z����B
		/// </summary>
		/// <param name="theta">��]�p��</param>
		/// <param name="axis">��]���x�N�g��</param>
		/// <returns>��]��\���l����</returns>
		public static Quaternion Rotator(double theta, Vector axis)
		{
			theta *= 0.5;
			axis *= Math.Sin(theta) / axis.Abs;
			return new Quaternion(Math.Cos(theta), axis);
		}

		#endregion
		#region ������

		public override string ToString()
		{
			return "(" + this.a.ToString() + ", " + this.u.ToString() + ")";
		}

	
		#endregion
	}
}
