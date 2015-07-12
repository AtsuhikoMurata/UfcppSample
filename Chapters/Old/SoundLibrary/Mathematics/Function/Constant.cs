using System;

namespace SoundLibrary.Mathematics.Function
{
	using ValueType = System.Double;

	/// <summary>
	/// �萔�B
	/// </summary>
	public class Constant : Function
	{
		#region �t�B�[���h

		/// <summary>
		/// �萔�̒l�B
		/// </summary>
		ValueType val;

		#endregion
		#region ������

		public Constant() : this(0){}

		public Constant(ValueType val)
		{
			this.val = val;
		}

		public static implicit operator Constant (ValueType val)
		{
			return new Constant(val);
		}

		#endregion
		#region �l

		public ValueType Value
		{
			get{return this.val;}
		}

		#endregion
		#region ���f���Ή�

		public override void GetComplexPart(out Function re, out Function im)
		{
			re = this;
			im = (Constant)0;
		}

		#endregion
		#region �l�̌v�Z

		public override System.Collections.ArrayList GetVariableList()
		{
			return new System.Collections.ArrayList();
		}

		public override ValueType GetValue(params Parameter[] x)
		{
			return this.val;
		}

		public override Function Bind(params Parameter[] x)
		{
			return this;
		}

		#endregion
		#region ���Z

		public override Function Negate()
		{
			return (Constant)(-this.val);
		}

		public override Function Add(Function f)
		{
			Constant c = f as Constant;

			if(c != null)
			{
				return (Constant)(this.val + c.val);
			}

			return f.Add(this);
		}

		public override Function Subtract(Function f)
		{
			Constant c = f as Constant;

			if(c != null)
			{
				return (Constant)(this.val - c.val);
			}

			return -f.Subtract(this);
		}

		public override Function Multiply(Function f)
		{
			Constant c = f as Constant;
			if(c != null)
			{
				return (Constant)(this.val * c.val);
			}

			Variable v = f as Variable;
			if(v != null)
			{
				return new Multiple(this.val, v);
			}

			return f.Multiply(this);
		}

		public override Function Divide(Function f)
		{
			Constant c = f as Constant;

			if(c != null)
			{
				return (Constant)(this.val / c.val);
			}

			return base.Divide(f);
		}

		#endregion
		#region ����

		public override Function Differentiate(Variable x)
		{
			return (Constant)0;
		}

		#endregion
		#region object

		public override string ToString()
		{
			return this.val.ToString();
		}

		public override bool Equals(object obj)
		{
			Constant c = obj as Constant;

			if(c == null)
			{
				return false;
			}

			return this.val == c.val;
		}

		public override int GetHashCode()
		{
			return this.val.GetHashCode();
		}

		#endregion
		#region ICloneable

		public override Function Clone()
		{
			return new Variable(this.val);
		}

		#endregion
	}
}
