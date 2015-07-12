using System;

namespace BitField
{
	/// <summary>
	/// BitField ����݂̗�O
	/// </summary>
	public class BitFieldException : System.Exception
	{
		/// <summary>
		/// �f�t�H���g�R���X�g���N�^�B
		/// </summary>
		public BitFieldException(){}

		/// <summary>
		/// ���b�Z�[�W�t���R���X�g���N�^�B
		/// </summary>
		/// <param name="msg">�G���[���b�Z�[�W</param>
		public BitFieldException(string msg) : base(msg){}
	}//class BitFieldException

	/// <summary>
	/// �r�b�g�t�B�[���h�N���X�B
	/// Verilog ���ۂ����삪�\�B
	/// 64�r�b�g�����E(�d�l�ł�)�B
	/// </summary>
	/// <example>
	/// reg [31:0] z;
	/// z &lt;= {z[30:1], z[31]};
	/// ��
	/// BitField z = BitField.Create(31, 0);
	/// z.Assign(BitField.Concat(z[30, 1], z[31])); 
	/// </example>
	/// <example>
	/// wire [3:0] a;
	/// assign a = 4'hA;
	/// ��
	/// BitField a = BitField.Create(3, 0);
	/// a.Assign(0xA);
	/// </example>
	abstract class BitField
	{
		/// <summary>
		/// MSB �� m�ALSB �� l �̃r�b�g�t�B�[���h���쐬�B
		/// verilog �� <c>wire [m:l] z;</c> �ɑ����B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>MSB �� m�ALSB �� l �̃r�b�g�t�B�[���h</returns>
		/// <exception cref="BitFieldException">
		/// m &lt; l �̂Ƃ������B
		/// </exception>
		public static BitField Create(int m, int l)
		{
			return new BitFieldImmediate(m, l);
		}

		/// <summary>
		/// MSB �� m�ALSB �� l �̃r�b�g�t�B�[���h���쐬�B
		/// verilog �� <c>wire [m:l] z;</c> �ɑ����B
		/// val �Œl������������B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <param name="val">�����l</param>
		/// <returns>MSB �� m�ALSB �� l�A�l�� val �̃r�b�g�t�B�[���h</returns>
		/// <exception cref="BitFieldException">
		/// m &lt; l �̂Ƃ������B
		/// </exception>
		public static BitField Create(int m, int l, ulong val)
		{
			return new BitFieldImmediate(m, l, val);
		}

		/// <summary>
		/// 1 �т��Ƃ̃r�b�g�t�B�[���h���쐬�B
		/// </summary>
		/// <param name="b">�r�b�g�̐^���l</param>
		/// <returns>1�r�b�g�̃r�b�g�t�B�[���h</returns>
		public static BitField Create(bool b)
		{
			return new BitFieldImmediate(0, 0, b ? 1UL : 0UL);
		}

		/// <summary>
		/// �i�[����Ă���l�� ulong �����ĕԂ��B
		/// </summary>
		/// <returns>�i�[����Ă���l</returns>
		internal abstract ulong GetValue();

		/// <summary>
		/// m�`l �r�b�g�ڂɊi�[����Ă���l�� ulong �����ĕԂ��B
		/// verilog �� <c>z[m:l]</c> �ɑ����B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>�i�[����Ă���l</returns>
		/// <exception cref="BitFieldException">
		/// m &lt; l �̂Ƃ������B
		/// </exception>
		internal abstract ulong GetValue(int m, int l);

		/// <summary>
		/// �l�����蓖�Ă�B
		/// </summary>
		/// <param name="val">���蓖�Ă����l(ulong)</param>
		public void Assign(ulong val)
		{
			BitField tmp = BitField.Create(this.Msb, this.Lsb, val);
			this.Assign(tmp);
		}

		/// <summary>
		/// �l�����蓖�Ă�B
		/// </summary>
		/// <param name="a">���蓖�Ă����l�̓������r�b�g�t�B�[���h�B</param>
		/// <exception cref="BitFieldException">
		/// this.Width != a.Width �̂Ƃ������B
		/// </exception>
		public abstract void Assign(BitField a);

