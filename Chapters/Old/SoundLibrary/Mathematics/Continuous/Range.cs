using System;

namespace SoundLibrary.Mathematics.Continuous
{
	/// <summary>
	/// ��`��͈̔͂�\���\���́B
	/// </summary>
	public struct Range
	{
		bool hasMin;
		bool hasMax;
		double inf;
		double sup;

		/// <summary>
		/// �ŏ��E�ő�l�̗L���A����E�������w�肵�ď������B
		/// ��:
		/// (1, 2] �� new Range(false, true, 1, 2)�A
		/// [0, 1] �� new Range(true, true, 0, 1)
		/// </summary>
		/// <param name="hasMin"></param>
		/// <param name="hasMax"></param>
		/// <param name="inf"></param>
		/// <param name="sup"></param>
		public Range(bool hasMin, bool hasMax, double inf, double sup)
		{
			this.hasMax = hasMax;
			this.hasMin = hasMin;
			this.sup = sup;
			this.inf = inf;
		}

		/// <summary>
		/// �ő�l�������ǂ����B
		/// </summary>
		public bool HasMaximum
		{
			get{return this.hasMax;}
			set{this.hasMax = value;}
		}

		/// <summary>
		/// �ŏ��l�������ǂ����B
		/// </summary>
		public bool HasMinimum
		{
			get{return this.hasMin;}
			set{this.hasMin = value;}
		}

		/// <summary>
		/// ����B
		/// </summary>
		public double Supremum
		{
			get{return this.sup;}
			set{this.sup = value;}
		}

		/// <summary>
		/// �����B
		/// </summary>
		public double Infimum
		{
			get{return this.inf;}
			set{this.inf = value;}
		}
	}
}
