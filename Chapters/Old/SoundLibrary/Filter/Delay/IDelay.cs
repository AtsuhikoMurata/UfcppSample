using System;

namespace SoundLibrary.Filter.Delay
{
	/// <summary>
	/// �x���t�B���^�̃C���^�t�F�[�X�B
	/// </summary>
	public interface IDelay : IFilter
	{
		/// <summary>
		/// �x������[�T���v��]�B
		/// </summary>
		double DelayTime
		{
			get;
			set;
		}

		/// <summary>
		/// DelayTime �T���v���x��̒l�����o�������B
		/// </summary>
		/// <returns>���o�����l</returns>
		double GetValue();

		/// <summary>
		/// �����o�b�t�@�̓r���̒l�����o���B
		/// ! ���o�[�u�p�B
		/// </summary>
		/// <param name="n">�l�����o�������ʒu</param>
		/// <returns>���o�����l</returns>
		double GetBufferValue(int n);

		/// <summary>
		/// �l�̃v�b�V���B
		/// </summary>
		/// <param name="x">�v�b�V���������l</param>
		void Push(double x);
	}
}
