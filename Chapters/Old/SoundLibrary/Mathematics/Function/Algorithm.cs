using System;

namespace SoundLibrary.Mathematics.Function
{
	using ValueType = System.Double;

	/// <summary>
	/// Algorithm �̊T�v�̐����ł��B
	/// </summary>
	/* static */
	public class Algorithm
	{
		/// <summary>
		/// ���l�ϕ���T���̂��߂̕ϐ��̕ω����B
		/// �̈� [min, max] �̊Ԃ����ݕ� step �ŒT������B
		/// </summary>
		public class Range
		{
			public Variable variable;
			public ValueType min;
			public ValueType max;
			public ValueType step;

			/// <summary>
			/// �ő�l�A�ŏ��l�ƁA���ݕ����w�肵�ď������B
			/// </summary>
			/// <param name="variable">�ϐ�</param>
			/// <param name="min">�ŏ��l</param>
			/// <param name="max">�ő�l</param>
			/// <param name="step">���ݕ�</param>
			public Range(Variable variable, ValueType min, ValueType max, ValueType step)
			{
				this.variable = variable;
				this.min = min;
				this.max = max;
				this.step = step;
			}

			/// <summary>
			/// ���������w�肵�ď������B
			/// </summary>
			/// <param name="variable">�ϐ�</param>
			/// <param name="min">�ŏ��l</param>
			/// <param name="max">�ő�l</param>
			/// <param name="n">������</param>
			public Range(Variable variable, ValueType min, ValueType max, int n)
			{
				this.variable = variable;
				this.min = min;
				this.max = max;
				this.step = (max - min) / (ValueType)n;
			}
		}

		/// <summary>
		/// �w�肵���͈͓�/���ݕ��ŁA�֐� f �̒l���ŏ��ɂȂ�悤�Ȉ�����T���B
		/// (������)
		/// </summary>
		/// <param name="f">�֐� f</param>
		/// <param name="rangeList">�T���͈�/���ݕ�</param>
		/// <returns>argmin f</returns>
		public static VariableTable Argmin(Function f, params Range[] rangeList)
		{
			VariableTable vars = new VariableTable();
			foreach(Range range in rangeList)
			{
				vars[range.variable] = range.min;
			}

			ValueType min = ValueType.MaxValue;
			VariableTable argmin = null;

			for(;;)
			{
				ValueType val = f[vars];
				if(val < min)
				{
					min = val;
					argmin = vars.Clone();
				}

				int i=0;
				for(; i<rangeList.Length; ++i)
				{
					ValueType x = vars[rangeList[i].variable];
					x += rangeList[i].step;

					if(x > rangeList[i].max)
					{
						vars[rangeList[i].variable] = rangeList[i].min;
					}
					else
					{
						vars[rangeList[i].variable] = x;
						break;
					}
				}
				if(i >= rangeList.Length) break;
			}

			return argmin;
		}

		/// <summary>
		/// ���l�ϕ��B(��`�������g�p�B)
		/// </summary>
		/// <param name="f">��ϕ��֐�</param>
		/// <param name="range">�ϕ��͈�</param>
		/// <returns>���l�ϕ�����</returns>
		public static Function Integral(Function f, Range range)
		{
			Variable x = range.variable;
			ValueType min = range.min;
			ValueType max = range.max;
			ValueType step = range.step;

			Function g = f.Bind(new Function.Parameter(x, min));
			g += f.Bind(new Function.Parameter(x, max));
			g /= 2;

			for(; min < max; min += step)
				g += f.Bind(new Function.Parameter(x, min));

			g *= step;
			return g;
		}
	}
}
