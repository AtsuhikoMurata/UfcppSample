#define A

using System;

namespace SoundLibrary.Pipe.Monaural
{
	/// <summary>
	/// ���ԐL���E�Z�k�������s���N���X�B
	/// ���Ԋu�Ńf�[�^���Ԉ����E�R�s�[���邱�ƂŁA
	/// ������ς��邱�ƂȂ��Đ����Ԃ�L���E�Z�k����B
	/// </summary>
	public class TimeStretcher : Pipe
	{
		#region �萔

		const int DEFAULT_SIZE = 2048;
		const int DEFAULT_OVERLAP = 256;
		const int DEFAULT_MAXSKIP = 512;
		const double DEFAULT_RATE = 1.0;

		#endregion
		#region �t�B�[���h

		#region �����N���X�i�I�[�o�[���b�v�p�̈ꎞ�o�b�t�@�j

		struct OverlapBuffer
		{
			short[] buffer;
			int write; // �������݈ʒu
			int read;  // �ǂݍ��݈ʒu

			public OverlapBuffer(int size, int write)
			{
				this.buffer = new short[size];
				this.write = write;
				this.read = 0;
			}

			/// <summary>
			/// �L���[�̃T�C�Y�ݒ�B
			/// </summary>
			/// <param name="size">�T�C�Y</param>
			/// <param name="full">�o�b�t�@�̏�����ԁitrue: full, false: empty �ŊJ�n�j</param>
			public void SetSize(int size, int write)
			{
				this.buffer = new short[size];
				this.write = write;
				this.read = 0;
			}

			/// <summary>
			/// �v�f�����擾�B
			/// </summary>
			public int Count
			{
				get{return this.buffer.Length;}
			}

			public short this[int i]
			{
				get{return this.buffer[i];}
			}

			/// <summary>
			/// �l��}���B
			/// </summary>
			/// <param name="data">�l</param>
			public void Enqueue(short data)
			{
				this.buffer[this.write] = data;
				++this.write;
			}

			/// <summary>
			/// �l�����o���B
			/// </summary>
			/// <returns>�l</returns>
			public short Dequeue()
			{
				short data = this.buffer[this.read];
				++this.read;

				if(this.read == this.buffer.Length)
				{
					this.write = this.read = 0;
				}

				return data;
			}

			/// <summary>
			/// �������݃o�b�t�@���c���Ă��邩�ǂ����B
			/// </summary>
			/// <returns>�t���Ȃ� true</returns>
			public bool Full()
			{
				return this.write == this.buffer.Length;
			}

			/// <summary>
			/// �ǂݏo���f�[�^���c���Ă��邩�ǂ����B
			/// </summary>
			/// <returns>����ۂȂ� true</returns>
			public bool Empty()
			{
				return this.write == this.read;
			}
		}

		#endregion

		// �p�����[�^
		int size;
		int overlapSize;
		int frameSize;
		int maxSkip;

		int skip;
/*
�E�T�v
    �t���[�� 1                       �t���[�� 2
  (1)    (2)    (3)                (1)    (2)    (3)
|----|--------|----|-- ....... --|----|--------|----|-- ....

�t���[�� k ��(3)�ƃt���[�� k+1 ��(1) ���N���X�t�F�[�h�����āA2�̃t���[�����q���B
�������A�I�[�o�[���b�v���ɉ����ςɂȂ�Ȃ��悤�ɁA

k(3) �� K+1(1) �̑��֒l�������Ȃ�悤�Ƀt���[���̐擪���T���v������ǂݔ�΂��B
�i���֒l�ɂ���Č��߂��ǂݔ�΂��T���v�����͕ϐ� skip �ɕۑ����Ă����B�j

�֋X��A
(1) �� overlap(�O)
(2) �� non overlap
(3) �� overlap(��)
�ƌĂԁB

�E��L�̃p�����[�^�̈Ӗ�
  (1)    (2)    (3)                (1)    (2)    (3)
|----|--------|----|-- ....... --|----|--------|----|
<------------------------------->
<------------>      ��
<--->   ��       frameSize
  ��   size
overlapSize

frameSize = rate * size
���� frameSize �̃f�[�^������ size �ɐL�k����B
�� �Đ����Ԃ� 1/rate �{�ɁB
 */

