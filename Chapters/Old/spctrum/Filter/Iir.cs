using System;

namespace Filter
{
	/// <summary>
	/// IIR �t�B���^�N���X�B
	/// </summary>
	public class IirFilter : IFilter
	{
		double[] a; // ����W���z��
		double[] b; // ���q�W���z��
		CircularBuffer buff; // �x���o�b�t�@

		/// <summary>
		/// �f�t�H���g�R���X�g���N�^�B
		/// </summary>
		public IirFilter() : this(null, null) {}

		/// <summary>
		/// �������w�肵�� IIR �����B
		/// </summary>
		/// <param name="order">IIR �̎���</param>
		public IirFilter(int order) : this(new double[order], new double[order + 1]) {}

		/// <summary>
		/// �W�����w�肵�� IIR �����B
		/// ��肽�� IIR �̓`�B�֐����A
		///      ��_0^N B_i z^i
		/// Y = ---------------- X
		///      ��_0^N A_i z^i
		/// �ł���Ƃ��A
		/// a[i] = - A_(i+1) / A_0    (i = 1�`N)
		/// b[i] = B_i / A_0        (i = 0�`N)
		/// </summary>
		/// <param name="a">����W���z��</param>
		/// <param name="b">���q�W���z��</param>
		public IirFilter(double[] a, double[] b)
		{
			this.SetCoefficient(a, b);
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// N: �t�B���^���� (= this.a.Length = this.b.Length - 1)
		/// x: ����
		/// t: ���ԃf�[�^
		/// y: �o��
		/// a[i]: ����W���z��
		/// b[i]: ���q�W���z��
		/// d[i]: i+1 �T���v���O�� t �̒l
		/// �Ƃ���ƁA
		/// t = x + ��_{i=0}^{N-1} a[i]d[i]
		/// y = t * b[0] + ��_{i=0}^{N} b[i+1]d[i]
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			int N = this.a.Length;
			double t = x;
			for(int i=0; i<N; ++i)
			{
				t += this.buff[i] * this.a[i];
			}
			double y = t * this.b[0];
			for(int i=0; i<N; ++i)
			{
				y += this.buff[i] * this.b[i+1];
			}
			this.buff.PushFront(t);
			return y;
		}

		/// <summary>
		/// ������Ԃ̃N���A
		/// </summary>
		public void Clear()
		{
			for(int i=0; i<this.buff.Length; ++i)
			{
				this.buff[i] = 0;
			}
		}

		/// <summary>
		/// �W���̐ݒ�
		/// </summary>
		/// <param name="a">����W���z��</param>
		/// <param name="b">���q�W���z��</param>
		public void SetCoefficient(double[] a, double[]b)
		{
			if(a == null || b == null ||
				a.Length + 1 != b.Length)
			{
				this.buff = null;
				return;
			}

			this.buff = new CircularBuffer(a.Length);
			this.a = a;
			this.b = b;
		}

		/// <summary>
		/// ����W���z��̎擾
		/// </summary>
		public double[] A
		{
			get{return this.a;}
		}

		/// <summary>
		/// ����W���z��̎擾
		/// </summary>
		public double[] B
		{
			get{return this.b;}
		}
	}//class IirFilter

	/// <summary>
	/// 2��IIR��p�����s�[�L���O�C�R���C�U�B
	/// </summary>
	public class PeakingEqualizer : IirFilter
	{
		/// <summary>
		/// �s�[�L���O�C�R���C�U���쐬�B
		/// </summary>
		/// <param name="w">���S���g��</param>
		/// <param name="Q">Q�l</param>
		/// <param name="A">������(���j�A�l)</param>
		public PeakingEqualizer(double w, double Q, double A) : base(2)
		{
#if true
			double a = Math.Sqrt(A);
			double sn = Math.Sin(w);
			double cs = Math.Cos(w);
			double alpha = sn / (2 * Q);
			double a0 = 1 + alpha / a;
			double a1 = -2 * cs;
			double a2 = 1 - alpha / a;
			double b0 = 1 + alpha * a;
			double b1 = -2 * cs;
			double b2 = 1 - alpha * a;

			this.A[0] = -a1 / a0;
			this.A[1] = -a2 / a0;
			this.B[0] = b0 / a0;
			this.B[1] = b1 / a0;
			this.B[2] = b2 / a0;
#else
			double g = A;
			double Ft = 1 / Math.Tan(w / 2);

			double term1 = 1.0 + Ft*g/Q + Ft*Ft;
			double term2 = 1.0 + Ft/Q + Ft*Ft;

			//Peek�����
			if(A > 1)
			{
				this.B[0] = term1 / term2;
				this.B[1] = 2.0*(1.0 - Ft*Ft)/term2;
				this.B[2] = (1.0 - Ft*g/Q + Ft*Ft) / term2;
				this.A[0] = - this.B[1];
				this.A[1] = - (1.0 - Ft/Q + Ft*Ft)/ term2;
			}
			//dip�����
			else if(A < 1)
			{
				this.B[0] = term2 / term1;
				this.B[1] = 2.0*(1.0 - Ft*Ft)/term1;
				this.B[2] = (1.0 - Ft / Q + Ft*Ft) / term1;
				this.A[0] = - this.B[1];
				this.A[1] = - (1.0 - Ft*g/Q + Ft*Ft)/ term1;
			}
			else
			{
				this.B[0] = 1.0;
				this.B[1] = 0.0;
				this.B[2] = 0.0;
				this.A[0] = 0.0; 
				this.A[1] = 0.0;   
			}
#endif
		}
	}//class PeakingEqualizer


	/// <summary>
	/// 2��IIR��p�����s�[�L���O�C�R���C�U�B
	/// </summary>
	public class ShelvingEqualizer : IirFilter
	{
		/// <summary>
		/// �s�[�L���O�C�R���C�U���쐬�B
		/// </summary>
		/// <param name="w">���S���g��</param>
		/// <param name="A">������(���j�A�l)</param>
		public ShelvingEqualizer(double w, double A) : base(1)
		{
			double tn = (Math.Sin(w) - 1) / Math.Cos(w);
			double g = 1/A;//Math.Sqrt(A);

			double term1 = g*(1.0+tn) - (1.0-tn);
			double term2 = g*(1.0+tn) + (1.0-tn);

			if(g > 1)
			{
				this.A[0]= term1/term2;
				this.B[0]= 2.0/term2;
				this.B[1]= tn;
			}
			else if(g < 1)
			{
				this.A[0]= tn;
				this.B[0]= term2/2.0;
				this.B[1]= term1/term2; 
			}
			else
			{
				this.A[0]= 0.0;
				this.B[0]= 1.0;
				this.B[1]= 0.0;
			}
		}
	}//class ShelvingEqualizer
}//namespace Filter
