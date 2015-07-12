using System;

using SoundLibrary.Filter;

namespace SoundLibrary.Pipe.Monaural
{
	/// <summary>
	/// �t�B���^�������s���p�C�v�B
	/// </summary>
	public class FilteredPipe : Pipe
	{
		#region �t�B�[���h

		IFilter filter;

		#endregion
		#region ������

		public FilteredPipe(Queue input, Queue output, IFilter filter)
			: base(input, output)
		{
			this.filter = filter;
		}

		#endregion
		#region ����

		public override void Process()
		{
			while(!this.input.IsEmpty)
			{
				double val = this.filter.GetValue(this.input.Front);
				this.output.Enqueue(SoundLibrary.Util.ClipShort(val));
				this.input.Dequeue();
			}
		}

		#endregion
	}
}
