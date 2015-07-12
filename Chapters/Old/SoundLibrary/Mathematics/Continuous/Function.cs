using System;

namespace SoundLibrary.Mathematics.Continuous
{
	using Type = System.Double;

	/// <summary>
	/// �A���֐���\���N���X�B
	/// </summary>
	public abstract class Function
	{
		/// <summary>
		/// �֐��l f(t) ���v�Z�B
		/// </summary>
		public abstract Type this[double t]
		{
			get;
		}
	}
}
