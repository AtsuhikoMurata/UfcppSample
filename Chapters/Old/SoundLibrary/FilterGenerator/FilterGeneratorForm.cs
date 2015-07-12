using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SoundLibrary.Filter
{
	/// <summary>
	/// FilterGeneratorForm �̊T�v�̐����ł��B
	/// </summary>
	public class FilterGeneratorForm : System.Windows.Forms.Form
	{
		#region
		FilterGenerator generator;

		public FilterGeneratorForm(FilterGenerator fg)
		{
			//
			// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B
			//
			InitializeComponent();

			this.generator = fg;
			//Init();
		}
		#endregion

		private System.Windows.Forms.Button buttonOk;
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h 
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonOk = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(328, 344);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(56, 24);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "OK";
			// 
			// FilterGeneratorForm
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(392, 373);
			this.Controls.Add(this.buttonOk);
			this.Name = "FilterGeneratorForm";
			this.Text = "FilterGeneratorForm";
			this.Load += new System.EventHandler(this.c);
			this.ResumeLayout(false);

		}
		#endregion

		private void c(object sender, System.EventArgs e)
		{
		
		}
	}
}
