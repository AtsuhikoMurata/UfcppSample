using System;

namespace SoundLibrary
{
	/// <summary>
	/// �r�b�g���Z�p�N���X�B
	/// </summary>
	public class BitOperation
	{
		/// <summary>
		/// Floor(Log_2(x)) �����߂�B
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int FloorLog2(int x)
		{
			if(x == 0)
				return 0;

			int n = 0;
			for(; x!=1; x/=2, ++n);
			return n;
		}

		/// <summary>
		/// Ceil(Log_2(x)) �����߂�B
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int CeilLog2(int x)
		{
			if(x == 0)
				return 0;

			return 1 + FloorLog2(x - 1);
		}

		/// <summary>
		/// x �𒴂��Ȃ��ő��2�ׂ̂������߂�B
		/// 2^FloorLog2(x)
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		[System.Obsolete("FloorPower2 �Ɉڍs")]
		public static int Power2(int x)
		{
			return FloorPower2(x);
		}

		/// <summary>
		/// x �𒴂��Ȃ��ő��2�ׂ̂������߂�B
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int FloorPower2(int x)
		{
			if(x == 0)
				return 0;

			int n = 1;
			for(; x!=1; x/=2, n*=2);
			return n;
		}

		/// <summary>
		/// x �ȏ�̍ŏ���2�ׂ̂������߂�B
		/// 2^CeilLog2(x)
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static int CeilPower2(int x)
		{
			if(x == 0)
				return 0;

			return 2 * FloorPower2(x - 1);
		}

		/// <summary>
		/// ���� n �r�b�g��1�A�c�肪0�̃}�X�N�����B
		/// </summary>
		/// <param name="n"></param>
		/// <returns></returns>
		public static int Mask(int n)
		{
			return (1 << n) - 1;
		}

		/// <summary>
		/// �l�̌ܓ����V�t�g�B
		/// </summary>
		/// <param name="val">�l</param>
		/// <param name="shift">�V�t�g��</param>
		/// <returns>�V�t�g��̒l</returns>
		public static int RoundShift(long val, int shift)
		{
			return (int)((val + (1 << (shift - 1))) >> shift);
		}
	}
}
