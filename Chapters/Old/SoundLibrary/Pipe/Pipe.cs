using System;

namespace SoundLibrary.Pipe
{
	/// <summary>
	/// �L���[�̊Ԃɓ����ď������s���p�C�v�B
	/// </summary>
	public abstract class Pipe
	{
		#region �t�B�[���h

		protected Queue input;
		protected Queue output;

		#endregion
		#region ������

		public Pipe(Queue input, Queue output)
		{
			this.input = input;
			this.output = output;
		}

		#endregion
		#region ����

		/// <summary>
		/// input �L���[�� output �L���[�̊Ԃɋ��ޏ����B
		/// �f�t�H���g�͑f�ʂ��B
		/// </summary>
		public virtual void Process()
		{
			while(!this.input.IsEmpty)
			{
				this.output.Enqueue(input.Front);
				this.input.Dequeue();
			}
		}

		/// <summary>
		/// �L���[�̊Ԃɏ����Ȃ��ŁA�Ƃɂ��� input ���̃f�[�^���t���b�V������B
		/// </summary>
		public virtual void Flush()
		{
			while(!this.input.IsEmpty)
			{
				this.output.Enqueue(input.Front);
				this.input.Dequeue();
			}
		}

		#endregion
		#region �v���p�e�B

		/// <summary>
		/// ���̓L���[�̎擾�B
		/// </summary>
		public Queue InputQueue
		{
			set{this.input = value;}
			get{return this.input;}
		}

		/// <summary>
		/// �o�̓L���[�̎擾�B
		/// </summary>
		public Queue OutputQueue
		{
			set{this.output = value;}
			get{return this.output;}
		}

		#endregion
	}
}
