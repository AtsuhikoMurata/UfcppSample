using System;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// �W���z��̃v���L�V�B
	/// </summary>
	public interface IFirCoefficient : System.Collections.IEnumerable
	{
		/// <summary>
		/// �W���̐����擾�B
		/// </summary>
		int Count{get;}

		/// <summary>
		/// i �Ԗڂ̌W�����擾�B
		/// </summary>
		double this[int i]{get; set;}
	}

	/// <summary>
	/// FIR �t�B���^�C���^�[�t�F�[�X�B
	/// </summary>
	public interface IFirFilter : IFilter
	{
		/// <summary>
		/// �W���z����擾�B
		/// </summary>
		IFirCoefficient Coefficient{get; set;}
	}

	/// <summary>
	/// FIR �t�B���^�N���X�B
	/// </summary>
	public class FirFilter : IFirFilter
	{
		protected double[] coef; // �W���z��
		protected CircularBuffer buff; // �x���o�b�t�@

		/// <summary>
		/// �f�t�H���g�R���X�g���N�^
		/// </summary>
		public FirFilter() : this(null){}

		/// <summary>
		/// �^�b�v�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="taps">�^�b�v��</param>
		public FirFilter(int taps) : this(new double[taps]){}

		/// <summary>
		/// �W�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="coef">�W�����i�[�����z��B</param>
		public FirFilter(double[] coef)
		{
			this.Coefficient = coef;

			this.Clear();
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// N: �t�B���^���� (= this.coef.Length - 1)
		/// x: ����
		/// y: �o��
		/// c[i]: �W���z��
		/// d[i]: i+1 �T���v���O�� x �̒l
		/// �Ƃ���ƁA
		/// y = x*c[N] + ��_{i=0}^{N-1} d[i]*c[N-1-i]
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			this.buff.PushFront(x);
			int N = this.coef.Length;
			double y = 0;
			for(int i=0; i<N; ++i)
			{
				y += this.buff[i] * this.coef[i];
			}
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

		#region IFirFilter �����o

		public class CoefficientProxy : IFirCoefficient
		{
			internal double[] x;

			public CoefficientProxy(double[] x){this.x = x;}
			public static implicit operator CoefficientProxy(double[] x){return new CoefficientProxy(x);}

			#region IFirCoefficient �����o

			public int Count
			{
				get{return this.x.Length;}
			}

			public double this[int i]
			{
				get
				{
					return this.x[i];
				}
				set
				{
					this.x[i] = value;
				}
			}

			#endregion
			#region IEnumerable �����o

			internal class Enumerator : System.Collections.IEnumerator
			{
				IFirCoefficient x;
				int current;

				public Enumerator(IFirCoefficient x){this.x = x; this.current = -1;}

				#region IEnumerator �����o

				public void Reset(){this.current = -1;}

				public object Current{get{return this.x[this.current];}}

				public bool MoveNext()
				{
					++this.current;
					return this.current < this.x.Count;
				}

				#endregion
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new Enumerator(this);
			}

			#endregion
		}

		IFirCoefficient IFirFilter.Coefficient
		{
			set{this.Coefficient = ((CoefficientProxy)value).x;}
			get{return new CoefficientProxy(this.Coefficient);}
		}

		/// <summary>
		/// �W���̎擾/�ݒ�
		/// </summary>
		public double[] Coefficient
		{
			set
			{
				this.coef = value;
				if(value == null)
					this.buff = null;
				else if(this.buff == null)
					this.buff = new CircularBuffer(this.coef.Length);
				else if(this.buff.Length < this.coef.Length)
					this.buff.Resize(this.coef.Length);
			}
			get
			{
				return this.coef;
			}
		}

		#endregion

		public object Clone()
		{
			return new FirFilter((double[])this.coef.Clone());
		}
	}//class FirFilter

	/// <summary>
	/// ���`�ʑ� FIR �t�B���^�N���X�B
	/// �W�������Ώۂł��邱�Ƃ𗘗p���Čv�Z��/�������ʍ팸�B
	/// ��^�b�v�o�[�W����
	/// (�^�b�v�� 2n - 1 �ŁAcoef[n-1-i] == coef[n-1+i] foreach i)
	/// </summary>
	public class OddLinearFir : IFirFilter
	{
		protected double[] coef;
		protected CircularBuffer buff;

		/// <summary>
		/// �f�t�H���g�R���X�g���N�^
		/// </summary>
		public OddLinearFir() : this(null){}

		/// <summary>
		/// �^�b�v�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="n">�^�b�v�� �� 2n - 1</param>
		public OddLinearFir(int n) : this(new double[n]){}

		/// <summary>
		/// �W�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="coef">�W�����i�[�����z��B</param>
		public OddLinearFir(double[] coef)
		{
			this.Coefficient = coef;

			this.Clear();
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// n: �^�b�v���� 2n + 1 (n = this.coef.Length - 1)
		/// x: ����
		/// y: �o��
		/// c[i]: �W���z��
		/// d[i]: i+1 �T���v���O�� x �̒l
		/// �Ƃ���ƁA
		/// y = d[n]*c[n] + ��_{i=1}^{n} (d[n+i] + d[n-i])*c[i]
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			this.buff.PushFront(x);

			int n = this.coef.Length - 1;
			double y = this.coef[n] * this.buff[n];
			for(int i=0; i<n; ++i)
			{
				y += this.coef[i] * (this.buff[i] + this.buff[2*n-i]);
			}
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

		#region IFirFilter �����o

		public class CoefficientProxy : IFirCoefficient
		{
			internal double[] x;

			public CoefficientProxy(double[] x){this.x = x;}
			public static implicit operator CoefficientProxy(double[] x){return new CoefficientProxy(x);}

			#region IFirCoefficient �����o

			public int Count
			{
				get{return 2 * this.x.Length - 1;}
			}

			public double this[int i]
			{
				get
				{
					if(i < this.x.Length)
						return this.x[i];
					else
						return this.x[2 * (this.x.Length - 1) - i];
				}
				set
				{
					if(i < this.x.Length)
						this.x[i] = value;
					else
						this.x[2 * (this.x.Length - 1) - i] = value;
				}
			}

			#endregion
			#region IEnumerable �����o

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new FirFilter.CoefficientProxy.Enumerator(this);
			}

			#endregion
		}

		IFirCoefficient IFirFilter.Coefficient
		{
			set{this.Coefficient = ((CoefficientProxy)value).x;}
			get{return new CoefficientProxy(this.Coefficient);}
		}

		/// <summary>
		/// �W���̎擾/�ݒ�
		/// </summary>
		public double[] Coefficient
		{
			set
			{
				this.coef = value;
				if(value == null)
					this.buff = null;
				else if(this.buff == null)
					this.buff = new CircularBuffer(2 * this.coef.Length - 1);
				else if(this.buff.Length < 2 * this.coef.Length - 1)
					this.buff.Resize(2 * this.coef.Length - 1);
			}
			get
			{
				return this.coef;
			}
		}

		#endregion

		public object Clone()
		{
			return new OddLinearFir((double[])this.coef.Clone());
		}
	}//class OddLinearFir

	/// <summary>
	/// ���`�ʑ� FIR �t�B���^�N���X�B
	/// �W�������Ώۂł��邱�Ƃ𗘗p���Čv�Z��/�������ʍ팸�B
	/// �����^�b�v�o�[�W����
	/// (�^�b�v�� 2n �ŁAcoef[n-1-i] == coef[n+i] foreach i)
	/// </summary>
	public class EvenLinearFir : IFirFilter
	{
		protected double[] coef;
		protected CircularBuffer buff;

		/// <summary>
		/// �f�t�H���g�R���X�g���N�^
		/// </summary>
		public EvenLinearFir() : this(null){}

		/// <summary>
		/// �^�b�v�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="n">�^�b�v�� �� 2n - 1</param>
		public EvenLinearFir(int n) : this(new double[n]){}

		/// <summary>
		/// �W�����w�肵�� FIR �����B
		/// </summary>
		/// <param name="coef">�W�����i�[�����z��B</param>
		public EvenLinearFir(double[] coef)
		{
			this.Coefficient = coef;

			this.Clear();
		}

		/// <summary>
		/// �t�B���^�����O���s���B
		/// n: �^�b�v���� 2n (n = this.coef.Length)
		/// x: ����
		/// y: �o��
		/// c[i]: �W���z��
		/// d[i]: i+1 �T���v���O�� x �̒l
		/// �Ƃ���ƁA
		/// y = ��_{i=1}^{n} (d[n+i] + d[n-1-i])*c[i]
		/// </summary>
		/// <param name="x">�t�B���^���́B</param>
		/// <returns>�t�B���^�o��</returns>
		public double GetValue(double x)
		{
			this.buff.PushFront(x);

			int n = this.coef.Length;
			double y = 0;
			for(int i=0; i<n; ++i)
			{
				y += this.coef[i] * (this.buff[i] + this.buff[2*n-1-i]);
			}
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

		#region IFirFilter �����o

		public class CoefficientProxy : IFirCoefficient
		{
			internal double[] x;

			public CoefficientProxy(double[] x){this.x = x;}
			public static implicit operator CoefficientProxy(double[] x){return new CoefficientProxy(x);}

			#region IFirCoefficient �����o

			public int Count
			{
				get{return 2 * this.x.Length;}
			}

			public double this[int i]
			{
				get
				{
					if(i < this.x.Length)
						return this.x[i];
					else
						return this.x[2 * this.x.Length - 1 - i];
				}
				set
				{
					if(i < this.x.Length)
						this.x[i] = value;
					else
						this.x[2 * this.x.Length - 1 - i] = value;
				}
			}

			#endregion
			#region IEnumerable �����o

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new FirFilter.CoefficientProxy.Enumerator(this);
			}

			#endregion
		}

		IFirCoefficient IFirFilter.Coefficient
		{
			set{this.Coefficient = ((CoefficientProxy)value).x;}
			get{return new CoefficientProxy(this.Coefficient);}
		}

		/// <summary>
		/// �W���̎擾/�ݒ�
		/// </summary>
		public double[] Coefficient
		{
			set
			{
				this.coef = value;
				if(value == null)
					this.buff = null;
				else if(this.buff == null)
					this.buff = new CircularBuffer(2 * this.coef.Length);
				else if(this.buff.Length < 2 * this.coef.Length)
					this.buff.Resize(2 * this.coef.Length);
			}
			get
			{
				return this.coef;
			}
		}

		#endregion

		public object Clone()
		{
			return new EvenLinearFir((double[])this.coef.Clone());
		}
	}//class EvenLinearFir

}//namespace Filter
