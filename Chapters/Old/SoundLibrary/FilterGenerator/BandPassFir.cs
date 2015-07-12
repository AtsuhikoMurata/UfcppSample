using System;
using System.Xml;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// Low Pass FIR ���쐬����B
	/// </summary>
	public class LowPassFirGenerator : FilterGenerator
	{
		const string FilterName = "��擧�߃t�B���^";
		const string CutoffName = "�J�b�g�I�t���g��";
		const string OrderName  = "�t�B���^����";
		const string TypeName   = "���֐��^�C�v";

		public LowPassFirGenerator()
		{
			this.properties = new FilterProperty[3];
			this.properties[0] = new FilterProperty(OrderName, 1);
			this.properties[1] = new FilterProperty(CutoffName, (double)0);
			this.properties[2] = new FilterProperty(TypeName, WindowType.Rectangular);
		}

		public int Order
		{
			get{return (int)this.properties[0].Value;}
			set{this.properties[0].Value = value;}
		}

		public double Cutoff
		{
			get{return (double)this.properties[1].Value;}
			set{this.properties[1].Value = value;}
		}

		public WindowType WindowType
		{
			get{return (WindowType)this.properties[2].Value;}
			set{this.properties[2].Value = value;}
		}

		public override string CheckConstraint()
		{
			if(this.Order <= 0)
				return "������ 1 �ȏ�łȂ���΂Ȃ�܂���B";

			double cutoff = this.Cutoff;
			if(cutoff < 0 || cutoff > Math.PI)
				return "�J�b�g�I�t���g���� 0 �` �� �̊ԂłȂ���΂Ȃ�܂���B";

			return null;
		}

		public override IFilter GetFilter()
		{
			return FirCommon.GetLowPassFilter(this.Order, this.Cutoff, this.WindowType);
		}

		public override void ToXml(XmlWriter xwriter)
		{
			xwriter.WriteStartElement("LowPassFir");
			xwriter.WriteAttributeString("order",  this.Order     .ToString());
			xwriter.WriteAttributeString("cutoff", this.Cutoff    .ToString());
			xwriter.WriteAttributeString("window", this.WindowType.ToString());
			xwriter.WriteEndElement();
		}

		public override void FromXml(XmlElement elem)
		{
			this.Order      = int.Parse(elem.Attributes["order"].Value);
			this.Cutoff     = this.converter.ToFrequency(elem.Attributes["cutoff"].Value);
			this.WindowType = (WindowType)Enum.Parse(typeof(WindowType), elem.Attributes["window"].Value);
		}
	}//class LowPassFirGenerator

	/// <summary>
	/// High Pass FIR ���쐬����B
	/// </summary>
	public class HighPassFirGenerator : FilterGenerator
	{
		const string FilterName = "���擧�߃t�B���^";
		const string CutoffName = "�J�b�g�I�t���g��";
		const string OrderName  = "�t�B���^����";
		const string TypeName   = "���֐��^�C�v";

		public HighPassFirGenerator()
		{
			this.properties = new FilterProperty[3];
			this.properties[0] = new FilterProperty(OrderName, 1);
			this.properties[1] = new FilterProperty(CutoffName, (double)0);
			this.properties[2] = new FilterProperty(TypeName, WindowType.Rectangular);
		}

		public int Order
		{
			get{return (int)this.properties[0].Value;}
			set{this.properties[0].Value = value;}
		}

		public double Cutoff
		{
			get{return (double)this.properties[1].Value;}
			set{this.properties[1].Value = value;}
		}

		public WindowType WindowType
		{
			get{return (WindowType)this.properties[2].Value;}
			set{this.properties[2].Value = value;}
		}

		public override string CheckConstraint()
		{
			if(this.Order <= 0)
				return "������ 1 �ȏ�łȂ���΂Ȃ�܂���B";

			double cutoff = this.Cutoff;
			if(cutoff < 0 || cutoff > Math.PI)
				return "�J�b�g�I�t���g���� 0 �` �� �̊ԂłȂ���΂Ȃ�܂���B";

			return null;
		}

		public override IFilter GetFilter()
		{
			return FirCommon.GetHighPassFilter(this.Order, this.Cutoff, this.WindowType);
		}

		public override void ToXml(XmlWriter xwriter)
		{
			xwriter.WriteStartElement("HighPassFir");
			xwriter.WriteAttributeString("order",  this.Order     .ToString());
			xwriter.WriteAttributeString("cutoff", this.Cutoff    .ToString());
			xwriter.WriteAttributeString("window", this.WindowType.ToString());
			xwriter.WriteEndElement();
		}

		public override void FromXml(XmlElement elem)
		{
			this.Order      = int.Parse(elem.Attributes["order"].Value);
			this.Cutoff     = this.converter.ToFrequency(elem.Attributes["cutoff"].Value);
			this.WindowType = (WindowType)Enum.Parse(typeof(WindowType), elem.Attributes["window"].Value);
		}
	}//class HighPassFirGenerator

	/// <summary>
	/// Band Pass FIR ���쐬����B
	/// </summary>
	public class BandPassFirGenerator : FilterGenerator
	{
		const string FilterName = "�ш擧�߃t�B���^";
		const string UpperName = "������g��";
		const string LowerfName = "�������g��";
		const string OrderName  = "�t�B���^����";
		const string TypeName   = "���֐��^�C�v";

		public BandPassFirGenerator()
		{
			this.properties = new FilterProperty[4];
			this.properties[0] = new FilterProperty(OrderName, 1);
			this.properties[1] = new FilterProperty(UpperName, (double)0);
			this.properties[2] = new FilterProperty(LowerfName, (double)0);
			this.properties[3] = new FilterProperty(TypeName, WindowType.Rectangular);
		}

		public int Order
		{
			get{return (int)this.properties[0].Value;}
			set{this.properties[0].Value = value;}
		}

		public double UpperBound
		{
			get{return (double)this.properties[1].Value;}
			set{this.properties[1].Value = value;}
		}

		public double LowerBound
		{
			get{return (double)this.properties[2].Value;}
			set{this.properties[2].Value = value;}
		}

		public WindowType WindowType
		{
			get{return (WindowType)this.properties[3].Value;}
			set{this.properties[3].Value = value;}
		}

		public override string CheckConstraint()
		{
			if(this.Order <= 0)
				return "������ 1 �ȏ�łȂ���΂Ȃ�܂���B";

			double upper = this.UpperBound;
			if(upper < 0 || upper > Math.PI)
				return "������g���� 0 �` �� �̊ԂłȂ���΂Ȃ�܂���B";

			double lower = this.LowerBound;
			if(lower < 0 || lower > Math.PI)
				return "�������g���� 0 �` �� �̊ԂłȂ���΂Ȃ�܂���B";

			if(lower > upper)
				return "����������𒴂��Ă��܂��B";

				return null;
		}

		public override IFilter GetFilter()
		{
			return FirCommon.GetBandPassFilter(this.Order, this.LowerBound, this.UpperBound, this.WindowType);
		}

		public override void ToXml(XmlWriter xwriter)
		{
			xwriter.WriteStartElement("BandPassFir");
			xwriter.WriteAttributeString("order",      this.Order.ToString());
			xwriter.WriteAttributeString("lowerBound", this.LowerBound.ToString());
			xwriter.WriteAttributeString("upperBound", this.UpperBound.ToString());
			xwriter.WriteAttributeString("window",     this.WindowType.ToString());
			xwriter.WriteEndElement();
		}

		public override void FromXml(XmlElement elem)
		{
			this.Order      = int.Parse(elem.Attributes["order"].Value);
			this.LowerBound = this.converter.ToFrequency(elem.Attributes["lowerBound"].Value);
			this.UpperBound = this.converter.ToFrequency(elem.Attributes["upperBound"].Value);
			this.WindowType = (WindowType)Enum.Parse(typeof(WindowType), elem.Attributes["window"].Value);
		}
	}//class BandPassFirGenerator
}
