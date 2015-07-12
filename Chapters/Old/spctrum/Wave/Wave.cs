using System;
using System.IO;

namespace Wave
{
	/// <summary>
	/// WaveReader/WaveWriter �Ŏg����O�N���X�B
	/// </summary>
	public class WaveException : Exception
	{
		public WaveException(){}
		public WaveException(string message) : base(message) {}
		public WaveException(string message, Exception innerException) : base(message, innerException) {}
	}

	/// <summary>
	/// Wave �t�@�C���̃t�H�[�}�b�g�w�b�_�B
	/// </summary>
	public class FormatHeader
	{
		public short id;         // �f�[�^�`��
		public short ch;         // �`���l����
		public int   sampleRate; // �T���v�����O���[�g
		public int   dataRate;   // �f�[�^���[�g(���`���l�����~�u���b�N�T�C�Y)
		public short blockSize;  // �u���b�N�T�C�Y(���`���l�����~�o�C�g/�`���l��)
		public short sampleBit;  // 1�T���v���ӂ�̃r�b�g��

		public const bool Stereo = true;
		public const bool Mono   = false;
		public const bool Bit16 = true;
		public const bool Bit8  = false;

		public FormatHeader(){}

		/// <summary>
		/// �T���v�����O���[�g���̃p�����[�^����w�b�_�쐬�B
		/// </summary>
		/// <param name="rate">�T���v�����O���[�g</param>
		/// <param name="stereo">true �Ȃ�X�e���I�Afalse �Ȃ烂�m����</param>
		/// <param name="type">true �Ȃ�16bit/sample�Afalse �Ȃ�8bit/sample</param>
		public FormatHeader(int rate, bool stereo, bool type)
		{
			this.id         = 1;
			this.ch         = stereo ? (short)2 : (short)1;
			this.sampleRate = rate;
			this.blockSize  = (short)(this.ch * (type ? 2 : 1));
			this.dataRate   = rate * this.blockSize;
			this.sampleBit  = type ? (short)16 : (short)8;
		}

		/// <summary>
		/// BinaryReader ����w�b�_�ǂݏo���B
		/// </summary>
		/// <param name="reader"></param>
		public FormatHeader(BinaryReader reader)
		{
			ReadFromStream(reader);
		}

		/// <summary>
		/// BinaryReader ����w�b�_�ǂݏo���B
		/// </summary>
		/// <param name="reader"></param>
		public void ReadFromStream(BinaryReader reader)
		{
			this.id         = reader.ReadInt16();
			this.ch         = reader.ReadInt16();
			this.sampleRate = reader.ReadInt32();
			this.dataRate   = reader.ReadInt32();
			this.blockSize  = reader.ReadInt16();
			this.sampleBit  = reader.ReadInt16();
		}

		public void WriteToStream(BinaryWriter writer)
		{
			writer.Write(this.id        );
			writer.Write(this.ch        );
			writer.Write(this.sampleRate);
			writer.Write(this.dataRate  );
			writer.Write(this.blockSize );
			writer.Write(this.sampleBit );
		}
	}//class FormatHeader

	/// <summary>
	/// RIFF Wave �`���̃t�@�C�����特���f�[�^��ǂݏo���B
	/// </summary>
	public class WaveReader : IDisposable
	{
		BinaryReader reader = null;
		FormatHeader header = null;
		uint dataLength = 0;

		public WaveReader(){}

		public WaveReader(string filename)
		{
			this.Open(filename);
		}

		public WaveReader(BinaryReader reader)
		{
			this.Open(reader);
		}

		public void Dispose()
		{
			this.Close();
		}

