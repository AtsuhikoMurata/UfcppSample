using System;
using System.Runtime.InteropServices;

namespace SoundLibrary.Wave
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
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public struct FormatHeader
	{
		#region �t�B�[���h

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

		#endregion
		#region �R���X�g���N�^

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

		#endregion
		#region �v���p�e�B

		/// <summary>
		/// �T���v�����[�g�B
		/// </summary>
		public int Rate
		{
			set
			{
				this.sampleRate = value;
				this.dataRate = value * this.blockSize;
			}
			get
			{
				return this.sampleRate;
			}
		}

		/// <summary>
		/// �T���v���r�b�g��16�r�b�g���ǂ����B
		/// </summary>
		public bool Is16Bit
		{
			set
			{
				this.blockSize  = (short)(this.ch * (value ? 2 : 1));
				this.sampleBit  = value ? (short)16 : (short)8;
			}
			get
			{
				return this.sampleBit == 16;
			}
		}

		/// <summary>
		/// �`���l�����X�e���I���ǂ����B
		/// </summary>
		public bool IsStereo
		{
			set
			{
				this.ch = value ? (short)2 : (short)1;
				this.blockSize = (short)(this.sampleBit / 8 * this.ch);
			}
			get
			{
				return this.ch == 2;
			}
		}

		#endregion
	}//class FormatHeader
}//namespace Wave
