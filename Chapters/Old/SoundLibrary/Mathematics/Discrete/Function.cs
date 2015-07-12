using System;

namespace SoundLibrary.Mathematics.Discrete
{
	using Type = System.Double;

	/// <summary>
	/// ���U�֐���\���N���X�B
	/// </summary>
	abstract public class Function
	{
		/// <summary>
		/// ��`��̉����B
		/// </summary>
		public abstract int Begin{get;}

		/// <summary>
		/// ��`��̏�� + 1�B
		/// </summary>
		public abstract int End{get;}

		/// <summary>
		/// ��`��̒����B
		/// </summary>
		public abstract int Length{get;}

		/// <summary>
		/// f[n] �����߂�B
		/// </summary>
		public abstract Type this[int n]{get;}

		/// <summary>
		/// �z�񉻁B
		/// </summary>
		/// <returns>�z�񉻂�������</returns>
		public virtual Type[] ToArray()
		{
			Type[] x = new Type[this.Length];

			for(int i=this.Begin; i<this.End; ++i)
			{
				x[i] = this[i];
			}
			return x;
		}

		/// <summary>
		/// func �̒l���ő�ɂ�������̒l��Ԃ��B
		/// </summary>
		/// <param name="func">argmax �����߂����֐��B</param>
		/// <returns>argmax �l</returns>
		public static int Argmax(Function func)
		{
			return Argmax(func, func.Begin, func.End);
		}

		/// <summary>
		/// func �̒l���ő�ɂ�������̒l��Ԃ��B
		/// </summary>
		/// <param name="func">argmax �����߂����֐��B</param>
		/// <param name="min">�����͈̔͂̍ŏ��l</param>
		/// <param name="max">�����͈̔͂̍ő�l</param>
		/// <returns>argmax �l</returns>
		public static int Argmax(Function func, int min, int max)
		{
			double maxVal = func[min];
			int argmax = min;

			for(int i=min+1; i<max; ++i)
			{
				double val = func[i];
				if(val > maxVal)
				{
					maxVal = val;
					argmax = i;
				}
			}

			return argmax;
		}

		#region Type[] �� Function �Ƃ��Ĉ������߂̃��b�p�[
		/// <summary>
		/// �z��̒l�����̂܂ܕԂ��֐��B
		/// </summary>
		class Array : Function
		{
			Type[] array;
			internal Array(Type[] array){this.array = array;}
			public override int Begin{get{return 0;}}
			public override int End{get{return this.array.Length;}}
			public override int Length{get{return this.array.Length;}}
			public override Double this[int n]{get{return this.array[n];}}
			public override Double[] ToArray(){return this.array;}
		}

		/// <summary>
		/// �z��̒l�����̂܂ܕԂ��֐������B
		/// </summary>
		/// <param name="array">�z��</param>
		/// <returns>�z��̒l�����̂܂ܕԂ��֐�</returns>
		public static Function FromArray(Type[] array)
		{
			return new Array(array);
		}
		#endregion
	}
}
