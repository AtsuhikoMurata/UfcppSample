using System;

using SoundLibrary.SpectrumAnalysis;
using SoundLibrary.Wave;

namespace SoundLibrary.WaveAnalysis
{
	/// <summary>
	/// Wave �t�@�C����͗p�N���X�B
	/// </summary>
	public class WaveAnalyzer
	{
		WaveData data = new WaveTime(); // �f�[�^��

		/// <summary>
		/// �f�[�^ Wave �̎擾
		/// </summary>
		public WaveData Data
		{
			get{return this.data;}
			set{this.data = value;}
		}

		/// <summary>
		/// �z��̃s�[�N�l�����߂�B
		/// </summary>
		/// <param name="x">�z��</param>
		/// <returns>�s�[�N�l</returns>
		static double GetPeekValue(double[] x)
		{
			if(x == null) return 0;

			double peek = 0;
			for(int i=0; i<x.Length; ++i)
			{
				if(Math.Abs(x[i]) > peek) peek = Math.Abs(x[i]);
			}
			return peek;
		}

		/// <summary>
		/// ������Ԃ̒��������߂�B
		/// 
		/// </summary>
		/// <param name="x">�z��</param>
		/// <param name="threshold">臒l</param>
		/// <returns>��Ԓ�</returns>
		static int GetSilentLength(double[] x, double threshold)
		{
			if(x == null) return int.MaxValue;

			int skip;
			for(skip=0; skip<x.Length; ++skip)
			{
				if(Math.Abs(x[skip]) >= threshold) break;
			}
			return skip;
		}

		/// <summary>
		/// �z����R�s�[
		/// </summary>
		/// <param name="src">�R�s�[��</param>
		/// <param name="index">�R�s�[�J�n�ʒu</param>
		/// <param name="length">�R�s�[���钷��</param>
		/// <returns></returns>
		static double[] CopyArray(double[] src, int index, int length)
		{
			double[] dst = new double[length];
			for(int i=0; i<length; ++i) dst[i] = src[i + index];
			return dst;
		}

		/// <summary>
		/// �z����R�s�[
		/// �R�s�[��̔z��̒����̕��������ꍇ�A����0�l��
		/// </summary>
		/// <param name="src">�R�s�[��</param>
		/// <param name="index">�R�s�[�J�n�ʒu</param>
		/// <param name="length">�R�s�[���钷��</param>
		/// <param name="dstLength">�R�s�[��̔z��̒���</param>
		/// <returns></returns>
		static double[] CopyArray(double[] src, int index, int length, int dstLength)
		{
			if(length > dstLength)
				return CopyArray(src, index, length);

			double[] dst = new double[dstLength];
			int i;
			for(i=0; i<length; ++i) dst[i] = src[i + index];
			for(; i<dstLength; ++i) dst[i] = 0;
			return dst;
		}

		/// <summary>
		/// �t�@�C������ Wave �f�[�^��ǂݏo���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <returns>�ǂݏo�����f�[�^</returns>
		static WaveData Read(string filename, int skip, int length)
		{
			WaveReader reader = null;
			FormatHeader header;
			double[] l;
			double[] r;

			using(reader = new WaveReader(filename))
			{
				header = reader.Header;
				reader.Skip(skip);
				reader.Read((uint)length, out l, out r);
			}

			if(header.IsStereo)
				return new WaveTime(header, l, r);
			return new WaveMonaural(header, l);
		}//Read

