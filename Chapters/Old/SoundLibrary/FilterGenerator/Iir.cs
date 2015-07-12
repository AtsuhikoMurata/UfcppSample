using System;
using System.Xml;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// FIR ���쐬����B
	/// </summary>
	public class IirFilterGenerator : FilterGenerator
	{
		const string FilterName = "FIR �t�B���^";
		const string CoefAName   = "�t�B���^����W��";
		const string CoefBName   = "�t�B���^���q�W��";

		public IirFilterGenerator()
		{
			this.properties = new FilterProperty[2];

			double[] coefA = new double[0];
			double[] coefB = new double[]{1};

			this.properties[0] = new FilterProperty(CoefAName, coefA);
			this.properties[1] = new FilterProperty(CoefBName, coefB);
		}

		public double[] CoefA
		{
			get{return (double[])this.properties[0].Value;}
			set{this.properties[0].Value = value;}
		}

		public double[] CoefB
		{
			get{return (double[])this.properties[1].Value;}
			set{this.properties[1].Value = value;}
		}

		public override string CheckConstraint()
		{
			if(this.CoefA.Length + 1 != this.CoefB.Length)
				return CoefAName + "�̒��� +1 ==" + CoefBName + "�̒����o�Ȃ���΂Ȃ�܂���B";

			return null;
		}

		public override IFilter GetFilter()
		{
			return new IirFilter(this.CoefA, this.CoefB);
		}

		public override void ToXml(XmlWriter xwriter)
		{
			xwriter.WriteStartElement("IirFilter");
			xwriter.WriteElementString("CoefA", Util.ArrayToString(this.CoefA));
			xwriter.WriteElementString("CoefB", Util.ArrayToString(this.CoefB));
			xwriter.WriteEndElement();
		}

		public override void FromXml(XmlElement elem)
		{
			this.CoefA = Util.StringToArray(elem["CoefA"].InnerText);
			this.CoefB = Util.StringToArray(elem["CoefB"].InnerText);
		}
	}//class IirFilterGenerator
}//namespace FilterGenerator
