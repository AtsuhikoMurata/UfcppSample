using System;
using SoundLibrary.Filter;

namespace SoundLibrary.Wave
{
	public interface IStereoFilter
	{
		void Filter(ref short l, ref short r);
		void Clear();
	}

	/// <summary>
	/// IFilter �� Stereo1SampleFilter �p�A�_�v�^�B
	/// ���� (Xl, Xr), �o�� (Yl, Yr)�A
	/// Yl = Hll Xl + Hrl Xr
	/// Yr = Hlr Xl + Hrr Xr
	/// </summary>
	public class CrossStereoFilter : IStereoFilter
	{
		#region �t�B�[���h

		IFilter ll; // ���� Hll �����t�B���^�B
		IFilter lr; // ���� Hlr �����t�B���^�B
		IFilter rl; // ���� Hrl �����t�B���^�B
		IFilter rr; // ���� Hrr �����t�B���^�B

		#endregion
		#region �R���X�g���N�^

		/// <summary>
		/// Hll, Hlr, Hrl, Hrr ��ʌɎw��B
		/// </summary>
		/// <param name="ll">���� Hll �����t�B���^</param>
		/// <param name="lr">���� Hlr �����t�B���^</param>
		/// <param name="rl">���� Hrl �����t�B���^</param>
		/// <param name="rr">���� Hrr �����t�B���^</param>
		public CrossStereoFilter(IFilter ll, IFilter lr, IFilter rl, IFilter rr)
		{
			this.ll = ll;
			this.lr = lr;
			this.rl = rl;
			this.rr = rr;
		}

		/// <summary>
		/// Hll �� Hrr, Hlr �� Hrl�̏ꍇ�B
		/// </summary>
		/// <param name="ll">���� Hll �����t�B���^</param>
		/// <param name="lr">���� Hlr �����t�B���^</param>
		public CrossStereoFilter(IFilter ll, IFilter lr)
		{
			this.ll = ll;
			this.lr = lr;
			this.rl = (IFilter)lr.Clone();
			this.rr = (IFilter)ll.Clone();
		}

		#endregion

		public void Filter(ref short l, ref short r)
		{
			double xl = this.ll.GetValue(l) + this.rl.GetValue(r);
			double xr = this.lr.GetValue(l) + this.rr.GetValue(r);
			l = SoundLibrary.Wave.Util.ClipToShort(xl);
			r = SoundLibrary.Wave.Util.ClipToShort(xr);
		}

		/// <summary>
		/// ��Ԃ̏������B
		/// </summary>
		public void Clear()
		{
			this.ll.Clear();
			this.lr.Clear();
			this.rl.Clear();
			this.rr.Clear();
		}
	}

	/// <summary>
	/// IFilter �� Stereo1SampleFilter �p�A�_�v�^�B
	/// L/R ���ꂼ��ɕʌ̃t�B���^��K�p�B
	/// ���� (Xl, Xr), �o�� (Yl, Yr)�A
	/// Yl = Hl Xl
	/// Yr = Hr Xr
	/// </summary>
	public class StereoFilter : IStereoFilter
	{
		#region �t�B�[���h

		IFilter l; // ���� Hl �����t�B���^�B
		IFilter r; // ���� Hr �����t�B���^�B

		#endregion
		#region �R���X�g���N�^

		/// <summary>
		/// Hl, Hr ��ʌɎw��B
		/// </summary>
		/// <param name="l">���� Hl �����t�B���^</param>
		/// <param name="r">���� Hr �����t�B���^</param>
		public StereoFilter(IFilter l, IFilter r)
		{
			this.l = l;
			this.r = r;
		}

		/// <summary>
		/// Hl �� Hr �̏ꍇ�B
		/// </summary>
		/// <param name="l">���� Hl �����t�B���^</param>
		public StereoFilter(IFilter l)
		{
			this.l = l;
			this.r = (IFilter)l.Clone();
		}

		#endregion

		public void Filter(ref short l, ref short r)
		{
			double xl = this.l.GetValue(l);
			double xr = this.r.GetValue(r);
			l = SoundLibrary.Wave.Util.ClipToShort(xl);
			r = SoundLibrary.Wave.Util.ClipToShort(xr);
		}

		/// <summary>
		/// ��Ԃ̏������B
		/// </summary>
		public void Clear()
		{
			this.l.Clear();
			this.r.Clear();
		}
	}

	/// <summary>
	/// IFilter �� Stereo1SampleFilter �p�A�_�v�^�B
	/// �X�e���I�������m���������Ă���t�B���^�����O�B
	/// ���� (Xl, Xr), �o�� (Yl, Yr)�A
	/// X = (Xl * Xr) / 2
	/// Yl = Yr = H X
	/// </summary>
	public class StereoToMonauralFilter : IStereoFilter
	{
		#region �t�B�[���h

		IFilter f; // ���� H �����t�B���^�B

		#endregion
		#region �R���X�g���N�^

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="f">���� H �����t�B���^</param>
		public StereoToMonauralFilter(IFilter f)
		{
			this.f = f;
		}

		#endregion

		public void Filter(ref short l, ref short r)
		{
			double x = this.f.GetValue(0.5 * ((int)l + r));
			l = r = SoundLibrary.Wave.Util.ClipToShort(x);
		}

		/// <summary>
		/// ��Ԃ̏������B
		/// </summary>
		public void Clear()
		{
			this.f.Clear();
		}
	}

	/// <summary>
	/// IFilter �� Monaural1SampleFilter �p�A�_�v�^�B
	/// Y = H X
	/// </summary>
	public class MonauralFilter
	{
		#region �t�B�[���h

		IFilter f; // ���� H �����t�B���^�B

		#endregion
		#region �R���X�g���N�^

		/// <summary>
		/// �������B
		/// </summary>
		/// <param name="f">���� H �����t�B���^</param>
		public MonauralFilter(IFilter f)
		{
			this.f = f;
		}

		#endregion

		public void Filter(ref short l)
		{
			double x = this.f.GetValue(l);
			l = SoundLibrary.Wave.Util.ClipToShort(x);
		}

		/// <summary>
		/// ��Ԃ̏������B
		/// </summary>
		public void Clear()
		{
			this.f.Clear();
		}
	}
}
