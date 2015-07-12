/* �������ɂ���\�[�X���Q�l�ɂ��Ă܂��B
 *  http://www.ldas-sw.ligo.caltech.edu/cgi-bin/cvsweb.cgi/?cvsroot=GDS#dirlist
 */

//! �e�X�g�܂��B�e�X�g�R�[�h�������B

using System;

namespace SoundLibrary.Mathematics
{
	/// <summary>
	/// �ȉ~�ϕ�/�ȉ~�֐��֘A�� static ���\�b�h�Q���`�B
	/// </summary>
	/// <remarks>
	/// u = �� d��/��(��)
	/// ��(��) = ��(1 - k^2 sin^2 ��)
	/// z = sn u = sin ��
	/// cn u = cos ��
	/// dn = ��i1 - k^2 sn^2�j
	/// </remarks>
	/*static*/
	public class Elliptic
	{
		const double EPSILON = 1.11022302462515654042E-16;

		#region �ȉ~�֐�

		/// <summary>
		/// �ȉ~�ϕ��̈��� u ����U���ӂ����߂�B
		/// </summary>
		/// <param name="u">���� u</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�U����</returns>
		public static double Phi(double u, double m)
		{
			if(m < 0.0 || m > 1.0) return double.NaN;

			double t;
			double b;

			if(m < EPSILON)
			{
				t = Math.Sin(u);
				b = Math.Cos(u);
				double ai = 0.25 * m * (u - t*b);

				return u - ai;
			}

			double twon;
   
			if(m >= 1 - EPSILON)
			{
				double ai = 0.25 * (1.0-m);
				b = Math.Cosh(u);
				t = Math.Tanh(u);
				twon =  b * Math.Sinh(u);

				return 2.0*Math.Atan(Math.Exp(u)) - Constant.PI2 + ai * (twon - u) / b;
			}
   
			const int MAX = 10;
			double[] a = new double[MAX], c = new double[MAX];
   
			b = Math.Sqrt(1.0 - m);
			twon = 1.0;
			a[0] = 1.0;
			c[0] = Math.Sqrt(m);
			int i = 0;
   
			while( Math.Abs(c[i]/a[i]) > EPSILON)
			{
				if(i >= MAX - 1)
				{
					break;
				}
				double ai = a[i];
				++i;
				c[i] = ( ai - b )/2.0;
				t = Math.Sqrt( ai * b );
				a[i] = ( ai + b )/2.0;
				b = t;
				twon *= 2.0;
			}
   
			/* backward recurrence */
			double phi = twon * a[i] * u;

			for(; i != 0; --i)
			{
				t = c[i] * Math.Sin(phi) / a[i];
				phi = (Math.Asin(t) + phi)/2.0;
			}

			return phi;
		}

		#region Jacobi �̑ȉ~�֐�

		/// <summary>
		/// Jacobi �̑ȉ~�֐� sn u �����߂�B
		/// </summary>
		/// <param name="u">���� u</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>sn(u, k)</returns>
		public static double Sn(double u, double m)
		{
			if(m < 0.0 || m > 1.0) return double.NaN;

			return Math.Sin(Elliptic.Phi(u, m));
		}

		/// <summary>
		/// Jacobi �̑ȉ~�֐� cn u �����߂�B
		/// </summary>
		/// <param name="u">���� u</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>cn(u, k)</returns>
		public static double Cn(double u, double m)
		{
			if(m < 0.0 || m > 1.0) return double.NaN;

			return Math.Cos(Elliptic.Phi(u, m));
		}

		/// <summary>
		/// Jacobi �̑ȉ~�֐� dn u �����߂�B
		/// </summary>
		/// <param name="u">���� u</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>dn(u, k)</returns>
		public static double Dn(double u, double m)
		{
			if(m < 0.0 || m > 1.0) return double.NaN;

			double sn = Elliptic.Sn(u, m);
			return Math.Sqrt(1 - m * sn * sn);
		}

		/// <summary>
		/// Jacobi �̑ȉ~�֐�(�U���ӂ��� sn, cn, dn)�����߂�B
		/// </summary>
		/// <param name="phi">�U����</param>
		/// <param name="m">�� k ��2��</param>
		/// <param name="sn">sn(u, k)</param>
		/// <param name="cn">cn(u, k)</param>
		/// <param name="dn">dn(u, k)</param>
		public static void Jacobi(double phi, double m, out double sn, out double cn, out double dn)
		{
			if(double.IsNaN(m) || m < 0.0 || m > 1.0)
			{
				sn = double.NaN;
				cn = double.NaN;
				dn = double.NaN;
				return;
			}

			sn  = Math.Sin(phi);
			cn  = Math.Cos(phi);
			dn  = Math.Sqrt(1 - m * sn * sn);;
		}

