using System;

namespace SoundLibrary.Mathematics.Function.Elementary
{
	//! �w���֐��̊|���Z�Ƃ��A�ΐ��֐��̑����Z�Ƃ��A�œK���\�B
	//! �w���֐��̒��ɑΐ��֐��Ƃ����B

	#region �w���E�ΐ�

	/// <summary>
	/// �w���֐��B
	/// </summary>
	public class Exp : Unary
	{
		public Exp(Function f) : base(new UnaryFunction(Math.Exp), f)
		{
		}

		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			Function exp = Exp(reX);
			reY = exp * Cos(imX);
			imY = exp * Sin(imX);
		}

		public override Function Clone()
		{
			return new Exp(this.inner);
		}

		protected override Function Differentiate()
		{
			return this.Clone();
		}

		protected override string FunctionName()
		{
			return "exp";
		}
	}

	/// <summary>
	/// �ΐ��֐� log_e�B
	/// </summary>
	public class LogE : Unary
	{
		public LogE(Function f) : base(new UnaryFunction(Math.Log), f)
		{
		}

		/*
		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
		//! Arg ���K�v�ɂȂ邯�ǁA�ǂ����悤�H�����ł������Ȃ�����������B
		}
		*/

		public override Function Clone()
		{
			return new LogE(this.inner);
		}

		protected override Function Differentiate()
		{
			return (Constant)1 / this.inner;
		}

		protected override string FunctionName()
		{
			return "log";
		}
	}

	/// <summary>
	/// �ΐ��֐� log_10�B
	/// </summary>
	public class Log10 : Unary
	{
		public Log10(Function f) : base(new UnaryFunction(Math.Log10), f)
		{
		}

		public override Function Clone()
		{
			return new Log10(this.inner);
		}

		protected override Function Differentiate()
		{
			double tmp = 1/Math.Log(10); //! tmp �͒萔�ɂ��Ă����΂�����ƌ����I�B
			return (Constant)(tmp) / this.inner;
		}

		protected override string FunctionName()
		{
			return "log10";
		}
	}

	/// <summary>
	/// �ΐ��֐�(�C�ӂ̒�)�B
	/// </summary>
	public class Log : Binary
	{
		public Log(Function f1, Function f2) : base(new BinaryFunction(Math.Log), f1, f2)
		{
		}

		public override Function Clone()
		{
			return new Log(this.inner0, this.inner1);
		}

		protected override string FunctionName()
		{
			return "log";
		}
	}

	/// <summary>
	/// �w���֐�(�C�ӂ̒�)�B
	/// </summary>
	public class Pow : Binary
	{
		public Pow(Function f1, Function f2) : base(new BinaryFunction(Math.Pow), f1, f2)
		{
		}

		public override Function Clone()
		{
			return new Pow(this.inner0, this.inner1);
		}

		protected override string FunctionName()
		{
			return "pow";
		}
	}

	#endregion
	#region �O�p�֐�

	/// <summary>
	/// sin �֐��B
	/// </summary>
	public class Sin : Unary
	{
		public Sin(Function f) : base(new UnaryFunction(Math.Sin), f)
		{
		}

		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			reY = Sin(reX) * Cosh(imX);
			imY = Cos(reX) * Sinh(imX);
		}

		public override Function Clone()
		{
			return new Sin(this.inner);
		}

		protected override Function Differentiate()
		{
			return new Cos(this.inner);
		}

		protected override string FunctionName()
		{
			return "sin";
		}
	}

	/// <summary>
	/// cos �֐��B
	/// </summary>
	public class Cos : Unary
	{
		public Cos(Function f) : base(new UnaryFunction(Math.Cos), f)
		{
		}

		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			reY = Cos(reX) * Cosh(imX);
			imY = -Sin(reX) * Sinh(imX);
		}

		public override Function Clone()
		{
			return new Cos(this.inner);
		}

		protected override Function Differentiate()
		{
			return -(new Sin(this.inner));
		}

		protected override string FunctionName()
		{
			return "cos";
		}
	}

	/// <summary>
	/// tan �֐��B
	/// </summary>
	public class Tan : Unary
	{
		public Tan(Function f) : base(new UnaryFunction(Math.Tan), f)
		{
		}

		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			Function reN = Tan(reX);
			Function imN = Tanh(imX);
			Function imD = reN * imN;
			Function absD = 1 + imD * imD;

			reY = (reN - imN * imD) / absD;
			imY = (imN + reN * imD) / absD;
		}

		public override Function Clone()
		{
			return new Tan(this.inner);
		}

		protected override Function Differentiate()
		{
			Function cos = new Cos(this.inner);
			return (Constant)1 / (cos * cos);
		}

		protected override string FunctionName()
		{
			return "tan";
		}
	}

	#endregion
	#region �o�Ȋ֐�

	/// <summary>
	/// sinh �֐��B
	/// </summary>
	public class Sinh : Unary
	{
		public Sinh(Function f) : base(new UnaryFunction(Math.Sinh), f)
		{
		}

		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			reY = Sinh(reX) * Cos(imX);
			imY = Cosh(reX) * Sin(imX);
		}

		public override Function Clone()
		{
			return new Sinh(this.inner);
		}

		protected override Function Differentiate()
		{
			return new Cosh(this.inner);
		}

		protected override string FunctionName()
		{
			return "sinh";
		}
	}

	/// <summary>
	/// cosh �֐��B
	/// </summary>
	public class Cosh : Unary
	{
		public Cosh(Function f) : base(new UnaryFunction(Math.Cosh), f)
		{
		}

		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			reY = Cosh(reX) * Cos(imX);
			imY = Sinh(reX) * Sin(imX);
		}

		public override Function Clone()
		{
			return new Cosh(this.inner);
		}

		protected override Function Differentiate()
		{
			return new Sinh(this.inner);
		}

		protected override string FunctionName()
		{
			return "cosh";
		}
	}

	/// <summary>
	/// tanh �֐��B
	/// </summary>
	public class Tanh : Unary
	{
		public Tanh(Function f) : base(new UnaryFunction(Math.Tanh), f)
		{
		}

		protected override void GetComplexPart(Function reX, Function imX, out Function reY, out Function imY)
		{
			Function reN = Tanh(reX);
			Function imN = Tan(imX);
			Function imD = reN * imN;
			Function absD = 1 + imD * imD;

			reY = (reN + imN * imD) / absD;
			imY = (imN - reN * imD) / absD;
		}

		public override Function Clone()
		{
			return new Tanh(this.inner);
		}

		protected override Function Differentiate()
		{
			Function cosh = new Cosh(this.inner);
			return (Constant)1 / (cosh * cosh);
		}

		protected override string FunctionName()
		{
			return "tanh";
		}
	}

	#endregion
	#region ���̑�

	/// <summary>
	/// �������B
	/// </summary>
	public class Sqrt : Unary
	{
		public Sqrt(Function f) : base(new UnaryFunction(Math.Sqrt), f)
		{
		}

		public override Function Clone()
		{
			return new Sqrt(this.inner);
		}

		protected override Function Differentiate()
		{
			return (Constant)1 / ((Constant)2 * new Sqrt(this.inner));
		}

		protected override string FunctionName()
		{
			return "sqrt";
		}
	}

	#endregion
}
