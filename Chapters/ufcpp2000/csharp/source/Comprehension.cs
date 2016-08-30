using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ConsoleApplication1
{
	class Program
	{
		/// <summary>
		/// �ȉ��̂悤�ȁA������ŉ������߂�ނ̖����A
		/// LINQ �Ƃ����g���ĉ����Ă݂�B
		/// 
		/// �ݖ�F
		/// Baker, Cooper, Fletcher, Miller��Smith�͌܊K���ăA�p�[�g�̈قȂ�K�ɏZ��ł���B
		/// Baker�͍ŏ�K�ɏZ�ނ̂ł͂Ȃ��B
		/// Cooper�͍ŉ��K�ɏZ�ނ̂ł͂Ȃ��B
		/// Fletcher�͍ŏ�K�ɂ��ŉ��K�ɂ��Z�ނ̂ł͂Ȃ��B
		/// Miller��Cooper����̊K�ɏZ��ł���B
		/// Smith��Fletcher�ׂ̗̊K�ɏZ�ނ̂ł͂Ȃ��B
		/// Fletcher��Cooper�ׂ̗̊K�ɏZ�ނ̂ł͂Ȃ��B
		/// ���ꂼ��͂ǂ̊K�ɏZ��ł��邩�B 
		/// </summary>
		/// <remarks>
		/// ���s���ʂ̂܂Ƃ߁F
		/// �E���̎�̒P���ȃN�G���́A
		///   �N�G���̏�������ւ���10�{�ȏ�p�t�H�[�}���X�������邱�Ƃ�����B
		/// �E�ł��Afrom ���O�Ɍł܂��Ă���̂Ɣ�ׂāA���������ւ����N�G���͌��\���Â炢�B
		/// �E���d from ���g�����N�G���������\�b�h�`���ŏ������Ƃ���� SelectMany �̓��ߎ��ʎq�����炢���ƂɁB
		/// �Efrom, where, select �� foreach, if, yield return �œW�J����ƁA�p�t�H�[�}���X1.5�`2�{���炢���������肷��B
		/// �Eyield return�i������C�e���[�^�j�ƁA��x List.Add ���Ă��炻�� List ��Ԃ��̂̃p�t�H�[�}���X�͑卷�Ȃ��B
		/// 
		/// ���_�F
		/// IQueryable / �����_�����g���āA
		/// from, where, select �� foreach, if, yield return �ɓW�J
		/// �� �N�G���̏����œK����������悤�ȃ��C�u�������~�����Ȃ��B
		/// </remarks>
		static void Main()
		{
			// �ݖ�ǂ���̏����ŃN�G��
			var answers1 =
				from baker in five
				from cooper in five
				from fletcher in five
				from miller in five
				from smith in five
				where Distinct(baker, cooper, fletcher, miller, smith)
				where baker != 5
				where cooper != 1
				where fletcher != 1 && fletcher != 5
				where miller > cooper
				where Discrete(smith, fletcher)
				where Discrete(fletcher, cooper)
				select new { baker, cooper, fletcher, miller, smith };

			// answers1 �̃N�G�����Ɠ����ȃN�G�����Z
			var answers0 = five
				.SelectMany(x => five, (baker, cooper) => new { baker, cooper })
				.SelectMany(x => five, (x, fletcher) => new { x, fletcher })
				.SelectMany(x => five, (x, miller) => new { x, miller })
				.SelectMany(x => five, (x, smith) => new { x, smith })
				.Where(x => Distinct(x.x.x.x.baker, x.x.x.x.cooper, x.x.x.fletcher, x.x.miller, x.smith))
				.Where(x => x.x.x.x.baker != 5)
				.Where(x => x.x.x.x.cooper != 1)
				.Where(x => x.x.x.fletcher != 1 && x.x.x.fletcher != 5)
				.Where(x => x.x.miller > x.x.x.x.cooper)
				.Where(x => Discrete(x.smith, x.x.x.fletcher))
				.Where(x => Discrete(x.x.x.fletcher, x.x.x.x.cooper))
				.Select(x => new { x.x.x.x.baker, x.x.x.x.cooper, x.x.x.fletcher, x.x.miller, x.smith });

			// answers0 �̓��ߎ��ʎq��������Ɛ���
			var answers01 = five
				.SelectMany(x => five, (baker, cooper) => new { baker, cooper })
				.SelectMany(x => five, (x, fletcher) => new { x.baker, x.cooper, fletcher })
				.SelectMany(x => five, (x, miller) => new { x.baker, x.cooper, x.fletcher, miller })
				.SelectMany(x => five, (x, smith) => new { x.baker, x.cooper, x.fletcher, x.miller, smith })
				.Where(x => Distinct(x.baker, x.cooper, x.fletcher, x.miller, x.smith))
				.Where(x => x.baker != 5)
				.Where(x => x.cooper != 1)
				.Where(x => x.fletcher != 1 && x.fletcher != 5)
				.Where(x => x.miller > x.cooper)
				.Where(x => Discrete(x.smith, x.fletcher))
				.Where(x => Discrete(x.fletcher, x.cooper));

			// answers1 �� from, where �̏��������ւ��čœK��
			var answers2 =
				from baker in five
				where baker != 5
				from cooper in five
				where cooper != 1
				from fletcher in five
				where fletcher != 1 && fletcher != 5
				where Discrete(fletcher, cooper)
				from miller in five
				where miller > cooper
				from smith in five
				where Discrete(smith, fletcher)
				where Distinct(baker, cooper, fletcher, miller, smith)
				select new { baker, cooper, fletcher, miller, smith };

			// answers2 �Ƃقړ����i���ߎ��ʎq���������j�ȃN�G�����Z
			var answers02 = five
				.Where(baker => baker != 5)
				.SelectMany(x => five, (baker, cooper) => new { baker, cooper })
				.Where(x => x.cooper != 1)
				.SelectMany(x => five, (x, fletcher) => new { x.baker, x.cooper, fletcher })
				.Where(x => x.fletcher != 1 && x.fletcher != 5)
				.Where(x => Discrete(x.fletcher, x.cooper))
				.SelectMany(x => five, (x, miller) => new { x.baker, x.cooper, x.fletcher, miller })
				.Where(x => x.miller > x.cooper)
				.SelectMany(x => five, (x, smith) => new { x.baker, x.cooper, x.fletcher, x.miller, smith })
				.Where(x => Discrete(x.smith, x.fletcher))
				.Where(x => Distinct(x.baker, x.cooper, x.fletcher, x.miller, x.smith));

			CheckPerformance(answers1, "�N�G���� �@�@�@");
			CheckPerformance(answers0, "���\�b�h (����)");
			CheckPerformance(answers01, "���\�b�h �@�@�@");
			CheckPerformance(YieldAnswers1(), "yield    �@�@�@");
			CheckPerformance<Tuple>(ListAnswers1, "list     �@�@�@");

			CheckPerformance(answers2, "�N�G���� �œK��");
			CheckPerformance(answers02, "���\�b�h �œK��");
			CheckPerformance(YieldAnswers2(), "yield    �œK��");
			CheckPerformance<Tuple>(ListAnswers2, "list     �œK��");
		}

		#region �⏕�֐�

		// 1�`5
		static IEnumerable<int> five = Enumerable.Range(1, 5);

		// x �̗v�f�ɏd�����Ȃ��Ƃ� true
		static bool Distinct(params int[] x)
		{
			return x.Distinct().Count() == x.Length;
		}

		// x, y ���ׂ荇�������łȂ��Ƃ� true
		static bool Discrete(int x, int y)
		{
			return Math.Abs(checked(x - y)) != 1;
		}

		#endregion
		#region �p�t�H�[�}���X�v��

		const int N = 500;
		static bool quiet = true;

		/// <summary>
		/// �N�G���̃p�t�H�[�}���X�̊m�F�B
		/// �V�[�P���X�� N �� ToList() ����̂ɂ����鎞�Ԃ��v���B
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="seq">�p�t�H�[�}���X���v�肽���V�[�P���X</param>
		/// <param name="label">���ʕ\���p�̃��x��</param>
		static void CheckPerformance<T>(IEnumerable<T> seq, string label)
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < N; ++i)
			{
				var x = seq.ToList();
			}
			sw.Stop();
			if (!quiet)
				foreach (var x in seq)
					Console.WriteLine(x);
			Console.Write(label + " {0}\n", sw.ElapsedMilliseconds);
		}

		// ��r�p�B���X�g�ŁB
		static void CheckPerformance<T>(Func<List<T>> getList, string label)
		{
			var sw = new Stopwatch();
			sw.Start();
			for (int i = 0; i < N; ++i)
			{
				var x = getList();
			}
			sw.Stop();
			if (!quiet)
				foreach (var x in getList())
					Console.WriteLine(x);
			Console.Write(label + " {0}\n", sw.ElapsedMilliseconds);
		}

		#endregion
		#region �C�e���[�^��

		/// <summary>
		/// ��r�̂��߁A�C�e���[�^�ł���肽�����ǁA
		/// �C�e���[�^�͓������\�b�h�ł͍��Ȃ��i�� �����^��Ԃ��Ȃ��j�̂�
		/// �����Ȍ^���쐬�B
		/// </summary>
		struct Tuple
		{
			public int baker { get; set; }
			public int cooper { get; set; }
			public int fletcher { get; set; }
			public int miller { get; set; }
			public int smith { get; set; }

			public override string ToString()
			{
				return
					"{ " +
					string.Format("baker = {0}, cooper = {1}, fletcher = {2}, miller = {3}, smith = {4}",
						baker, cooper, fletcher, miller, smith) +
					" }";
			}
		}

		// answers1 �����̃C�e���[�^
		static IEnumerable<Tuple> YieldAnswers1()
		{
			foreach (var baker in five)
			foreach (var cooper in five)
			foreach (var fletcher in five)
			foreach (var miller in five)
			foreach (var smith in five)
			if (Distinct(baker, cooper, fletcher, miller, smith))
			if (baker != 5)
			if (cooper != 1)
			if (fletcher != 1 && fletcher != 5)
			if (miller > cooper)
			if (Discrete(smith, fletcher))
			if (Discrete(fletcher, cooper))
			yield return new Tuple { baker = baker, cooper = cooper, fletcher = fletcher, miller = miller, smith = smith };
		}

		// answers2 �����̃C�e���[�^
		static IEnumerable<Tuple> YieldAnswers2()
		{
			foreach (var baker in five)
			if (baker != 5)
			foreach (var cooper in five)
			if (cooper != 1)
			foreach (var fletcher in five)
			if (fletcher != 1 && fletcher != 5)
			if (Discrete(fletcher, cooper))
			foreach (var miller in five)
			if (miller > cooper)
			foreach (var smith in five)
			if (Discrete(smith, fletcher))
			if (Distinct(baker, cooper, fletcher, miller, smith))
			yield return new Tuple { baker = baker, cooper = cooper, fletcher = fletcher, miller = miller, smith = smith };
		}

		#endregion
		#region ���X�g��

		// ��r�p�BYieldAnswers1 �̃��X�g��
		static List<Tuple> ListAnswers1()
		{
			var list = new List<Tuple>();
			foreach (var baker in five)
			foreach (var cooper in five)
			foreach (var fletcher in five)
			foreach (var miller in five)
			foreach (var smith in five)
			if (Distinct(baker, cooper, fletcher, miller, smith))
			if (baker != 5)
			if (cooper != 1)
			if (fletcher != 1 && fletcher != 5)
			if (miller > cooper)
			if (Discrete(smith, fletcher))
			if (Discrete(fletcher, cooper))
			list.Add(new Tuple { baker = baker, cooper = cooper, fletcher = fletcher, miller = miller, smith = smith });
			return list;
		}

		// ��r�p�BYieldAnswers2 �̃��X�g��
		static List<Tuple> ListAnswers2()
		{
			var list = new List<Tuple>();
			foreach (var baker in five)
			if (baker != 5)
			foreach (var cooper in five)
			if (cooper != 1)
			foreach (var fletcher in five)
			if (fletcher != 1 && fletcher != 5)
			if (Discrete(fletcher, cooper))
			foreach (var miller in five)
			if (miller > cooper)
			foreach (var smith in five)
			if (Discrete(smith, fletcher))
			if (Distinct(baker, cooper, fletcher, miller, smith))
			list.Add(new Tuple { baker = baker, cooper = cooper, fletcher = fletcher, miller = miller, smith = smith });
			return list;
		}

		#endregion
	}
}
