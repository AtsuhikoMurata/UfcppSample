using System;

namespace SpectrumAnalysis
{
	/// <summary>
	/// Spectrum ���O��ԓ��̋��ʊ֐��Q
	/// </summary>
	class Common
	{
		/// <summary>
		/// ���g���𐳋K���B
		/// </summary>
		/// <param name="w">���K�����������g��</param>
		/// <param name="ws">�T���v�����O���g��</param>
		/// <returns></returns>
		public static double Normalize(double w, double ws)
		{
			return 2 * Math.PI / ws * w;
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
	}//class Common
}