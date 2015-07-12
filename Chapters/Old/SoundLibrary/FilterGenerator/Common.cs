using System;
using System.Collections;
using System.Xml;
using System.Reflection;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// �t�B���^�̍\�����B
	/// </summary>
	public class FilterProperty
	{
		string name;
		object obj;
		Type type;

		internal FilterProperty(string name, object obj)
		{
			this.name = name;
			this.obj = obj;
			this.type = obj.GetType();
		}

		/// <summary>
		/// �v���p�e�B�̒l���擾�E�ݒ�B
		/// </summary>
		public object Value
		{
			set
			{
				if(value == null)
					throw new ArgumentNullException("null �͐ݒ�ł��܂���");

				if(!this.type.IsAssignableFrom(value.GetType()))
					throw new ArgumentException("�^����v���Ă��܂���");

				this.obj = value;
			}
			get
			{
				return this.obj;
			}
		}

		/// <summary>
		/// �v���p�e�B�����擾�B
		/// </summary>
		public string Name()
		{
			return this.name;
		}

		/// <summary>
		/// �v���p�e�B�̌^���擾�B
		/// </summary>
		public Type Type()
		{
			return this.type;
		}
	}

	/// <summary>
	/// �t�B���^�̔z��^�\�����
	/// </summary>
	public class FilterArrayProperty
	{
		public struct Tuple
		{
			public string name;
			public Type type;

			public Tuple(string name, Type type)
			{
				this.name = name;
				this.type = type;
			}
		}

		Tuple[] informations;
		ArrayList propertyList;

		public FilterArrayProperty(params Tuple[] informations)
		{
			this.informations = informations;
			this.propertyList = new ArrayList();
		}

		/// <summary>
		/// i �Ԗڂ̔z�񑮐��� j �Ԗڂ̑������擾/�ݒ肷��B
		/// </summary>
		public object this[int i, int j]
		{
			set
			{
				if(value == null)
					throw new ArgumentNullException("null �͐ݒ�ł��܂���");

				if(!this.informations[i].type.IsAssignableFrom(value.GetType()))
					throw new ArgumentException("�^����v���Ă��܂���");

				((object[])this.propertyList[i])[j] = value;
			}
			get
			{
				return ((object[])this.propertyList[i])[j];
			}
		}

		/// <summary>
		/// �\������ǉ�����B
		/// </summary>
		/// <param name="objs">���ڂ̑���</param>
		public void Add(params object[] objs)
		{
			if(objs == null)
				throw new ArgumentNullException("null �͐ݒ�ł��܂���");

			if(this.informations.Length != objs.Length)
				throw new ArgumentException("��������v���Ă��܂���");

			for(int i=0; i<objs.Length; ++i)
			{
				if(!this.informations[i].type.IsAssignableFrom(objs[i].GetType()))
					throw new ArgumentException("�^����v���Ă��܂���");
			}

			this.propertyList.Add(objs);
		}

		/// <summary>
		/// �\�������폜����B
		/// </summary>
		/// <param name="i">�폜�ʒu</param>
		public void Remove(int i)
		{
			this.propertyList.RemoveAt(i);
		}

		/// <summary>
		/// ���X�g�̒����B
		/// </summary>
		public int ListLength
		{
			get{return this.propertyList.Count;}
		}

		/// <summary>
		/// �\�����̐����擾����B
		/// </summary>
		public int Count
		{
			get{return this.informations.Length;}
		}

		/// <summary>
		/// �v���p�e�B�����擾�B
		/// </summary>
		public string Name(int i)
		{
			return this.informations[i].name;
		}

		/// <summary>
		/// �v���p�e�B�̌^���擾�B
		/// </summary>
		public Type Type(int i)
		{
			return this.informations[i].type;
		}
	}

	/// <summary>
	/// �t�B���^�쐬�N���X�̋��ʕ������W�߂����ۊ��N���X�B
	/// �v���p�e�B�̊Ǘ������͂��̃N���X�ł���B
	/// </summary>
	public abstract class FilterGenerator
	{
		/// <summary>
		/// �t�B���^�̍\�����B
		/// Amplifier �� gain �Ƃ��AFirFilter �� coef �Ƃ��A
		/// ���ʂ̍\�����B
		/// </summary>
		protected FilterProperty[] properties = null;

		/// <summary>
		/// ���X�g�ɂȂ��Ă���\�����(�z��^�\�����ƌĂԂ��Ƃɂ���)�B
		/// SerialConnector �� (filter) �Ƃ� Mixer �� (gain, filter)�Ƃ��A
		/// �ϒ��̂��̍\�����B
		/// </summary>
		protected FilterArrayProperty[] arrayProperties = null;

		/// <summary>
		/// ���l�ϊ��p�B
		/// </summary>
		protected Converter converter = new Converter();

		/// <summary>
		/// �\�����̐����擾����B
		/// </summary>
		public int Count
		{
			get
			{
				if(this.properties == null)
					return 0;
				else
					return this.properties.Length;
			}
		}

		/// <summary>
		/// �t�B���^�̍\�������擾����B
		/// </summary>
		[System.Runtime.CompilerServices.IndexerName("Property")]
		public FilterProperty this[int i]
		{
			get
			{
				return this.properties[i];
			}
		}

		/// <summary>
		/// �z��^�\�����̐����擾����B
		/// </summary>
		public int ArrayCount
		{
			get
			{
				if(this.arrayProperties == null)
					return 0;
				else
					return this.arrayProperties.Length;
			}
		}

		/// <summary>
		/// �t�B���^�̔z��^�\�������擾����B
		/// </summary>
		public FilterArrayProperty GetArrayProperty(int i)
		{
			return this.arrayProperties[i];
		}

		/// <summary>
		/// �t�B���^���쐬����B
		/// </summary>
		/// <returns>�쐬�����t�B���^</returns>
		public abstract IFilter GetFilter();

		/// <summary>
		/// �ݒ肵���������t�B���^�̐���𖞂����Ă��邩�ǂ����`�F�b�N����B
		/// </summary>
		/// <returns>����𖞂����Ă���� null�A�������Ă��Ȃ���΃G���[���b�Z�[�W��Ԃ�</returns>
		public abstract string CheckConstraint();

		/// <summary>
		/// XML �Ƀt�B���^�\�����o�́B
		/// </summary>
		/// <param name="xwriter">�o�͐�</param>
		public abstract void ToXml(XmlWriter xwriter);

		/// <summary>
		/// XML ����t�B���^�\������́B
		/// </summary>
		/// <param name="elem">���͌�</param>
		public abstract void FromXml(XmlElement elem);

		/// <summary>
		/// �t�B���^�\���� XML �`���Ńt�@�C���o�́B
		/// </summary>
		/// <param name="filename">XML �t�@�C����</param>
		public void WriteXml(string filename)
		{
			XmlTextWriter xwriter = new XmlTextWriter(filename, System.Text.Encoding.Default);
			xwriter.WriteStartDocument();
			this.ToXml(xwriter);
			xwriter.WriteEndDocument();
			xwriter.Close();
		}

		/// <summary>
		/// XML �t�@�C�����̃t�B���^�\����񂩂� FilterGenerator ���쐬�B
		/// </summary>
		/// <param name="filename">XML �t�@�C����</param>
		/// <returns>�쐬���� FilterGenerator</returns>
		public static FilterGenerator CreateFromXml(string filename)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(filename);
			FilterGenerator gen = CreateFromXml(xdoc.DocumentElement);
			return gen;
		}

		/// <summary>
		/// XML �t�@�C�����̃t�B���^�\����񂩂� FilterGenerator ���쐬�B
		/// </summary>
		/// <param name="filename">XML �t�@�C����</param>
		/// <returns>�쐬���� FilterGenerator</returns>
		public static FilterGenerator CreateFromXml(string filename, Converter converter)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(filename);
			FilterGenerator gen = CreateFromXml(xdoc.DocumentElement, converter);
			return gen;
		}

		/// <summary>
		/// XML ���� FilterGenerator ���쐬�B
		/// </summary>
		/// <param name="xreader">����</param>
		/// <returns>�쐬���� FilterGenerator</returns>
		internal static FilterGenerator CreateFromXml(XmlElement elem)
		{
			return FilterGenerator.CreateFromXml(elem, null);
		}

		/// <summary>
		/// XML ���� FilterGenerator ���쐬�B
		/// </summary>
		/// <param name="xreader">����</param>
		/// <returns>�쐬���� FilterGenerator</returns>
		internal static FilterGenerator CreateFromXml(XmlElement elem, Converter converter)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			FilterGenerator gen = (FilterGenerator)asm.CreateInstance("SoundLibrary.Filter." + elem.LocalName + "Generator");

			if(gen == null)
				throw new NotSupportedException("SoundLibrary.Filter." + elem.LocalName + " �͎�������Ă��܂���B");

			if(converter != null)
				gen.converter = converter;

			XmlAttribute att;
			att = elem.Attributes["samplingRate"];
			if(att != null)
				gen.SamplingRate = double.Parse(att.Value);
			att = elem.Attributes["dB"];
			if(att != null)
				gen.IsDB = att.Value == "true";

			gen.FromXml(elem);
			return gen;
		}

		/// <summary>
		/// IsDB ���^�̂Ƃ��A���͂��ꂽ������� dB �l��\���Ă���̂Ƃ��āA
		/// dB �� ���j�A�l�̕ϊ����s���B
		/// </summary>
		public bool IsDB
		{
			set{this.converter.IsDB = value;}
			get{return this.converter.IsDB;}
		}

		/// <summary>
		/// SamplingRate ���� 0 �̂Ƃ��A���̎��g���Ő��K�����s���B
		/// </summary>
		public double SamplingRate
		{
			set{this.converter.SamplingRate = value;}
			get{return this.converter.SamplingRate;}
		}
	}//class FilterGenerator

	public class Util
	{
		public static string ArrayToString(double[] array)
		{
			string str = array[0].ToString();
			for(int i=1; i<array.Length; ++i)
				str += ',' + array[i].ToString();
			return str;
		}

		public static double[] StringToArray(string str)
		{
			string[] tokens = str.Split(',');
			double[] array = new double[tokens.Length];
			for(int i=0; i<tokens.Length; ++i)
				array[i] = double.Parse(tokens[i]);
			return array;
		}
	}

	/// <summary>
	/// �����񁨐��l�ϊ��N���X�B
	/// dB �� ���j�A�l�̕ϊ���A���g���̐��K�����s���B
	/// </summary>
	public class Converter
	{
		bool isDB = false;
		double samplingRate = 0;

		/// <summary>
		/// IsDB ���^�̂Ƃ��A���͂��ꂽ������� dB �l��\���Ă���̂Ƃ��āA
		/// dB �� ���j�A�l�̕ϊ����s���B
		/// </summary>
		public bool IsDB
		{
			set{this.isDB = value;}
			get{return this.isDB;}
		}

		/// <summary>
		/// SamplingRate ���� 0 �̂Ƃ��A���̎��g���Ő��K�����s���B
		/// </summary>
		public double SamplingRate
		{
			set{this.samplingRate = value;}
			get{return this.samplingRate;}
		}

		/// <summary>
		/// �����񁨐��l�ϊ��B�U���ŁB
		/// IsDB �̒l�ɉ����Đ��l��ϊ�����B
		/// </summary>
		/// <param name="str">�ϊ���</param>
		/// <returns>�ϊ�����</returns>
		public double ToPower(string str)
		{
			double x = double.Parse(str);

			if(this.isDB)
				return SoundLibrary.Util.DBToLinear(x);

			return x;
		}

		/// <summary>
		/// �����񁨐��l�ϊ��B���g���ŁB
		/// SamplingRate �̒l�ɉ����Đ��l��ϊ�����B
		/// </summary>
		/// <param name="str">�ϊ���</param>
		/// <returns>�ϊ�����</returns>
		public double ToFrequency(string str)
		{
			double x = double.Parse(str);

			if(this.samplingRate != 0)
				return SoundLibrary.Util.Normalize(x, this.samplingRate);

			return x;
		}
	}
}//namespace FilterGenerator
