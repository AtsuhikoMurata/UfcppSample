using System;

using SoundLibrary.Mathematics;

namespace SoundLibrary.Filter.Equalizer
{
	/// <summary>
	/// ��(��_�܂��͋�)�B
	/// </summary>
	/// <remarks>
	/// �M�������̕���ł́A�t�B���^�̗�/�ɂ����𕡑f���̏ꍇ�ł������W���t�B���^�Ŏ����ł���悤�ɁA
	/// �t�B���^��2�����ɋ�؂��Ď������邱�Ƃ������̂ŁA����2���y�A�ɂ��ĕ\���B
	/// 	���̃^�C�v �c
	/// 		�P���A�����~2(�d���܂�)�A���𕡑f�����A�Ȃ�(�萔���̂�)�B
	/// 	a, b �c
	/// 		�P���̏ꍇ�Aa �ɒl���Bb �͖����B
	/// 		�����̏ꍇ�Aa, b �ɂ��ꂼ��̍��̒l���B
	/// 		���𕡑f���Aa �Ɏ����Ab �ɋ����B
	/// </remarks>
	public class Root : ICloneable
	{
		#region �C���i�[�N���X

		/// <summary>
		/// ���̃^�C�v
		/// </summary>
		public enum Type
		{
			None,    // ���Ȃ�(�萔���̂�)
			Real,    // �������~2
			Single,  // �������~1
			Complex, // ���𕡑f����
		}

		#endregion
		#region public �t�B�[���h

		public Type type;
		public double a;
		public double b;

		#endregion
		#region ������

		public Root() : this(Type.Complex, 0, 0) {}

		public Root(Type type, double a, double b)
		{
			this.type = type;
			this.a = a;
			this.b = b;
		}

		#endregion
		#region ���̎��o��

		/// <summary>
		/// �����Ƃ������f���ɂ��ĉ��̒l��Ԃ��B
		/// </summary>
		/// <param name="x1">��1</param>
		/// <param name="x2">��2(�������~1�̏ꍇ�ɂ�0��)</param>
		/// <returns>���̐�(0 �` 2)</returns>
		public int GetRoots(out Complex x1, out Complex x2)
		{
			switch(this.type)
			{
				case Type.None:
					x1 = 0;
					x2 = 0;
					return 0;
				case Type.Real:
					x1 = this.a;
					x2 = this.b;
					return 2;
				case Type.Single:
					x1 = this.a;
					x2 = 0;
					return 1;
				default:
					x1 = new Complex(this.a, this.b);
					x2 = new Complex(this.a, -this.b);
					return 2;
			}
		}

		#endregion
		#region ICloneable �����o

		public Root Clone()
		{
			return new Root(this.type, this.a, this.b);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
	}

	/// <summary>
	/// ��_�Ƌɂ̃y�A�B
	/// </summary>
	/// <remarks>
	/// IIR �t�B���^��z��B
	/// 2����IIR�t�B���^����_�Ƌɂ�1�y�A���B
	/// </remarks>
	public class ZeroPole : ICloneable
	{
		#region public �t�B�[���h

		public Root zero;
		public Root pole;

		#endregion
		#region ������

		public ZeroPole() : this(new Root(), new Root()) {}

		public ZeroPole(Root zero, Root pole)
		{
			this.zero = zero;
			this.pole = pole;
		}

		#endregion
		#region ICloneable �����o

		public ZeroPole Clone()
		{
			return new ZeroPole(this.zero, this.pole);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
	}

	/// <summary>
	/// �t�B���^�W���B
	/// �A�i���O�v���g�^�C�v/�f�B�W�^�����p�B
	/// </summary>
	/// <remarks>
	/// �A�i���O  : ��b[i]s^i  / ��a[i]s^i
	/// �f�B�W�^��: ��b[i]z^-i / ��a[i]z^-i
	/// �o�ꎟ�ϊ��ł͕���/���q�̎������ς��Ȃ����Ƃ𗘗p���āA
	/// �A�i���O�E�f�B�W�^���ŌW���N���X���g���܂킷�B
	/// </remarks>
	public class Coefficient : ICloneable
	{
		#region public �t�B�[���h

		public double[] a = new double[3];
		public double[] b = new double[3];

		#endregion
		#region ������

		public Coefficient() : this(1, 0, 0, 1, 0, 0) {}

		public Coefficient(double a0, double a1, double a2, double b0, double b1, double b2)
		{
			this.a[0] = a0;
			this.a[1] = a1;
			this.a[2] = a2;
			this.b[0] = b0;
			this.b[1] = b1;
			this.b[2] = b2;
		}

		#endregion
		#region ICloneable �����o

		public Coefficient Clone()
		{
			return new Coefficient(
				this.a[0], this.a[1], this.a[2],
				this.b[0], this.b[1], this.b[2]);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
	}
}
