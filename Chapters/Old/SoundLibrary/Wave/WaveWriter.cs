using System;
using System.IO;

namespace SoundLibrary.Wave
{
	/// <summary>
	/// RIFF Wave �`���̃t�@�C���ɉ����f�[�^���������ށB
	/// </summary>
	public class WaveWriter : IDisposable
	{
		BinaryWriter writer;
		FormatHeader header;
		uint dataLength = 0;

		public WaveWriter(){}

		/// <summary>
		/// �t�@�C�������w�肵�ĊJ���B
		/// </summary>
		/// <param name="filename">�������ݐ� Wave �t�@�C����</param>
		/// <param name="header">Wave �̃w�b�_</param>
		public WaveWriter(string filename, FormatHeader header)
		{
			this.Open(filename, header);
		}

		/// <summary>
		/// �X�g���[���ɏ������ށB
		/// </summary>
		/// <param name="writer">�������ݐ�X�g���[��</param>
		/// <param name="header">Wave �̃w�b�_</param>
		public WaveWriter(BinaryWriter writer, FormatHeader header)
		{
			this.Open(writer, header);
		}

		public void Dispose()
		{
			this.Close();
		}

		/// <summary>
		/// Wave �t�@�C�����J���B
		/// </summary>
		/// <param name="filename">Wave �t�@�C����</param>
		public void Open(string filename, FormatHeader header)
		{
			Open(new BinaryWriter(new BufferedStream(File.Create(filename))), header);
		}

		/// <summary>
		/// Wave �w�b�_(RIFF, fmt chunk, data chunk �̃f�[�^���܂�)���X�g���[���ɏ����o���B
		/// </summary>
		/// <param name="writer">�������ݐ�X�g���[��</param>
		/// <param name="header">Wave �̃w�b�_</param>
		/// <param name="length">�f�[�^��(�T���v����)</param>
		public static void WriteHeader(BinaryWriter writer, FormatHeader header, int length)
		{
			byte[] buf;

			length *= header.blockSize;

			writer.Write(Util.RIFF);
			writer.Write((uint)length + 36u);
			writer.Write(Util.WAVE);
			writer.Write(Util.FMT);
			writer.Write((uint)16);

			unsafe
			{
				buf = new byte[16];
				fixed(byte* p = buf)
				{
					*(FormatHeader*)p = header;
				}
			}
			writer.Write(buf);
			writer.Write(Util.DATA);
			writer.Write((uint)length);
		}

		/// <summary>
		/// �w�b�_�̃f�[�^���̕������C������B
		/// </summary>
		/// <param name="writer">�������ݐ�X�g���[��</param>
		/// <param name="length">�C����̃f�[�^��(�T���v����)</param>
		/// <param name="blockSize">�u���b�N�T�C�Y</param>
		public static void ModifyHeader(BinaryWriter writer, int length, int blockSize)
		{
			length = length * blockSize;

			writer.Seek(4, SeekOrigin.Begin);
			writer.Write((uint)length + 36u);

			writer.Seek(40, SeekOrigin.Begin);
			writer.Write((uint)length);
		}

