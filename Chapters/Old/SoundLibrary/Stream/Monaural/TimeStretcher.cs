using System;

namespace SoundLibrary.Stream.Monaural
{
	/// <summary>
	/// TimeStretcher �̊T�v�̐����ł��B
	/// </summary>
	public class TimeStretcher : Stream
	{
		#region �t�B�[���h

		Stream stream; // inner stream
		double rate; // �ϊ����[�g�B�Đ����Ԃ� 1/rate �{�ɁB
		short[] inputBuffer; // ���͗p�o�b�t�@�B
		int readSize; // ���ɓǂݏo���ꂽ��Ԃɂ���f�[�^�̒����B
		short[] overlapBuffer; // �I�[�o�[���b�v�p�o�b�t�@�B
		int margin; // �I�t�Z�b�g�T���p�̃}�[�W���B

		const int DEFAULT_OVERLAP = 512;
		const int DEFAULT_MARGIN = 1024;

		#endregion
		#region ������

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="stream">�����X�g���[��</param>
		/// <param name="rate">�ϊ����[�g</param>
		public TimeStretcher(Stream stream, double rate) : this(stream, rate, DEFAULT_OVERLAP, DEFAULT_MARGIN)
		{
		}

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="stream">�����X�g���[��</param>
		/// <param name="rate">�ϊ����[�g</param>
		/// <param name="overlapSize">�I�[�o�[���b�v�����̒���</param>
		public TimeStretcher(Stream stream, double rate, int overlapSize, int margin)
		{
			this.stream = stream;
			this.rate = rate;
			this.inputBuffer = null;
			this.readSize = 0;
			this.overlapBuffer = new short[overlapSize];
			this.margin = margin;
		}

		#endregion
		#region �v���p�e�B

		/// <summary>
		/// �ϊ����[�g�B
		/// �Đ����Ԃ� 1/rate �{�ɁB
		/// </summary>
		public double Rate
		{
			get{return this.rate;}
			set{this.rate = value;}
		}

		/// <summary>
		/// �ϊ����[�g�B
		/// �Đ����Ԃ� temp �{�ɁB
		/// </summary>
		public double Tempo
		{
			get{return 1 / this.rate;}
			set{this.rate = 1 / value;}
		}

		/// <summary>
		/// �ǂݏo���f�[�^�̃}�[�W���B
		/// </summary>
		public int Margin
		{
			get{return this.margin;}
			set{this.margin = value;}
		}

		#endregion
		#region Stream �����o

		/// <remarks>
		/// size �� overlapSize �����������Ƃ��A����ۏؑΏۊO�B
		/// </remarks>
		public override int FillBuffer(short[] buffer, int offset, int size)
		{
			// �f�[�^����̓X�g���[������ǂݏo���B
			int overlap = this.overlapBuffer.Length;

			int frameSize = (int)(size * this.rate);
			int inputSize = size + overlap + this.margin;

			if(this.inputBuffer == null || this.inputBuffer.Length < inputSize)
			{
				this.inputBuffer = new short[inputSize];
			}

			if(inputSize > this.readSize)
			{
				int readSize = this.stream.FillBuffer(this.inputBuffer, this.readSize, inputSize - this.readSize);

				if(readSize + this.readSize != inputSize)
				{
					size = Math.Min(readSize + this.readSize, size);
					for(int i=readSize + this.readSize; i<inputSize; ++i)
					{
						this.inputBuffer[i] = 0;
					}
					inputSize = readSize + this.readSize;
				}
			}

			// �t���[���J�n�I�t�Z�b�g�̌���B
			int frameOffset = GetOffset(this.overlapBuffer, this.inputBuffer, this.margin);

			// �I�[�o�[���b�v�����̃R�s�[�B
			Crossfade(this.overlapBuffer, this.inputBuffer, frameOffset, buffer);

			if(size > overlap)
			{
				// ��I�[�o�[���b�v�����̃R�s�[�B
				SoundLibrary.Wave.Util.MemCopy(this.inputBuffer, overlap + frameOffset, buffer, overlap, size - overlap);

				// ���̃t���[���p�̃I�[�o�[���b�v�f�[�^���ꎞ�o�b�t�@�ɃR�s�[�B
				SoundLibrary.Wave.Util.MemCopy(this.inputBuffer, size + frameOffset, this.overlapBuffer, 0, overlap);
			}

			// ���̃t���[���̏����B
			if(inputSize > frameSize)
			{
				// �ߏ�ɓǂ񂾕��͎c���Ă����B
				SoundLibrary.Wave.Util.MemCopy(this.inputBuffer, frameSize, this.inputBuffer, 0, inputSize - frameSize);
				this.readSize = inputSize - frameSize;
			}
			else
			{
				// �ǂݑ���Ȃ����A��ǂ݂���B
				int skipSize = frameSize - inputSize;
				this.stream.Skip(skipSize);
				this.readSize = 0;
			}

			return size;
		}

		public override bool Skip(int size)
		{
			int inputSize = (int)(size * this.rate);
			return this.stream.Skip(inputSize);
		}

		#endregion
		#region �����֐�

		/// <summary>
		/// a �� b �̐M�����N���X�t�F�[�h�����Ȃ��獬����B
		/// </summary>
		/// <param name="a">�M�� a</param>
		/// <param name="b">�M�� b</param>
		/// <param name="offset">b �̃I�t�Z�b�g</param>
		/// <param name="dest">�������M���̏������ݐ�</param>
		static void Crossfade(short[] a, short[] b, int offset, short[] dest)
		{
			int len = a.Length;

			for(int i=0; i<len; ++i)
			{
				int val = ((len - i) * a[i] + i * b[i + offset]) / len;
				dest[i] = (short)val;
			}
		}

		/// <summary>
		/// 2�̐M�� a �� b ��������Ƃ��A�ł���a���Ȃ�������ʒu�I�t�Z�b�g��T���B
		/// a �� b �̑��ݑ��ւ��ł������ʒu��T���B
		/// </summary>
		/// <param name="a">�M�� a</param>
		/// <param name="b">�M�� b</param>
		/// <param name="max">�T���͈� [0, max)</param>
		/// <returns>�ʒu�I�t�Z�b�g</returns>
		static int GetOffset(short[] a, short[] b, int max)
		{
			//return 0;
			//*
			double maxVal = int.MinValue;
			int offset = 0;
			for(int i=0; i<max; ++i)
			{
				double x = Correlation(a, b, i);
				if(x > maxVal)
				{
					maxVal = x;
					offset = i;
				}
			}
			return offset;
			//*/
		}

		/// <summary>
		/// a �� b+offset �̑��֒l�����߂�B
		/// </summary>
		/// <param name="a">�M�� a</param>
		/// <param name="b">�M�� b</param>
		/// <param name="offset">b �̃I�t�Z�b�g</param>
		/// <returns></returns>
		static double Correlation(short[] a, short[] b, int offset)
		{
			double x = 0;
			double ax = 0;
			double bx = 0;
			for(int i=0; i<a.Length; ++i)
			{
				short ai = a[i];
				short bi = b[i + offset];
				x += ai * bi;
				ax += ai * ai;
				bx += bi * bi;
			}
			ax *= bx;
			if(ax == 0)
				return int.MaxValue;
			return x * x / ax;
		}

		#endregion
	}
}
