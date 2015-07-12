using System;

namespace SoundLibrary.Stream.Monaural
{
	/// <summary>
	/// �ڒ��X�g���[���B
	/// �ʂ̃X�g���[������ǂݏo���������ڒ����ďo�͂���B
	/// �����ƍĐ����x�̗������ς��B
	/// ���`��Ԃɂ��A�b�v�T���v��/�_�E���T���v�����Ă��邾���Ȃ̂ŁA
	/// ��������ڎw���Ȃ�A�ʓr�A�A���`�G�C���A�X�t�B���^��������K�v������B
	/// </summary>
	public class RateTransposer : Stream
	{
		#region �t�B�[���h

		Stream stream; // inner stream
		double rate; // �ϊ����[�g�B������ rate �A�Đ����Ԃ� 1/rate �{�ɁB
		short[] inputBuffer; // ���͗p�o�b�t�@�B
		short prev; // 1���O�̃f�[�^���ꎞ�I�ɕۑ����Ă����B

		#endregion
		#region ������

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="stream">�����X�g���[��</param>
		/// <param name="rate">�ϊ����[�g</param>
		public RateTransposer(Stream stream, double rate)
		{
			this.stream = stream;
			this.rate = rate;
			this.inputBuffer = null;
			this.prev = 0;
		}

		#endregion
		#region �v���p�e�B

		/// <summary>
		/// �ϊ����[�g�B
		/// ������ rate �A�Đ����Ԃ� 1/rate �{�ɁB
		/// </summary>
		public double Rate
		{
			get{return this.rate;}
			set{this.rate = value;}
		}

		#endregion
		#region Stream �����o

		public override int FillBuffer(short[] buffer, int offset, int size)
		{
			// �f�[�^����̓X�g���[������ǂݏo���B

			int inputSize = (int)(size * this.rate);

			if(this.inputBuffer == null || this.inputBuffer.Length < inputSize)
			{
				this.inputBuffer = new short[inputSize];
			}

			int readSize = this.stream.FillBuffer(this.inputBuffer, 0, inputSize);

			if(readSize != inputSize)
			{
				size = (int)(readSize / this.rate);
			}

			double delta = this.rate / 2;

			// �T���v�����O���[�g��ύX���o�̓o�b�t�@�Ƀf�[�^���R�s�[�B
			// Bresenham �A���S���Y���I�ȓ���B

			for(int i=0, j=offset;;)
			{
				while(delta >= 0)
				{
					buffer[j] = Interpolate(delta, this.prev, this.inputBuffer[i]); //! ������Aprev �� input[i] ���t�B
					this.prev = this.inputBuffer[i]; //! �� ������� delta < 0 �̑��ɂ���ׂ��ł́H

					delta -= this.rate;
					++j;
					if(j - offset >= size)
						goto END;
				}

				while(delta < 0)
				{
					delta += 1;
					++i;
					if(i >= inputSize)
						goto END;
				}
				//! ��
				// while(!(j >= size + offset || i >= inputSize))
				//   if(delta >= 0) ....
				//   else ....
				// �̕������R�Ȃ̂ł́H
			}
			END:
			return size;
		}

		public override bool Skip(int size)
		{
			int inputSize = (int)(size * this.rate);
			return this.stream.Skip(inputSize);
		}

		#endregion
		#region �⏕�֐�(private)

		/// <summary>
		/// ���`��Ԋ֐��B
		/// </summary>
		/// <param name="delta">val1 �� val2 �������銄���i���������͖��������j</param>
		/// <param name="val1">�l1</param>
		/// <param name="val2">�l2</param>
		/// <returns></returns>
		static short Interpolate(double delta, short val1, short val2)
		{
			delta -= (int)delta;
			double val = val1 + delta * (val2 - val1);
#if CHECK_RANGE
			if(val > short.MaxValue) val = short.MaxValue;
			if(val < short.MinValue) val = short.MinValue;
#endif
			return (short)val;
		}

		#endregion
	}
}
