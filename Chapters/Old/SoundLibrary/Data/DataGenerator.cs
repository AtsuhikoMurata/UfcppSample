using System;

namespace SoundLibrary.Data
{
	/// <summary>
	/// �e�X�g�p�̃f�[�^�����C���^�[�t�F�[�X
	/// </summary>
	public interface IDataGenerator : ICloneable
	{
		/// <summary>
		/// ���̃f�[�^�����o���B
		/// </summary>
		/// <returns>�f�[�^</returns>
		double Next();

		/// <summary>
		/// ������Ԃɖ߂��B
		/// </summary>
		void Reset();
	}
}//namespace test
