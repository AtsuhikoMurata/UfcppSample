#define CHECK_LENGTH

using System;
using System.Diagnostics;
using FftCpp  = Fft.Fft;
using CFftCpp = Fft.CFft;

namespace SoundLibrary.SpectrumAnalysis
{
	/// <summary>
	/// Managed C++ �ō���� Fft �N���X�̃��b�p�[�B
	/// FFT �̓���̏ڍׂ� fft\fft.cpp �̕����Q�ƁB
	/// </summary>
	public class Fft
	{
		FftCpp fft;

		public Fft(int length)
		{
			CheckLength(length);

			fft = new FftCpp(length);
		}

		/// <summary>
		/// �t�[���G�ϊ����s���B
		/// </summary>
		/// <param name="x">�ϊ��������f�[�^</param>
		unsafe public void Transform(double[] x)
		{
			fixed(double* px = x)
			{
				fft.Transform(1, px);
			}
		}

		/// <summary>
		/// �t�t�[���G�ϊ����s���B
		/// </summary>
		/// <param name="x">�ϊ��������f�[�^</param>
		unsafe public void Invert(double[] x)
		{
			fixed(double* px = x)
			{
				fft.Transform(-1, px);
			}
		}

		/// <summary>
		/// len ��2�ׂ̂��悩�ǂ������ׁA��������O�𓊂���B
		/// </summary>
		/// <param name="len">���ׂ钷��</param>
		[Conditional("CHECK_LENGTH")]
		public static void CheckLength(int len)
		{
			if(!IsPower2(len))
				throw new ArgumentException("������2�ׂ̂���ȊO���w�肵����ʖ�");
		}

		/// <summary>
		/// len ��2�ׂ̂��悩�ǂ������ׂ�B
		/// </summary>
		/// <param name="len">���ׂ钷��</param>
		/// <returns>2�ׂ̂���Ȃ�true</returns>
		static bool IsPower2(int len)
		{
			if(len < 0)
				return false;

			while(len != 0)
			{
				if(len%2 != 0)
					return len/2 == 0;
				len /= 2;
			}

			return false;
		}
	}//class Fft

	/// <summary>
	/// Managed C++ �ō���� Fft �N���X�̃��b�p�[�B
	/// FFT �̓���̏ڍׂ� fft\fft.cpp �̕����Q�ƁB
	/// </summary>
	public class CFft
	{
		CFftCpp fft;

		public CFft(int length)
		{
			Fft.CheckLength(length);

			fft = new CFftCpp(length);
		}

		/// <summary>
		/// �t�[���G�ϊ����s���B
		/// </summary>
		/// <param name="x">�ϊ��������f�[�^</param>
		unsafe public void Transform(double[] x)
		{
			fixed(double* px = x)
			{
				fft.Transform(1, px);
			}
		}

		/// <summary>
		/// �t�t�[���G�ϊ����s���B
		/// </summary>
		/// <param name="x">�ϊ��������f�[�^</param>
		unsafe public void Invert(double[] x)
		{
			fixed(double* px = x)
			{
				fft.Transform(-1, px);
			}
		}
	}//class CFft
}//namespace Spectrum
