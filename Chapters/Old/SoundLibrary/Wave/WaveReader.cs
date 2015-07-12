using System;
using System.IO;

namespace SoundLibrary.Wave
{
	/// <summary>
	/// RIFF Wave �`���̃t�@�C�����特���f�[�^��ǂݏo���B
	/// </summary>
	public class WaveReader : IDisposable
	{
		BinaryReader reader;
		FormatHeader header;
		uint dataLength = 0;

		public WaveReader(){}

		/// <summary>
		/// �t�@�C�������w�肵�ĊJ���B
		/// </summary>
		/// <param name="filename">Wave �t�@�C����</param>
		public WaveReader(string filename)
		{
			this.Open(filename);
		}

		/// <summary>
		/// �X�g���[������J���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		public WaveReader(BinaryReader reader)
		{
			this.Open(reader);
		}

		public void Dispose()
		{
			this.Close();
		}

		/// <summary>
		/// Wave �t�@�C���X�g���[������w�b�_��ǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����̃X�g���[��</param>
		/// <returns>�ǂݏo�����w�b�_</returns>
		public static FormatHeader ReadHeader(BinaryReader reader)
		{
			byte[] buf;

			buf= reader.ReadBytes(4);

			if(!Util.Equal(buf, Util.RIFF))
			{
				throw new WaveException("���̃t�@�C����RIFF�`���ł͂���܂���B");
			}

			reader.ReadBytes(4); //�t�@�C���T�C�Y�ǂݔ�΂��B

			buf= reader.ReadBytes(4);
			if(!Util.Equal(buf, Util.WAVE))
			{
				throw new WaveException("���̃t�@�C����wave�`���ł͂���܂���B");
			}

			// fmt chunk �ǂݏo��
			FormatHeader header;
			while(true)
			{
				buf = reader.ReadBytes(4);
				int length = reader.ReadInt32();
				byte[] data = reader.ReadBytes(length);

				if(length < 16)
				{
					throw new WaveException("�w�b�_�����Z�����܂��B");
				}
				if(Util.Equal(buf, Util.FMT))
				{
					unsafe
					{
						fixed(byte* p= data)
						{
							header = *(FormatHeader*)p;
						}
					}
					break;
				}
			}
			return header;
		}

