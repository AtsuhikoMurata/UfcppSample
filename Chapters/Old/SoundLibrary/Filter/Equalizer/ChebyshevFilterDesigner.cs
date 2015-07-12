using System;

namespace SoundLibrary.Filter.Equalizer
{
	/// <summary>
	/// �`�F�r�V�F�t�t�B���^�݌v�B�B
	/// </summary>
	public class ChebyshevFilterDesigner : FilterDesigner
	{
		#region �t�B�[���h

		/// <summary>
		/// �t�B���^��ʁBfalse: I�^�Atrue: II�^(�t�`�F�r�V�F�t)
		/// </summary>
		bool kind;

		/// <summary>
		/// ��
		/// </summary>
		double epsilon;

		#endregion
		#region ������

		public ChebyshevFilterDesigner(int order, double epsilon) : this(order, epsilon, false) {}

		public ChebyshevFilterDesigner(int order, double epsilon, bool kind) : base(order)
		{
			this.kind = kind;
			this.epsilon = epsilon;
		}

		#endregion
		#region �p�����[�^�ݒ�

		/// <summary>
		/// �`�F�r�V�F�t�t�B���^��ʁB
		/// </summary>
		public bool Kind
		{
			get{return this.kind;}
			set{this.kind = value;}
		}

		public double Epsilon
		{
			get{return this.epsilon;}
			set{this.epsilon = value;}
		}

		double Beta0
		{
			get
			{
				double t = Math.Pow(
					(1 + Math.Sqrt(1 + this.epsilon * this.epsilon)) / this.epsilon,
					1.0 / this.order);

				return (t * t - 1) / (2 * t);
			}
		}

		#endregion
		#region ��_/�ɂ̌v�Z

		/// <summary>
		/// �t�B���^�̗�_/�ɂ��v�Z�B
		/// </summary>
		/// <param name="roots">��_/�Ɉꗗ�̊i�[��</param>
		public override void GetZeroPole(ZeroPole[] roots)
		{
			//!
			/*
			for(int i=this.order-1, j=0; i>0; i-=2, ++j)
			{
				double w = Math.PI / 2.0 * (double)i / this.order;
				double sin, cos;
				GetSinCos(w, out sin, out cos);

				roots[j].zero = new Root(Root.Type.None, 0, 0);
				roots[j].pole = new Root(Root.Type.Complex, -cos, -sin);
			}

			if((this.order & 1) == 1)
			{
				roots[this.order / 2].zero = new Root(Root.Type.None, 0, 0);
				roots[this.order / 2].pole = new Root(Root.Type.Single, -1, 0);
			}
			*/
		}

		#endregion
		#region �A�i���O�v���g�^�C�v

		public override void GetAnalogPrototype(Coefficient[] coefs)
		{
			double beta0 = this.Beta0;

			for(int i=this.order-1, j=0; i>0; i-=2, ++j)
			{
				Coefficient coef = coefs[j];

				double sin, cos;
				GetSinCos(Math.PI / 2 * (double)i / this.order, out sin, out cos);

				double alpha = 2 * beta0 * cos;
				double beta = sin;
				beta *= beta;
				double gamma = beta;
				beta += beta0 * beta0;

				if(this.kind)
				{
					coef.a[0] = 1; coef.a[1] = alpha; coef.a[2] = beta;
					coef.b[0] = 1; coef.b[1] = 0;     coef.b[2] = gamma;
				}
				else
				{
					coef.a[0] = beta; coef.a[1] = alpha; coef.a[2] = 1;
					coef.b[0] = beta; coef.b[1] = 0;     coef.b[2] = 0;
				}
			}

			if((this.order & 1) == 1)
			{
				Coefficient coef = coefs[this.order / 2];

				if(this.kind)
				{
					coef.a[0] = 1; coef.a[1] = beta0; coef.a[2] = 0;
					coef.b[0] = 1; coef.b[1] = 0;     coef.b[2] = 0;
				}
				else
				{
					coef.a[0] = beta0; coef.a[1] = 1; coef.a[2] = 0;
					coef.b[0] = beta0; coef.b[1] = 0; coef.b[2] = 0;
				}
			}
		}

		#endregion
	}
}
