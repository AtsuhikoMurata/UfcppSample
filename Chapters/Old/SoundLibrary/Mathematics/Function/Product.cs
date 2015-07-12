using System;
using System.Collections;

namespace SoundLibrary.Mathematics.Function
{
	using ValueType = System.Double;

	/// <summary>
	/// �֐��̐ρB
	/// </summary>
	public class Product : Series
	{
		#region ������

		public Product(params Function[] functions) : base(functions) {}
		public Product(ArrayList functions) : base(functions) {}

		#endregion
		#region �l�̌v�Z

		public override System.Collections.ArrayList GetVariableList()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			foreach(Function f in this.functions)
			{
				list = Misc.Merge(list, f.GetVariableList());
			}
			return list;
		}

		public override ValueType GetValue(params Parameter[] x)
		{
			ValueType y = 1;

			foreach(Function f in this.functions)
			{
				y *= f.GetValue(x);
			}

			return y;
		}

		public override Function Bind(params Parameter[] x)
		{
			Function y = (Constant)1;

			foreach(Function f in this.functions)
			{
				Function g = f.Bind(x);
				y *= g;
			}

			return y;
		}

		#endregion
		#region ���f���Ή�

		public override void GetComplexPart(out Function re, out Function im)
		{
			Function f;

			f = this.functions[0] as Function;
			f.GetComplexPart(out re, out im);

			for(int i=1; i<this.functions.Count; ++i)
			{
				Function re1, im1;

				f = this.functions[i] as Function;
				f.GetComplexPart(out re1, out im1);

				Function re0 = re;
				re = re0 * re1 - im * im1;
				im = re0 * im1 + im * re1;
			}
		}

		#endregion
		#region ���Z

		public override Function Multiply(Function f)
		{
			Product h = this.Clone() as Product;

			if(f is Product)
			{
				Product g = f as Product;
				h.AddList(g.functions);
			}
			else
			{
				h.AddList(f);
			}
			// f �� 0 �̂Ƃ��̍œK���B

			return h;
		}

		#endregion
		#region ����

		public override Function Differentiate(Variable x)
		{
			ArrayList func;
			Function sum = (Constant)0;

			foreach(Function f in this.functions)
			{
				func = (ArrayList)this.functions.Clone();
				func.Remove(f);
				func.Add(f.Differentiate(x));
				sum += new Product(func);
			}

			return sum;
		}

		#endregion
		#region �����\���̍œK��

		static Function Sort(Function f)
		{
			Product p = f as Product;
			if(p == null) return f;
			p.functions.Sort();
			return p;
		}

		/// <remarks>
		/// ������ނ̊֐����ƂɊ|���Ă����������œK�ȍ\���������邽�߁A
		/// �֐��̃��X�g����x�\�[�g���Ă���|���Ȃ����B
		/// ���ƁAConstant �Ƃ� Variable �Ƃ� Function �̕ӂ�̏�Z���œK���B
		/// </remarks>
		public override Function Optimize()
		{
			Hashtable table = new Hashtable();

			// ��ނ킯
			foreach(Function f in this.functions)
			{
				Function g = f.Optimize();

				Type t = g.GetType();
				if(table[t] == null)
				{
					table[t] = g;
				}
				else
				{
					table[t] = (Function)table[t] * g;
				}
			}

			// �q���Ȃ���
			Constant c = table[typeof(Constant)] as Constant;

			if(c != null && c.Equals((Constant)0))
				return (Constant)0;

			Function v = table[typeof(Variable)] as Function;
			Multiple m = table[typeof(Multiple)] as Multiple;

			Function h = (Constant)1;
			if(v != null)
			{
				h = Sort(v);
				table.Remove(typeof(Variable));
			}
			if(c != null)
			{
				table.Remove(typeof(Constant));
			}
			if(m != null)
			{
				if(c == null)
					c = (Constant)m.Factor;
				else
					c = (Constant)(c.Value * m.Factor);
				h *= m.Inner;
				table.Remove(typeof(Multiple));
			}

			ArrayList func = new ArrayList();

			if(!h.Equals((Constant)1))
			{
				func.Add(h);
			}

			foreach(Function f in table.Values)
			{
				if(f is Product)
				{
					Product p = f as Product;
					func.AddRange(p.functions);
				}
				else
				{
					if(!f.Equals((Constant)1))
						func.Add(f);
				}
			}

			// ����ȏꍇ
			if(c == null)
			{
				if(func.Count == 0)
					return (Constant)1;

				if(func.Count == 1)
					return func[0] as Function;
			}

			// �|���Ȃ����B
			h = func[0] as Function;
			for(int i=1; i<func.Count; ++i)
			{
				h *= func[i] as Function;
			}

			if(c == null || c.Equals((Constant)1))
				return h;
			return new Multiple(c.Value, h);
		}

		#endregion
		#region object

		public override bool Equals(object obj)
		{
			if(!(obj is Product)) return false;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			int code = base.GetHashCode();
			code ^= 45872368;
			return code;
		}

		public override string ToString()
		{
			string str = "";
#if DEBUG
			str = "p";
#endif
			str += this.functions[0].ToString();

			for(int i=1; i<this.functions.Count; ++i)
			{
				string tmp = this.functions[i].ToString();
				if(tmp[0] == '-' || tmp.IndexOf('+') >= 0)
				{
					str += " * (" + tmp + ")";
				}
				else
				{
					str += " * " + tmp;
				}
			}

			return str;
		}

		#endregion
		#region ICloneable

		public override Function Clone()
		{
			return new Product((ArrayList)this.functions.Clone());
		}

		#endregion
	}
}
