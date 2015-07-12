using System;

namespace SoundLibrary.Mathematics.ThreeDimension
{
	/// <summary>
	/// �A�t�B���ϊ��p��4�~4�s��B
	/// </summary>
	/// <remarks>
	/// 4��ڂ� (0, 0, 0, 1) �Œ�B
	/// �A�t�B���s�� B ��1���ϊ��s�� A �� �x�N�g�� t ��p���āA
	/// B = (A t)
	///     (0 1)
	/// �ƕ\�����B
	/// </remarks>
	public class AffineMatrix
	{
		#region �t�B�[���h

		Matrix a;
		Vector t;

		#endregion
		#region ������

		public AffineMatrix() : this(new Matrix(), new Vector()) {}

		public AffineMatrix(Matrix a) : this(a, new Vector()) {}

		public AffineMatrix(Vector t) : this(Matrix.I, t) {}

		public AffineMatrix(Matrix a, Vector t)
		{
			this.a = a;
			this.t = t;
		}

		#endregion
		#region �v���p�e�B

		/// <summary>
		/// �A�t�B���ϊ���1���ϊ��s�񕔕��B
		/// </summary>
		public Matrix A
		{
			get{return this.a;}
			set{this.a = value;}
		}

		/// <summary>
		/// �A�t�B���ϊ��̕��s�ړ������B
		/// </summary>
		public Vector T
		{
			get{return this.t;}
			set{this.t = value;}
		}

		#endregion
		#region ���Z�q

		public static AffineMatrix operator+ (AffineMatrix a, AffineMatrix b)
		{
			return new AffineMatrix(a.a + b.a, a.t + b.t);
		}

		public static AffineMatrix operator- (AffineMatrix a, AffineMatrix b)
		{
			return new AffineMatrix(a.a - b.a, a.t - b.t);
		}

		public static AffineMatrix operator* (AffineMatrix a, AffineMatrix b)
		{
			return new AffineMatrix(a.a * b.a, a.a * b.t + a.t);
		}

		public static AffineMatrix operator/ (AffineMatrix a, AffineMatrix b)
		{
			return a * b.Inverse();
		}

		public static Vector operator* (AffineMatrix a, Vector v)
		{
			return a.a * v + a.t;
		}

		#endregion
		#region �t��

		public AffineMatrix Inverse()
		{
			Matrix ai = this.a.Inverse();
			return new AffineMatrix(ai, -(ai * this.t));
		}

		#endregion
	}
}
