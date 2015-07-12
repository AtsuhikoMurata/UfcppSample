using System;

using SpectrumAnalysis;
using Wave;

namespace WaveAnalysis
{
	/// <summary>
	/// Wave �f�[�^�i�[�p�N���X�B
	/// ���g���n��Ńf�[�^��ێ��B
	/// </summary>
	class WaveFrequency : WaveData
	{
		Spectrum l; // L ch ���g���n��
		Spectrum r; // R ch ���g���n��

		public WaveFrequency(){}

		public WaveFrequency(FormatHeader header, Spectrum l, Spectrum r) : base(header)
		{
			this.l = l;
			this.r = r;
		}

		public override double[] TimeL
		{
			set{this.l = Spectrum.FromTimeSequence(value, this.Header.sampleRate);}
			get{return this.l.TimeSequence;}
		}

		public override double[] TimeR
		{
			set{this.r = Spectrum.FromTimeSequence(value, this.Header.sampleRate);}
			get{return this.r.TimeSequence;}
		}

		public override Spectrum Left
		{
			set{this.l = value;}
			get{return this.l;}
		}

		public override Spectrum Right
		{
			set{this.r = value;}
			get{return this.r;}
		}
	}//class WaveFrequency

	/// <summary>
	/// ���t�@�����X���̃^�C�v
	/// </summary>
	public enum ReferenceType
	{
		Both,    // �� ch �g��
		Left,    // L ch �����g��
		Right,   // R ch �����g��
		Reverse, // L,R �t�� ch ���g��
		Cross,   // L �����ډ��AR ���N���X�g�[�N���̂Ƃ�
		// (L ���N���X�g�[�N���AR �����ډ��ł����ʂ͓���)
		// a = data.L, b = data.R
		// c = ref.L,  d = ref.R
		// [a b][c d]-1              [ac-bd bc-ad]
		// [b a][d c]   = 1/(c^2-d^2)[bc-ad ac-bd]
		// ��
		// data.L = (ac-bd) / (c^2-d^2)
		// data.R = (bc-ad) / (c^2-d^2)
	}//enum ReferenceType

	/// <summary>
	/// Wave �t�@�C����͗p�N���X�B
	/// </summary>
	public class WaveAnalyzer
	{
		WaveData data = new WaveTime(); // �f�[�^��

		/// <summary>
		/// �f�[�^ Wave �̎擾
		/// </summary>
		public WaveData Data{get{return this.data;}}

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

			return new WaveTime(header, l, r);
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
			WaveReader reader = null;
			FormatHeader header;
			double[] l;
			double[] r;

			using(reader = new WaveReader(filename))
			{
				header = reader.Header;
				reader.Skip(0);
				reader.Read(reader.Length, out l, out r);
			}

			if(relative) threshold *= Math.Max(GetPeekValue(l), GetPeekValue(r));
			skip += Math.Min(GetSilentLength(l, threshold), GetSilentLength(r, threshold));
			if(skip < 0 || skip + length >= l.Length) skip = 0;

			return new WaveTime(header, CopyArray(l, skip, length), CopyArray(r, skip, length));
		}//Read

		/// <summary>
		/// �t�@�C������ Wave �f�[�^��ǂݏo���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="threshold">臒l�B���̒l�ȉ��̉��͖����Ƃ݂Ȃ�</param>
		/// <param name="relative">臒l�Ƀs�[�N�l����݂����Βl���g��</param>
		/// <param name="type">���t�@�����X�̃^�C�v</param>
		/// <returns>�ǂݏo�����f�[�^</returns>
		static WaveData Read(
			string filename, int skip, int length,
			double threshold ,bool relative, ReferenceType type)
		{
			WaveReader reader = null;
			FormatHeader header;
			double[] l;
			double[] r;

			using(reader = new WaveReader(filename))
			{
				header = reader.Header;

				// R ch ���g�������̂� wave �t�@�C�������m�����̏ꍇ
				if(type != ReferenceType.Left && reader.Header.ch == 1)
				{
					return new WaveTime();
				}

				reader.Skip(0);
				reader.Read(reader.Length, out l, out r);
			}

			if(type == ReferenceType.Left)
			{
				if(relative) threshold *= GetPeekValue(l);
				skip += GetSilentLength(l, threshold);
			}
			else if(type == ReferenceType.Right)
			{
				if(relative) threshold *= GetPeekValue(r);
				skip += GetSilentLength(r, threshold);
			}
			else
			{
				if(relative) threshold *= Math.Max(GetPeekValue(l), GetPeekValue(r));
				skip += Math.Min(GetSilentLength(l, threshold), GetSilentLength(r, threshold));
			}
			if(skip < 0 || skip + length >= l.Length) skip = 0;

			return new WaveTime(header, CopyArray(l, skip, length), CopyArray(r, skip, length));
		}//Read

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

		static void Deconvolute(ref WaveData data, WaveData reference, ReferenceType type, bool isNormalize)
		{
			data = data.ToSpectrum();
			reference =reference.ToSpectrum();

			if(isNormalize)
			{
				reference.Left.Normalize();
				reference.Right.Normalize();
			}

			switch(type)
			{
				case ReferenceType.Left:
					data.Left /= reference.Left;
					data.Right /= reference.Left;
					break;
				case ReferenceType.Right:
					data.Left /= reference.Right;
					data.Right /= reference.Right;
					break;
				case ReferenceType.Both:
					data.Left /= reference.Left;
					data.Right /= reference.Right;
					break;
				case ReferenceType.Cross:
					Spectrum a = data.Left;
					Spectrum b = data.Right;
					Spectrum c = reference.Left;
					Spectrum d = reference.Right;
					Spectrum det = c*c - d*d;
					data.Left  = (a*c - b*d) / det;
					data.Right = (b*c - a*d) / det;
					break;
			}
		}

		/// <summary>
		/// ���t�@�����X wave �̓ǂݏo���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="type">���t�@�����X�̃^�C�v</param>
		/// <param name="isNormalize">���t�@�����X�̐U�������𐳋K�����邩�ǂ���</param>
		public void DeconvoluteReference(
			string filename, int skip, int length,
			ReferenceType type, bool isNormalize)
		{
			WaveData reference = WaveAnalyzer.Read(filename, skip, length);
			Deconvolute(ref this.data, reference, type, isNormalize);
		}

		/// <summary>
		/// ���t�@�����X wave �̓ǂݏo���B
		/// ������Ԃ̏������s���B
		/// </summary>
		/// <param name="filename">�t�@�C����</param>
		/// <param name="length">�ǂݍ��ޒ���</param>
		/// <param name="skip">�t�@�C���̐擪��ǂݔ�΂�����</param>
		/// <param name="type">���t�@�����X�̃^�C�v</param>
		/// <param name="isNormalize">���t�@�����X�̐U�������𐳋K�����邩�ǂ���</param>
		/// <param name="threshold">臒l</param>
		/// <param name="relative">臒l�Ƀs�[�N�l���猩�����Βl���g��</param>
		public void DeconvoluteReference(
			string filename, int skip, int length,
			ReferenceType type, bool isNormalize,
			double threshold, bool relative)
		{
			WaveData reference = WaveAnalyzer.Read(filename, skip, length, threshold, relative, type);
			Deconvolute(ref this.data, reference, type, isNormalize);
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
