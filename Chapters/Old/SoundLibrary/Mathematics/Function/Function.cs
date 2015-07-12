using System;
using System.Collections;

namespace SoundLibrary.Mathematics.Function
{
	using ValueType = System.Double;

	/// <summary>
	/// �֐���\���N���X�̒��ۊ��B
	/// </summary>
	public abstract class Function : ICloneable
	{
		#region �t�B�[���h

		//ArrayList variables = new ArrayList();

		#endregion
		#region �����N���X

		public class Parameter
		{
			public Variable  x;
			public ValueType val;

			public Parameter(Variable x, ValueType val)
			{
				this.x = x;
				this.val = val;
			}

			public override bool Equals(object obj)
			{
				Parameter v = (Parameter)obj;
				return this.x.Equals(v.x) && this.val.Equals(v.val);
			}

			public override int GetHashCode()
			{
				return this.x.GetHashCode() ^ this.val.GetHashCode();
			}
		}

		#endregion
		#region �l�̌v�Z

		public VariableTable GetVariableTable()
		{
			return new VariableTable(this.GetVariableList());
		}

		/// <summary>
		/// ���̊֐��Ɋ܂܂�Ă���ϐ��̃��X�g�����߂�B
		/// �Ⴆ�΁Af(g(x, y), h(x)) + i(y, z) �Ƃ������悤�ɁA
		/// �֐��̍����E�l�����Z���g���č�����֐��̏ꍇ�A{x, y, z} �Ƃ������X�g��Ԃ��B
		/// </summary>
		/// <returns></returns>
		public abstract System.Collections.ArrayList GetVariableList();

		/// <summary>
		/// �l�̌v�Z�B
		/// </summary>
		public virtual ValueType this[VariableTable t]
		{
			get
			{
				return this.GetValue(t.GetParameterList());
			}
		}

		public abstract ValueType GetValue(params Parameter[] x);

		/// <summary>
		/// �l���Œ肷��B
		/// �Ⴆ�΁Af(x, y) ���Ă����֐����������Ƃ��āA
		/// y = 1 �ŌŒ肵���֐� g(x) = f(x, 1) �����߂���B
		/// </summary>
		/// <param name="x">�Œ肵�����l�̃��X�g</param>
		/// <returns>�l�Œ��̊֐�</returns>
		public abstract Function Bind(params Parameter[] x);

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
		public virtual void GetComplexPart(out Function re, out Function im)
		{
			throw new InvalidOperationException("���̊֐��͕��f�����Ή��ł��B");
		}

		/// <summary>
		/// ���f�֐��̃m�����֐� |f(z)|^2 �����߂�B
		/// </summary>
		/// <returns>�m����</returns>
		public Function Norm()
		{
			Function re, im;
			this.GetComplexPart(out re, out im);
			return re * re + im * im;
		}

		#endregion
		#region ���Z