		/// <summary>
		/// Wave �t�@�C�����J���B
		/// </summary>
		/// <param name="filename">Wave �t�@�C����</param>
		public void Open(string filename)
		{
			Open(new BinaryReader(File.OpenRead(filename)));
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
			byte[] buf;

			buf= this.reader.ReadBytes(4);
			if(buf[0] != 'R' || buf[1] != 'I' || buf[2] != 'F' || buf[3] != 'F')
			{
				throw new WaveException("���̃t�@�C����RIFF�`���ł͂���܂���B");
			}

			this.reader.ReadBytes(4); //�t�@�C���T�C�Y�ǂݔ�΂��B

			buf= this.reader.ReadBytes(4);
			if(buf[0] != 'W' || buf[1] != 'A' || buf[2] != 'V' || buf[3] != 'E')
			{
				throw new WaveException("���̃t�@�C����wave�`���ł͂���܂���B");
			}

			buf= this.reader.ReadBytes(4);
			if(buf[0] != 'f' || buf[1] != 'm' || buf[2] != 't' || buf[3] != ' ')
			{
				throw new WaveException("fmt�^�O��������܂���ł����B");
			}

			int headerLength = this.reader.ReadInt32();
			if(headerLength < 16)
			{
				throw new WaveException("�w�b�_�����Z�����܂��B");
			}

			this.header = new FormatHeader(this.reader);
			if(header.id != 0x0001)
			{
				throw new WaveException("�Ή����Ă��Ȃ��t�H�[�}�b�g�ł��B");
			}

			if(headerLength != 16)
			{
				this.reader.ReadBytes(headerLength - 16); // �w�b�_�[�̎c��̕����ǂݔ�΂��B
			}

			buf= this.reader.ReadBytes(4);
			if(buf[0] != 'd' || buf[1] != 'a' || buf[2] != 't' || buf[3] != 'a')
			{
				throw new WaveException("data�^�O��������܂���ł����B");
			}

			this.dataLength = (uint)(this.reader.ReadUInt32() / this.header.blockSize);
		}//Open

