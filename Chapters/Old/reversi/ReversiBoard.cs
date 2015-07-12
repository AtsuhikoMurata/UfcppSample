using System;

namespace Reversi
{
	/// <summary>
	/// ���o�[���̋�̏��
	/// </summary>
	public enum ReversiColor
	{
		Wall = 0, //�Ֆʂ̕�
		Black,    //������
		White,    //������
		None      //�}�X�ڂɉ����u����Ă��Ȃ����
	}

	/// <summary>
	/// ���o�[�V�̔Ֆ�
	/// </summary>
	public class ReversiBoard
	{
		private readonly int width;
		private readonly int height;
		private ReversiColor[,] board;

		/// <summary>
		/// �Ֆʂ̏�����
		/// �Ֆʂ̕��͎��R�Ɍ��߂��
		/// </summary>
		/// <param name="width">�Ֆʂ̕�</param>
		/// <param name="height">�Ֆʂ̍���</param>
		public ReversiBoard(int width, int height)
		{
			this.width = width;
			this.height = height;
			//�Ֆʗp�̃������m��
			this.board = new ReversiColor[width+2, height+2];//����ɔԕ���u���̂�+2
			//�Ֆʂ̕��ȊO��None�ɃN���A
			for(int x=1; x<=width; ++x)
				for(int y=1; y<=height; ++y)
				{
					this.board[x, y] = ReversiColor.None;
				}
			//�Ֆʂ̕��ɔԕ���u��
			for(int i=0; i<width+2; i++)
			{
				this.board[i, 0] = ReversiColor.Wall;
				this.board[i, height+1] = ReversiColor.Wall;
			}
			for(int i=1; i<height+1; i++)
			{
				this.board[0, i] = ReversiColor.Wall;
				this.board[width+1, i] = ReversiColor.Wall;
			}
			//�Ֆʂ̒����ɍŏ��̋��u��
			this.board[width/2  , height/2  ] = ReversiColor.Black;
			this.board[width/2  , height/2+1] = ReversiColor.White;
			this.board[width/2+1, height/2  ] = ReversiColor.White;
			this.board[width/2+1, height/2+1] = ReversiColor.Black;
		}

		/// <summary>
		/// �Ֆʂ̃R�s�[���쐬����R���X�g���N�^
		/// </summary>
		/// <param name="b">�R�s�[��</param>
		protected ReversiBoard(ReversiBoard b)
		{
			this.width = b.width;
			this.height = b.height;
			//�Ֆʗp�̃������m��
			this.board = new ReversiColor[b.width+2, b.height+2];//����ɔԕ���u���̂�+2
			//�Ֆʂ̕��ȊO��None�ɃN���A
			for(int x=0; x<b.width+2; ++x)
				for(int y=0; y<b.height+2; ++y)
				{
					this.board[x, y] = b.board[x, y];
				}
		}

		/// <summary>
		/// �������g�̃R�s�[�𐶐�
		/// </summary>
		public ReversiBoard Clone()
		{
			return new ReversiBoard(this);
		}

		/// <summary>
		/// �Ֆʂ̏�����
		/// �Ֆʂ̃T�C�Y�̓f�t�H���g�ł�8�~8
		/// </summary>
		public ReversiBoard():this(8, 8){}

		//=========================================================
		// public methods

		/// <summary>
		/// ���W(x,y)�ɃR�}�������邩�ǂ����𒲂ׂ�
		/// <param name="x">���ׂ�ꏊ��x���W 0�`width-1</param>
		/// <param name="y">���ׂ�ꏊ��y���W 0�`width-1</param>
		/// <returns>�u���邩�ǂ���</returns>
		/// </summary>
		public bool Check(int x, int y, ReversiColor color)
		{
			++x; ++y;
			return this.board[x,y] == ReversiColor.None &&
				( CheckLine(x, y, -1,  0, color)  //��
				|| CheckLine(x, y,  1,  0, color)  //�E
				|| CheckLine(x, y,  0, -1, color)  //��
				|| CheckLine(x, y,  0,  1, color)  //��
				|| CheckLine(x, y, -1, -1, color)  //����
				|| CheckLine(x, y,  1, -1, color)  //�E��
				|| CheckLine(x, y, -1,  1, color)  //����
				|| CheckLine(x, y,  1,  1, color));//�E��
		}
		
