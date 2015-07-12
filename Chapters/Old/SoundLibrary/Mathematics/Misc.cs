using System;
using System.Collections;

namespace SoundLibrary.Mathematics
{
	/// <summary>
	/// �����ނ̊֐��Q�B
	/// ���ސ悪���܂莟��A���̏ꏊ�Ɉڂ������B
	/// </summary>
	public class Misc
	{
		/// <summary>
		/// �g�ݍ��킹�̐� nCk �����߂�B
		/// </summary>
		/// <param name="n">n</param>
		/// <param name="k">k</param>
		/// <returns>nCk</returns>
		public static int Combination(int n, int k)
		{
			int c = 1;
			for(int i=0; i<k;)
			{
				++i;
				c *= n;
				c /= i;
				--n;
			}
			return c;
		}

		public static ArrayList Merge(IList l1, IList l2)
		{
			ArrayList merged = new ArrayList();

			int i=0, j=0;
			while(i<l1.Count && j<l2.Count)
			{
				IComparable c1 = (IComparable)l1[i];
				IComparable c2 = (IComparable)l2[j];

				int cmp = c1.CompareTo(c2);
				if(cmp < 0)
				{
					merged.Add(c1);
					++i;
				}
				else if(cmp > 0)
				{
					merged.Add(c2);
					++j;
				}
				else
				{
					merged.Add(c1);
					++i;
					++j;
				}
			}

			for(; i<l1.Count; ++i) merged.Add(l1[i]);
			for(; j<l2.Count; ++j) merged.Add(l2[j]);

			return merged;
		}
	}
}