		/// <summary>
		/// Wave �t�@�C���X�g���[������f�[�^�`�����N��T���B
		/// fmt chunk ���� data chunk �����ɂ���ƌ����O��ŁA
		/// ReadHeader �̌�ɌĂяo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����̃X�g���[��</param>
		/// <returns>�f�[�^�`�����N�T�C�Y(�o�C�g��)</returns>
		public static int ReadDataChunk(BinaryReader reader)
		{
			int length = 0;

			while(true)
			{
				byte[] buf = reader.ReadBytes(4);
				length = reader.ReadInt32();

				if(length < 16)
				{
					throw new WaveException("�w�b�_�����Z�����܂��B");
				}
				if(Util.Equal(buf, Util.DATA))
				{
					break;
				}
				reader.ReadBytes(length);
			}
			return length;
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂ܓǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo����</param>
		/// <param name="data">�ǂݏo����</param>
		/// <returns>�ǂݏo�����f�[�^</returns>
		public static int ReadRawData(BinaryReader reader, byte[] data)
		{
			return ReadRawData(reader, data, 0, data.Length);
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂ܓǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo����</param>
		/// <param name="data">�ǂݏo����</param>
		/// <param name="offset">�ǂݏo����̊J�n�n�_</param>
		/// <returns>���ۓǂݏo�����f�[�^�̒���</returns>
		public static int ReadRawData(BinaryReader reader, byte[] data, int offset)
		{
			return ReadRawData(reader, data, offset, data.Length - offset);
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂ܓǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo����</param>
		/// <param name="data">�ǂݏo����</param>
		/// <param name="offset">�ǂݏo����̊J�n�n�_</param>
		/// <param name="length">�ǂݏo������(�o�C�g��)</param>
		/// <returns>���ۓǂݏo�����f�[�^�̒���</returns>
		public static int ReadRawData(BinaryReader reader, byte[] data, int offset, int length)
		{
			return reader.Read(data, offset, length);
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂ܓǂݏo���B
		/// </summary>
		/// <param name="data">�Ǎ���</param>
		/// <param name="offset">�Ǎ���̊J�n�I�t�Z�b�g(�o�C�g��)</param>
		/// <param name="length">�ǂݏo������(�o�C�g��)</param>
		/// <returns>���ۂɓǂݍ��񂾒���(�o�C�g��)</returns>
		public int ReadRawData(byte[] data, int offset, int length)
		{
			if(this.dataLength * this.header.blockSize < length)
				length = (int)(this.dataLength * this.header.blockSize);

			length = WaveReader.ReadRawData(this.reader, data, offset, length);
			this.dataLength -= (uint)(length / this.header.blockSize);
			return length;
		}

		/// <summary>
		/// Wave �̐��f�[�^�����̂܂ܓǂݏo���B
		/// </summary>
		/// <param name="data">�f�[�^�i�[��</param>
		/// <returns>�ǂݍ��񂾒���(�T���v����)</returns>
		public int ReadRawData(byte[] data)
		{
			return this.ReadRawData(data, 0, data.Length);
		}

		/// <summary>
		/// �f�[�^�ǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="length">�ǂݏo����������</param>
		/// <param name="l">�ǂݏo����z��(L ch)</param>
		/// <param name="r">�ǂݏo����z��(R ch)</param>
		/// <returns>�ǂݏo�����f�[�^�T���v����</returns>
		public static int Read(BinaryReader reader, FormatHeader header, double[]l, double[] r)
		{
			uint length = (uint)l.Length;

			int i = 0;
			try
			{
				if(!header.IsStereo) // ���m����
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)(reader.ReadByte() - 128);
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)reader.ReadInt16();
						}
					}
				}
				else //�X�e���I
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)(reader.ReadByte() - 128);
							r[i] = (double)(reader.ReadByte() - 128);
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)reader.ReadInt16();
							r[i] = (double)reader.ReadInt16();
						}
					}
				}//�X�e���I
			}
			catch(EndOfStreamException){}
			catch(IOException){}

			return i;
		}

		/// <summary>
		/// �f�[�^�ǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="length">�ǂݏo����������</param>
		/// <param name="l">�ǂݏo����z��(L ch)</param>
		/// <param name="r">�ǂݏo����z��(R ch)</param>
		/// <returns>�ǂݏo�����f�[�^�T���v����</returns>
		public static int Read(BinaryReader reader, FormatHeader header, uint length, out double[]l, out double[] r)
		{
			if(header.IsStereo)
			{
				l = new double[length];
				r = new double[length];
			}
			else
			{
				l = new double[length];
				r = null;
			}
			return WaveReader.Read(reader, header, l, r);
		}

		/// <summary>
		/// �f�[�^�ǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="length">�ǂݏo����������</param>
		/// <param name="l">�ǂݏo����z��(L ch)</param>
		/// <param name="r">�ǂݏo����z��(R ch)</param>
		/// <returns>�ǂݏo�����f�[�^�T���v����</returns>
		public static int Read(BinaryReader reader, FormatHeader header, float[]l, float[] r)
		{
			uint length = (uint)l.Length;

			int i = 0;
			try
			{
				if(!header.IsStereo) // ���m����
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)(reader.ReadByte() - 128);
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)reader.ReadInt16();
						}
					}
				}
				else //�X�e���I
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)(reader.ReadByte() - 128);
							r[i] = (float)(reader.ReadByte() - 128);
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)reader.ReadInt16();
							r[i] = (float)reader.ReadInt16();
						}
					}
				}//�X�e���I
			}
			catch(EndOfStreamException){}
			catch(IOException){}

			return i;
		}

		/// <summary>
		/// �f�[�^�ǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="length">�ǂݏo����������</param>
		/// <param name="l">�ǂݏo����z��(L ch)</param>
		/// <param name="r">�ǂݏo����z��(R ch)</param>
		/// <returns>�ǂݏo�����f�[�^�T���v����</returns>
		public static int Read(BinaryReader reader, FormatHeader header, uint length, out float[]l, out float[] r)
		{
			if(header.IsStereo)
			{
				l = new float[length];
				r = new float[length];
			}
			else
			{
				l = new float[length];
				r = null;
			}
			return WaveReader.Read(reader, header, l, r);
		}

		/// <summary>
		/// �f�[�^�ǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="length">�ǂݏo����������</param>
		/// <param name="l">�ǂݏo����z��(L ch)</param>
		/// <param name="r">�ǂݏo����z��(R ch)</param>
		/// <returns>�ǂݏo�����f�[�^�T���v����</returns>
		public static int Read(BinaryReader reader, FormatHeader header, short[]l, short[] r)
		{
			uint length = (uint)l.Length;

			int i = 0;
			try
			{
				if(!header.IsStereo) // ���m����
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (short)(reader.ReadByte() - 128);
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (short)reader.ReadInt16();
						}
					}
				}
				else //�X�e���I
				{
					if(!header.Is16Bit)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (short)(reader.ReadByte() - 128);
							r[i] = (short)(reader.ReadByte() - 128);
						}
					}
					else
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (short)reader.ReadInt16();
							r[i] = (short)reader.ReadInt16();
						}
					}
				}//�X�e���I
			}
			catch(EndOfStreamException){}
			catch(IOException){}

			return i;
		}

		/// <summary>
		/// �f�[�^�ǂݏo���B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="length">�ǂݏo����������</param>
		/// <param name="l">�ǂݏo����z��(L ch)</param>
		/// <param name="r">�ǂݏo����z��(R ch)</param>
		/// <returns>�ǂݏo�����f�[�^�T���v����</returns>
		public static int Read(BinaryReader reader, FormatHeader header, uint length, out short[]l, out short[] r)
		{
			if(header.IsStereo)
			{
				l = new short[length];
				r = new short[length];
			}
			else
			{
				l = new short[length];
				r = null;
			}
			return WaveReader.Read(reader, header, l, r);
		}

		/// <summary>
		/// �f�[�^�ǂݔ�΂��B
		/// </summary>
		/// <param name="reader">�ǂݏo�����X�g���[��</param>
		/// <param name="header">Wave �w�b�_</param>
		/// <param name="length">�ǂݏo����������</param>
		/// <returns>�X�g���[���̍Ō�܂ŒB�����ꍇ�� false</returns>
		public static bool Skip(BinaryReader reader, FormatHeader header, int length)
		{
			try
			{
				int readSize;
				if(!header.IsStereo) // ���m����
					if(!header.Is16Bit) readSize = length;
					else         readSize = length * 2;
				else          // �X�e���I
					if(!header.Is16Bit) readSize = length * 2;
				else         readSize = length * 4;
				reader.ReadBytes(readSize);
			}
			catch(EndOfStreamException)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Wave �t�@�C�����J���B
		/// </summary>
		/// <param name="filename">Wave �t�@�C����</param>
		public void Open(string filename)
		{
			Open(new BinaryReader(new BufferedStream(File.OpenRead(filename))));
		}

		/// <summary>
		/// Wave �t�@�C�����J���B
		/// </summary>
		/// <param name="reader">Wave �t�@�C�����i�[�����X�g���[��</param>
		public void Open(BinaryReader reader)
		{
			if(this.reader != null)
			{
				this.reader.Close();
			}

			this.reader = reader;

			// �w�b�_�ǂݏo��
			this.header = WaveReader.ReadHeader(reader);

			if(this.header.id != 0x0001)
			{
				throw new WaveException("�Ή����Ă��Ȃ��t�H�[�}�b�g�ł��B");
			}

			// data chunk �ǂݏo��
			int length = ReadDataChunk(reader);
			this.dataLength = (uint)(length / this.header.blockSize);
		}//Open

		/// <summary>
		/// Wave �t�@�C�������B
		/// </summary>
		public void Close()
		{
			if(this.reader != null)
			{
				this.reader.Close();
				this.reader = null;
			}
		}

		/// <summary>
		/// �w�b�_���̎擾�B
		/// </summary>
		public FormatHeader Header
		{
			get{return this.header;}
		}

		/// <summary>
		/// �c��̃f�[�^�����A���B
		/// </summary>
		public uint Length
		{
			get{return this.dataLength;}
		}

		/// <summary>
		/// �f�[�^�̓ǂݏo���B
		/// </summary>
		/// <param name="l">���`���l���̃f�[�^�i�[��B</param>
		/// <param name="r">�E�`���l���̃f�[�^�i�[��B</param>
		/// <returns>���ۂɓǂݏo�����T���v�����B</returns>
		public int Read(double[] l, double[] r)
		{
			uint length = (uint)l.Length;
			int i = WaveReader.Read(this.reader, this.header, l, r);
			this.dataLength -= (uint)i;
			return i;
		}//Read

		/// <summary>
		/// �f�[�^�̓ǂݏo���B
		/// </summary>
		/// <param name="l">���`���l���̃f�[�^�i�[��B</param>
		/// <param name="r">�E�`���l���̃f�[�^�i�[��B</param>
		/// <returns>���ۂɓǂݏo�����T���v�����B</returns>
		public int Read(float[] l, float[] r)
		{
			uint length = (uint)l.Length;
			int i = WaveReader.Read(this.reader, this.header, l, r);
			this.dataLength -= (uint)i;
			return i;
		}//Read

		/// <summary>
		/// �f�[�^�̓ǂݏo���B
		/// </summary>
		/// <param name="l">���`���l���̃f�[�^�i�[��B</param>
		/// <param name="r">�E�`���l���̃f�[�^�i�[��B</param>
		/// <returns>���ۂɓǂݏo�����T���v�����B</returns>
		public int Read(short[] l, short[] r)
		{
			uint length = (uint)l.Length;
			int i = WaveReader.Read(this.reader, this.header, l, r);
			this.dataLength -= (uint)i;
			return i;
		}//Read

		/// <summary>
		/// �f�[�^�̓ǂݏo���B
		/// </summary>
		/// <param name="length">�ǂݏo���T���v�����B</param>
		/// <param name="l">���`���l���̃f�[�^�i�[��B</param>
		/// <param name="r">�E�`���l���̃f�[�^�i�[��B</param>
		/// <returns>���ۂɓǂݏo�����T���v�����B</returns>
		public int Read(uint length, out double[] l, out double[] r)
		{
			l = null;
			r = null;
			if(this.reader == null) return 0;

			int i = WaveReader.Read(this.reader, this.header, length, out l, out r);
			this.dataLength -= (uint)i;
			return i;
		}//Read

		/// <summary>
		/// �f�[�^�̓ǂݏo���B
		/// </summary>
		/// <param name="length">�ǂݏo���T���v�����B</param>
		/// <param name="l">���`���l���̃f�[�^�i�[��B</param>
		/// <param name="r">�E�`���l���̃f�[�^�i�[��B</param>
		/// <returns>���ۂɓǂݏo�����T���v�����B</returns>
		public int Read(uint length, out float[] l, out float[] r)
		{
			l = null;
			r = null;
			if(this.reader == null) return 0;

			int i = WaveReader.Read(this.reader, this.header, length, out l, out r);
			this.dataLength -= (uint)i;
			return i;
		}//Read

		/// <summary>
		/// �f�[�^�̓ǂݏo���B
		/// </summary>
		/// <param name="length">�ǂݏo���T���v�����B</param>
		/// <param name="l">���`���l���̃f�[�^�i�[��B</param>
		/// <param name="r">�E�`���l���̃f�[�^�i�[��B</param>
		/// <returns>���ۂɓǂݏo�����T���v�����B</returns>
		public int Read(uint length, out short[] l, out short[] r)
		{
			l = null;
			r = null;
			if(this.reader == null) return 0;

			int i = WaveReader.Read(this.reader, this.header, length, out l, out r);
			this.dataLength -= (uint)i;
			return i;
		}//Read

		/// <summary>
		/// �f�[�^��ǂݔ�΂��B
		/// </summary>
		/// <param name="length">�ǂݔ�΂�����</param>
		/// <returns>�t�@�C���̖����܂œ��B������ false ��Ԃ�</returns>
		public bool Skip(int length)
		{
			if(this.reader == null) return false;

			if(!WaveReader.Skip(this.reader, this.header, length))
			{
				this.dataLength = 0;
				return false;
			}

			this.dataLength -= (uint)length;
			return true;
		}//Skip

		/// <summary>
		/// �E�F�[�u�f�[�^�̐擪�ɖ߂�B
		/// </summary>
		public void Restart()
		{
			this.reader.BaseStream.Seek(0, SeekOrigin.Begin);
			WaveReader.ReadDataChunk(this.reader);
		}

		/// <summary>
		/// 1�T���v���ǂݏo���B
		/// ���m����16�r�b�g�ȊO�̏ꍇ�A�T�|�[�g�ΏۊO�B
		/// </summary>
		/// <returns>1�T���v�����̃f�[�^</returns>
		public short ReadShort()
		{
			if(!this.header.Is16Bit || this.header.IsStereo)
				return 0;

			short data = this.reader.ReadInt16();
			--this.dataLength;
			return data;
		}

		/// <summary>
		/// 1�T���v���ǂݏo���B
		/// ���m����16�r�b�g�ȊO�̏ꍇ�A�T�|�[�g�ΏۊO�B
		/// </summary>
		/// <returns>1�T���v�����̃f�[�^</returns>
		public void ReadShort(out short l, out short r)
		{
			System.Diagnostics.Debug.Assert(this.header.Is16Bit);

			if(this.header.IsStereo)
			{
				l = this.reader.ReadInt16();
				r = this.reader.ReadInt16();
				--this.dataLength;
			}
			else
			{
				l = this.reader.ReadInt16();
				r = 0;
				--this.dataLength;
			}
		}
	}//class WaveReader
}
