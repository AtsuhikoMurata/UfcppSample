using System;
using SoundLibrary.Wave;

namespace SoundLibrary.Stream.Monaural
{
	/// <summary>
	/// WaveReader ����f�[�^��ǂݏo�������邾���̃X�g���[���B
	/// </summary>
	public class WaveStream : Stream, IDisposable
	{
		#region �t�B�[���h

		WaveReader reader;
		byte[] buffer;

		#endregion
		#region ������

		public WaveStream(WaveReader reader)
		{
			this.reader = reader;
			this.buffer = null;
		}

		#endregion
		#region Stream �����o

		public override int FillBuffer(short[] buffer, int offset, int size)
		{
			if(this.buffer == null || this.buffer.Length < size * 2)
			{
				this.buffer = new byte[size * 2];
			}

			size = this.reader.ReadRawData(this.buffer, 0, size * 2);

			SoundLibrary.Wave.Util.MemCopy(this.buffer, 0, buffer, offset, size);

			return size / 2;
		}

		public override bool Skip(int size)
		{
			return this.reader.Skip(size);
		}

		#endregion
		#region IDisposable �����o

		public void Close()
		{
			this.reader.Close();
		}

		public void Dispose()
		{
			this.Close();
		}

		#endregion
	}
}