		/// <summary>
		/// �f�[�^�����o���B
		/// </summary>
		/// <param name="writer">�������ݐ�X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="l">�������݂����f�[�^(L ch)</param>
		/// <param name="r">�������݂����f�[�^(R ch)</param>
		/// <returns></returns>
		public static int Write(BinaryWriter writer, FormatHeader header, double[] l, double[] r)
		{
			int i = 0;

			try
			{
				int length = l.Length;

				if(!header.IsStereo) // ���m����
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToByte(l[i]));
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToShort(l[i]));
						}
					}
				}
				else // �X�e���I
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToByte(l[i]));
							writer.Write(Util.ClipToByte(r[i]));
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToShort(l[i]));
							writer.Write(Util.ClipToShort(r[i]));
						}
					}
				}//�X�e���I
			}
			catch(IOException){return 0;}
			return i;
		}

		/// <summary>
		/// �f�[�^�����o���B
		/// </summary>
		/// <param name="writer">�������ݐ�X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="l">�������݂����f�[�^(L ch)</param>
		/// <param name="r">�������݂����f�[�^(R ch)</param>
		/// <returns></returns>
		public static int Write(BinaryWriter writer, FormatHeader header, float[] l, float[] r)
		{
			int i = 0;

			try
			{
				int length = l.Length;

				if(!header.IsStereo) // ���m����
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToByte(l[i]));
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToShort(l[i]));
						}
					}
				}
				else // �X�e���I
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToByte(l[i]));
							writer.Write(Util.ClipToByte(r[i]));
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToShort(l[i]));
							writer.Write(Util.ClipToShort(r[i]));
						}
					}
				}//�X�e���I
			}
			catch(IOException){return 0;}
			return i;
		}

		/// <summary>
		/// �f�[�^�����o���B
		/// </summary>
		/// <param name="writer">�������ݐ�X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="l">�������݂����f�[�^(L ch)</param>
		/// <param name="r">�������݂����f�[�^(R ch)</param>
		/// <returns></returns>
		public static int Write(BinaryWriter writer, FormatHeader header, short[] l, short[] r)
		{
			int i = 0;

			try
			{
				int length = l.Length;

				if(!header.IsStereo) // ���m����
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToByte(l[i]));
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToShort(l[i]));
						}
					}
				}
				else // �X�e���I
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToByte(l[i]));
							writer.Write(Util.ClipToByte(r[i]));
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(Util.ClipToShort(l[i]));
							writer.Write(Util.ClipToShort(r[i]));
						}
					}
				}//�X�e���I
			}
			catch(IOException){return 0;}
			return i;
		}

		/// <summary>
		/// Wave �t�@�C�����J���B
		/// </summary>
		/// <param name="reader">Wave �t�@�C�����i�[�����X�g���[��</param>
		public void Open(BinaryWriter writer, FormatHeader header)
		{
			if(this.writer != null)
			{
				this.writer.Close();
			}

			this.writer = writer;
			this.header = header;

			WaveWriter.WriteHeader(writer, header, 0); // �f�[�^���͉���0�����Ă����B
		}//Open

		/// <summary>
		/// Wave �t�@�C�������B
		/// </summary>
		public void Close()
		{
			if(writer == null) return;

			WaveWriter.ModifyHeader(this.writer, (int)this.dataLength, this.header.blockSize);

			writer.Close();
			writer = null;
		}

		/// <summary>
		/// �f�[�^�̏������݁B
		/// </summary>
		/// <param name="length">�������ރT���v�����B</param>
		/// <param name="l">���`���l���̃f�[�^�B</param>
		/// <param name="r">�E�`���l���̃f�[�^�B</param>
		/// <returns>���ۂɏ������񂾃T���v�����B</returns>
		public int Write(double[] l, double[] r)
		{
			if(this.writer == null) return 0;

			uint length = (uint)l.Length;
			int i = WaveWriter.Write(this.writer, this.header, l, r);

			this.dataLength += (uint)i;
			return i;
		}//Write

		/// <summary>
		/// �f�[�^�̏������݁B
		/// </summary>
		/// <param name="length">�������ރT���v�����B</param>
		/// <param name="l">���`���l���̃f�[�^�B</param>
		/// <param name="r">�E�`���l���̃f�[�^�B</param>
		/// <returns>���ۂɏ������񂾃T���v�����B</returns>
		public int Write(float[] l, float[] r)
		{
			if(this.writer == null) return 0;

			uint length = (uint)l.Length;
			int i = WaveWriter.Write(this.writer, this.header, l, r);
	
			this.dataLength += (uint)i;
			return i;
		}//Write

		/// <summary>
		/// �f�[�^�̏������݁B
		/// </summary>
		/// <param name="length">�������ރT���v�����B</param>
		/// <param name="l">���`���l���̃f�[�^�B</param>
		/// <param name="r">�E�`���l���̃f�[�^�B</param>
		/// <returns>���ۂɏ������񂾃T���v�����B</returns>
		public int Write(short[] l, short[] r)
		{
			if(this.writer == null) return 0;

			uint length = (uint)l.Length;
			int i = WaveWriter.Write(this.writer, this.header, l, r);
	
			this.dataLength += (uint)i;
			return i;
		}//Write

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂܏������ށB
		/// </summary>
		/// <param name="writer">�������ݐ�</param>
		/// <param name="data">�������ރf�[�^</param>
		public static void WriteRawData(BinaryWriter writer, byte[] data)
		{
			writer.Write(data);
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂܏������ށB
		/// </summary>
		/// <param name="writer">�������ݐ�</param>
		/// <param name="data">�������ރf�[�^</param>
		/// <param name="length">�������ޒ���(�o�C�g��)</param>
		public static void WriteRawData(BinaryWriter writer, byte[] data, int length)
		{
			writer.Write(data, 0, length);
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂܏������ށB
		/// </summary>
		/// <param name="data">�������ރf�[�^</param>
		public void WriteRawData(byte[] data)
		{
			this.WriteRawData(data, data.Length);
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂܏������ށB
		/// </summary>
		/// <param name="data">�������ރf�[�^</param>
		/// <param name="length">�������ޒ���(�o�C�g��)</param>
		public void WriteRawData(byte[] data, int length)
		{
			WaveWriter.WriteRawData(this.writer, data, length);
			this.dataLength += (uint)(length / this.header.blockSize);
		}

		public uint Length
		{
			get{return this.dataLength;}
		}

		/// <summary>
		/// 1�T���v���ǂݏo���B
		/// ���m����16�r�b�g�ȊO�̏ꍇ�A�T�|�[�g�ΏۊO�B
		/// </summary>
		/// <param name="data">1�T���v�����̃f�[�^</param>
		public void WriteShort(short data)
		{
			if(!this.header.Is16Bit || this.header.IsStereo)
				return;

			this.writer.Write(data);
			++this.dataLength;
		}

		public void WriteShort(short l, short r)
		{
			System.Diagnostics.Debug.Assert(this.header.Is16Bit);

			if(this.header.IsStereo)
			{
				this.writer.Write(l);
				this.writer.Write(r);
				++this.dataLength;
			}
			else
			{
				this.writer.Write(l);
				++this.dataLength;
			}
		}
	}//class WaveWriter
}
