using System;

namespace SoundLibrary.Filter
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

			this.Clear();
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

			if(this.buff == null)
				this.buff = new CircularBuffer(a.Length);
			else if(this.buff.Length < a.Length)
				this.buff.Resize(a.Length);

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

		public object Clone()
		{
			return new IirFilter(this.a, this.b);
		}
	}//class IirFilter
}//namespace Filter