		/// <summary>
		/// m�`l �r�b�g�ڂɒl�����蓖�Ă�B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <param name="a">���蓖�Ă����l�̓������r�b�g�t�B�[���h</param>
		/// <exception cref="BitFieldException">
		/// this.Sub(m, l).Width != a.Width �̂Ƃ������B
		/// </exception>
		public abstract void Assign(int m, int l, BitField a);

		/// <summary>
		/// �r�b�g���B
		/// </summary>
		public int Width
		{
			get{return this.Msb - this.Lsb + 1;}
		}

		/// <summary>
		/// MSB���擾�B
		/// </summary>
		public abstract int Msb
		{
			get;
		}

		/// <summary>
		/// LSB���擾�B
		/// </summary>
		public abstract int Lsb
		{
			get;
		}

		/// <summary>
		/// i �r�b�g�ڂ̒l��ǂݏ����B
		/// </summary>
		/// <exception cref="BitFieldException">
		/// i ���͈͊O�̂Ƃ������B
		/// </exception>
		public BitField this[int i]
		{
			set
			{
				this.Assign(i, i, value);
			}
			get
			{
				return this.Sub(i);
			}
		}

		/// <summary>
		/// m�`l �r�b�g�ڂ̒l��ǂݏ����B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <exception cref="BitFieldException">
		/// m, l  ���͈͊O�̂Ƃ��� m &lt; l �̂Ƃ������B
		/// </exception>
		public BitField this[int m, int l]
		{
			set
			{
				this.Assign(m, l, value);
			}
			get
			{
				return this.Sub(m, l);
			}
		}

		/// <summary>
		/// �i�[����Ă���l�� ulong �����ĕԂ��B
		/// </summary>
		public ulong Value
		{
			get
			{
				return GetValue();
			}
		}

		/// <summary>
		/// i �r�b�g�ڂ݂̂�؂�o���B
		/// verilog �� <c>z[i]</c> �ɑ����B
		/// <c>z.Sub(i)</c> ������������� <c>z</c> ���̂��̂�������������B
		/// </summary>
		/// <param name="i">�؂�o�������r�b�g�̃C���f�b�N�X</param>
		/// <returns>�؂�o���ꂽ�r�b�g�t�B�[���h</returns>
		/// <exception cref="BitFieldException">
		/// i ���͈͊O�̂Ƃ������B
		/// </exception>
		internal BitField Sub(int i)
		{
			return Sub(i, i);
		}

		/// <summary>
		/// m�`l �r�b�g�ڂ݂̂�؂�o���B
		/// verilog �� <c>z[m:l]</c> �ɑ����B
		/// <c>z.Sub(m, l)</c> ������������� <c>z</c> ���̂��̂�������������B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>�؂�o���ꂽ�r�b�g�t�B�[���h</returns>
		/// <exception cref="BitFieldException">
		/// m, l  ���͈͊O�̂Ƃ��� m &lt; l �̂Ƃ������B
		/// </exception>
		internal abstract BitField Sub(int m, int l);

		/// <summary>
		/// 2�̃r�b�g�t�B�[���h����������B
		/// verilog �� <c>{x, y, z}</c> �ɑ����B
		/// �����ł��Ȃ�����B
		/// </summary>
		/// <param name="a">�Ȃ������r�b�g�t�B�[���h</param>
		/// <returns>�Ȃ������r�b�g�t�B�[���h</returns>
		static public BitField Concat(params BitField[] a)
		{
			ulong val = 0L;
			int width = 0;

			foreach(BitField bf in a)
			{
				width += bf.Width;
				val <<= bf.Width;
				val |= bf.Value;
			}
			return new BitFieldImmediate(width - 1, 0, val);
		}

		/// <summary>
		/// �e�r�b�g�ɑ΂��� AND ���Z�B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <returns>�v�Z����</returns>
		/// <exception cref="BitFieldException">
		/// a.Width != b.Width �̂Ƃ������B
		/// </exception>
		static public BitField operator& (BitField a, BitField b)
		{
			if(a.Width != b.Width)
				throw new BitFieldException("width not match");

			ulong val = a.Value & b.Value;
			return new BitFieldImmediate(a.Msb, a.Lsb, val);
		}

