using System;
using System.Xml;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// ShelvingEqualizer ���쐬����B
	/// </summary>
	public class ShelvingEqualizerGenerator : FilterGenerator
	{
		const string FilterName = "��擧�߃t�B���^";
		const string CenterName = "���S���g��";
		const string QName = "Q�l";
		const string GainName = "������";

		public ShelvingEqualizerGenerator()
		{
			this.properties = new FilterProperty[2];
			this.properties[0] = new FilterProperty(CenterName, (double)0);
			this.properties[1] = new FilterProperty(GainName, (double)1);
		}

		public double CenterFrequency
		{
			get{return (double)this.properties[0].Value;}
			set{this.properties[0].Value = value;}
		}

		public double Gain
		{
			get{return (double)this.properties[1].Value;}
			set{this.properties[1].Value = value;}
		}

		public override string CheckConstraint()
		{
			double center = this.CenterFrequency;
			if(center < 0 || center > Math.PI)
				return "���S���g���� 0 �` �� �̊ԂłȂ���΂Ȃ�܂���B";

			return null;
		}

		public override IFilter GetFilter()
		{
			return new ShelvingEqualizer(this.CenterFrequency, this.Gain);
		}

		public override void ToXml(XmlWriter xwriter)
		{
			xwriter.WriteStartElement("ShelvingEqualizer");
			xwriter.WriteAttributeString("center", this.CenterFrequency.ToString());
			xwriter.WriteAttributeString("gain"  , this.Gain           .ToString());
			xwriter.WriteEndElement();
		}

		public override void FromXml(XmlElement elem)
		{
			this.CenterFrequency = this.converter.ToFrequency(elem.Attributes["center"].Value);
			this.Gain            = this.converter.ToPower(elem.Attributes["gain"  ].Value);
		}
	}//class ShelvingEqualizerGenerator

	/// <summary>
	/// ShelvingEqualizer ���쐬����B
	/// </summary>
	public class PeakingEqualizerGenerator : FilterGenerator
	{
		const string FilterName = "��擧�߃t�B���^";
		const string CenterName = "���S���g��";
		const string QName = "Q�l";
		const string GainName = "������";

		public PeakingEqualizerGenerator()
		{
			this.properties = new FilterProperty[3];
			this.properties[0] = new FilterProperty(CenterName, (double)0);
			this.properties[1] = new FilterProperty(QName, (double)1);
			this.properties[2] = new FilterProperty(GainName, (double)1);
		}

		public double CenterFrequency
		{
			get{return (double)this.properties[0].Value;}
			set{this.properties[0].Value = value;}
		}

		public double Q
		{
			get{return (double)this.properties[1].Value;}
			set{this.properties[1].Value = value;}
		}

		public double Gain
		{
			get{return (double)this.properties[2].Value;}
			set{this.properties[2].Value = value;}
		}

		public override string CheckConstraint()
		{
			double center = this.CenterFrequency;
			if(center < 0 || center > Math.PI)
				return "���S���g���� 0 �` �� �̊ԂłȂ���΂Ȃ�܂���B";

			if(this.Q == 0)
				return "Q�l�͔�0�łȂ���΂Ȃ�܂���B";

			return null;
		}

		public override IFilter GetFilter()
		{
			return new PeakingEqualizer(this.CenterFrequency, this.Q, this.Gain);
		}

		public override void ToXml(XmlWriter xwriter)
		{
			xwriter.WriteStartElement("PeakingEqualizer");
			xwriter.WriteAttributeString("center", this.CenterFrequency.ToString());
			xwriter.WriteAttributeString("q"     , this.Q              .ToString());
			xwriter.WriteAttributeString("gain"  , this.Gain           .ToString());
			xwriter.WriteEndElement();
		}

		public override void FromXml(XmlElement elem)
		{
			this.CenterFrequency = this.converter.ToFrequency(elem.Attributes["center"].Value);
			this.Q               = double.Parse(elem.Attributes["q"     ].Value);
			this.Gain            = this.converter.ToPower(elem.Attributes["gain"  ].Value);
		}
	}//class PeakingEqualizerGenerator
}
