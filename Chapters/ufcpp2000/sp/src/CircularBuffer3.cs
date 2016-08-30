public class CircularBuffer : ICloneable
{
	#region �t�B�[���h

	double[] buf;
	int length;
	int top;
	int mask;

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
		this.length = len;

		len = Util.CeilPower2(len);
		this.buf = new double[len];
		this.mask = len - 1;
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
		get { return this.buf[(this.top + n) & this.mask]; }
		set { this.buf[(this.top + n) & this.mask] = value; }
	}

	/// <summary>
	/// �l�̑}��
	/// </summary>
	/// <param name="x">�}���������l</param>
	public void Insert(double x)
	{
		--this.top;
		this.top &= this.mask;
		this.buf[this.top] = x;
	}

	/// <summary>
	/// �v�f��
	/// </summary>
	public int Count
	{
		get { return this.length; }
	}

	#endregion
	#region ICloneable �����o

	public object Clone()
	{
		CircularBuffer cb = new CircularBuffer(this.length);
		cb.buf = (double[])this.buf.Clone();
		cb.top = this.top;
		cb.length = this.length;
		cb.mask = this.mask;
		return cb;
	}

	#endregion
}