		/// <summary>
		/// �e�r�b�g�ɑ΂��� OR ���Z�B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <returns>�v�Z����</returns>
		/// <exception cref="BitFieldException">
		/// a.Width != b.Width �̂Ƃ������B
		/// </exception>
		static public BitField operator| (BitField a, BitField b)
		{
			if(a.Width != b.Width)
				throw new BitFieldException("width not match");

			ulong val = a.Value | b.Value;
			return new BitFieldImmediate(a.Msb, a.Lsb, val);
		}

		/// <summary>
		/// �e�r�b�g�ɑ΂��� XOR ���Z�B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <returns>�v�Z����</returns>
		/// <exception cref="BitFieldException">
		/// a.Width != b.Width �̂Ƃ������B
		/// </exception>
		static public BitField operator^ (BitField a, BitField b)
		{
			if(a.Width != b.Width)
				throw new BitFieldException("width not match");

			ulong val = a.Value ^ b.Value;
			return new BitFieldImmediate(a.Msb, a.Lsb, val);
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <returns>�v�Z����</returns>
		static public BitField operator+ (BitField a, BitField b)
		{
			if(a.Width != b.Width)
				throw new BitFieldException("width not match");

			ulong val = a.Value + b.Value;
			return new BitFieldImmediate(a.Msb, a.Lsb, val);
		}

		/// <summary>
		/// ���Z�B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <returns>�v�Z����</returns>
		static public BitField operator- (BitField a, BitField b)
		{
			if(a.Width != b.Width)
				throw new BitFieldException("width not match");

			ulong val = a.Value - b.Value;
			return new BitFieldImmediate(a.Msb, a.Lsb, val);
		}

		/// <summary>
		/// �������]�B
		/// </summary>
		/// <param name="a">�I�y�����h</param>
		/// <returns>�v�Z����</returns>
		static public BitField operator- (BitField a)
		{//! Max
			ulong val = ~a.Value + 1;
			return new BitFieldImmediate(a.Msb, a.Lsb, val);
		}

		/// <summary>
		/// �␔�B
		/// </summary>
		/// <param name="a">�I�y�����h</param>
		/// <returns>�v�Z����</returns>
		static public BitField operator~ (BitField a)
		{//! Max
			ulong val = ~a.Value;
			return new BitFieldImmediate(a.Msb, a.Lsb, val);
		}

		/// <summary>
		/// �u�[���l����r�b�g�t�B�[���h�ɕϊ�
		/// </summary>
		/// <param name="b">�r�b�g�t�B�[���h�̐^���l</param>
		/// <returns>�r�b�g�t�B�[���h</returns>
		static public implicit operator BitField(bool b)
		{
			return BitField.Create(b);
		}

		/// <summary>
		/// �r�b�g�t�B�[���h����u�[���l�ɕϊ�
		/// </summary>
		/// <param name="bf">�r�b�g�t�B�[���h</param>
		/// <returns>�r�b�g�t�B�[���h�̐^�U</returns>
		static public implicit operator bool(BitField bf)
		{
			return bf.Value != 0;
		}

		static public bool operator true(BitField bf)
		{
			return bf.Value != 0;
		}

		static public bool operator false(BitField bf)
		{
			return bf.Value == 0;
		}

		/// <summary>
		/// �����񉻁B
		/// 0 �� 1 �̗���B
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = "";

			for(int i=this.Msb; i>=this.Lsb; --i)
			{
				str += this[i] ? "1" : "0";
			}

			return str;
		}
	}//class BitField

	/// <summary>
	/// �l�𒼐ڊi�[���Ă���r�b�g�t�B�[���h�B
	/// ���ʂ� <c>BitFiled.Create()</c> �� BitField �����Ƃ������ł���B
	/// </summary>
	internal class BitFieldImmediate : BitField
	{
		int msb; // MSB
		int lsb; // LSB
		ulong n; // �l