		/// <summary>
		/// Jacobi �̑ȉ~�֐�(���� u �Ɨ� k ����U���ӂ���� sn, cn, dn)�����߂�B
		/// </summary>
		/// <param name="u">���� u</param>
		/// <param name="m">�� k ��2��</param>
		/// <param name="phi">�U����</param>
		/// <param name="sn">sn(u, k)</param>
		/// <param name="cn">cn(u, k)</param>
		/// <param name="dn">dn(u, k)</param>
		public static void Jacobi(double u, double m, out double phi, out double sn, out double cn, out double dn)
		{
			if(m < 0.0 || m > 1.0)
			{
				phi = double.NaN;
				sn  = double.NaN;
				cn  = double.NaN;
				dn  = double.NaN;
				return;
			}

			if(m < EPSILON)
			{
				double t = Math.Sin(u);
				double b = Math.Cos(u);
				double ai = 0.25 * m * (u - t*b);

				phi = u - ai;
				sn  = t - ai*b;
				cn  = b + ai*t;
				dn  = 1.0 - 0.5*m*t*t;
				return;
			}
 
			if(m >= 1 - EPSILON)
			{
				double ai = 0.25 * (1.0-m);
				double b = Math.Cosh(u);
				double t = Math.Tanh(u);
				double binv = 1.0/b;
				double twon =  b * Math.Sinh(u);

				phi = 2.0*Math.Atan(Math.Exp(u)) - Constant.PI2 + ai*(twon - u)/b;
				sn  = t + ai * (twon - u)/(b*b);
				ai *= t * phi;
				cn  = binv - ai * (twon - u);
				dn  = binv + ai * (twon + u);
				return;
			}

			phi = Elliptic.Phi(u, m);
			Elliptic.Jacobi(phi, m, out sn, out cn, out dn);
		}

		#endregion
		#region Jacobi �̑ȉ~�֐��̋t�֐�

#if false

		/// <summary>
		/// Jacobi �̋t�ȉ~�֐� u = sn^-1 v �̋t�����߂�B
		/// </summary>
		/// <param name="v">���� v</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>sn(u, k)</returns>
		public static double InverseSn(double v, double m)
		{
			return 0;
		}

		// InverseCn
		// InverseDn

#endif

		#endregion
		#region �e�[�^�֐�

#if false
		/// <summary>
		/// �ȉ~�e�[�^�֐���_a(q, u) (a = 1, 2, 3, 4)�����߂�B
		/// </summary>
		/// <param name="a">��_a �� a</param>
		/// <param name="u">���� u</param>
		/// <param name="q">�p�����[�^ q</param>
		/// <returns></returns>
		public static double Theta(int a, double u, double q)
		{
			return 0;
		}

		/// <summary>
		/// �ȉ~�e�[�^�֐���_a(q, u) (a = 1, 2, 3, 4)�̓��֐������߂�B
		/// </summary>
		/// <param name="a">��_a �� a</param>
		/// <param name="u">���� u</param>
		/// <param name="q">�p�����[�^ q</param>
		/// <returns></returns>
		public static double ThetaPrime(int a, double u, double q)
		{
			return 0;
		}

		// WeierstrassP
		// WeierstrassSigma
		// WeierstrassZeta
#endif

		#endregion
		#region �m�[��

#if false
		/// <summary>
		/// �m�[�� q(m) ���v�Z����B
		/// </summary>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�m�[�� q(m)</returns>
		/// <remarks>
		/// �m�[�� q(m) = exp( - pi K(1-m)/K(m) )
		/// </remarks>
		public double Q(double m)
		{
			return 0;
		}
#endif

		/// <summary>
		/// �m�[�� q(m) �̋t���v�Z����B
		/// </summary>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�m�[���̋t m(q)</returns>
		public static double InverseQ(double q)
		{
			double t1, t2;

			double a = 1.0; // Theta3(0,q)
			double b = 1.0; // Theta2(0,q)/(2 q^(1/4))
			double r = 1.0;
			double p = q;

			do
			{
				r *= p;
				a += 2.0 * r;
				t1 = Math.Abs( r/a );

				r *= p;
				b += r;
				p *= q;
				t2 = Math.Abs( r/b );
      
			}
			while(t1 > EPSILON || t2 > EPSILON);

			b /= a;
			b *= b;
			b *= b; // b = (b / a)^4

			return 16.0 * q * b;
		}

