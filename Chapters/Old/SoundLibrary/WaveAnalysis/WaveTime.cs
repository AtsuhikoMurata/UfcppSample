using System;

using SoundLibrary.SpectrumAnalysis;
using SoundLibrary.Wave;

namespace SoundLibrary.WaveAnalysis
{
	/// <summary>
	/// Wave �f�[�^�i�[�p�N���X�B
	/// ���n��Ńf�[�^��ێ��B
	/// </summary>
	public class WaveTime : WaveData
	{
		double[] l;  // L ch ���n��
		double[] r;  // R ch ���n��

		public WaveTime(){}

		public WaveTime(FormatHeader header, double[] l, double[] r) : base(header)
		{
			this.l = l;
			this.r = r;
		}

		public override double[] TimeL
		{
			set{this.l = value;}
			get{return this.l;}
		}

		public override double[] TimeR
		{
			set{this.r = value;}
			get{return this.r;}
		}

		public override Spectrum Left
		{
			set{this.l = value.TimeSequence;}
			get{return Spectrum.FromTimeSequence(this.l);}
		}

		public override Spectrum Right
		{
			set{this.r = value.TimeSequence;}
			get{return Spectrum.FromTimeSequence(this.r);}
		}
	}
}