		/// <summary>
		/// MSB �� m�A LSB �� l �̃r�b�g�t�B�[���h���쐬�B
		/// </summary>
		/// <param name="msb">MSB</param>
		/// <param name="lsb">LSB</param>
		public BitFieldImmediate(int msb, int lsb) : this(msb, lsb, 0){}

		/// <summary>
		/// MSB �� m�A LSB �� l �̃r�b�g�t�B�[���h���쐬�B
		/// �l�̏��������s���B
		/// </summary>
		/// <param name="msb">MSB</param>
		/// <param name="lsb">LSB</param>
		/// <param name="n">�����l</param>
		public BitFieldImmediate(int msb, int lsb, ulong n)
		{
			if(this.msb < this.lsb)
			{
				throw new BitFieldException("msb must be greater than lsb");
			}

			this.msb = msb;
			this.lsb = lsb;
			this.n = n << lsb;
		}

		/// <summary>
		/// �i�[����Ă���l�� ulong �����ĕԂ��B
		/// </summary>
		/// <returns>�i�[����Ă���l</returns>
		internal override ulong GetValue()
		{
			return (this.n & this.GetMask(this.msb, this.lsb)) >> this.lsb;
		}

		/// <summary>
		/// m�`l �r�b�g�ڂɊi�[����Ă���l�� ulong �����ĕԂ��B
		/// verilog �� <c>z[m:l]</c> �ɑ����B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>�i�[����Ă���l</returns>
		/// <exception cref="BitFieldException">
		/// m &lt; l �̂Ƃ������B
		/// </exception>
		internal override ulong GetValue(int m, int l)
		{
			if(m < l || this.msb < m || this.lsb > l)
				throw new BitFieldException("illegal range");

			return (this.n & this.GetMask(m, l)) >> l;
		}

		/// <summary>
		/// �l�����蓖�Ă�B
		/// </summary>
		/// <param name="a">���蓖�Ă����l�̓������r�b�g�t�B�[���h�B</param>
		/// <exception cref="BitFieldException">
		/// this.Width != a.Width �̂Ƃ������B
		/// </exception>
		public override void Assign(BitField a)
		{
			if(a.Width != this.Width)
				throw new BitFieldException("width don't match");

			ulong val = a.Value;
			this.n = val << this.lsb;
		}

		/// <summary>
		/// m�`l �r�b�g�ڂɒl�����蓖�Ă�B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <param name="a">���蓖�Ă����l�̓������r�b�g�t�B�[���h</param>
		/// <exception cref="BitFieldException">
		/// this.Sub(m, l).Width != a.Width �̂Ƃ������B
		/// </exception>
		public override void Assign(int m, int l, BitField a)
		{
			if(a.Width != m-l+1)
				throw new BitFieldException("width don't match");

			if(m < l || this.msb < m || this.lsb > l)
				throw new BitFieldException("illegal range");

			ulong val = a.Value;
			ulong mask = this.GetMask(m, l);
			val <<= l;
			val &= mask;

			this.n = val | (this.n & ~mask);
		}

		/// <summary>
		/// MSB���擾�B
		/// </summary>
		public override int Msb
		{
			get
			{
				return this.msb;
			}
		}

		/// <summary>
		/// LSB���擾�B
		/// </summary>
		public override int Lsb
		{
			get
			{
				return this.lsb;
			}
		}

		/// <summary>
		/// m�`l �r�b�g�ڂ݂̂�؂�o���B
		/// verilog �� <c>z[m:l]</c> �ɑ����B
		/// <c>z.Sub(m, l)</c> ������������� <c>z</c> ���̂��̂�������������B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>�؂�o���ꂽ�r�b�g�t�B�[���h</returns>
		/// <exception cref="BitFieldException">
		/// m, l  ���͈͊O�̂Ƃ��� m &lt; l �̂Ƃ������B
		/// </exception>
		internal override BitField Sub(int m, int l)
		{
			if(m < l || this.msb < m || this.lsb > l)
				throw new BitFieldException("illegal range");
			
			return new BitFieldSub(m, l, this);
		}

