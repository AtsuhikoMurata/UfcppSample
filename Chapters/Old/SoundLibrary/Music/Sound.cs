using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// 1����\���B
	/// </summary>
	public abstract class Sound
	{
		/// <summary>
		/// ���̒����B
		/// </summary>
		public abstract int Length{get;}

		/// <summary>
		/// �z�񉻁B
		/// </summary>
		/// <returns>���f�[�^���i�[�����z��</returns>
		public abstract double[] ToArray();
	}//class Sound
}//namespace Music
