using System;

namespace SoundLibrary.Filter.Equalizer
{
	using Function = SoundLibrary.Mathematics.Function.Function;
	using Cached = SoundLibrary.Mathematics.Function.CachedFunction;

	/// <summary>
	/// �p���C�R�� IIR �t�B���^�݌v�N���X�B
	/// </summary>
	/// <remarks>
	/// �t�B���^�̗�/�Ɍv�Z�B
	/// ��/�� �� �A�i���O�v���g�^�C�v�t�B���^�W���v�Z�B
	/// AP �t�B���^ ��[�o�ꎟ�ϊ�]�� �f�B�W�^���t�B���^���B
	/// �f�B�W�^���t�B���^�W���� ParametricEqualizer �N���X�̌`���ɕϊ��B
	/// 
	/// ��/��                 : AP �t�B���^�̗�/�ɁB���𕡑f�����A�����~2�A�������͎����~1�B
	/// AP �t�B���^�W��       : a[3], b[3]�B��b[i]s^i / ��a[i]s^i�B
	/// �f�B�W�^���t�B���^�W��: a[3], b[3]�B��b[i]z^-i / ��a[i]z^-i�B
	/// �p���C�R�N���X�̌W��  : a[2], b[2], c�Bc * (1 + ��b[i]z^-i) / (1 - ��a[i]z^-i)�B
	/// </remarks>
	public abstract class FilterDesigner
	{
		#region �t�B�[���h

		protected int order;

		#endregion
		#region ������

		protected FilterDesigner() : this(0) {}
		protected FilterDesigner(int order){this.order = order;}

		#endregion
		#region �v���p�e�B

		/// <summary>
		/// �����B
		/// </summary>
		public int Order
		{
			get{return this.order;}
			set{this.order = value;}
		}

		/// <summary>
		/// ��/�Ƀy�A�̐��B
		/// </summary>
		public virtual int Length
		{
			get{return (this.order + 1) / 2;}
		}

		#endregion
		#region ���ۃ��\�b�h

		/// <summary>
		/// �t�B���^�̗�_/�ɂ��v�Z�B
		/// </summary>
		/// <param name="roots">��_/�Ɉꗗ�̊i�[��</param>
		public abstract void GetZeroPole(ZeroPole[] roots);

		#endregion
		#region ��_/�ɂ̌v�Z

		/// <summary>
		/// �t�B���^�̗�_/�ɂ��v�Z�B
		/// ���ʊi�[�p�̔z����֐����Ŋm�ہB
		/// </summary>
		/// <returns>�t�B���^�̗�_/�Ɉꗗ</returns>
		public virtual ZeroPole[] GetZeroPole()
		{
			ZeroPole[] roots = new ZeroPole[this.Length];
			for(int i=0; i<roots.Length; ++i) roots[i] = new ZeroPole();

			this.GetZeroPole(roots);
			return roots;
		}

		#endregion
		#region �A�i���O�v���g�^�C�v�t�B���^�W���݌v

		/// <summary>
		/// �A�i���O�v���g�^�C�v�t�B���^�̌W�����v�Z�B
		/// </summary>
		/// <returns>AP �t�B���^�W��</returns>
		public Coefficient[] GetAnalogPrototype()
		{
			Coefficient[] coefs = new Coefficient[this.Length];
			for(int i=0; i<coefs.Length; ++i) coefs[i] = new Coefficient();

			this.GetAnalogPrototype(coefs);

			return coefs;
		}

		/// <summary>
		/// �A�i���O�v���g�^�C�v�t�B���^�̌W�����v�Z�B
		/// </summary>
		/// <param name="coefs">�v�Z���ʂ̊i�[��</param>
		public virtual void GetAnalogPrototype(Coefficient[] coefs)
		{
			ZeroPole[] roots = this.GetZeroPole();
			ZeroPoleToAnalogPrototype(roots, coefs);
		}

		#endregion
		#region �f�B�W�^���t�B���^�W���݌v

		/// <summary>
		/// �f�B�W�^�� LPF �W�����v�Z�B
		/// </summary>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <returns>�f�B�W�^�� LPF �W��</returns>
		public Coefficient[] GetDigitalLPF(double w)
		{
			Coefficient[] coefs = new Coefficient[this.Length];
			for(int i=0; i<coefs.Length; ++i) coefs[i] = new Coefficient();

			this.GetDigitalLPF(w, coefs);

			return coefs;
		}

		/// <summary>
		/// �f�B�W�^�� LPF �W�����v�Z�B
		/// </summary>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="coefs">�v�Z���ʂ̊i�[��</param>
		public virtual void GetDigitalLPF(double w, Coefficient[] coefs)
		{
			this.GetAnalogPrototype(coefs);
			BilinearTransform(coefs, coefs, w);
		}

		#endregion
		#region PEQ �W��

		/// <summary>
		/// PEQ �W��(LPF)���v�Z�B
		/// </summary>
		/// <param name="w"></param>
		/// <returns></returns>
		public virtual ParametricEqualizer.Parameter[] GetLPF(double w)
		{
			ParametricEqualizer.Parameter[] peq = new ParametricEqualizer.Parameter[this.Length];
			for(int i=0; i<peq.Length; ++i) peq[i] = new ParametricEqualizer.Parameter();

			ToPeqCoefficient(this.GetDigitalLPF(w), peq);
			return peq;
		}

		#endregion
		#region �ϊ�
		#region ��_/�ɔz�u���A�i���O�v���g�^�C�v�t�B���^�W��

		public static void RootToAnalogPrototype(Root root, double[] c)
		{
			switch(root.type)
			{
				case Root.Type.Complex:
					c[2] = 1;
					c[1] = -2 * root.a;
					c[0] = root.a * root.a + root.b * root.b;
					break;
				case Root.Type.Real:
					c[2] = 1;
					c[1] = -(root.a + root.b);
					c[0] = root.a * root.b;
					break;
				case Root.Type.None:
					c[2] = 0;
					c[1] = 0;
					c[0] = 1;
					break;
				default:
					c[2] = 0;
					c[1] = 1;
					c[0] = -root.a;
          break;
			}
		}

		public static void ZeroPoleToAnalogPrototype(ZeroPole zeropole, Coefficient coef)
		{
			RootToAnalogPrototype(zeropole.zero, coef.b);
			RootToAnalogPrototype(zeropole.pole, coef.a);
		}

		public static void ZeroPoleToAnalogPrototype(ZeroPole[] roots, Coefficient[] coefs)
		{
			for(int i=0; i<roots.Length; ++i)
			{
				ZeroPoleToAnalogPrototype(roots[i], coefs[i]);
			}
		}

		#endregion
		#region �o�ꎟ�ϊ�

		/// <summary>
		/// 1���̓`�B�֐���o1���ϊ�����B
		/// </summary>
		/// <remarks>
		/// (b0 + b1 s)/(a0 + a1 s) ���A�o1���ϊ��������ʂ�
		/// (b0' + b1' z^-1)/(a0' + a1' z^-1)�Ƃ���Ƃ��A
		/// a0, a1 �� a0', a1' �����߂�B
		/// (b �Ɋւ��Ă����l�̎菇�ŕϊ��\�B)
		/// </remarks>
		/// <param name="a0">a0(a0'�̒l�ɏ㏑�������)</param>
		/// <param name="a1">a1(a1'�̒l�ɏ㏑�������)</param>
		/// <param name="sin">sin ��s</param>
		/// <param name="cos">cos ��s</param>
		public static void BilinearTransform(
			double a0, double a1,
			out double d0, out double d1,
			double sin, double cos)
		{
			a0 = a0 * sin;
			a1 = a1 * (1 + cos);

			d0 = a0 + a1;
			d1 = a0 - a1;
		}

		/// <summary>
		/// 2���̓`�B�֐���o1���ϊ�����B
		/// </summary>
		/// <remarks>
		/// (b0 + b1 s + b2 s^2)/(a0 + a1 s + a2 s^2) ���A�o1���ϊ��������ʂ�
		/// (b0' + b1' z^-1 + b2' z^-2)/(a0' + a1' z^-1 + a2' z^-2)�Ƃ���Ƃ��A
		/// a0, a1, a2 �� a0', a1', a2' �����߂�B
		/// (b �Ɋւ��Ă����l�̎菇�ŕϊ��\�B)
		/// </remarks>
		/// <param name="a0">a0(a0'�̒l�ɏ㏑�������)</param>
		/// <param name="a1">a1(a1'�̒l�ɏ㏑�������)</param>
		/// <param name="a2">a2(a2'�̒l�ɏ㏑�������)</param>
		/// <param name="sin">sin ��s</param>
		/// <param name="cos">cos ��s</param>
		public static void BilinearTransform(
			double a0, double a1, double a2,
			out double d0, out double d1, out double d2,
			double sin, double cos)
		{
			a0 = a0 * (1 - cos);
			a1 = a1 * sin;
			a2 = a2 * (1 + cos);

			d0 = d2 = a0 + a2;
			d1 = 2 * (a0 - a2);

			d0 += a1;
			d2 -= a1;
		}

