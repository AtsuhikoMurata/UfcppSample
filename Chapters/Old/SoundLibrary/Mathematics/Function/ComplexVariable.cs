using System;

namespace SoundLibrary.Mathematics.Function
{
	/// <summary>
	/// ���f���ϐ��B
	/// </summary>
	public class ComplexVariable : Variable
	{
		public ComplexVariable(IComparable id) : base(id) {}

		#region ���f���Ή�

		/// <summary>
		/// �����B
		/// </summary>
		public Variable Re
		{
			get{return new Variable(new ComplexId(ComplexId.ReIm.Re, this.id));}
		}

		/// <summary>
		/// �����B
		/// </summary>
		public Variable Im
		{
			get{return new Variable(new ComplexId(ComplexId.ReIm.Im, this.id));}
		}

		public override void GetComplexPart(out Function re, out Function im)
		{
			re = this.Re;
			im = this.Im;
		}

		#endregion
	}

	/// <summary>
	/// �������ϐ��B
	/// </summary>
	public class ImaginaryVariable : Variable
	{
		public ImaginaryVariable(IComparable id) : base(id) {}

		#region ���f���Ή�

		public override void GetComplexPart(out Function re, out Function im)
		{
			re = (Constant)0;
			im = this;
		}

		#endregion
	}

	/// <summary>
	/// Variable �𕡑f���Ή������邽�߂̃N���X�B
	/// Variable x �� id 'x' �����Ƃ��A
	/// x.GetComplexPart(out re, out im); �̌��� re, im ��
	/// re �� id = ComplexId(Re, 'x'),
	/// im �� id = ComplexId(Im, 'x')
	/// �ɂ���B
	/// </summary>
	internal struct ComplexId : IComparable
	{
		#region �����^

		public enum ReIm
		{
			Re, Im
		}

		#endregion
		#region �t�B�[���h

		public ReIm ri;
		public IComparable id;

		#endregion
		#region ������

		public ComplexId(ReIm ri, IComparable id)
		{
			this.ri = ri;
			this.id = id;
		}

		#endregion
		#region object

		public override bool Equals(object obj)
		{
			ComplexId id = (ComplexId)obj;
			return this.ri == id.ri && this.id.Equals(id.id);
		}

		public override int GetHashCode()
		{
			return this.ri.GetHashCode() ^ this.id.GetHashCode();
		}

		public override string ToString()
		{
			if(this.ri == ReIm.Re)
			{
				return "Re(" + this.id.ToString() + ")";
			}
			else
			{
				return "Im(" + this.id.ToString() + ")";
			}
		}

		#endregion
		#region IComparable �����o

		public int CompareTo(object obj)
		{
			ComplexId id = (ComplexId)obj;

			int comp = this.id.CompareTo(id.id);

			if(comp != 0) return comp;
			
			return this.ri.CompareTo(id.ri);
		}

		#endregion
	}
}