		/// <summary>
		/// Wave �t�@�C�������B
		/// </summary>
		public void Close()
		{
			if(reader == null) return;

			reader.Close();
			reader =null;
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
		/// <param name="length">�ǂݏo���T���v�����B</param>
		/// <param name="l">���`���l���̃f�[�^�i�[��B</param>
		/// <param name="r">�E�`���l���̃f�[�^�i�[��B</param>
		/// <returns>���ۂɓǂݏo�����T���v�����B</returns>
		public int Read(uint length, out double[] l, out double[] r)
		{
			l = null;
			r = null;
			if(this.reader == null) return 0;

			int i = 0;
			try
			{
				if(this.header.ch == 1) // ���m����
				{
					l = new double[length];

					if(this.header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)(this.reader.ReadByte() - 128);
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)this.reader.ReadInt16();
						}
					}
				}//���m����
				else if(header.ch == 2) // �X�e���I
				{
					l = new double[length];
					r = new double[length];

					if(header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)(this.reader.ReadByte() - 128);
							r[i] = (double)(this.reader.ReadByte() - 128);
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (double)this.reader.ReadInt16();
							r[i] = (double)this.reader.ReadInt16();
						}
					}
				}//�X�e���I
			}
			catch(EndOfStreamException){}
			catch(IOException){}

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

			int i = 0;
			try
			{
				if(this.header.ch == 1) // ���m����
				{
					l = new float[length];

					if(this.header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)(this.reader.ReadByte() - 128);
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)this.reader.ReadInt16();
						}
					}
				}//���m����
				else if(header.ch == 2) // �X�e���I
				{
					l = new float[length];
					r = new float[length];

					if(header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)(this.reader.ReadByte() - 128);
							r[i] = (float)(this.reader.ReadByte() - 128);
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							l[i] = (float)this.reader.ReadInt16();
							r[i] = (float)this.reader.ReadInt16();
						}
					}
				}//�X�e���I
			}
			catch(EndOfStreamException){}
			catch(IOException){}

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

			try
			{
				if(header.ch == 1) // ���m����
				{
					if(header.sampleBit == 8)
					{
						this.reader.ReadBytes(length);
					}

					if(header.sampleBit == 16)
					{
						this.reader.ReadBytes(length * 2);
					}
				}
				else if(header.ch == 2) // �X�e���I
				{
					if(header.sampleBit == 8)
					{
						this.reader.ReadBytes(length * 2);
					}
					else if(header.sampleBit == 16)
					{
						this.reader.ReadBytes(length * 4);
					}
				}
			}
			catch(EndOfStreamException)
			{
				this.dataLength = 0;
				return false;
			}

			this.dataLength -= (uint)length;
			return true;
		}//Skip
	}//class WaveReader

	/// <summary>
	/// RIFF Wave �`���̃t�@�C���ɉ����f�[�^���������ށB
	/// </summary>
	public class WaveWriter : IDisposable
	{
		BinaryWriter writer = null;
		FormatHeader header = null;
		uint dataLength = 0;

		public WaveWriter(){}

		public WaveWriter(string filename, FormatHeader header)
		{
			this.Open(filename, header);
		}

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
			Open(new BinaryWriter(File.OpenWrite(filename)), header);
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
			byte[] buf;

			buf = System.Text.Encoding.ASCII.GetBytes("RIFF");
			this.writer.Write(buf);

			this.writer.Write((uint)0); //�f�[�^��(���̒l�����Ă���)

			buf = System.Text.Encoding.ASCII.GetBytes("WAVE");
			this.writer.Write(buf);

			buf = System.Text.Encoding.ASCII.GetBytes("fmt ");
			this.writer.Write(buf);

			this.writer.Write((uint)16);
			this.header.WriteToStream(writer);

			buf = System.Text.Encoding.ASCII.GetBytes("data");
			this.writer.Write(buf);

			this.writer.Write((uint)0); //�f�[�^��(���̒l�����Ă���)
		}//Open

		/// <summary>
		/// Wave �t�@�C�������B
		/// </summary>
		public void Close()
		{
			if(writer == null) return;

			uint length = (uint)(this.dataLength * this.header.blockSize);

			writer.Seek(40, SeekOrigin.Begin);
			writer.Write(length);

			writer.Seek(4, SeekOrigin.Begin);
			writer.Write(length + 36u);

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
			int i = 0;

			try
			{
				if(header.ch == 1) // ���m����
				{
					if(header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToByte(l[i]));
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToShort(l[i]));
						}
					}
				}//���m����
				else if(header.ch == 2) // �X�e���I
				{
					if(header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToByte(l[i]));
							writer.Write(this.DoubleToByte(r[i]));
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToShort(l[i]));
							writer.Write(this.DoubleToShort(r[i]));
						}
					}
				}//�X�e���I
			}
			catch(IOException){return 0;}
	
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
			int i = 0;

			try
			{
				if(header.ch == 1) // ���m����
				{
					if(header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToByte(l[i]));
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToShort(l[i]));
						}
					}
				}//���m����
				else if(header.ch == 2) // �X�e���I
				{
					if(header.sampleBit == 8)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToByte(l[i]));
							writer.Write(this.DoubleToByte(r[i]));
						}
					}
					else if(header.sampleBit == 16)
					{
						for(i=0; i<length; ++i)
						{
							writer.Write(this.DoubleToShort(l[i]));
							writer.Write(this.DoubleToShort(r[i]));
						}
					}
				}//�X�e���I
			}
			catch(IOException){return 0;}
	
			this.dataLength += (uint)i;
			return i;
		}//Write

		/// <summary>
		/// double �� byte �̕ϊ��B
		/// </summary>
		/// <param name="x">��</param>
		/// <returns>��</returns>
		private byte DoubleToByte(double x)
		{
			x += 128;
			if(x < 0) x = 0;
			else if(x > 255) x = 255;
			return (byte)x;
		}

		/// <summary>
		/// double �� short �̕ϊ��B
		/// </summary>
		/// <param name="x">��</param>
		/// <returns>��</returns>
		private short DoubleToShort(double x)
		{
			if(x < short.MinValue) x = short.MinValue;
			else if(x > short.MaxValue) x = short.MaxValue;
			return (short)x;
		}
	}//class WaveWriter
}//namespace Wave
