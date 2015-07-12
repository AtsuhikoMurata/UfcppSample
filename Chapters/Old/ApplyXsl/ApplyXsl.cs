using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Text.RegularExpressions;

/// <summary>
/// �w�肵���t�H���_���ɂ��邷�ׂĂ� XML �t�@�C����
/// XSL �X�^�C���V�[�g��K�p���� HTML ���������ʂ�
/// ���t�@�C�����̊g���q�� xml ���� html �ɕς������O�ŕۑ�����B
/// </summary>
class XslApplier
{
	/// <summary>
	/// �A�v���P�[�V�����̃��C�� �G���g�� �|�C���g�ł��B
	/// </summary>
	[STAThread]
	static void Main(string[] args)
	{
		if(args.Length == 0)
		{
			Console.Write("�t�H���_�����w�肵�Ă�������\n");
		}

		ApplyXslFiles(args[0]);
	}

	/// <summary>
	/// �t�H���_���̑S�Ă� XML �t�@�C���� XSLT ��K�p�B
	/// </summary>
	static void ApplyXslFiles(string dirName)
	{
		foreach(string subdirName in Directory.GetDirectories(dirName))
		{
			ApplyXslFiles(subdirName);
		}

		foreach(string fileName in Directory.GetFiles(dirName, "*.xml"))
		{
			ApplyXsl(fileName);
		}
	}

	/// <summary>
	/// XML �t�@�C���� XSLT ��K�p�B
	/// </summary>
	static void ApplyXsl(string fileName)
	{
		string xslName = GetXSlUri(fileName);
		if(xslName == null)
			return;
		xslName = Path.GetDirectoryName(fileName) + @"\" + xslName.Replace('/', '\\');

		string htmlName = Path.ChangeExtension(fileName, ".html");

		Console.Write("xml : {0}\nxsl : {1}\nhtml:{2}\n\n", fileName, xslName, htmlName);

		XslTransform xslt = new XslTransform();
		xslt.Load(xslName);

		xslt.Transform(fileName, htmlName);
	}

	/// <summary>
	/// XML ���� <?xml-stylesheet ?> �������߂��� .xsl �t�@�C���̖��O�����o���B
	/// </summary>
	static string GetXSlUri(string fileName)
	{
		XmlDocument xdoc = new XmlDocument();
		xdoc.Load(fileName);

		Regex regType = new Regex("type\\s*=\\s*\"(?<1>[^\\\"]*)\"");
		Regex regHref = new Regex("href\\s*=\\s*\"(?<1>[^\\\"]*)\"");

		foreach(XmlNode node in xdoc.ChildNodes)
		{
			if(
				node.NodeType == XmlNodeType.ProcessingInstruction &&
				node.LocalName == "xml-stylesheet")
			{
				Match m = regType.Match(node.Value);
				if(m.Success && m.Groups[1].Value == "text/xsl")
				{
					m = regHref.Match(node.Value);
					return m.Groups[1].Value;
				}
			}
		}
		return null;
	}
}
