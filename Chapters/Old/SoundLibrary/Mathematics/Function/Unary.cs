using System;

namespace SoundLibrary.Mathematics.Function
{
	using ValueType = System.Double;

	/// <summary>
	/// �P���֐��f���Q�[�g�B
	/// </summary>
	public delegate ValueType UnaryFunction(ValueType x);

	/// <summary>
	/// �P���֐��B
	/// </summary>
	public class Unary : Function
	{
		#region �t�B�[���h

		/// <summary>
		/// �֐��{�́B
		/// </summary>
		protected UnaryFunction func;

		/// <summary>
		/// �����֐��B
		/// this.GetValue(x) == func(inner.GetValue(x))
		/// </summary>
		protected Function inner;

		#endregion
		#region ������

		public Unary(UnaryFunction func, Function inner)
		{
			this.func = func;
			this.inner = inner;
		}

		#endregion
		#region �l�̌v�Z

		public override System.Collections.ArrayList GetVariableList()
		{
			return this.inner.GetVariableList();
		}

		public override ValueType GetValue(params Parameter[] x)
		{
			return this.func(this.inner.GetValue(x));
		}

		public override Function Bind(params Parameter[] x)
		{
			Function inner = this.inner.Bind(x);

			Constant c = inner as Constant;

			if(c != null)
			{
				return (Constant)this.func(c.Value);
			}

			Unary f = this.Clone() as Unary;
			f.inner = inner;
			return f;
		}

		#endregion
		#region ���f���Ή�

		/// <summary>
		/// �֐� f(z) �𕡑f�֐��Ƃ݂Ȃ����Ƃ��A���̎����Ƌ���
		/// Re[f](Re(z), Im(z)), Im[f](Re(z), Im(z) �����߂�B
		/// �Ⴆ�΁AExp �̏ꍇ�A
		/// reY = Exp(reX) * Cos(imX),
		/// imY = Exp(reX) * Sin(imX)
		/// </summary>
		/// <param name="reX">�p�����[�^�̎���</param>
		/// <param name="imX">�p�����[�^�̋���</param>
		/// <param name="reY">�֐��l�̎���</param>
		/// <param name="imY">�֐��l�̋���</param>
		protected virtual void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			throw new InvalidOperationException("���̊֐��͕��f�����Ή��ł��B");
		}

		public override void GetComplexPart(out Function re, out Function im)
		{
			this.inner.GetComplexPart(out re, out im);
			this.GetComplexPart(re, im, out re, out im);
		}

		#endregion
		#region ����

		/// <summary>
		/// this.func ���̂̓��֐������߂�B
		/// </summary>
		/// <returns>this.func �̓��֐��ɑ������� Function �^�C���X�^���X</returns>
		protected virtual Function Differentiate()
		{
			throw new InvalidOperationException("�����ł��܂���");
		}

		public override Function Differentiate(Variable x)
		{
			Function innerDeriv = this.inner.Differentiate(x);
			Function deriv = this.Differentiate();

			return innerDeriv * deriv;
		}

		#endregion
		#region �����\���̍œK��

		public override Function Optimize()
		{
			Function f = this.inner.Optimize();
			Unary u = (Unary)this.Clone();
			u.inner = f;
			return u;
		}

		#endregion
		#region object

		protected virtual string FunctionName()
		{
			return string.Empty;
		}

		public override string ToString()
		{
			return this.FunctionName() + "(" + this.inner.ToString() + ")";
		}

		public override bool Equals(object obj)
		{
			Unary f = obj as Unary;

			if(f == null)
			{
				return false;
			}

			return this.func.Equals(f.func) && this.inner.Equals(f.inner);
		}

		public override int GetHashCode()
		{
			return this.func.GetHashCode() ^ this.inner.GetHashCode();
		}

		#endregion
		#region ICloneable

		public override Function Clone()
		{
			return new Unary((UnaryFunction)this.func.Clone(), this.inner.Clone());
		}

		#endregion
	}
}
