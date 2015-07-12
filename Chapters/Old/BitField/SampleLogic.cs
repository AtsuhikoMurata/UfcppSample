using System;

namespace BitField
{
	/// <summary>
	/// �r�b�g�t�B�[���h�N���X�̃T���v�����W���[���W�B
	/// �揜�Z���BCD�̃o�C�i���ϊ���B
	/// </summary>
	class SampleLogic
	{
		/// <summary>
		/// �揜�Z��B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <param name="mul">true �̂Ƃ���Z�A false �̂Ƃ����Z</param>
		/// <returns>���Z����(��Z�̏ꍇ�A����32�r�b�g�ɏ��A���32�r�b�g�ɏ�])</returns>
		public static BitField MulDiv(BitField a, BitField b, bool mul)
		{
			bool sgn = a[a.Msb] ^ b[b.Msb];
			BitField p = BitField.Create(63, 0);
			BitField q = BitField.Create(31, 0);

			if(a[a.Msb]) a = Negate(a);
			if(b[b.Msb]) b = Negate(b);

			p.Assign(BitField.Concat(BitField.Create(31, 0, 0), a));
			q.Assign(b);

			for(int i=0; i<32; ++i)
			{
				BitField addin1, addin2, addout;
				addin1 = mul ? p[63, 32] : p[62, 31];
				addin2 = mul | p[63] ? q : Negate(q);
				addout = addin1 + addin2;

				if(mul)
					if(p[0])
						p.Assign(BitField.Concat(BitField.Create(0, 0, 0), addout, p[31, 1]));
					else
						p.Assign(BitField.Concat(BitField.Create(0, 0, 0), p[63, 1]));
				else
					p.Assign(BitField.Concat(addout, p[30, 0], ~addout[addout.Msb]));
			}
			if(!mul & !p[0])
			{
				BitField tmp = q + p[63,32];
				p.Assign(BitField.Concat(tmp, p[31, 0]));
			}

			if(sgn) p = Negate(p);

			return p;
		}

		/// <summary>
		/// ��Z��B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <returns>���Z����</returns>
		public static BitField Mul(BitField a, BitField b)
		{
			bool sgn = a[a.Msb] ^ b[b.Msb];
			BitField p = BitField.Create(63, 0);
			BitField q = BitField.Create(31, 0);

			if(a[a.Msb]) a = Negate(a);
			if(b[b.Msb]) b = Negate(b);

			p.Assign(BitField.Concat(BitField.Create(31, 0, 0), a));
			q.Assign(b);

			for(int i=0; i<32; ++i)
			{
				BitField addin1, addin2, addout;

				addin1 = p[63, 32];
				addin2 = q;
				addout = addin1 + addin2;

				if(p[0])
					p.Assign(BitField.Concat(BitField.Create(0, 0, 0), addout, p[31, 1]));
				else
					p.Assign(BitField.Concat(BitField.Create(0, 0, 0), p[63, 1]));
			}

			if(sgn) p = Negate(p);

			return p;
		}

		/// <summary>
		/// ���Z��B
		/// </summary>
		/// <param name="a">�I�y�����h1</param>
		/// <param name="b">�I�y�����h2</param>
		/// <returns>���Z����(����32�r�b�g�ɏ��A���32�r�b�g�ɏ�])</returns>
		public static BitField Div(BitField a, BitField b)
		{
			bool sgn = a[a.Msb] ^ b[b.Msb];
			BitField p = BitField.Create(63, 0);
			BitField q = BitField.Create(31, 0);

			if(a[a.Msb]) a = Negate(a);
			if(b[b.Msb]) b = Negate(b);

			p.Assign(BitField.Concat(BitField.Create(31, 0, 0), a));
			q.Assign(b);

			for(int i=0; i<32; ++i)
			{
				BitField addin1, addin2, addout;

				addin1 = p[62, 31];
				addin2 = p[63] ? q : Negate(q);
				addout = addin1 + addin2;

				p.Assign(BitField.Concat(addout, p[30, 0], ~addout[addout.Msb]));
			}
			if(!p[0])
			{
				BitField tmp = q + p[63,32];
				p.Assign(BitField.Concat(tmp, p[31, 0]));
			}

			if(sgn) p = Negate(p);

			return p;
		}

