using System;
using System.Collections;

namespace SoundLibrary.Filter
{
#if false
	// ��������ƃo�O����B
	// #else �̕��̎����Ɣ�ׂ�10�`20�����x�����B
	// 10�����x�̂��߂Ƀf�o�b�O����̂��ʓ|�ŁB
	/// <summary>
	/// �z�o�b�t�@
	/// </summary>
	public class CircularBuffer : IEnumerable
	{
		double[] buff;
		int current;
		int length;

		/// <summary>
		/// �z�o�b�t�@�R���X�g���N�^�B
		/// </summary>
		/// <param name="len">�z�o�b�t�@�̒����B</param>
		public CircularBuffer(int len)
		{
			this.buff   = new double[2 * len];
			this.length = len;
		}

		/// <summary>
		/// �z�o�b�t�@���̗v�f�̃A�N�Z�X�B
		/// </summary>
		public double this[int n]
		{
			set{this.buff[this.current + n] = value;}
			get{return this.buff[this.current + n];}
		}

		/// <summary>
		/// �z�o�b�t�@�̖����ɒl��}���B
		/// </summary>
		/// <param name="x">�}������l�B</param>
		public void PushBack(double x)
		{
			this.buff[this.current] = this.buff[this.current + this.length] = x;
			++this.current;
			if(this.current >= this.length) this.current -= this.length;
		}

		/// <summary>
		/// �z�o�b�t�@�̐擪�ɒl��}���B
		/// </summary>
		/// <param name="x">�}������l�B</param>
		public void PushFront(double x)
		{
			--this.current;
			if(this.current < 0) this.current += this.length;
			this.buff[this.current] = this.buff[this.current + this.length] = x;
		}

		/// <summary>
		/// �z�o�b�t�@�̐擪�̗v�f��Ԃ��B
		/// </summary>
		public double Top
		{
			get{return this.buff[this.current];}
		}

		/// <summary>
		/// �o�b�t�@��(��this.buff.Length)��Ԃ��B
		/// </summary>
		public int Length
		{
			get{return this.length;}
		}

		/// <summary>
		/// this.buff �̗񋓎q��Ԃ��B
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return this.buff.GetEnumerator();
		}

		/// <summary>
		/// �z�o�b�t�@�̒�����ύX����B
		/// </summary>
		/// <param name="length">�V��������</param>
		public void Resize(int length)
		{
			double[] tmp = new double[2 * length];
			int len = Math.Min(length, this.Length);

			int i=0;
			for(; i<len; ++i)
				tmp[i] = this[i];
			for(; i<length; ++i)
				tmp[i] = 0;
			for(int j=0; j<length; ++i, ++j)
				tmp[i] = tmp[j];
			this.buff = tmp;
		}
	}//class CircularBuffer

#else

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
		/// �z�o�b�t�@�̐擪�ɒl��}���B
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

		/// <summary>
		/// �z�o�b�t�@�̒�����ύX����B
		/// </summary>
		/// <param name="length">�V��������</param>
		public void Resize(int length)
		{
			double[] tmp = new double[length];
			int len = Math.Min(length, this.Length);
			int i=0;
			for(; i<len; ++i)
				tmp[i] = this[i];
			for(; i<length; ++i)
				tmp[i] = 0;
			this.buff = tmp;
		}

		/// <summary>
		/// ���g��0�N���A�B
		/// </summary>
		public void Clear()
		{
			for(int i=0; i<this.buff.Length; ++i)
			{
				this.buff[i] = 0;
			}
		}

		/// <summary>
		/// �W���Ƃ̐Ϙa���Z�B
		/// ��this[i]*coef[i] ���v�Z�B
		/// </summary>
		/// <param name="coef">�W��</param>
		/// <returns>�Ϙa����</returns>
		public double Mac(double[] coef)
		{
			int n = coef.Length;
			double sum = 0;
			for(int i=0; i<n; ++i)
			{
				sum += this.buff[i] * coef[i];
			}
			return sum;
		}

		/// <summary>
		/// �W���Ƃ̐Ϙa���Z�B
		/// ��this[i + offset]*coef[i] ���v�Z�B
		/// </summary>
		/// <param name="offset">�􍞂݂̊J�n�ʒu�I�t�Z�b�g</param>
		/// <param name="coef">�W��</param>
		/// <returns>�Ϙa����</returns>
		public double Mac(int offset, double[] coef)
		{
			int n = coef.Length;
			double sum = 0;
			for(int i=0, j=offset; i<n; ++i, ++j)
			{
				sum += this.buff[j] * coef[i];
			}
			return sum;
		}
	}//class CircularBuffer
#endif
}
