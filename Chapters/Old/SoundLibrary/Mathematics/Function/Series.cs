using System;
using System.Collections;

namespace SoundLibrary.Mathematics.Function
{
	/// <summary>
	/// Sum(�ݘa)/Product(�ݐ�)�N���X�̋��ʊ��B
	/// </summary>
	public abstract class Series : Function
	{
		#region �t�B�[���h

		protected ArrayList functions = new ArrayList();

		#endregion
		#region ������

		public Series(params Function[] functions)
		{
			foreach(Function f in functions)
			{
				this.functions.Add(f);
			}
		}

		public Series(ArrayList functions)
		{
			this.functions = functions;
		}

		#endregion
		#region �֐��̒ǉ�

		/// <summary>
		/// ���Z�Ώۃ��X�g�Ɋ֐���ǉ�����B
		/// </summary>
		/// <param name="f">�ǉ�����֐�</param>
		internal void AddList(Function f)
		{
			if(f is Constant)
			{
				Constant c = f as Constant;
				if(c.Value == 0)
					return;
			}

			this.functions.Add(f);
		}

		/// <summary>
		/// ���Z�Ώۃ��X�g�Ɋ֐���ǉ�����B
		/// </summary>
		/// <param name="list">�ǉ�����֐��̃��X�g</param>
		internal void AddList(IList list)
		{
			foreach(Function f in list)
				this.AddList(f);
		}

		#endregion
		#region object

		public override bool Equals(object obj)
		{
			Series s = obj as Series;
			if(s == null) return false;
			if(this.functions.Count != s.functions.Count) return false;

			for(int i=0; i<this.functions.Count; ++i)
			{
				if(!this.functions[i].Equals(s.functions[i])) return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			int code = 0;
			foreach(Function f in this.functions)
			{
				code <<= 1;
				code ^= f.GetHashCode();
			}

			return code;
		}

		#endregion
	}
}