		/// <summary>
		/// �������]��B
		/// </summary>
		/// <param name="a">�I�y�����h</param>
		/// <returns>���Z����</returns>
		public static BitField Negate(BitField a)
		{
			BitField tmp = BitField.Create(a.Msb, a.Lsb);
			tmp.Assign(~a.Value + 1);
			return tmp;
		}

		/// <summary>
		/// BCD �� Binary �ϊ�
		/// </summary>
		/// <param name="bcd">�ϊ���</param>
		/// <returns>�ϊ�����</returns>
		public static BitField BcdToBin(BitField bcd)
		{
			BitField bin = BitField.Concat(BitField.Create(0, 0, 0), bcd, BitField.Create(29, 0, 0));

			for(int i=0; i<30; ++i)
			{
				BitField a0 = BcdToBinAddIn(bin[62, 59]);
				BitField a1 = BcdToBinAddIn(bin[58, 55]);
				BitField a2 = BcdToBinAddIn(bin[54, 51]);
				BitField a3 = BcdToBinAddIn(bin[50, 47]);
				BitField a4 = BcdToBinAddIn(bin[46, 43]);
				BitField a5 = BcdToBinAddIn(bin[42, 39]);
				BitField a6 = BcdToBinAddIn(bin[38, 35]);
				BitField a7 = BcdToBinAddIn(bin[34, 31]);
				BitField add_in = BitField.Concat(a0, a1, a2, a3, a4, a5, a6, a7);
				BitField add_out = bin[62, 31] + add_in + BitField.Create(31, 0, 1);

				bin.Assign(BitField.Concat(BitField.Create(0, 0, 0), add_out, bin[30, 1]));
			}

			return bin[31, 0];
		}

		/// <summary>
		/// BCD ���o�C�i���ϊ��̕␳�l���߂�B
		/// </summary>
		/// <param name="a">1��(4�r�b�g)BCD</param>
		/// <returns>�␳�l</returns>
		static private BitField BcdToBinAddIn(BitField a)
		{
			return BitField.Create(3, 0, a.Value >= 8 ? 0xCUL : 0xFUL);
		}

		/// <summary>
		/// Binary �� BCD �ϊ�
		/// </summary>
		/// <param name="bin">�ϊ���</param>
		/// <param name="overflow">�I�[�o�[�t���[���N������true�ɃZ�b�g�����B</param>
		/// <returns>�ϊ�����</returns>
		public static BitField BinToBcd(BitField bin, out bool overflow)
		{
			BitField bcd = BitField.Concat(BitField.Create(30, 0, 0), bin);

			overflow = false;
			for(int i=0; i<30; ++i)
			{
				BitField a0 = BinToBcdAddIn(bcd[61, 58]);
				BitField a1 = BinToBcdAddIn(bcd[57, 54]);
				BitField a2 = BinToBcdAddIn(bcd[53, 50]);
				BitField a3 = BinToBcdAddIn(bcd[49, 46]);
				BitField a4 = BinToBcdAddIn(bcd[45, 42]);
				BitField a5 = BinToBcdAddIn(bcd[41, 38]);
				BitField a6 = BinToBcdAddIn(bcd[37, 34]);
				BitField a7 = BinToBcdAddIn(bcd[33, 30]);
				BitField add_in = BitField.Concat(a0, a1, a2, a3, a4, a5, a6, a7);
				BitField add_out = bcd[61, 30] + add_in;


				bcd.Assign(BitField.Concat(add_out, bcd[29, 0], BitField.Create(0, 0, 0)));
				overflow |= add_out[add_out.Msb];
			}

			return bcd[61, 30];
		}

		/// <summary>
		/// �o�C�i����BCD �ϊ��̕␳�l���߂�B
		/// </summary>
		/// <param name="a">4�r�b�g�o�C�i��</param>
		/// <returns>�␳�l</returns>
		static private BitField BinToBcdAddIn(BitField a)
		{
			return BitField.Create(3, 0, a.Value >= 5 ? 0x3UL : 0x0UL);
		}
	}//class SampleLogic
}//namespace BitField
