using System;
using System.Collections;

namespace SoundLibrary.Filter.Equalizer
{
	/// <summary>
	/// �p�����g���b�N�C�R���C�U�B
	/// 2��IIR�̒���ڑ��Ŏ����B
	/// IIR �̓`�B�֐��̎��͈ȉ��̒ʂ�
	/// c * (1 + b1 * z^-1 + b2 z^-2) / (1 - a1 * z^-1 - a2 z^-2)
	/// </summary>
	public class ParametricEqualizer : IFilter
	{
		#region �����f�[�^�^

		/// <summary>
		/// 2�� IIR �̃p�����[�^
		/// </summary>
		public class Parameter
		{
			public double c;
			public double a1;
			public double a2;
			public double b1;
			public double b2;
		}

		/// <summary>
		/// 2�� IIR ��1��1�̏�ԁ{�p�����[�^��\���N���X�B
		/// </summary>
		private class Tuple
		{
			public double t1;
			public double t2;
			public Parameter parameter;

			public Tuple(Parameter parameter)
			{
				this.parameter = parameter;
				this.Clear();
			}

			public void Clear()
			{
				this.t1 = 0;
				this.t2 = 0;
			}
		}

		#endregion
		#region �t�B�[���h

		double gain;
		ArrayList filters;

		#endregion
		#region ������

		public ParametricEqualizer()
		{
			this.gain = 1;
			this.filters = new ArrayList();
		}

		public ParametricEqualizer(params Parameter[] parameters) : this()
		{
			this.Add(parameters);
		}

		private ParametricEqualizer(ArrayList filters)
		{
			this.gain = 1;
			this.filters = new ArrayList();

			foreach(Tuple filter in filters)
			{
				this.Add(filter.parameter);
			}
		}

		#endregion
		#region �p�����[�^�ݒ�

		public void Add(Parameter parameter)
		{
			this.filters.Add(new Tuple(parameter));
			this.gain *= parameter.c;
		}

		public void Add (params Parameter[] parameters)
		{
			foreach(Parameter parameter in parameters)
			{
				this.Add(parameter);
			}
		}

		public void RemoveAt(int i)
		{
			Tuple tuple = (Tuple)this.filters[i];
			this.gain /= tuple.parameter.c;
			this.filters.RemoveAt(i);
		}

		public Parameter this[int i]
		{
			get
			{
				Tuple tuple = (Tuple)this.filters[i];
				return tuple.parameter;
			}
			set
			{
				Tuple tuple = (Tuple)this.filters[i];
				this.gain /= tuple.parameter.c;
				this.gain *= value.c;
				tuple.parameter = value;
			}
		}

		public void UpdateGain()
		{
			this.gain = 1;
			foreach(Tuple f in this.filters)
			{
				this.gain *= f.parameter.c;
			}
		}

		#endregion
		#region IFilter �����o

		public double GetValue(double x)
		{
			x *= gain;

			foreach(Tuple filter in this.filters)
			{
				x = GetValue(filter, x);
			}

			return x;
		}

		static double GetValue(Tuple filter, double x)
		{
			x += filter.parameter.a1 * filter.t1;
			x += filter.parameter.a2 * filter.t2;
			double y = x;
			y += filter.parameter.b1 * filter.t1;
			y += filter.parameter.b2 * filter.t2;
			filter.t2 = filter.t1;
			filter.t1 = x;
			return y;
		}

		public void Clear()
		{
			foreach(Tuple filter in this.filters)
			{
				filter.Clear();
			}
		}

		#endregion
		#region ICloneable �����o