		public static void BilinearTransform(Coefficient ap, Coefficient digital, double sin, double cos)
		{
			if(ap.a[2] != 0 || ap.b[2] != 0)
			{
				BilinearTransform(
					ap.a[0], ap.a[1], ap.a[2],
					out digital.a[0], out digital.a[1], out digital.a[2],
					sin, cos);
				BilinearTransform(
					ap.b[0], ap.b[1], ap.b[2],
					out digital.b[0], out digital.b[1], out digital.b[2],
					sin, cos);
				return;
			}

			digital.a[2] = 0;
			digital.b[2] = 0;

			if(ap.a[1] != 0 || ap.b[1] != 0)
			{
				BilinearTransform(
					ap.a[0], ap.a[1],
					out digital.a[0], out digital.a[1],
					sin, cos);
				BilinearTransform(
					ap.b[0], ap.b[1],
					out digital.b[0], out digital.b[1],
					sin, cos);
				return;
			}

			digital.a[1] = 0;
			digital.b[1] = 0;

			digital.a[0] = ap.a[0];
			digital.b[0] = ap.b[0];
		}

		public static void BilinearTransform(Coefficient[] ap, Coefficient[] digital, double w)
		{
			double sin, cos;
			GetSinCos(w, out sin, out cos);

			for(int i=0; i<ap.Length; ++i)
			{
				BilinearTransform(ap[i], digital[i], sin, cos);
			}
		}

		protected static void GetSinCos(double w, out double sin, out double cos)
		{
			cos = Math.Cos(w);
			sin = Math.Sqrt(1 - cos * cos); //Math.Sin(w);
		}

		#endregion
		#region �f�B�W�^���t�B���^�W����PEQ�̌W��

		public static void ToPeqCoefficient(Coefficient digital, ParametricEqualizer.Parameter peq)
		{
			double[] a = digital.a;
			double[] b = digital.b;

			peq.c  =  b[0] / a[0];
			peq.a1 = -a[1] / a[0];
			peq.a2 = -a[2] / a[0];
			peq.b1 =  b[1] / b[0];
			peq.b2 =  b[2] / b[0];
		}

		public static void ToPeqCoefficient(Coefficient[] digital, ParametricEqualizer.Parameter[] peq)
		{
			for(int i=0; i<digital.Length; ++i)
			{
				ToPeqCoefficient(digital[i], peq[i]);
			}
		}

		#endregion
		#endregion
		#region Function �N���X�Ƃ̘A�g

		/// <summary>
		/// z^-1 = exp(-j��)
		/// </summary>
		/// <param name="w">���g����</param>
		/// <returns>z^-1</returns>
		public static Function ZInv(Function w)
		{
			return new Cached(
				Function.Exp(-Function.I(w))
				);
		}

		/// <summary>
		/// �o1���ϊ�
		/// s = 1/tan(��s/2) * (z^-1 + 1) / (z^-1 - 1)
		///   = j tan(��/2) / tan(��s/2)
		/// </summary>
		/// <param name="w">��</param>
		/// <param name="ws">��s</param>
		/// <returns>�f�B�W�^�� s</returns>
		public static Function DigitalS(Function w, Function ws)
		{
			return new Cached(
				Function.I(
				Function.Tan(w / 2) / Function.Tan(ws / 2)
				));
		}

		/// <summary>
		/// �o1���ϊ�
		/// s = 1/tan(��s/2) * (z^-1 + 1) / (z^-1 - 1)
		///   = j tan(��/2) / tan(��s/2)
		/// </summary>
		/// <param name="w">��</param>
		/// <param name="ws">��s</param>
		/// <returns>�f�B�W�^�� s</returns>
		public static Function DigitalS(Function w, double ws)
		{
			return new Cached(
				Function.I(
				Function.Tan(w / 2) / Math.Tan(ws / 2)
				));
		}

		/// <summary>
		/// �o1���ϊ�(��s����/2)
		/// s = (z^-1 + 1) / (z^-1 - 1)
		///   = j tan(��/2)
		/// </summary>
		/// <param name="w">��</param>
		/// <returns>�f�B�W�^�� s</returns>
		public static Function DigitalS(Function w)
		{
			return new Cached(
				Function.I(
				Function.Tan(w / 2)
				));
		}

		/// <summary>
		/// �A�i���O�`�B�֐����擾�B
		/// </summary>
		/// <param name="s">s �̈�ϐ�</param>
		/// <returns>�`�B�֐�</returns>
		public Function GetTransferFunction(Function s)
		{
			Coefficient[] coefs = this.GetAnalogPrototype();
			Function f = (Function)1;

			foreach(Coefficient coef in coefs)
			{
				f *=
					(coef.b[0] + coef.b[1] * s + coef.b[2] * s * s) /
					(coef.a[0] + coef.a[1] * s + coef.a[2] * s * s);
			}

			return f;
		}

		/// <summary>
		/// �f�B�W�^���`�B�֐��̎��g���������擾�B
		/// </summary>
		/// <param name="w">���g���ϐ���(���K���p���g��)</param>
		/// <param name="ws">�J�b�g�I�t���g����s(���K���p���g��)</param>
		/// <returns>���g������</returns>
		public Function GetTransferFunction(Function w, Function ws)
		{
			return this.GetTransferFunction(DigitalS(w, ws));
		}

		#endregion
	}
}
