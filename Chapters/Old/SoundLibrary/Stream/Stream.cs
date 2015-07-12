using System;

namespace SoundLibrary.Stream
{
	/// <summary>
	/// �����X�g���[���p�C���^�t�F�[�X�B
	/// �߂�ǂ�������ŁA16�r�b�gPCM����B
	/// ���m�����ŁB
	/// </summary>
	public abstract class Stream
	{
		/// <summary>
		/// �o�b�t�@�Ƀf�[�^��ǂݍ��ށB
		/// </summary>
		/// <param name="buffer">�f�[�^�ǂݍ��ݐ�̃o�b�t�@</param>
		/// <param name="offset">�f�[�^���������ݎn�߂�ꏊ�̃I�t�Z�b�g</param>
		/// <param name="size">�ǂݏo�������f�[�^��</param>
		/// <returns>���ۂɓǂݍ��񂾃f�[�^��</returns>
		public abstract int FillBuffer(short[] buffer, int offset, int size);

		/// <summary>
		/// �o�b�t�@�Ƀf�[�^��ǂݍ��ށB
		/// </summary>
		/// <param name="buffer">�f�[�^�ǂݍ��ݐ�̃o�b�t�@</param>
		/// <returns>���ۂɓǂݍ��񂾃f�[�^��</returns>
		public int FillBuffer(short[] buffer)
		{
			return this.FillBuffer(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// �f�[�^����ǂ݂���B
		/// </summary>
		/// <param name="size">��ǂ݂������T�C�Y</param>
		/// <returns>���ۂ���ǂ݂����T�C�Y</returns>
		public abstract bool Skip(int size);
	}

	/// <summary>
	/// �����X�g���[���Ƀo�b�t�@��t�������́B
	/// </summary>
	public class BufferedStream
	{
		#region �萔�E�t�B�[���h

		const int DEFAULT_BUFFER_SIZE = 1024;

		Stream stream;
		short[] buffer;
		int current;
		int last;

		#endregion
		#region ������

		/// <summary>
		/// �f�t�H���g�T�C�Y�̃o�b�t�@��p�ӁB
		/// </summary>
		/// <param name="stream">���͌��̃X�g���[��</param>
		public BufferedStream(Stream stream) : this(stream, DEFAULT_BUFFER_SIZE) {}

		/// <summary>
		/// �o�b�t�@�T�C�Y���w�肵�ăo�b�t�@��p�ӁB
		/// </summary>
		/// <param name="stream">���͌��̃X�g���[��</param>
		/// <param name="size">�o�b�t�@�T�C�Y</param>
		public BufferedStream(Stream stream, int size) : this(stream, new short[size]) {}

		/// <summary>
		/// �o�b�t�@�𒼐ڎw��B
		/// </summary>
		/// <param name="stream">���͌��̃X�g���[��</param>
		/// <param name="buffer">�o�b�t�@</param>
		public BufferedStream(Stream stream, short[] buffer)
		{
			this.stream = stream;
			this.buffer = buffer;
			this.current = -1;
			this.last = 0;
		}

		#endregion
		#region �l�̎擾�Ȃ�

		/// <summary>
		/// ���̃f�[�^�Ɉړ��B
		/// </summary>
		/// <returns>�܂��f�[�^���o�b�t�@���Ɏc���Ă��邩�ǂ���</returns>
		public bool MoveNext()
		{
			++this.current;

			if(this.current >= this.last)
			{
				this.last = this.stream.FillBuffer(this.buffer);				
				this.current = 0;
			}

			return this.last != 0;
		}

		/// <summary>
		/// �l�̎擾�B
		/// </summary>
		public short Value
		{
			get{return this.buffer[this.current];}
		}

		#endregion
	}
}
