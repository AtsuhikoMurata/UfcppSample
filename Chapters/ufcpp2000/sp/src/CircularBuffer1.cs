public class CircularBuffer : ICloneable
{
	#region �t�B�[���h

	double[] buf;

	#endregion
	#region �R���X�g���N�^

	public CircularBuffer() : this(0) { }

	/// <summary>
	/// �o�b�t�@�����w�肵�ď�����
	/// </summary>
	/// <param name="len">�o�b�t�@��</param>
	public CircularBuffer(int len)
	{
		this.buf = new double[len];
	}

	#endregion
	#region �l�̑}���E�擾

	/// <summary>
	/// n �T���v���O�̒l�̎擾
	/// </summary>
	/// <param name="n">���T���v���O�̒l��ǂݏ������邩</param>
	/// <returns>n �T���v���O�̒l</returns>
	public double this[int n]
	{
		get { return this.buf[n]; }
		set { this.buf[n] = value; }
	}

	/// <summary>
	/// �l�̑}��
	/// </summary>
	/// <param name="x">�}���������l</param>
	public void Insert(double x)
	{
		for (int n = this.buf.Length - 1; n > 0; --n)
		{
			this.buf[n] = this.buf[n - 1];
		}
		this.buf[0] = x;
	}

	/// <summary>
	/// �v�f��
	/// </summary>
	public int Count
	{
		get { return this.buf.Length; }
	}

	#endregion
	#region ICloneable �����o

	public object Clone()
	{
		CircularBuffer cb = new CircularBuffer(this.Count);
		cb.buf = (double[])this.buf.Clone();
		return cb;
	}

	#endregion
}
