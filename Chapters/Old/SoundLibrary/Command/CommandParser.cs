using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace SoundLibrary.Command
{
	/// <summary>
	/// �R�}���h�����p�̃f���Q�[�g�B
	/// </summary>
	/// <remarks>
	/// �������X�g <paramref name="args"/> ���󂯎���ď������s���A
	/// �o�̓X�g���[�� <paramref name="sout"/> �Ɍ��ʂ��o�͂���B
	/// �R�}���h���߂��I���������Ƃ��� true ��Ԃ��B
	/// </remarks>
	public delegate bool CommandHandlar(string[] args, TextWriter sout);

	/// <summary>
	/// �e�L�X�g�X�g���[��������͂��ꂽ�R�}���h�̉��߂��s���N���X�B
	/// </summary>
	public class CommandParser
	{
		#region �����N���X

		class Tuple
		{
			public CommandHandlar ope;
			public string help;

			public Tuple(CommandHandlar ope, string help)
			{
				this.ope = ope;
				this.help = help;
			}
		}

		#endregion
		#region �t�B�[���h

		Hashtable commands;
		CommandHandlar notFound; // �R�}���h��������Ȃ������Ƃ��̏���

		string prompt;
		Regex delim1; // �R�}���h�ƈ����̋�؂蕶��
		Regex delim2; // �������m�̊Ԃ̋�؂蕶��
		Regex delim3; // ���_�C���N�g�p�̋�؂蕶��

		#endregion
		#region ������

		/// <summary>
		/// �������B
		/// ��؂蕶���ɋ󔒕��� ("\s+") ���g�p�B
		/// ���_�C���N�g�p�� > (\s*>\s*) ���g�p�B
		/// </summary>
		public CommandParser() : this("> ", @"\s+", @"\s+", @"\s*>\s*")
		{
		}

		/// <summary>
		/// �������B
		/// �R�}���h�A�����Ԃ̋�؂蕶��(���K�\��)���w��B
		/// </summary>
		/// <param name="delim1">�R�}���h�ƈ����̋�؂蕶��</param>
		/// <param name="delim2">�������m�̊Ԃ̋�؂蕶��</param>
		/// <param name="delim3">���_�C���N�g�p�̋�؂蕶��</param>
		public CommandParser(string prompt, string delim1, string delim2, string delim3)
		{
			this.commands = new Hashtable();
			this.commands[HELP_COMMAND] = new Tuple(new CommandHandlar(this.ShowHelp), HELP_MESSAGE);
			this.commands[QUIT_COMMAND] = new Tuple(new CommandHandlar(Quit), QUIT_MESSAGE);
			this.commands[SOURCE_COMMAND] = new Tuple(new CommandHandlar(this.Source), SOURCE_MESSAGE);

			this.notFound = new CommandHandlar(this.DefaultNotFound);

			this.prompt = prompt;
			this.delim1 = new Regex(delim1);
			this.delim2 = new Regex(delim2);
			this.delim3 = new Regex(delim3);
		}

		#endregion
		#region �R�}���h�̉���

		/// <summary>
		/// �W�����̓X�g���[������R�}���h��ǂݏo���ĉ��߁B
		/// </summary>
		/// <returns>�R�}���h���߂��I������Ƃ� true ��Ԃ�</returns>
		public bool Parse()
		{
			return this.Parse(Console.In, Console.Out);
		}

		/// <summary>
		/// �X�g���[������R�}���h��ǂݏo���ĉ��߁B
		/// �W���o�̓X�g���[���ɏo�́B
		/// </summary>
		/// <param name="sin">���͌�</param>
		/// <returns>�R�}���h���߂��I������Ƃ� true ��Ԃ�</returns>
		public bool Parse(TextReader sin)
		{
			return this.Parse(sin, Console.Out);
		}

		/// <summary>
		/// �X�g���[������R�}���h��ǂݏo���ĉ��߁B
		/// </summary>
		/// <param name="sin">���͌�</param>
		/// <param name="sout">�o�͐�</param>
		/// <returns>�R�}���h���߂��I������Ƃ� true ��Ԃ�</returns>
		public bool Parse(TextReader sin, TextWriter sout)
		{
			while(true)
			{
				if(sin == Console.In && sout == Console.Out)
					sout.Write(this.prompt);

				string line = sin.ReadLine();

				if(line == null)
					break;

				string[] tmp = this.delim3.Split(line);
				if(tmp.Length <= 1)
				{
					if(Parse(tmp[0], sout))
						return true;
				}
				else
				{
					StreamWriter sout1 = null;
					try
					{
						sout1 = new StreamWriter(tmp[1], false, System.Text.Encoding.Default);
						if(Parse(tmp[0], sout1))
							return true;
					}
					catch(Exception exc)
					{
						Console.Error.Write(exc);
					}
					finally
					{
						if(sout1 != null) sout1.Close();
					}
				}
			}

			return false;
		}

		/// <summary>
		/// �R�}���h��1���C�����߁B
		/// </summary>
		/// <param name="commandLine">�R�}���h���C��</param>
		/// <returns>�R�}���h���߂��I������Ƃ� true ��Ԃ�</returns>
		bool Parse(string commandLine, TextWriter sout)
		{
			if(commandLine == null || commandLine.Length == 0)
			{
				ShowHelp(sout);
				return false;
			}

			string[] tmp = this.delim1.Split(commandLine, 2);

			Tuple t = (Tuple)this.commands[tmp[0]];

			CommandHandlar ope;
			string[] args;

			if(t == null)
			{
				ope = this.notFound;
				args = tmp;
			}
			else
			{
				ope = t.ope;
				if(tmp.Length < 2)
					args = null;
				else
					args = this.delim2.Split(tmp[1]);
			}
			
			if(ope == null)
			{
				return true;
			}

			return ope(args, sout);
		}


		#endregion
		#region �R�}���h�̒ǉ��E�폜

		/// <summary>
		/// �R�}���h��ǉ�����B
		/// </summary>
		/// <param name="command">�R�}���h��</param>
		/// <param name="ope">�R�}���h�����f���Q�[�g</param>
		public void Add(string command, CommandHandlar ope)
		{
			this.Add(command, ope, "");
		}

		/// <summary>
		/// �R�}���h��ǉ�����B
		/// </summary>
		/// <param name="command">�R�}���h��</param>
		/// <param name="ope">�R�}���h�����f���Q�[�g</param>
		/// <param name="help">�w���v���b�Z�[�W</param>
		public void Add(string command, CommandHandlar ope, string help)
		{
			this.commands[command] = new Tuple(ope, help);
		}

		/// <summary>
		/// �R�}���h���폜����B
		/// </summary>
		/// <param name="command">�R�}���h��</param>
		public void Remove(string command)
		{
			this.commands.Remove(command);
		}

		#endregion
		#region �f�t�H���g�R�}���h

		const string QUIT_COMMAND = "quit";
		const string QUIT_MESSAGE = "�I�����܂��B\n";

		const string HELP_COMMAND = "help";
		const string HELP_MESSAGE = "�w���v��\�����܂��B\n";

		const string SOURCE_COMMAND = "source";
		const string SOURCE_MESSAGE = "source [�t�@�C����]\n�e�L�X�g�t�@�C������R�}���h��ǂݍ��݂܂��B\n";

		#region �R�}���h��������Ȃ������Ƃ��p

		/// <summary>
		/// �R�}���h���o�^���p�f���Q�[�g��o�^�B
		/// </summary>
		/// <param name="notFound">�R�}���h���o�^���p�f���Q�[�g</param>
		void SetNotFound(CommandHandlar notFound)
		{
			if(notFound == null)
				this.notFound = new CommandHandlar(this.DefaultNotFound);
			else
				this.notFound = notFound;
		}

		/// <summary>
		/// �R�}���h���o�^���A�f�t�H���g�̓���B
		/// </summary>
		bool DefaultNotFound(string[] args, TextWriter sout)
		{
			if(args.Length >= 1)
				sout.Write("{0} �Ƃ����R�}���h�͂���܂���B\n", args[0]);
			this.ShowHelp(sout);
			return false;
		}

		#endregion
		#region �w���v�p

		/// <summary>
		/// �w���v�\���B
		/// </summary>
		void ShowHelp(TextWriter sout)
		{
			sout.Write("help [�R�}���h��]\n�Ɠ��͂��邱�ƂŁA�e�R�}���h�̃w���v��\���ł��܂��B\n");

			int i=0;
			foreach(string command in commands.Keys)
			{
				if(i % 4 == 3)
					sout.Write("\n");
				sout.Write("{0,10}", command);
			}
			sout.Write("\n");
		}

		/// <summary>
		/// �e�R�}���h�̃w���v�\���B
		/// </summary>
		/// <param name="command">�R�}���h��</param>
		/// <param name="sout">�o�͐�B</param>
		bool ShowHelp(string[] command, TextWriter sout)
		{
			if(command == null || command.Length == 0)
				this.ShowHelp(sout);
			else
			{
				Tuple t = (Tuple)this.commands[command[0]];

				if(t == null)
					return this.notFound(command, sout);

				sout.Write(t.help);
			}
			return false;
		}

		#endregion
		#region �I�������p

		static bool Quit(string[] args, TextWriter sout)
		{
			return true;
		}

		#endregion
		#region �O���t�@�C���ǂݍ���

		/// <summary>
		/// �e�L�X�g�t�@�C������R�}���h��ǂݏo���B
		/// </summary>
		bool Source(string[] args, TextWriter sout)
		{
			if(args.Length < 1)
			{
				sout.Write(SOURCE_MESSAGE);
				return false;
			}

			StreamReader sin = null;
			try
			{
				sin = new StreamReader(args[0], System.Text.Encoding.Default);
				return this.Parse(sin, sout);
			}
			catch(Exception exc)
			{
				Console.Error.Write(exc);
				return true;
			}
			finally
			{
				if(sin != null) sin.Close();
			}
		}

		#endregion
		
		#endregion
	}
}
