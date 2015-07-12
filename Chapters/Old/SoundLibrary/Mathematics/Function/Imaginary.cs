using System;

namespace SoundLibrary.Mathematics.Function
{
	/// <summary>
	/// �����{�B
	/// �~i (i �͋����P��)�B
	/// GetValue (�����l�v�Z)�ɂ͓K�p����Ȃ��B
	/// </summary>
	public class Imaginary : Function
	{
		Function f;

		public Imaginary(Function f)
		{
			this.f = f;
		}

		public override System.Collections.ArrayList GetVariableList()
		{
			return this.f.GetVariableList();
		}

		public override Function Bind(params SoundLibrary.Mathematics.Function.Function.Parameter[] x)
		{
			return new Imaginary(this.f.Bind(x));
		}

		public override Double GetValue(params SoundLibrary.Mathematics.Function.Function.Parameter[] x)
		{
			return this.f.GetValue(x);
		}

		public override void GetComplexPart(out Function re, out Function im)
		{
			Function r, i;
			this.f.GetComplexPart(out r, out i);
			re = -i;
			im = r;
		}

		public override Function Clone()
		{
			return new Imaginary(this.f.Clone());
		}
	}
}