		#endregion

		#endregion
		#region �ȉ~�ϕ�

		/// <summary>
		/// ��1�튮�S�ȉ~�ϕ��B
		/// </summary>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�ϕ��l</returns>
		public static double K(double m)
		{
			if(m < 0.0 || m >= 1.0) 
			{
				return double.NaN;
			}

			if(m == 0) 
			{
				return Math.PI / 2;
			}
   
			double a0 = 1, a1 = 1;
			double b0 = Math.Sqrt( 1 - m );
			double s0 = m;
			double mm = 1;
			int i = 0;
			double pow2i = 1;
   
			while(mm > EPSILON) 
			{
				a1 = (a0 + b0) / 2;
				double b1 = Math.Sqrt(a0 * b0);
				double c1 = (a0 - b0) / 2;
				++i;
				pow2i *= 2;
				double w1 = pow2i * c1 * c1;
				mm = w1;
      
				s0 += w1;
				a0 = a1;
				b0 = b1;
			}
   
			return Math.PI / 2 / a1;
		}

		/// <summary>
		/// ��1��s���S�ȉ~�ϕ��B
		/// </summary>
		/// <param name="phi">�U����</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�ϕ��l</returns>
		public static double F(double phi, double m)
		{
			if(m == 0.0)
				return phi;
   
			if(m == 1.0)
			{
				if(Math.Abs(phi) >= Math.PI / 2)
				{
					return double.NaN;
				}
				return Math.Log(Math.Tan((Math.PI / 2 + phi) / 2.0));
			}
   
			int npio2 = (int)Math.Floor(phi / (Math.PI / 2));
			if((npio2 & 1) != 0)
				++npio2;
   
			double K = npio2 == 0 ? Elliptic.K(1.0 - m) : 0.0;
   
			phi -= npio2 * Math.PI / 2;
			int sign = phi < 0.0 ? -1 : 1;
			phi = Math.Abs(phi);
   
			double t = Math.Tan(phi);
			if(Math.Abs(t) > 10.0)
			{
				/* Transform the amplitude */
				double e = 1.0 / (Math.Sqrt(1.0 - m) * t);
				/* ... but avoid multiple recursions.  */
				if(Math.Abs(e) < 10.0)
				{
					if(npio2 == 0)
						K = Elliptic.K(1.0 - m);
					double ret = K - Elliptic.F(Math.Atan(e), m);
					return sign * ret + npio2 * K;
				}
			}
   
			double a = 1.0;
			double b = Math.Sqrt(1.0 - m);
			double c = Math.Sqrt(m);
			int d = 1;
			int mod = 0;
			while(Math.Abs(c/a) > EPSILON)
			{
				double tmp = b/a;
				phi = phi + (Math.Atan(t * tmp) + mod * Math.PI);
				mod = (int)((phi + Math.PI / 2) / Math.PI);
				t = t * (1.0 + tmp) / (1.0 - tmp * t * t);
				c = (a - b) / 2.0;
				tmp = Math.Sqrt(a * b);
				a = (a + b) / 2.0;
				b = tmp;
				d += d;
			}
   
			return sign * (Math.Atan(t) + mod * Math.PI) / (d * a) + npio2 * K;
		}

#if false
		/// <summary>
		/// ��2�튮�S�ȉ~�ϕ��B
		/// </summary>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�ϕ��l</returns>
		public static double E(double m)
		{
			//!
			return 0;
		}

		/// <summary>
		/// ��2��s���S�ȉ~�ϕ��B
		/// </summary>
		/// <param name="phi">�U����</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�ϕ��l</returns>
		public static double E(double phi, double m)
		{
			//!
			return 0;
		}

		/// <summary>
		/// ��3�튮�S�ȉ~�ϕ��B
		/// </summary>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�ϕ��l</returns>
		public static double Pi(double n, double m)
		{
			//!
			return 0;
		}

		/// <summary>
		/// ��3��s���S�ȉ~�ϕ��B
		/// </summary>
		/// <param name="phi">�U����</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�ϕ��l</returns>
		public static double Pi(double phi, double n, double m)
		{
			//!
			return 0;
		}

		/// <summary>
		/// Jacobi �̃[�[�^�֐��B
		/// </summary>
		/// <param name="phi">�U����</param>
		/// <param name="m">�� k ��2��</param>
		/// <returns>�ϕ��l</returns>
		public static double JacobiZeta(double phi, double m)
		{
			//!
			return 0;
		}
#endif

		#endregion
	}
}
