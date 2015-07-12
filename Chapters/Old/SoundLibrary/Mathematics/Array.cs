using System;

namespace SoundLibrary.Mathematics
{
	using Type = System.Double;

	/// <summary>
	/// Array �̊T�v�̐����ł��B
	/// </summary>
	public class Array
	{
		/// <summary>
		/// �z������E���]����B
		/// </summary>
		/// <param name="x">���̔z��</param>
		/// <param name="y">���ʊi�[��</param>
		public static Type[] Reverse(Type[] x)
		{
			int len = x.Length;
			Type[] y = new Type[len];
			for(int i=0, j=len-1; i<len; ++i, --j)
				y[j] = x[i];
			return y;
		}

		/// <summary>
		/// �z�� x ���E�� delay �������炷�B
		/// </summary>
		/// <param name="x">���̔z��</param>
		/// <param name="y">���ʊi�[��</param>
		public static void Delay(Type[] x, int delay, Type[] y)
		{
			int i = x.Length - 1;
			for(; i>=delay; --i)
				y[i] = x[i - delay];
			for(; i>=0; --i)
				y[i] = 0;
		}
	}
}