		// ���݂̏��
		int current;
		// �I�[�o�[���b�v�p�̈ꎞ�o�b�t�@
		OverlapBuffer buffer;

		#endregion
		#region ������

		/// <summary>
		/// �f�t�H���g�l�ŏ������B
		/// </summary>
		/// <param name="input">���̓L���[</param>
		/// <param name="output">�o�̓L���[</param>
		public TimeStretcher(Queue input, Queue output)
			: this(input, output, DEFAULT_SIZE, DEFAULT_OVERLAP, DEFAULT_RATE)
		{
		}

		/// <summary>
		/// �p�����[�^�̐ݒ�B
		/// </summary>
		/// <param name="input">���̓L���[</param>
		/// <param name="output">�o�̓L���[</param>
		/// <param name="size">�u���b�N�T�C�Y</param>
		/// <param name="overlap">�I�[�o�[���b�v�����镔���̒���</param>
		/// <param name="rate">�ϊ����[�g�B�Đ����Ԃ� 1/rate �{�ɁB</param>
		public TimeStretcher(Queue input, Queue output, int size, int overlap, double rate)
			: this(input, output, size, overlap, rate, DEFAULT_MAXSKIP)
		{
		}

		public TimeStretcher(Queue input, Queue output, int size, int overlap, double rate, int maxSkip)
			: base(input, output)
		{
			this.SetParameter(size, overlap, rate, maxSkip);
		}

		/// <summary>
		/// �p�����[�^�̐ݒ�B
		/// </summary>
		/// <param name="size">�u���b�N�T�C�Y</param>
		/// <param name="overlap">�I�[�o�[���b�v�����镔���̒���</param>
		/// <param name="rate">�ϊ����[�g�B�Đ����Ԃ� 1/rate �{�ɁB</param>
		public void SetParameter(int size, int overlap, double rate, int maxSkip)
		{
			this.size = size;
			this.overlapSize = overlap;
			this.frameSize = (int)(size * rate);
			this.maxSkip = maxSkip;

			this.last = this.size < this.frameSize ?
				this.frameSize + this.maxSkip :
				this.size + this.overlapSize + this.maxSkip;

			this.buffer = new OverlapBuffer(overlap, overlap);
			this.current = 0;
			this.skip = 0;
		}

		#endregion
		#region ����

		int maxCorrelation;
		int skipNext;
		int last;

		int Correlation(int pos, int len)
		{
			long corr = 0;
			for(int i=0, j=pos; i<len; ++i, ++j)
			{
				short x = this.buffer[i];
				short y = this.input[j];

				corr += x * y;
			}
			return SoundLibrary.BitOperation.RoundShift(corr, 16);
#if false
			long corr = 0;
			long xabs = 0; long yabs = 0;

// �����ɑ��֒l���v�Z�i�v�Z�ʑ�j�B
// Corr = E[xy]^2 / E[x^2]E[y^2]
// �����܂ł���K�v�͂Ȃ����B

			for(int i=0, j=pos; i<len; ++i, ++j)
			{
				short x = this.buffer[i];
				short y = this.input[j];

				corr += x * y;
				xabs += x * x;
				yabs += y * y;
			}
			if(xabs == 0 || yabs == 0)
			{
				corr = int.MaxValue;
			}
			else
			{
				float temp = xabs;
				temp *= yabs;
				temp = (float)Math.Sqrt(temp);
				temp = corr / temp;
				corr = (long)(int.MaxValue * temp);
			}
			return (int)corr;
#endif
		}

		/// <summary>
		/// ���݈ʒu(current)���X�V����B
		/// ���łɁA�s�b�`�\���Ƃ��̏������s���B
		/// </summary>
		void MoveNext()
		{
			int len = this.buffer.Count;

			int start, offset;
			if(this.size < this.frameSize)
			{
				start = this.frameSize;
				offset = 0;
			}
			else
			{
				start = this.size + this.overlapSize;
				offset = this.frameSize - this.size;
			}

			if(this.current >= start &&
				this.current < start + this.maxSkip)
			{
				int corr = this.Correlation(this.current +offset, len);

				if(this.maxCorrelation < corr)
				{
					this.maxCorrelation = corr;
					this.skipNext = this.current + offset;
				}
			}

			++this.current;
		}

