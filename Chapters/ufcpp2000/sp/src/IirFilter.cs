/// <summary>
/// FIR �t�B���^�B
/// </summary>
public class IirFilter : IFilter
{
	#region �t�B�[���h

	CircularBuffer buf;
	double c;
	double[] a; // ����W��
	double[] b; // ���q�W��

	#endregion
	#region �R���X�g���N�^

	public IirFilter() : this(0, 0, 0) { }

	/// <summary>
	/// �W�����w�肵�ď�����
	/// </summary>
	/// <param name="a0">����W�� a0</param>
	/// <param name="b0">����W�� b0</param>
	/// <param name="c">�Q�C��</param>
	public IirFilter(double a0, double b0, double c)
		: this(
		new double[] { a0 },
		new double[] { b0 },
		c)
	{
	}

	/// <summary>
	/// �W����z��Ŏw�肵�ď�����
	/// </summary>
	/// <param name="a">����W�����i�[�����z��</param>
	/// <param name="b">���q�W�����i�[�����z��</param>
	/// <param name="c">�Q�C��</param>
	public IirFilter(double[] a, double[] b, double c)
	{
		this.a = a;
		this.b = b;
		this.c = c;
		this.buf = new CircularBuffer(a.Length);
	}

	#endregion
	#region IFilter �����o

	/// <summary>
	/// �e���� n �ŁA
	/// t[n] = c * x[n] + �� a[i - 1] * t[n - i]
	/// y[n] =     t[n] + �� b[i - 1] * t[n - i]
	/// </summary>
	/// <param name="x">����</param>
	/// <returns>�t�B���^�o��</returns>
	public double GetValue(double x)
	{
		double t = this.c * x;
		double y = 0;

		for (int i = 0; i < this.buf.Count; ++i)
		{
			t += this.buf[i] * this.a[i];
			y += this.buf[i] * this.b[i];
		}

		y += t;
		this.buf.Insert(t);
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
		IirFilter f = new IirFilter(this.a, this.b, this.c);
		for (int i = 0; i < this.buf.Count; ++i)
		{
			f.GetValue(this.buf[i]);
		}
		return f;
	}

	#endregion
}
