public class CircularBuffer : ICloneable
{
	#region �t�B�[���h

	double[] buf;
	int top;

	#endregion
	#region �R���X�g���N�^

	public CircularBuffer() : this(0) { }

	/// <summary>
	/// �o�b�t�@�����w�肵�ď�����
	/// </summary>
	/// <param name="len">�o�b�t�@��</param>
	public CircularBuffer(int len)
	{
		this.top = 0;
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
		get { return this.buf[(n + this.top) % this.buf.Length]; }
		set { this.buf[(n + this.top) % this.buf.Length] = value; }
	}

	/// <summary>
	/// �l�̑}��
	/// </summary>
	/// <param name="x">�}���������l</param>
	public void Insert(double x)
	{
		--this.top;
		if (this.top < 0) this.top += this.buf.Length;
		this.buf[this.top] = x;
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
		cb.top = this.top;
		return cb;
	}

	#endregion
}