		/// <summary>
		/// ���݈ʒu�����Z�b�g����(current �� 0 ��)�B
		/// ���łɁA�X�L�b�v�ʂ��X�V�B
		/// </summary>
		void Reset()
		{
			this.skip = this.skipNext == 0 ? 0 : this.skipNext - this.frameSize;

			this.maxCorrelation = int.MinValue;
			this.skipNext = 0;
			this.current = 0;
		}

		public override void Process()
		{
			// overlap(�O)
			while(
				this.current < this.overlapSize &&
				this.input.Count > this.current + this.overlapSize + this.skip)
			{
				System.Diagnostics.Debug.Assert(!this.buffer.Empty());

				short temp1 = this.buffer.Dequeue();
				short temp2 = this.input[this.current + this.skip];
				short data = Interpolate(temp1, temp2, this.current, this.overlapSize);
				this.output.Enqueue(data);

				this.MoveNext();
			}

			// non overlap
			while(
				this.current >= this.overlapSize &&
				this.current < this.size &&
				this.input.Count > this.current + this.overlapSize + this.skip)
			{
				short data = this.input[this.current + this.skip];
				this.output.Enqueue(data);

				this.MoveNext();
			}

			// overlap(��)
			while(
				this.current >= this.size &&
				this.current < this.size + this.overlapSize &&
				this.input.Count > this.current + this.overlapSize + this.skip)
			{
				short data = this.input[this.current + this.skip];
				this.buffer.Enqueue(data);

				this.MoveNext();
			}

			// ���t���[���̊J�n�n�_�܂œǂݔ�΂��B
			while(
				this.current >= this.size + this.overlapSize &&
				this.current < this.last &&
				this.input.Count > this.current + this.overlapSize + this.skip)
			{
				this.MoveNext();
			}

			// ���̃t���[���Ɉڍs
			if(this.current == this.last)
			{
				if(this.input.Count >= this.frameSize)
				{
					this.input.Dequeue(this.frameSize);
					this.Reset();
				}
			}
		}

		/// <summary>
		/// �t���[���̃X�L�b�v�������߂�B
		/// �O�t���[���� overlap(��) �ƌ��t���[���� overlap(�O) �̑��ݑ��֒l�������Ȃ�悤�ɁA
		/// �t���[���̍ŏ����T���v�����X�L�b�v����B
		/// </summary>
		/// <remarks>
		/// �X�L�b�v�������肷�邽�߂ɁA���T���v�������̃f�[�^���Q�Ƃ��邽�߁A
		/// ���̓L���[�ɂ�����x�f�[�^�����܂�Ȃ��ƃX�L�b�v��������ł��Ȃ��B
		/// �X�L�b�v��������ł��Ȃ��Ԃ� false ��Ԃ��B
		/// </remarks>
		/// <returns>�X�L�b�v��������ł����ꍇ true ��Ԃ�</returns>
		bool SeekSkipSize()
		{
			if(this.input.Count < this.maxSkip + this.buffer.Count)
				return false;

			long corrMax = long.MinValue;
			this.skip = 0;

			for(int offset = 0; offset<this.maxSkip; ++offset)
			{
				long corr = 0;
				for(int i=0; i<this.buffer.Count; ++i)
					corr += this.input[i + offset] * this.buffer[i];

				if(corr > corrMax)
				{
					corrMax = corr;
					this.skip = offset;
				}
			}

			return true;
		}

		#endregion
		#region �����֐�

		/// <summary>
		/// a �� b �̐M�����N���X�t�F�[�h�����Ȃ��獬����B
		/// </summary>
		/// <param name="a">�M�� a</param>
		/// <param name="b">�M�� b</param>
		/// <param name="fade">������䗦</param>
		/// <param name="overlap">�������Ԃ̒���</param>
		static short Interpolate(short a, short b, int fade, int overlap)
		{
			int val = (overlap - fade) * a + fade * b;
			val /= overlap;
			return (short)val;
		}

		#endregion
	}
}
