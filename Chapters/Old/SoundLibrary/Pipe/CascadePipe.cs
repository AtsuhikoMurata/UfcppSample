using System;

namespace SoundLibrary.Pipe
{
	/// <summary>
	/// Pipe �𒼗�Ɍq���B
	/// </summary>
	public class CascadePipe : Pipe
	{
		#region �t�B�[���h

		Pipe[] pipes;

		#endregion
		#region ������

		public CascadePipe(params Pipe[] pipes)
			: base(pipes[0].InputQueue, pipes[pipes.Length - 1].OutputQueue)
		{
			this.pipes = pipes;
		}

		#endregion
		#region ����

		public override void Process()
		{
			foreach(Pipe pipe in this.pipes)
				pipe.Process();
		}

		#endregion
	}
}
