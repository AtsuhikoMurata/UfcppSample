/// <summary>
/// ���������p�t�B���^�C���^�[�t�F�[�X�B
/// </summary>
public interface IFilter : ICloneable
{
	/// <summary>
	/// �t�B���^�����O���s���A���̌��ʂ�Ԃ��B
	/// </summary>
	/// <param name="x">�t�B���^���́B</param>
	/// <returns>�t�B���^�o�́B</returns>
	double GetValue(double x);

	/// <summary>
	/// �t�B���^�̓�����Ԃ��N���A����B
	/// </summary>
	void Clear();
}
