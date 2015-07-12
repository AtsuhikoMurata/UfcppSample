#define CHECK_RANGE

using System;

using SoundLibrary.Filter;

namespace SoundLibrary.Stream.Monaural
{
	/// <summary>
	/// Stream �Ƀt�B���^���|����B
	/// </summary>
	public class FilteredStream : Stream
	{
		#region �t�B�[���h

		Stream stream; // inner stream
		IFilter filter;

		#endregion
		#region ������

		/// <summary>
		/// stream �� filter ���|����B
		/// </summary>
		/// <param name="stream">�����X�g���[��</param>
		/// <param name="filter">�|�������t�B���^�[</param>
		public FilteredStream(Stream stream, IFilter filter)
		{
			this.stream = stream;
			this.filter = filter;
		}

		#endregion
		#region Stream �����o

		public override int FillBuffer(short[] buffer, int offset, int size)
		{
			size = this.stream.FillBuffer(buffer, offset, size);

			for(int i=0; i<size; ++i)
			{
				double val= this.filter.GetValue(buffer[i]);
#if CHECK_RANGE
			if(val > short.MaxValue) val = short.MaxValue;
			if(val < short.MinValue) val = short.MinValue;
#endif
				buffer[i] = (short)val;
			}

			return size;
		}

		public override bool Skip(int size)
		{
			return this.stream.Skip(size);
		}

		#endregion
	}
}
