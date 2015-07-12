using System;

namespace SoundLibrary.Music
{
	/// <summary>
	/// �G���x���[�v�����p�̃p�����[�^�B
	/// </summary>
	public class EnvelopeParameter
	{
		public double attackLevel;  // �A�^�b�N���x��(���j�A�l)
		public double sustainLevel; // �T�X�e�C�����x��(���j�A�l)
		public int attackTime;      // �A�^�b�N�^�C��(�X�e�b�v��)
		public int decayTime;       // �f�B�P�C�^�C��(�X�e�b�v��)
		public int releaseTime;     // �����[�X�^�C��(�X�e�b�v��)

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="al">�A�^�b�N���x��(���j�A�l)</param>
		/// <param name="sl">�T�X�e�C�����x��(���j�A�l)</param>
		/// <param name="at">�A�^�b�N�^�C��(�X�e�b�v��)</param>
		/// <param name="dt">�����[�X�^�C��(�X�e�b�v��)</param>
		/// <param name="rt">�����[�X�^�C��(�X�e�b�v��)</param>
		public EnvelopeParameter(double al, double sl, int at, int dt, int rt)
		{
			this.attackLevel  = al;
			this.sustainLevel = sl;
			this.attackTime   = at;
			this.decayTime    = dt;
			this.releaseTime  = rt;
		}
	}

	/// <summary>
	/// ���ƂȂ� Sound �ɃG���x���[�v�Ȑ����|���� Sound �𐶐�����B
	/// 
	/// �A�^�b�N�^�C��
	/// �@�@�@�@�f�B�P�C�^�C���@�@�@�����[�X�^�C��
	/// �������������@�@�@�@�@�@�@�@����
	/// �@�@�@�^�_�@���A�^�b�N���x��
	/// �@�@�^�@�@�_
	/// �@�^�@�@�@�@�P�P�P�P�P�P�P�P�_ �@���T�X�e�C�����x��
	/// �^�@�@�@�@�@�@�@�@�@�@�@�@�@�@�_
	/// </summary>
	public class Envelope : Sound
	{
		EnvelopeParameter parameter;
		Sound sound;

		/// <summary>
		/// �G���x���[�v�p�����[�^�ƌ��ƂȂ� Sound ���w�肵�Đ����B
		/// </summary>
		/// <param name="al">�A�^�b�N���x��(sound �̐U���Ƃ̑��Βl)</param>
		/// <param name="sl">�T�X�e�C�����x��(sound �̐U���Ƃ̑��Βl)</param>
		/// <param name="at">�A�^�b�N�^�C��(�X�e�b�v��)</param>
		/// <param name="dt">�����[�X�^�C��(�X�e�b�v��)</param>
		/// <param name="rt">�����[�X�^�C��(�X�e�b�v��)</param>
		/// <param name="sound">���ƂȂ鉹</param>
		public Envelope(double al, double sl, int at, int dt, int rt, Sound sound)
			: this(new EnvelopeParameter(al, sl, at, dt, rt), sound)
		{
		}

		/// <summary>
		/// �G���x���[�v�p�����[�^�ƌ��ƂȂ� Sound ���w�肵�Đ����B
		/// </summary>
		/// <param name="sound">���ƂȂ鉹</param>
		public Envelope(EnvelopeParameter parameter, Sound sound)
		{
			CheckParameter(parameter, sound.Length);
			this.parameter = parameter;
			this.sound = sound;
		}

		/// <summary>
		/// �p�����[�^�̐��������`�F�b�N�B
		/// </summary>
		/// <param name="parameter">�p�����[�^</param>
		/// <param name="length">���̒���</param>
		static void CheckParameter(EnvelopeParameter parameter, int length)
		{
			if(parameter.attackTime < 0)
				throw new ArgumentException("�A�^�b�N�^�C������");
			if(parameter.decayTime < 0)
				throw new ArgumentException("�f�B�P�C�^�C������");
			if(parameter.releaseTime < 0)
				throw new ArgumentException("�����[�X�^�C������");
			if(parameter.attackTime + parameter.decayTime + parameter.releaseTime > length)
				throw new ArgumentException("�����Z����");
		}

		public override int Length
		{
			get
			{
				return this.sound.Length;
			}
		}

		public override double[] ToArray()
		{
			double[] x = this.sound.ToArray();
			double grad;

			double al = this.parameter.attackLevel;
			double sl = this.parameter.sustainLevel;
			int    at = this.parameter.attackTime;
			int    dt = this.parameter.decayTime;
			int    rt = this.parameter.releaseTime;
			int   len = this.sound.Length;

			int i = 0;

			// �A�^�b�N
			grad = al / at;
			for(; i <= at; ++i)
			{
				int n = i + 1;
				x[i] *= n * grad;
			}

			// �f�B�P�C
			grad = grad = (al - sl) / dt;
			for(; i <= at + dt; ++i)
			{
				int n = at + dt - i;
				x[i] *= n * grad  + sl;
			}

			// �T�X�e�C��
			for(; i < len - rt; ++i)
			{
				x[i] *= sl;
			}

			// �����[�X
			grad = sl / rt;
			for(; i<len; ++i)		
			{
				int n = len - i;
				x[i] *= n * grad;
			}

			return x;
		}
	}
}
