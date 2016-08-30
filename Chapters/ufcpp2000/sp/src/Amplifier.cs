/// <summary>
/// ������B
/// </summary>
public class Amplifier : IFilter
{
	#region �t�B�[���h

	double amp; // �{��

	#endregion
	#region �R���X�g���N�^

	public Amplifier() : this(0) { }

	/// <summary>
	/// �{�����w�肵�ď�����
	/// </summary>
	/// <param name="amp">�{��</param>
	public Amplifier(double amp)
	{
		this.amp = amp;
	}

	#endregion
	#region �v���p�e�B

	/// <summary>
	/// �{��
	/// </summary>
	public double Amplitude
	{
		get { return this.amp; }
		set { this.amp = value; }
	}

	#endregion
	#region IFilter �����o

	public double GetValue(double x)
	{
		return this.amp * x;
	}

	public void Clear()
	{
	}

	#endregion
	#region ICloneable �����o

	public object Clone()
	{
		return new Amplifier(this.amp);
	}

	#endregion
}
