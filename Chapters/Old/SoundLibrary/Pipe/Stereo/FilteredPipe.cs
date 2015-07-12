using System;

using SoundLibrary.Filter;

namespace SoundLibrary.Pipe.Stereo
{
	/// <summary>
	/// �t�B���^�������s���p�C�v�B
	/// </summary>
	public class FilteredPipe : Pipe
	{
		#region �t�B�[���h

		IFilter filterL;
		IFilter filterR;

		#endregion
		#region ������

		public FilteredPipe(Queue input, Queue output, IFilter filter)
			: this(input, output, filter, (IFilter)filter.Clone())
		{
		}

		public FilteredPipe(Queue input, Queue output, IFilter filterL, IFilter filterR)
			: base(input, output)
		{
			this.filterL = filterL;
			this.filterR = filterR;
		}

		#endregion
		#region �v���p�e�B

		/// <summary>
		/// L �`���l���Ɋ|����t�B���^�B
		/// </summary>
		public IFilter Left
		{
			get{return this.filterL;}
			set{this.filterL = value;}
		}

		/// <summary>
		/// R �`���l���Ɋ|����t�B���^�B
		/// </summary>
		public IFilter Right
		{
			get{return this.filterR;}
			set{this.filterR = value;}
		}

		#endregion
		#region ����

		public override void Process()
		{
			while(this.input.Count >= 2)
			{
				double l = this.filterL.GetValue(this.input.Front);
				this.output.Enqueue(SoundLibrary.Util.ClipShort(l));
				this.input.Dequeue();

				double r = this.filterR.GetValue(this.input.Front);
				this.output.Enqueue(SoundLibrary.Util.ClipShort(r));
				this.input.Dequeue();
			}
		}

		#endregion
	}
}
