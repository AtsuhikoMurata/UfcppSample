using System;
using System.IO;
using System.Text;
using System.Collections;

namespace SoundLibrary.WaveAnalysis
{
	/// <summary>
	/// �f�[�^���X�g�̍��ځB
	/// </summary>
	public class DataItem
	{
		public string title;
		public double[] data;

		public DataItem(string title, double[] data)
		{
			this.title = title;
			this.data = data;
		}
	}

	/// <summary>
	/// �����f�[�^�̃��X�g�B
	/// </summary>
	public class DataList
	{
		ArrayList list = new ArrayList();
		int count = -1;

		/// <summary>
		/// ���ڂ̒ǉ�
		/// </summary>
		/// <param name="title">���ږ�</param>
		/// <param name="data">�f�[�^</param>
		public void Add(string title, double[] data)
		{
			if(this.count == -1) this.count = data.Length;
			else if(this.count != data.Length) return;

			list.Add(new DataItem(title, data));
		}

		/// <summary>
		/// �f�[�^���X�g�� CSV �`���Ńt�@�C���ɕۑ��B
		/// </summary>
		/// <param name="writer">�������ݐ�</param>
		/// <param name="delim">��؂蕶��</param>
		/// <param name="outputTitle">�w�b�_�s���o�͂��邩�ǂ���</param>
		public void Save(StreamWriter writer, char delim, bool outputTitle)
		{
			if(this.count == -1) return;

			if(outputTitle)
			{
				int i=0;
				for(; i<list.Count-1; ++i)
				{
					writer.Write("{0}{1}", ((DataItem)this.list[i]).title, delim);
				}
				writer.Write("{0}\n", ((DataItem)this.list[i]).title);
			}

			for(int j=0; j<this.count; ++j)
			{
				int i=0;
				for(; i<list.Count-1; ++i)
				{
					writer.Write("{0}{1}", ((DataItem)this.list[i]).data[j], delim);
				}
				writer.Write("{0}\n", ((DataItem)this.list[i]).data[j]);
			}
		}//Save

		/// <summary>
		/// �f�[�^���X�g�� CSV �`���Ńt�@�C���ɕۑ��B
		/// </summary>
		/// <param name="writer">�������ݐ�</param>
		/// <param name="delim">��؂蕶��</param>
		public void Save(StreamWriter writer, char delim)
		{
			this.Save(writer, delim, false);
		}

		/// <summary>
		/// �f�[�^���X�g�� CSV �`���Ńt�@�C���ɕۑ��B
		/// </summary>
		/// <param name="filename">�o�̓t�@�C����</param>
		/// <param name="delim">��؂蕶��</param>
		/// <param name="outputTitle">�w�b�_�s���o�͂��邩�ǂ���</param>
		public void Save(string filename, char delim, bool outputHeader)
		{
			using(StreamWriter writer = new StreamWriter(filename, false, Encoding.Default))
			{
				this.Save(writer, delim, outputHeader);
			}
		}

		/// <summary>
		/// �f�[�^���X�g�� CSV �`���Ńt�@�C���ɕۑ��B
		/// </summary>
		/// <param name="filename">�o�̓t�@�C����</param>
		/// <param name="delim">��؂蕶��</param>
		public void Save(string filename, char delim)
		{
			this.Save(filename, delim, false);
		}
	}//class DataList
}//namespace WaveAnalysis