		/// <summary>
		/// m�`l �r�b�g�ڂ̂� 1 �̃}�X�N�𐶐��B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>�}�X�N</returns>
		private ulong GetMask(int m, int l)
		{
			ulong mask = 0UL;

			for(int i=l; i<=m; ++i)
				mask |= 1UL << i;

			return mask;
		}
	}//class BitFieldImmediate

	/// <summary>
	/// <c>BitField.Sub</c> �Ŏ��o�������r�b�g�t�B�[���h�B
	/// </summary>
	internal class BitFieldSub : BitField
	{
		int msb;
		int lsb;
		BitField bf;

		/// <summary>
		/// bf �� msb�`lsb �ځB
		/// </summary>
		/// <param name="msb">MSB</param>
		/// <param name="lsb">LSB</param>
		/// <param name="bf">���ƂȂ�r�b�g�t�B�[���h</param>
		public BitFieldSub(int msb, int lsb, BitField bf)
		{
			this.msb = msb;
			this.lsb = lsb;
			this.bf = bf;
		}

		/// <summary>
		/// �i�[����Ă���l�� ulong �����ĕԂ��B
		/// </summary>
		/// <returns>�i�[����Ă���l</returns>
		internal override ulong GetValue()
		{
			return this.bf.GetValue(this.msb, this.lsb);
		}

		/// <summary>
		/// m�`l �r�b�g�ڂɊi�[����Ă���l�� ulong �����ĕԂ��B
		/// verilog �� <c>z[m:l]</c> �ɑ����B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>�i�[����Ă���l</returns>
		/// <exception cref="BitFieldException">
		/// m &lt; l �̂Ƃ������B
		/// </exception>
		internal override ulong GetValue(int m, int l)
		{
			if(m < l || this.msb < m || this.lsb > l)
				throw new BitFieldException("illegal range");

			return this.bf.GetValue(m, l);
		}

		/// <summary>
		/// �l�����蓖�Ă�B
		/// </summary>
		/// <param name="a">���蓖�Ă����l�̓������r�b�g�t�B�[���h�B</param>
		/// <exception cref="BitFieldException">
		/// this.Width != a.Width �̂Ƃ������B
		/// </exception>
		public override void Assign(BitField a)
		{
			if(a.Width != this.Width)
				throw new BitFieldException("width don't match");

			this.bf.Assign(this.msb, this.lsb, a);
		}

		/// <summary>
		/// m�`l �r�b�g�ڂɒl�����蓖�Ă�B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <param name="a">���蓖�Ă����l�̓������r�b�g�t�B�[���h</param>
		/// <exception cref="BitFieldException">
		/// this.Sub(m, l).Width != a.Width �̂Ƃ������B
		/// </exception>
		public override void Assign(int m, int l, BitField a)
		{
			if(a.Width != m-l+1)
				throw new BitFieldException("width don't match");

			if(m < l || this.msb < m || this.lsb > l)
				throw new BitFieldException("illegal range");

			this.bf.Assign(m, l, a);
		}

		/// <summary>
		/// MSB���擾�B
		/// </summary>
		public override int Msb
		{
			get
			{
				return this.msb;
			}
		}

		/// <summary>
		/// LSB���擾�B
		/// </summary>
		public override int Lsb
		{
			get
			{
				return this.lsb;
			}
		}

		/// <summary>
		/// m�`l �r�b�g�ڂ݂̂�؂�o���B
		/// verilog �� <c>z[m:l]</c> �ɑ����B
		/// <c>z.Sub(m, l)</c> ������������� <c>z</c> ���̂��̂�������������B
		/// </summary>
		/// <param name="m">MSB</param>
		/// <param name="l">LSB</param>
		/// <returns>�؂�o���ꂽ�r�b�g�t�B�[���h</returns>
		/// <exception cref="BitFieldException">
		/// m, l  ���͈͊O�̂Ƃ��� m &lt; l �̂Ƃ������B
		/// </exception>
		internal override BitField Sub(int m, int l)
		{
			if(m < l || this.msb < m || this.lsb > l)
				throw new BitFieldException("illegal range");
			
			return new BitFieldSub(m, l, this);
		}
	}//class BitFieldSub
}//namespace BitField
