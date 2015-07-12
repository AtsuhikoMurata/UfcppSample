using System;
using System.Collections;

namespace Filter
{
	/// <summary>
	/// �z�o�b�t�@
	/// </summary>
	public class CircularBuffer : IEnumerable
	{
		double[] buff;

		/// <summary>
		/// �z�o�b�t�@�R���X�g���N�^�B
		/// </summary>
		/// <param name="len">�z�o�b�t�@�̒����B</param>
		public CircularBuffer(int len)
		{
			this.buff = new double[len];
		}

		/// <summary>
		/// �z�o�b�t�@���̗v�f�̃A�N�Z�X�B
		/// </summary>
		public double this[int n]
		{
			set{this.buff[n] = value;}
			get{return this.buff[n];}
		}

		/// <summary>
		/// �z�o�b�t�@�̖����ɒl��}���B
		/// </summary>
		/// <param name="x">�}������l�B</param>
		public void PushBack(double x)
		{
			for(int i=0; i<this.buff.Length-1; ++i)
			{
				this.buff[i] = this.buff[i+1];
			}
			this.buff[this.buff.Length-1] = x;
		}

		/// <summary>
		/// �z�o�b�t�@�̖����ɒl��}���B
		/// </summary>
		/// <param name="x">�}������l�B</param>
		public void PushFront(double x)
		{
			for(int i=this.buff.Length-1; i>0; --i)
			{
				this.buff[i] = this.buff[i-1];
			}
			this.buff[0] = x;
		}

		/// <summary>
		/// �z�o�b�t�@�̐擪�̗v�f��Ԃ��B
		/// </summary>
		public double Top
		{
			get{return this.buff[0];}
		}

		/// <summary>
		/// �o�b�t�@��(��this.buff.Length)��Ԃ��B
		/// </summary>
		public int Length
		{
			get{return this.buff.Length;}
		}

		/// <summary>
		/// this.buff �̗񋓎q��Ԃ��B
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return this.buff.GetEnumerator();
		}
	}//class CircularBuffer
}
