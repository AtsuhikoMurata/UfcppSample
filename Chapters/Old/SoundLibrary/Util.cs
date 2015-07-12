using System;

namespace SoundLibrary
{
	//! C# 2.0 ���������J���ꂽ�� static �N���X�ɁB

	/// <summary>
	/// ���ʊ֐��Q�B
	/// </summary>
	public class Util
	{
		#region ���K���EdB�̃��j�A�l

		/// <summary>
		/// ���g���𐳋K���B
		/// </summary>
		/// <param name="f">���K�����������g��</param>
		/// <param name="fs">�T���v�����O���g��</param>
		/// <returns>���K���p���g��</returns>
		public static double Normalize(double f, double fs)
		{
			return 2 * Math.PI / fs * f;
		}

		/// <summary>
		/// ���K���p���g�������̎��g���ɖ߂��B
		/// </summary>
		/// <param name="w">���K���p���g��</param>
		/// <param name="fs">�T���v�����O���g��</param>
		/// <returns>���̎��g��</returns>
		public static double Denormalize(double w, double fs)
		{
			return fs / (2 * Math.PI) * w;
		}

		/// <summary>
		/// ���g���𐳋K���B
		/// �T���v�����O���g���� 48000Hz�B
		/// </summary>
		/// <param name="w">���K�����������g��</param>
		/// <returns></returns>
		public static double Normalize(double w)
		{
			return Normalize(w, 48000);
		}

		/// <summary>
		/// ���f���̐�Βl(�p���[��dB�l)�����߂�B
		/// </summary>
		/// <param name="re">����</param>
		/// <param name="im">����</param>
		/// <returns>�p���[��dB�l</returns>
		public static double Amp(double re, double im)
		{
			return 10 * Math.Log10(re*re + im*im);
		}

		/// <summary>
		/// ��Βl(�p���[��dB�l)�����߂�B
		/// </summary>
		/// <param name="re">���j�A�l</param>
		/// <returns>�p���[��dB�l</returns>
		public static double Amp(double re)
		{
			return 20 * Math.Log10(Math.Abs(re));
		}

		/// <summary>
		/// ���f���̕Ίp�����߂�B
		/// </summary>
		/// <param name="re">����</param>
		/// <param name="im">����</param>
		/// <returns>�Ίp</returns>
		public static double Phase(double re, double im)
		{
			return Math.Atan2(im, re);
		}

		/// <summary>
		/// dB�l �� ���j�A�l�ɕϊ��B
		/// </summary>
		/// <param name="x">dB�l</param>
		/// <returns>���j�A�l</returns>
		public static double DBToLinear(double x)
		{
			return Math.Pow(10, x/20);
		}

		/// <summary>
		/// ���j�A�l �� dB�l�ɕϊ��B
		/// </summary>
		/// <param name="x">���j�A�l</param>
		/// <returns>dB�l</returns>
		public static double LinearToDB(double x)
		{
			return 20 * Math.Log10(x);
		}

		#endregion
		#region �l�̃N���b�s���O

		/// <summary>
		/// �l�� short �͈̔͂ɃN���b�s���O����B
		/// </summary>
		/// <param name="val">�l</param>
		/// <returns>�N���b�s���O��̒l</returns>
		/// <remarks>C# 2.0 ���������J���ꂽ�� generics ������Ǝv���B</remarks>
		public static short ClipShort(double val)
		{
			if(val < short.MinValue) val = short.MinValue;
			else if(val > short.MaxValue) val = short.MaxValue;
			return (short)val;
		}

		#endregion
		#region �C���p���X����

		/// <summary>
		/// �t�B���^�̃C���p���X�������v�Z����B
		/// </summary>
		/// <param name="f">�t�B���^</param>
		/// <param name="len">�C���p���X�����̒���</param>
		/// <returns>�C���p���X����</returns>
		public static double[] GetImpulseResponse(Filter.IFilter f, int len)
		{
			double[] x = new double[len];
			x[0] = f.GetValue(1);
			for(int i=1; i<len; ++i)
				x[i] = f.GetValue(0);
			return x;
		}

		/// <summary>
		/// �t�B���^�̎��g���������v�Z����B
		/// </summary>
		/// <param name="f">�t�B���^</param>
		/// <param name="len">�C���p���X�����̒���</param>
		/// <returns>���g������</returns>
		public static SpectrumAnalysis.Spectrum GetFrequencyResponse(Filter.IFilter f, int len)
		{
			len = SoundLibrary.BitOperation.FloorPower2(len);
			double[] x = GetImpulseResponse(f, len);
			return SpectrumAnalysis.Spectrum.FromTimeSequence(x);
		}

		#endregion
	}
}