		/// <summary>
		/// �Ֆʂɒu����܂������邩�ǂ����𒲂ׂ�
		/// </summary>
		/// <returns>�u����܂������݂��邩�ǂ���</returns>
		public bool CheckAll(ReversiColor color)
		{
			for(int x=1; x<=this.width; ++x)
				for(int y=1; y<=this.height; ++y)
					if(Check(x,y, color)) return true;

			return false;
		}

		/// <summary>
		/// set �ՖʂɐV���ɋ��u���A�Ֆʂ̍X�V���s��
		/// get �Ֆʂɒu���ꂽ��̐F��Ԃ�
		/// x : 0�`width-1
		/// y : 0�`height-1
		/// </summary>
		public ReversiColor this[int x, int y]
		{
			set
			{
				++x; ++y;
				this.board[x,y] = value;//���̃}�X�ڂɂ��܂�u��
				UpdateLine(x, y, -1,  0, value);//��
				UpdateLine(x, y,  1,  0, value);//�E
				UpdateLine(x, y,  0, -1, value);//��
				UpdateLine(x, y,  0,  1, value);//��
				UpdateLine(x, y, -1, -1, value);//����
				UpdateLine(x, y,  1, -1, value);//�E��
				UpdateLine(x, y, -1,  1, value);//����
				UpdateLine(x, y,  1,  1, value);//�E��
			}
			get
			{
				if(x >= this.width || x < 0 || y >= this.height || y < 0)
					return ReversiColor.Wall;
				return this.board[x+1, y+1];
			}
		}

		public int Width{get{return width;}}
		public int Height{get{return height;}}

		/// <summary>
		/// ��̐��𐔂���
		/// </summary>
		/// <param name="black_num">������̐���Ԃ�</param>
		/// <param name="white_num">������̐���Ԃ�</param>
		public void CountUp(out int black_num, out int white_num)
		{
			black_num = 0;
			white_num = 0;
			for(int x=1; x<=width; ++x)
				for(int y=1; y<=height; ++y)
				{
					if(board[x, y] == ReversiColor.Black) black_num++;
					if(board[x, y] == ReversiColor.White) white_num++;
				}
		}

		public int GetScore(ReversiColor color)
		{
			int black, white;
			this.CountUp(out black, out white);
			if(color == ReversiColor.Black) return black;
			if(color == ReversiColor.White) return white;
			return 0;
		}

		//=========================================================
		// private methods

		/// <summary>
		/// �Ֆʂ̍X�V��1���C�����s��
		/// <param name="x">�u���ꏊ��x���W</param>
		/// <param name="y">�u���ꏊ��y���W</param>
		/// <param name="color">�u����̐F</param>
		/// </summary>
		private void UpdateLine(int x, int y, int dx, int dy, ReversiColor color)
		{
			int i, j;
			ReversiColor inverse_color = InverseColor(color);
			for(i=x+dx, j=y+dy; this.board[i,j] == inverse_color; i+=dx, j+=dy);
			if(!(i==x+dx && j==y+dy) && this.board[i,j]==color)
				for(i-=dx, j-=dy; !(i==x && j==y); i-=dx, j-=dy)
					this.board[i,j] = color;
		}

		/// <summary>
		/// ��̐F�Ƌt�̐F��Ԃ�
		/// </summary>
		/// <param name="color">��̐F</param>
		/// <returns>�t�̐F</returns>
		static public ReversiColor InverseColor(ReversiColor color)
		{
			if(color == ReversiColor.Black) return ReversiColor.White;
			if(color == ReversiColor.White) return ReversiColor.Black;
			return color;
		}

		/// <summary>
		/// ���W(x,y)�ɃR�}�������邩�ǂ����A1���C�������ׂ�
		/// (Check���\�b�h�ŗ��p����)
		/// <param name="x">���ׂ�ꏊ��x���W</param>
		/// <param name="y">���ׂ�ꏊ��y���W</param>
		/// <returns>�u���邩�ǂ���</returns>
		/// </summary>
		private bool CheckLine(int x, int y, int dx, int dy, ReversiColor color)
		{
			int i, j;
			ReversiColor inverse_color = InverseColor(color);
			for(i=x+dx, j=y+dy; this.board[i,j] == inverse_color; i+=dx, j+=dy);
			return !(i==x+dx && j==y+dy) && this.board[i,j]==color;
		}

	}//class ReversiBoard
}