		public ParametricEqualizer Clone()
		{
			return new ParametricEqualizer(this.filters);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		#endregion
		#region �p�����[�^�݌v�p static �֐�

		#region �o1���ϊ�

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
		static void BilinearTransform(ref double a0, ref double a1, double sin, double cos)
		{
			double t0 = a0 * sin;
			double t1 = a1 * (1 + cos);

			a0 = t0 + t1;
			a1 = t0 - t1;
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
		static void BilinearTransform(ref double a0, ref double a1, ref double a2, double sin, double cos)
		{
			double t0 = a0 * (1 - cos);
			double t1 = a1 * sin;
			double t2 = a2 * (1 + cos);

			a0 = a2 = t0 + t2;
			a1 = 2 * (t0 - t2);

			a0 += t1;
			a2 -= t1;
		}

		/// <summary>
		/// �n�C�p�X�t�B���^�p�o1���ϊ��B
		/// </summary>
		static void BilinearTransformHPF(ref double a0, ref double a1, ref double a2, double sin, double cos)
		{
			BilinearTransform(ref a2, ref a1, ref a0, sin, cos);
			Swap(ref a0, ref a2);
		}

		/// <summary>
		/// �n�C�p�X�t�B���^�p�o1���ϊ��B
		/// </summary>
		static void BilinearTransformHPF(ref double a0, ref double a1, double sin, double cos)
		{
			BilinearTransform(ref a1, ref a0, sin, cos);
			Swap(ref a0, ref a1);
		}

		static void Swap(ref double a, ref double b)
		{
			double tmp = a; a = b; b = tmp;
		}

		#endregion
		#region RBJ Audio-EQ-Cookbook Biquad �t�B���^

		/// <summary>
		/// ���[�p�X�t�B���^�̐݌v�B
		/// H(s) = 1 / (s^2 + s/Q + 1)
		/// b0 =  (1 - cos)/2
		/// b1 =   1 - cos
		/// b2 =  (1 - cos)/2
		/// a0 =   1 + alpha
		/// a1 =  -2*cos
		/// a2 =   1 - alpha
		/// </summary>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="q">Q�l</param>
		/// <returns>�p�����[�^</returns>
		public static Parameter GetLowPass(double w, double q)
		{
			double cos = Math.Cos(w);
			double alpha = Math.Sin(w) / (2 * q);
			double a0 = 1 + alpha;

			Parameter parameter = new Parameter();
			parameter.c = (1 - cos) / (2 * a0);
			parameter.a1 = 2 * cos / a0;
			parameter.a2 = -(1 - alpha) / a0;
			parameter.b1 = 2;
			parameter.b2 = 1;

			return parameter;
		}

		/// <summary>
		/// �n�C�p�X�t�B���^�̐݌v�B
		/// H(s) = s^2 / (s^2 + s/Q + 1)
		/// b0 =  (1 + cos)/2
		/// b1 =  -(1 + cos)
		/// b2 =  (1 + cos)/2
		/// a0 =   1 + alpha
		/// a1 =  -2*cos
		/// a2 =   1 - alpha
		/// </summary>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="q">Q�l</param>
		/// <returns>�p�����[�^</returns>
		public static Parameter GetHighPass(double w, double q)
		{
			double cos = Math.Cos(w);
			double alpha = Math.Sin(w) / (2 * q);
			double a0 = 1 + alpha;

			Parameter parameter = new Parameter();
			parameter.c = (1 + cos) / (2 * a0);
			parameter.a1 = 2 * cos / a0;
			parameter.a2 = -(1 - alpha) / a0;
			parameter.b1 = -2;
			parameter.b2 = 1;

			return parameter;
		}

		/// <summary>
		/// �s�[�L���O�t�B���^�̐݌v�B
		/// H(s) = (s^2 + s*(A/Q) + 1) / (s^2 + s/(A*Q) + 1)
		/// b0 =   1 + alpha*A
		/// b1 =  -2*cos
		/// b2 =   1 - alpha*A
		/// a0 =   1 + alpha/A
		/// a1 =  -2*cos
		/// a2 =   1 - alpha/A
		/// </summary>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="q">Q�l</param>
		/// <returns>�p�����[�^</returns>
		public static Parameter GetPeaking(double w, double q, double g)
		{
			double cos = Math.Cos(w);
			double alpha = Math.Sin(w) / (2 * q);
			double a0, a1, a2, b0, b1, b2;
#if true
			double A = Math.Sqrt(g);

			a0 = 1 + alpha / A;
			a1 = -2 * cos;
			a2 = 1 - alpha / A;
			b0 = 1 + alpha * A;
			b1 = -2 * cos;
			b2 = 1 - alpha * A;
#else
			if(g > 1)
			{
				a0 = 1 + alpha;
				a1 = -2 * cos;
				a2 = 1 - alpha;
				b0 = 1 + alpha * g;
				b1 = -2 * cos;
				b2 = 1 - alpha * g;
			}
			else
			{
				a0 = 1 + alpha / g;
				a1 = -2 * cos;
				a2 = 1 - alpha / g;
				b0 = 1 + alpha;
				b1 = -2 * cos;
				b2 = 1 - alpha;
			}
#endif

			Parameter parameter = new Parameter();
			parameter.c = b0 / a0;
			parameter.a1 = -a1 / a0;
			parameter.a2 = -a2 / a0;
			parameter.b1 = b1 / b0;
			parameter.b2 = b2 / b0;

			return parameter;
		}

		public static void GetPeaking(double w, double q, double g, Parameter parameter)
		{
			double cos = Math.Cos(w);
			double alpha = Math.Sin(w) / (2 * q);
			double a0, a1, a2, b0, b1, b2;

			double A = Math.Sqrt(g);

			a0 = 1 + alpha / A;
			a1 = -2 * cos;
			a2 = 1 - alpha / A;
			b0 = 1 + alpha * A;
			b1 = -2 * cos;
			b2 = 1 - alpha * A;

			parameter.c = b0 / a0;
			parameter.a1 = -a1 / a0;
			parameter.a2 = -a2 / a0;
			parameter.b1 = b1 / b0;
			parameter.b2 = b2 / b0;
		}

#if false
		// �����ꂩ��ǉ����Ă����B

BPF:        H(s) = s / (s^2 + s/Q + 1)          (constant skirt gain, peak gain = Q)

            b0 =   sin/2  =   Q*alpha
            b1 =   0 
            b2 =  -sin/2  =  -Q*alpha
            a0 =   1 + alpha
            a1 =  -2*cos
            a2 =   1 - alpha


BPF:        H(s) = (s/Q) / (s^2 + s/Q + 1)      (constant 0 dB peak gain)

            b0 =   alpha
            b1 =   0
            b2 =  -alpha
            a0 =   1 + alpha
            a1 =  -2*cos
            a2 =   1 - alpha



notch:      H(s) = (s^2 + 1) / (s^2 + s/Q + 1)

            b0 =   1
            b1 =  -2*cos
            b2 =   1
            a0 =   1 + alpha
            a1 =  -2*cos
            a2 =   1 - alpha



APF:        H(s) = (s^2 - s/Q + 1) / (s^2 + s/Q + 1)

            b0 =   1 - alpha
            b1 =  -2*cos
            b2 =   1 + alpha
            a0 =   1 + alpha
            a1 =  -2*cos
            a2 =   1 - alpha



peakingEQ:  H(s) = (s^2 + s*(A/Q) + 1) / (s^2 + s/(A*Q) + 1)

            b0 =   1 + alpha*A
            b1 =  -2*cos
            b2 =   1 - alpha*A
            a0 =   1 + alpha/A
            a1 =  -2*cos
            a2 =   1 - alpha/A



lowShelf:   H(s) = A * (s^2 + (sqrt(A)/Q)*s + A) / (A*s^2 + (sqrt(A)/Q)*s + 1)

            b0 =    A*[ (A+1) - (A-1)*cos + beta*sin ]
            b1 =  2*A*[ (A-1) - (A+1)*cos            ]
            b2 =    A*[ (A+1) - (A-1)*cos - beta*sin ]
            a0 =        (A+1) + (A-1)*cos + beta*sin
            a1 =   -2*[ (A-1) + (A+1)*cos            ]
            a2 =        (A+1) + (A-1)*cos - beta*sin



highShelf:  H(s) = A * (A*s^2 + (sqrt(A)/Q)*s + 1) / (s^2 + (sqrt(A)/Q)*s + A)

            b0 =    A*[ (A+1) + (A-1)*cos + beta*sin ]
            b1 = -2*A*[ (A-1) + (A+1)*cos            ]
            b2 =    A*[ (A+1) + (A-1)*cos - beta*sin ]
            a0 =        (A+1) - (A-1)*cos + beta*sin
            a1 =    2*[ (A-1) - (A+1)*cos            ]
            a2 =        (A+1) - (A-1)*cos - beta*sin
#endif

		#endregion
		#region Butterworth �t�B���^

		/// <summary>
		/// �o�^�[���[�X�t�B���^�̐݌v�B
		/// n �������̂Ƃ�
		/// H(s) = ��_(k=0)^(n/2-1) {1 / (s^2 + (2cos((2k+1)/2n))s + 1)}
		/// n ����̂Ƃ�
		/// H(s) = ��_(k=0)^(n/2-1) {1 / (s^2 + (2cos(k/n))s + 1)} �~ 1 / (1 + s)
		/// 
		/// �ȉ��̌W�������� n/2 ��IIR�𒼗�ڑ��B
		/// (k=0�`n/2-1)
		/// alpha(k,n) = sin�� �~ cos((2k+1)/2n)) �c (n ����)
		/// alpha(k,n) = sin�� �~ cos(k/n)) �c (n �)
		/// b0 =  (1 - cos)/2
		/// b1 =   1 - cos
		/// b2 =  (1 - cos)/2
		/// a0 =   1 + alpha(k,n)
		/// a1 =  -2*cos
		/// a2 =   1 - alpha(k,n)
		/// (n����̎��ɂ͂���ɁAb0 = sin, b1 = sin, a0 = sin+cos+1, a1 = sin-cos-1)
		/// </summary>
		/// <param name="n">�o�^�[���[�X�t�B���^�̎���</param>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <returns>�t�B���^�W��</returns>
		public static Parameter[] GetButterworthLowPass(int n, double w)
		{
			return GetButterworth(n, w, 0);
		}

		/// <summary>
		/// �o�^�[���[�X�t�B���^�̐݌v�B
		/// </summary>
		/// <remarks>
		/// ���� n �������̂Ƃ�
		/// H(s) = ��_(k=0)^(n/2-1) {s^2 / (s^2 + (2cos((2k+1)/2n))s + 1)}
		/// n ����̂Ƃ�
		/// H(s) = ��_(k=0)^(n/2-1) {s^2 / (s^2 + (2cos(k/n))s + 1)} �~ s / (1 + s)
		/// 
		/// �ȉ��̌W�������� n/2 ��IIR�𒼗�ڑ��B
		/// (k=0�`n/2-1)
		/// alpha(k,n) = sin�� �~ cos((2k+1)/2n)) �c (n ����)
		/// alpha(k,n) = sin�� �~ cos(k/n)) �c (n �)
		/// b0 =  (1 + cos)/2
		/// b1 =  -(1 + cos)
		/// b2 =  (1 + cos)/2
		/// a0 =   1 + alpha(k,n)
		/// a1 =  -2*cos
		/// a2 =   1 - alpha(k,n)
		/// (n����̎��ɂ͂���ɁAb0 = 1+cos, b1 = -(1+cos), a0 = sin+cos+1, a1 = sin-cos-1)
		/// </remarks>
		/// <param name="n">�o�^�[���[�X�t�B���^�̎���</param>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <returns></returns>
		public static Parameter[] GetButterworthHighPass(int n, double w)
		{
			return GetButterworth(n, w, 1);
		}

		/// <summary>
		/// Butterworth �t�B���^�̐݌v
		/// </summary>
		/// <param name="n">�o�^�[���[�X�t�B���^�̎���</param>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="type">0�̂Ƃ�LPF�A1�̂Ƃ�HPF</param>
		/// <returns>�t�B���^�W��</returns>
		public static Parameter[] GetButterworth(int n, double w, int type)
		{
			double sin = Math.Sin(w);
			double cos = Math.Cos(w);
			double a0, a1, a2, b0;
			if(type == 0)
				b0 = (1 - cos) / 2;
			else
				b0 = (1 + cos) / 2;
			a1 = -2 * cos;

			int len = n/2;
			double nu;

			Parameter[] parameters;
			if(n % 2 == 0)
			{
				parameters = new Parameter[len];
				nu = 0.5;
			}
			else
			{
				parameters = new Parameter[len + 1];
				nu = 1;

				a0 = sin + cos + 1;
				a2 = sin - cos - 1;
				if(type == 0)
					parameters[len].c = sin / a0;
				else
					parameters[len].c = (1+cos) / a0;
				parameters[len].a1 = -a2 / a0;
				parameters[len].a2 = 0;
				if(type == 0)
					parameters[len].b1 = 1;
				else
					parameters[len].b1 = -1;
				parameters[len].b2 = 0;
			}

			for(int i=0; i<len; ++i, ++nu)
			{
				double alpha = sin * Math.Cos(nu / n * Math.PI);
				a0 = 1 + alpha;
				a2 = 1 - alpha;

				parameters[i].c = b0 / a0;
				parameters[i].a1 = -a1 / a0;
				parameters[i].a2 = -a2 / a0;
				if(type == 0)
					parameters[i].b1 = 2;
				else
					parameters[i].b1 = -2;
				parameters[i].b2 = 1;
			}

			return parameters;
		}

		#endregion
		#region Chebyshev �t�B���^

		public static Parameter[] GetChebyshev1LowPass(int n, double w, double epsilon)
		{
			return GetChebyshev1(n, w, epsilon, 0);
		}

		public static Parameter[] GetChebyshev1HighPass(int n, double w, double epsilon)
		{
			return GetChebyshev1(n, w, epsilon, 1);
		}

		/// <summary>
		/// Chebyshev I �^�t�B���^�̐݌v
		/// </summary>
		/// <param name="n">�o�^�[���[�X�t�B���^�̎���</param>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="epsilon">���v���̋��e��</param>
		/// <param name="type">0�̂Ƃ�LPF�A1�̂Ƃ�HPF</param>
		/// <returns>�t�B���^�W��</returns>
		public static Parameter[] GetChebyshev1(int n, double w, double epsilon, int type)
		{
			double k = Math.Pow( (1 + Math.Sqrt(1 + epsilon*epsilon)) / epsilon, 1.0/n);
			double beta0 = (k * k - 1) / (2 * k);

			double sin = Math.Sin(w);
			double cos = Math.Cos(w);
			double a0, a1, a2, b0, tmp;

			int len = n/2;
			double nu;

			Parameter[] parameters;
			if(n % 2 == 0)
			{
				parameters = new Parameter[len];
				nu = 0.5;
			}
			else
			{
				parameters = new Parameter[len + 1];
				nu = 1;

				tmp = 1 + cos;
				if(type == 0)
				{
					b0 = beta0 * sin;
					a0 = b0 + tmp;
					a1 = b0 - tmp;
					parameters[len].b1 = 1;
				}
				else
				{
					b0 = beta0 * tmp;
					a0 = sin + b0;
					a1 = sin - b0;
					parameters[len].b1 = -1;
				}
				parameters[len].c = b0 / a0;
				parameters[len].a1 = -a1 / a0;
				parameters[len].a2 = 0;
				parameters[len].b2 = 0;
			}

			for(int i=0; i<len; ++i, ++nu)
			{
				double alpha = sin * 2 * beta0 * Math.Cos(nu / n * Math.PI);
				double beta = Math.Sin(nu / n * Math.PI);
				beta *= beta;
				beta += beta0 * beta0;

				if(type == 0)
				{
					b0 = beta * (1 - cos);
					tmp = 1 + cos;
					a1 = 2 * (b0 - tmp);
					parameters[i].b1 = 2;
				}
				else
				{
					b0 = beta * (1 + cos);
					tmp = 1 - cos;
					a1 = 2 * (tmp - b0);
					parameters[i].b1 = -2;
				}
				a0 = b0 + tmp + alpha;
				a2 = b0 + tmp - alpha;

				parameters[i].c = b0 / a0;
				parameters[i].a1 = -a1 / a0;
				parameters[i].a2 = -a2 / a0;
				parameters[i].b2 = 1;
			}

			return parameters;
		}

		/// <summary>
		/// Chebyshev �t�B���^�̐݌v
		/// </summary>
		/// <param name="n">�o�^�[���[�X�t�B���^�̎���</param>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="epsilon">���v���̋��e��</param>
		/// <param name="kind">false �̂Ƃ� I �^�t�B���^�Atrue �̂Ƃ� II �^�t�B���^</param>
		/// <param name="type">false �̂Ƃ�LPF�Atrue�̂Ƃ�HPF</param>
		/// <returns>�t�B���^�W��</returns>
		public static Parameter[] GetChebyshev(int n, double w, double epsilon, bool kind, bool type)
		{
			double t = Math.Pow( (1 + Math.Sqrt(1 + epsilon*epsilon)) / epsilon, 1.0/n);
			double beta0 = (t * t - 1) / (2 * t);

			double cos = Math.Cos(w);
			double sin = Math.Sqrt(1 - cos * cos);//Math.Sin(w);
			double a0, a1, a2, b0, b1, b2;

			int len = n/2;
			int k;

			Parameter[] parameters;
			if(n % 2 == 0)
			{
				parameters = new Parameter[len];
				k = 1;
			}
			else
			{
				parameters = new Parameter[len + 1];
				k = 2;

				if(kind)
				{
					a0 = 1; a1 = beta0;
					b0 = 1; b1 = 0;
				}
				else
				{
					a0 = beta0; a1 = 1;
					b0 = beta0; b1 = 0;
				}
				/* �e�X�g�p(Butterworth ����)
				a0 = 1; a1 = 1;
				b0 = 1; b1 = 0;
				//*/

				if(type)
				{
					BilinearTransformHPF(ref a0, ref a1, sin, cos);
					BilinearTransformHPF(ref b0, ref b1, sin, cos);
				}
				else
				{
					BilinearTransform(ref a0, ref a1, sin, cos);
					BilinearTransform(ref b0, ref b1, sin, cos);
				}

				parameters[len].c = b0 / a0;
				parameters[len].a1 = -a1 / a0;
				parameters[len].a2 = 0;
				parameters[len].b1 = b1 / b0;
				parameters[len].b2 = 0;
			}

			for(int i=0; i<len; ++i, k += 2)
			{
				double wk = (double)k / n * Math.PI / 2;

				double alpha = 2 * beta0 * Math.Cos(wk);
				double beta = Math.Sin(wk);
				beta *= beta;
				double gamma = beta;
				beta += beta0 * beta0;

				if(kind)
				{
					a0 = 1; a1 = alpha; a2 = beta;
					b0 = 1; b1 = 0; b2 = gamma;
				}
				else
				{
					a0 = beta; a1 = alpha; a2 = 1;
					b0 = beta; b1 = 0; b2 = 0;
				}
				/* �e�X�g�p(Butterworth ����)
				double alpha = 2 * Math.Cos(wk);
				a0 = 1; a1 = alpha; a2 = 1;
				b0 = 1; b1 = 0; b2 = 0;
				//*/

				if(type)
				{
					BilinearTransformHPF(ref a0, ref a1, ref a2, sin, cos);
					BilinearTransformHPF(ref b0, ref b1, ref b2, sin, cos);
				}
				else
				{
					BilinearTransform(ref a0, ref a1, ref a2, sin, cos);
					BilinearTransform(ref b0, ref b1, ref b2, sin, cos);
				}

				parameters[i].c = b0 / a0;
				parameters[i].a1 = -a1 / a0;
				parameters[i].a2 = -a2 / a0;
				parameters[i].b1 = b1 / b0;
				parameters[i].b2 = b2 / b0;
			}

			return parameters;
		}

		#endregion
#if false
		#region �ȉ~�t�B���^

		/// <summary>
		/// �ȉ~�t�B���^�̐݌v
		/// </summary>
		/// <param name="n">�o�^�[���[�X�t�B���^�̎���</param>
		/// <param name="w">�J�b�g�I�t���g��</param>
		/// <param name="epsilon">��</param>
		/// <param name="k1">k1</param>
		/// <param name="type">false �̂Ƃ�LPF�Atrue�̂Ƃ�HPF</param>
		/// <returns>�t�B���^�W��</returns>
		public static Parameter[] GetElliptic(int n, double w, double epsilon, double k1, bool type)
		{
			double t = Math.Pow( (1 + Math.Sqrt(1 + epsilon*epsilon)) / epsilon, 1.0/n);
			double beta0 = (t * t - 1) / (2 * t);

			double cos = Math.Cos(w);
			double sin = Math.Sqrt(1 - cos * cos);//Math.Sin(w);
			double a0, a1, a2, b0, b1, b2;

			int len = n/2;
			int k;

			Parameter[] parameters;
			if(n % 2 == 0)
			{
				parameters = new Parameter[len];
				k = 1;
			}
			else
			{
				parameters = new Parameter[len + 1];
				k = 2;

				a0 = 1; a1 = beta0;
				b0 = 1; b1 = 0;
				/* �e�X�g�p(Butterworth ����)
				a0 = 1; a1 = 1;
				b0 = 1; b1 = 0;
				//*/

				if(type)
				{
					BilinearTransformHPF(ref a0, ref a1, sin, cos);
					BilinearTransformHPF(ref b0, ref b1, sin, cos);
				}
				else
				{
					BilinearTransform(ref a0, ref a1, sin, cos);
					BilinearTransform(ref b0, ref b1, sin, cos);
				}

				parameters[len].c = b0 / a0;
				parameters[len].a1 = -a1 / a0;
				parameters[len].a2 = 0;
				parameters[len].b1 = b1 / b0;
				parameters[len].b2 = 0;
			}

			for(int i=0; i<len; ++i, k += 2)
			{
				double wk = (double)k / n * Math.PI / 2;

				double alpha = 2 * beta0 * Math.Cos(wk);
				double beta = Math.Sin(wk);
				beta *= beta;
				double gamma = beta;
				beta += beta0 * beta0;

				a0 = 1; a1 = alpha; a2 = beta;
				b0 = 1; b1 = 0; b2 = gamma;
				/* �e�X�g�p(Butterworth ����)
				double alpha = 2 * Math.Cos(wk);
				a0 = 1; a1 = alpha; a2 = 1;
				b0 = 1; b1 = 0; b2 = 0;
				//*/

				if(type)
				{
					BilinearTransformHPF(ref a0, ref a1, ref a2, sin, cos);
					BilinearTransformHPF(ref b0, ref b1, ref b2, sin, cos);
				}
				else
				{
					BilinearTransform(ref a0, ref a1, ref a2, sin, cos);
					BilinearTransform(ref b0, ref b1, ref b2, sin, cos);
				}

				parameters[i].c = b0 / a0;
				parameters[i].a1 = -a1 / a0;
				parameters[i].a2 = -a2 / a0;
				parameters[i].b1 = b1 / b0;
				parameters[i].b2 = b2 / b0;
			}

			return parameters;
		}

		#endregion
#endif

		#endregion
	}
}
