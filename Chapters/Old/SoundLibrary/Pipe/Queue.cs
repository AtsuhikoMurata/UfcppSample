using System;

namespace SoundLibrary.Pipe
{
	/// <summary>
	/// FIFO �̃o�b�t�@�B
	/// </summary>
	public class Queue
	{
		#region �t�B�[���h

		short[] buffer;
		int mask;
		int bottom;
		int top;

		#endregion
		#region ������

		/// <summary>
		/// �ő�v�f�� max �̃L���[���쐬�B
		/// </summary>
		/// <param name="max">�ő�v�f��</param>
		public Queue(int max)
		{
			max = SoundLibrary.BitOperation.CeilPower2(max);

			this.buffer = new short[max];
			this.mask = max - 1;
			this.bottom = 0;
			this.top = 0;
		}

		#endregion
		#region �L���[�̑���

		/// <summary>
		/// �L���[���󂩂ǂ����B
		/// </summary>
		/// <returns>��̂Ƃ� true</returns>
		public bool IsEmpty
		{
			get{ return this.bottom == this.top; }
		}

		/// <summary>
		/// �L���[�̗v�f�����擾�B
		/// </summary>
		public int Count
		{
			get
			{
				return this.mask & (this.bottom - this.top);
			}
		}

		/// <summary>
		/// �L���[�ɒl������B
		/// </summary>
		/// <param name="val">�l</param>
		public void Enqueue(short val)
		{
			this.buffer[this.bottom] = val;
			this.bottom = this.mask & (this.bottom + 1);
		}

		/// <summary>
		/// �L���[�̐擪�̗v�f�����o���B
		/// �i�l�͏o�͂��Ȃ��B
		///   STL �� queue �̂悤�ɁAFront �Ŏ��o���Ă��� Dequeue ����悤�ɁB�j
		/// </summary>
		public void Dequeue()
		{
			this.Dequeue(1);
		}

		/// <summary>
		/// �L���[���� n �v�f���o���B
		/// </summary>
		public void Dequeue(int n)
		{
			this.top = this.mask & (this.top + n);
		}

		/// <summary>
		/// �擪�̗v�f��ǂݏo���B
		/// </summary>
		public short Front
		{
			get{return this.buffer[this.top];}
		}

		/// <summary>
		/// �l�̓ǂݏ����B
		/// </summary>
		public short this[int i]
		{
			get
			{
				i = this.mask & (this.top + i);
				return this.buffer[i];
			}
			set
			{
				i = this.mask & (this.top + i);
				this.buffer[i] = value;
			}
		}

		#endregion
	}
}