		/// <summary>
		/// �t�@�C������ Wave �f�[�^��ǂݏo���B
		/// (���0�l��)
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="dstLength">�R�s�[��̔z��̒���</param>
		/// <returns>�ǂݏo�����f�[�^</returns>
		static WaveData Read(string filename, int skip, int length, int dstLength)
		{
			WaveReader reader = null;
			FormatHeader header;
			double[] l;
			double[] r;

			using(reader = new WaveReader(filename))
			{
				header = reader.Header;
				reader.Skip(skip);
				reader.Read((uint)length, out l, out r);
			}

			if(header.IsStereo)
				return new WaveTime(header, CopyArray(l, 0, length, dstLength), CopyArray(r, 0, length, dstLength));
			return new WaveMonaural(header, CopyArray(l, 0, length, dstLength));
		}//Read

		/// <summary>
		/// �t�@�C������ Wave �f�[�^��ǂݏo���B
		/// (���0�l��)
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="threshold">臒l�B���̒l�ȉ��̉��͖����Ƃ݂Ȃ�</param>
		/// <param name="relative">臒l�Ƀs�[�N�l����݂����Βl���g��</param>
		/// <param name="dstLength">�R�s�[��̔z��̒���</param>
		/// <returns>�ǂݏo�����f�[�^</returns>
		static WaveData Read(
			string filename, int skip, int length,
			double threshold ,bool relative, int dstLength)
		{
			WaveReader reader = null;
			FormatHeader header;
			double[] l;
			double[] r;

			using(reader = new WaveReader(filename))
			{
				header = reader.Header;
				reader.Read(reader.Length, out l, out r);
			}

			if(relative) threshold *= Math.Max(GetPeekValue(l), GetPeekValue(r));
			skip += Math.Min(GetSilentLength(l, threshold), GetSilentLength(r, threshold));
			if(skip < 0 || skip + length >= l.Length) skip = 0;

			if(header.IsStereo)
				return new WaveTime(header, CopyArray(l, skip, length, dstLength), CopyArray(r, skip, length, dstLength));
			return new WaveMonaural(header, CopyArray(l, skip, length, dstLength));
		}//Read


		/// <summary>
		/// �t�@�C������ Wave �f�[�^��ǂݏo���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="threshold">臒l�B���̒l�ȉ��̉��͖����Ƃ݂Ȃ�</param>
		/// <param name="relative">臒l�Ƀs�[�N�l����݂����Βl���g��</param>
		/// <returns>�ǂݏo�����f�[�^</returns>
		static WaveData Read(
			string filename, int skip, int length,
			double threshold ,bool relative)
		{
			return Read(filename, skip, length, threshold, relative, length);
		}

		/// <summary>
		/// �f�[�^ wave �̓ǂݏo���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		public void ReadData(string filename, int skip, int length)
		{
			this.data = WaveAnalyzer.Read(filename, skip, length);
		}

		/// <summary>
		/// �f�[�^ wave �̓ǂݏo���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="dstLength">�R�s�[��̔z��̒���</param>
		public void ReadData(string filename, int skip, int length, int dstLength)
		{
			this.data = WaveAnalyzer.Read(filename, skip, length, dstLength);
		}

		/// <summary>
		/// �f�[�^ wave �̓ǂݏo���B
		/// ������Ԃ̏������s���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="threshold">臒l</param>
		/// <param name="relative">臒l�Ƀs�[�N�l���猩�����Βl���g��</param>
		public void ReadData(string filename, int skip, int length, double threshold, bool relative)
		{
			this.data = WaveAnalyzer.Read(filename, skip, length, threshold, relative);
		}

		/// <summary>
		/// �f�[�^ wave �̓ǂݏo���B
		/// ������Ԃ̏������s���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="threshold">臒l</param>
		/// <param name="relative">臒l�Ƀs�[�N�l���猩�����Βl���g��</param>
		/// <param name="dstLength">�R�s�[��̔z��̒���</param>
		public void ReadData(string filename, int skip, int length, double threshold, bool relative, int dstLength)
		{
			this.data = WaveAnalyzer.Read(filename, skip, length, threshold, relative, dstLength);
		}

		/// <summary>
		/// �f�[�^ wave �̏������݁B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		public void WirteData(string filename)
		{
			using(WaveWriter writer = new WaveWriter(filename, this.data.Header))
			{
				writer.Write(this.data.TimeL, this.data.TimeR);
			}
		}
	}//class WaveAnalyzer
}//namespace WaveAnalysis
