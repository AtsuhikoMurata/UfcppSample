/// <summary>
/// �x����B
/// </summary>
public class Delay : IFilter
{
	#region �t�B�[���h

	CircularBuffer buf;

	#endregion
	#region �R���X�g���N�^

	public Delay() : this(1) { }

	/// <summary>
	/// �{�����w�肵�ď�����
	/// </summary>
	/// <param name="delaytime">�x������[sample��]</param>
	public Delay(int delaytime)
	{
		this.buf = new CircularBuffer(delaytime);
	}

	#endregion
	#region IFilter �����o

	public double GetValue(double x)
	{
		double y = this.buf[this.buf.Count - 1];
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
		Delay d = new Delay(this.buf.Count);
		for (int i = 0; i < this.buf.Count; ++i)
		{
			d.GetValue(this.buf[i]);
		}
		return d;
	}

	#endregion
}