		/// <summary>
		/// �������]�B
		/// </summary>
		/// <returns>�������]����</returns>
		public virtual Function Negate()
		{
			return this.Multiply((Constant)(-1.0));
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="f">�I�y�����h</param>
		/// <returns>���Z����</returns>
		public virtual Function Add(Function f)
		{
			return new Sum(this, f);
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="f">�I�y�����h</param>
		/// <returns>���Z����</returns>
		public virtual Function Subtract(Function f)
		{
			return this.Add(f.Negate());
		}

		/// <summary>
		/// ��Z�B
		/// </summary>
		/// <param name="f">�I�y�����h</param>
		/// <returns>��Z����</returns>
		public virtual Function Multiply(Function f)
		{
			Constant c = f as Constant;
			if(c != null)
			{
				return new Multiple(c.Value, this);
			}

			return new Product(this, f);
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="f">�I�y�����h</param>
		/// <returns>���Z����</returns>
		public virtual Function Divide(Function f)
		{
			Constant c = f as Constant;
			if(c != null)
			{
				return new Multiple(1 / c.Value, this);
			}

			return Fraction.Create(this, f);
		}

		#endregion
		#region ���Z�q

		public static Function operator+ (Function f)
		{
			return f.Clone();
		}

		public static Function operator- (Function f)
		{
			return f.Negate();
		}

		public static Function operator+ (Function f, Function g)
		{
			if(f.Equals((Constant)0))
				return g;
			if(g.Equals((Constant)0))
				return f;
			if(f.Equals(g))
				return new Multiple(2, f);

			return f.Add(g);
		}

		public static Function operator- (Function f, Function g)
		{
			if(f.Equals(g))
				return (Constant)0;
			return f.Subtract(g);
		}

		public static Function operator* (Function f, Function g)
		{
			if(f.Equals((Constant)0) || g.Equals((Constant)0))
				return (Constant)0;
			if(f.Equals((Constant)1))
				return g;
			if(g.Equals((Constant)1))
				return f;
//			if(f.Equals(g))
//				return Function.X(f, 2);

			return f.Multiply(g);
		}

		public static Function operator/ (Function f, Function g)
		{
			if(f.Equals(g))
				return (Constant)1;
			return f.Divide(g);
		}

		public static implicit operator Function (double x)
		{
			return (Constant)x;
		}

		#endregion
		#region ����

		/// <summary>
		/// (x �ɂ���)���֐������߂�B
		/// ��͓I�Ɍv�Z�B
		/// ��͓I�Ɍv�Z�ł��Ȃ��֐��̏ꍇ�AInvalidOperationException �� throw ����B
		/// </summary>
		/// <param name="x">�����ΏۂƂȂ�ϐ�</param>
		/// <returns>���֐�</returns>
		public virtual Function Differentiate(Variable x)
		{
			throw new InvalidOperationException("�����ł��܂���");
		}

		#endregion
		#region �����\���̍œK��

		/// <summary>
		/// �����\�����œK������B
		/// �Ⴆ�΁A1�~f �� f, 0�~f �� 0�B
		/// </summary>
		/// <returns>�œK����̊֐�</returns>
		/// <remarks>
		/// �ꔭ��100�������ȍœK�����|����킯�ł͂Ȃ��B
		/// 2�E3�x�J��Ԃ����ƂŁA���œK����������ꍇ������B
		/// </remarks>
		public virtual Function Optimize()
		{
			return this;
		}

		/// <summary>
		/// �����\�����œK������B
		/// </summary>
		/// <param name="n">������</param>
		/// <returns>�œK����̊֐�</returns>
		/// <remarks>
		/// �ꔭ�Ŋ��S�ȍœK�����o���Ȃ��̂ŁA���x���������J��Ԃ��B
		/// </remarks>
		public Function Optimize(int n)
		{
			Function opt = this.Optimize();
			for(; n>0; --n) opt = opt.Optimize();
			return opt;
		}

		#endregion
		#region ICloneable �����o

		public abstract Function Clone();

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
		#region ����Ȋ֐�

		#region �P���֐�

		public static Unary Exp(Function f)
		{
			return new Elementary.Exp(f);
		}

		public static Unary Log(Function f)
		{
			return new Elementary.LogE(f);
		}

		public static Unary Log10(Function f)
		{
			return new Elementary.Log10(f);
		}

		public static Unary Sin(Function f)
		{
			return new Elementary.Sin(f);
		}

		public static Unary Cos(Function f)
		{
			return new Elementary.Cos(f);
		}

		public static Unary Tan(Function f)
		{
			return new Elementary.Tan(f);
		}

		public static Unary Asin(Function f)
		{
			return new Unary(
				new UnaryFunction(Math.Asin),
				f);
		}

		public static Unary Acos(Function f)
		{
			return new Unary(
				new UnaryFunction(Math.Acos),
				f);
		}

		public static Unary Atan(Function f)
		{
			return new Unary(
				new UnaryFunction(Math.Atan),
				f);
		}

		public static Unary Sinh(Function f)
		{
			return new Elementary.Sinh(f);
		}

		public static Unary Cosh(Function f)
		{
			return new Elementary.Cosh(f);
		}

		public static Unary Tanh(Function f)
		{
			return new Elementary.Tanh(f);
		}

		public static Unary Abs(Function f)
		{
			return new Unary(
				new UnaryFunction(Math.Abs),
				f);
		}

		public static Unary Sqrt(Function f)
		{
			return new Elementary.Sqrt(f);
		}

		#endregion
		#region �񍀊֐�

		public static Binary Pow(Function f, Function g)
		{
			return new Elementary.Pow(f, g);
		}

		public static Binary Log(Function f, Function g)
		{
			return new Elementary.Log(f, g);
		}

		public static Binary Atan2(Function f, Function g)
		{
			return new Binary(
				new BinaryFunction(Math.Atan2),
				f, g);
		}

		#endregion
		#region ������

		public static Unary X(Function f)
		{
			return X(f, 1, 1);
		}

		public static Unary X(Function f, int order)
		{
			return X(f, order, 1);
		}

		public static Unary X(Function f, int order, double coef)
		{
			return new Elementary.Polynomial(f, Mathematics.Expression.Polynomial.X(order, coef));
		}

		#endregion
		#region ����

		public static Function I(Function f)
		{
			return new Imaginary(f);
		}

		#endregion

		#endregion
	}
}
