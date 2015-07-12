using System;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// 2��IIR��p�����s�[�L���O�C�R���C�U�B
	/// </summary>
	public class PeakingEqualizer : IirFilter
	{
		/// <summary>
		/// �s�[�L���O�C�R���C�U���쐬�B
		/// </summary>
		/// <param name="w">���S���g��</param>
		/// <param name="q">Q�l</param>
		/// <param name="a">������(���j�A�l)</param>
		public PeakingEqualizer(double w, double q, double a) : base(2)
		{
			this.SetParameter(w, q, a);
		}

		/// <summary>
		/// �s�[�L���O�C�R���C�U�̃p�����[�^��ݒ�B
		/// </summary>
		/// <param name="w">���S���g��</param>
		/// <param name="q">Q�l</param>
		/// <param name="a">������(���j�A�l)</param>
		public void SetParameter(double w, double q, double a)
		{
#if false
			double x = Math.Sqrt(a);
			double c = Math.Cos(w);
			double s = Math.Sqrt(1 - c * c);// Math.Sin(w);
			double y = s / (2 * q);

			double a0 = 1 + y / x;
			double a1 = -2 * c;
			double a2 = 1 - y / x;
			double b0 = 1 + y * x;
			double b1 = -2 * c;
			double b2 = 1 - y * x;

			this.A[0] = -a1 / a0;
			this.A[1] = -a2 / a0;
			this.B[0] = b0 / a0;
			this.B[1] = b1 / a0;
			this.B[2] = b2 / a0;
#elif false
			// ���̃R�[�h�́��̃R�[�h�Ɠ���(�덷��������)�̂͂��B
			double c = Math.Cos(w);
			double s = Math.Sqrt(1 - c * c);// Math.Sin(w);
			double y = s / (2 * q);

			double a0, a1, a2, b0, b1, b2;

			if(a > 1)
			{
				a0 = 1 + y;
				a1 = -2 * c;
				a2 = 1 - y;
				b0 = 1 + y * a;
				b1 = -2 * c;
				b2 = 1 - y * a;
			}
			else
			{
				a0 = 1 + y / a;
				a1 = -2 * c;
				a2 = 1 - y / a;
				b0 = 1 + y;
				b1 = -2 * c;
				b2 = 1 - y;
			}

			this.A[0] = -a1 / a0;
			this.A[1] = -a2 / a0;
			this.B[0] = b0 / a0;
			this.B[1] = b1 / a0;
			this.B[2] = b2 / a0;
#else
			double g = a > 1 ? a : 1/a;
			double Ft = 1 / Math.Tan(w / 2);

			double term1 = 1.0 + Ft*g/q + Ft*Ft;
			double term2 = 1.0 + Ft/q + Ft*Ft;

			//Peek�����
			if(a > 1)
			{
				this.B[0] = term1 / term2;
				this.B[1] = 2.0*(1.0 - Ft*Ft)/term2;
				this.B[2] = (1.0 - Ft*g/q + Ft*Ft) / term2;
				this.A[0] = - this.B[1];
				this.A[1] = - (1.0 - Ft/q + Ft*Ft)/ term2;
			}
				//dip�����
			else if(a < 1)
			{
				this.B[0] = term2 / term1;
				this.B[1] = 2.0*(1.0 - Ft*Ft)/term1;
				this.B[2] = (1.0 - Ft / q + Ft*Ft) / term1;
				this.A[0] = - this.B[1];
				this.A[1] = - (1.0 - Ft*g/q + Ft*Ft)/ term1;
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
	/// 1��IIR��p�����s�[�V�F���r���O�C�R���C�U�B
	/// </summary>
	public class ShelvingEqualizer : IirFilter
	{
		/// <summary>
		/// �V�F���r���O�C�R���C�U�쐬�B
		/// </summary>
		/// <param name="w">���S���g��</param>
		/// <param name="a">������(���j�A�l)</param>
		public ShelvingEqualizer(double w, double a) : base(1)
		{
			this.SetParameter(w, a);
		}

		/// <summary>
		/// �V�F���r���O�C�R���C�U�̃p�����[�^��ݒ�B
		/// </summary>
		/// <param name="w">���S���g��</param>
		/// <param name="a">������(���j�A�l)</param>
		public void SetParameter(double w, double a)
		{
			double tn = (Math.Sin(w) - 1) / Math.Cos(w);
			double g = 1/a;//Math.Sqrt(a);

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
