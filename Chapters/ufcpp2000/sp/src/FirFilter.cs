/// <summary>
/// FIR �t�B���^�B
/// </summary>
public class FirFilter : IFilter
{
	#region �t�B�[���h

	CircularBuffer buf;
	double[] a;

	#endregion
	#region �R���X�g���N�^

	public FirFilter() : this(0) { }

	/// <summary>
	/// �W�����w�肵�ď�����
	/// </summary>
	/// <param name="a0">�W�� a0</param>
	public FirFilter(double a0)
		: this(new double[] { a0 })
	{
	}

	/// <summary>
	/// �W����z��Ŏw�肵�ď�����
	/// </summary>
	/// <param name="a">�W�����i�[�����z��</param>
	public FirFilter(params double[] a)
	{
		this.a = a;
		this.buf = new CircularBuffer(a.Length - 1);
	}

	#endregion
	#region IFilter �����o

	/// <summary>
	/// �e���� n �ŁA
	/// y[n] = �� a[i] * x[n - i]
	/// </summary>
	/// <param name="x">����</param>
	/// <returns>�t�B���^�o��</returns>
	public double GetValue(double x)
	{
		double y = x * this.a[0];

		for (int i = 0; i < this.buf.Count; ++i)
		{
			y += this.buf[i] * this.a[i + 1];
		}

		this.buf.Insert(x);
		return y;
	}

	public void Clear()
	{
		for (int n = this.buf.Count; n > 0; --n)
			this.buf.Insert(0);
	}

	#endregion
	#region ICloneable �����o

	public object Clone()
	{
		FirFilter f = new FirFilter(this.a);
		for (int i = 0; i < this.buf.Count; ++i)
		{
			f.GetValue(this.buf[i]);
		}
		return f;
	}

	#endregion
}
