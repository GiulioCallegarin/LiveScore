using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

/*  TODO:   Surrender 
 *          TimeOut / Yellow-Red Cards
 *          Team Graphics 
 *          Update Points
 *          Update team results
 */         

namespace LiveScore
{
    public partial class frmLiveScore : Form
    {

        #region Global Variables

        private Match currentMatch;
        private TeamMatch currentTeamMatch;
        private Keys[] keyMapping, tempKeyMapping;
        private static int GAME_NUMBER = 9;
        private int MAIN_HEIGHT, AUX_HEIGHT, MAIN_WIDTH, TAG_WIDTH, SP_WIDTH, GP_WIDTH, SCORE_WIDTH, SET_START, BASE_OFFSET,
            ADD_OFFSET, MAIN_X_OFF, TAG_X_OFF, SP_X_OFF, GP_X_OFF, MAIN_Y_OFF, REGULAR_Y_OFF, AUX_Y_OFF, NAME_1_OFF, NAME_2_OFF,
            LATERAL_WIDTH, LATERAL_HEIGHT, RECAP_WIDTH, RECAP_SCORE_START,
            TEAM_WIDTH, TEAM_TAG_WIDTH, TEAM_X_OFF, TEAM_PREVIEW_START_POINT,
            TEAM_RECAP_WIDTH1, TEAM_RECAP_WIDTH2, LATERAL_RECAP_HEIGTH, TEAM_RECAP_SCORE_START, RECAP_AUX_X_OFF, RECAP_TOTAL_WIDTH;
        private int RECTANGLE_X, RECTANGLE_Y, RECTANGLE_WIDTH, RECTANGLE_HEIGHT;
        private LinearGradientBrush lBrush;
        private SolidBrush sBrush;
        private SolidBrush textBrush;
        private SolidBrush auxBrush;
        private SolidBrush loserBrush;
        //private Pen mainPen;
        private Font auxFont;
        private Font mainFont;
        private Font regularFont;
        private Color startingColor, endColor;
        private LinearGradientMode gradientMode;
        private bool first;
        private bool listen, modDone;
        private int index, counter;
        private String final;
        private Keys[] keybuffer;
        private bool[] keyPressed, keyDone, keybindOn;

        #endregion

        #region Initialization
        public frmLiveScore()
        {
            InitializeComponent();
        }
        private void frmLiveScore_Load(object sender, EventArgs e)
        {
            init();
            initGComponents();
            updateGraphics();
            initKeybinds();
        }
        private void init()
        {
            currentMatch = new Match();
            currentTeamMatch = new TeamMatch();
            cmbSets.SelectedIndex = 1;
            pnlDoubles.Visible = false;
            grbTeams.Visible = false;
            grbMatch.Visible = false;
            pnlKeybinds.Visible = false;
            grbTeamPreview.Visible = false;
            txtSet1Player1.Enabled = txtSet1Player2.Enabled = txtSet2Player1.Enabled = txtSet2Player2.Enabled = txtSet3Player1.Enabled = txtSet3Player2.Enabled = txtSet4Player1.Enabled = txtSet4Player2.Enabled = txtSet5Player1.Enabled = txtSet5Player2.Enabled = txtSet6Player1.Enabled = txtSet6Player2.Enabled = txtSet7Player1.Enabled = txtSet7Player2.Enabled = false;
            txtPlayerTeam1Game1.Enabled = txtPlayerTeam1Game2.Enabled = txtPlayerTeam1Game3.Enabled = txtPlayerTeam1Game4.Enabled = txtPlayerTeam1Game5.Enabled = txtPlayerTeam1Game6.Enabled = txtPlayerTeam1Game7.Enabled = txtPlayerTeam1Game8.Enabled = txtPlayerTeam1Game9.Enabled = false;
            txtPlayerTeam2Game1.Enabled = txtPlayerTeam2Game2.Enabled = txtPlayerTeam2Game3.Enabled = txtPlayerTeam2Game4.Enabled = txtPlayerTeam2Game5.Enabled = txtPlayerTeam2Game6.Enabled = txtPlayerTeam2Game7.Enabled = txtPlayerTeam2Game8.Enabled = txtPlayerTeam2Game9.Enabled = false;
            txtScoreTeam1Game1.Enabled = txtScoreTeam1Game2.Enabled = txtScoreTeam1Game3.Enabled = txtScoreTeam1Game4.Enabled = txtScoreTeam1Game5.Enabled = txtScoreTeam1Game6.Enabled = txtScoreTeam1Game7.Enabled = txtScoreTeam1Game8.Enabled = txtScoreTeam1Game9.Enabled = false;
            txtScoreTeam2Game1.Enabled = txtScoreTeam2Game2.Enabled = txtScoreTeam2Game3.Enabled = txtScoreTeam2Game4.Enabled = txtScoreTeam2Game5.Enabled = txtScoreTeam2Game6.Enabled = txtScoreTeam2Game7.Enabled = txtScoreTeam2Game8.Enabled = txtScoreTeam2Game9.Enabled = false;
            ckbPlayerTag.Checked = ckbTeamTag.Checked = false;
            btnNext.Enabled = btnUpdateTeams.Enabled = false;
            btnSurrenderPlayer1.Enabled = btnSurrenderPlayer1.Enabled = true;
            btnPointScorePlayer1.Enabled = btnPointScorePlayer2.Enabled = btnSetScorePlayer1.Enabled = btnSetScorePlayer2.Enabled = true;
            lblSurrender1.Visible = lblSurrender2.Visible = false;
            btnSurrenderPlayer1.Enabled = btnSurrenderPlayer2.Enabled = false;
            txtOffset.Text = "0";
            this.Height = 900;
        }
        private void initGComponents()
        {
            MAIN_HEIGHT = 50;
            AUX_HEIGHT = 30;
            MAIN_X_OFF = 20;
            TAG_X_OFF = 20;
            SP_X_OFF = 20;
            GP_X_OFF = 20;
            BASE_OFFSET = ADD_OFFSET = 100;
            SCORE_WIDTH = MAIN_HEIGHT;

            LATERAL_WIDTH = 10;
            LATERAL_HEIGHT = 2 * MAIN_HEIGHT + AUX_HEIGHT;


            sBrush = new SolidBrush(Color.FromArgb(223, 43, 80));
            textBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
            auxBrush = new SolidBrush(Color.Black);
            loserBrush = new SolidBrush(Color.DarkGray);
            //mainPen = new Pen(Color.Gray, 1);
            auxFont = new Font("Segoe UI Semibold", 14, FontStyle.Bold);
            mainFont = new Font("Segoe UI Black", 22, FontStyle.Bold);
            regularFont = new Font("Segoe UI Bold", 22, FontStyle.Bold);

            startingColor = Color.Black;
            endColor = Color.White;
            gradientMode = LinearGradientMode.Vertical;
            RECTANGLE_X = 0;
            RECTANGLE_Y = 0;
            RECTANGLE_WIDTH = 5;
            RECTANGLE_HEIGHT = 300;

            first = true;
        }
        private void initKeybinds()
        {
            keybindOn = new bool[15];
            keyDone = new bool[15];
            keyPressed = new bool[45];
            keyMapping = new Keys[45];
            tempKeyMapping = new Keys[45];
            if (!File.Exists("LiveScoreKeybinds.txt"))
            {
                using (StreamWriter sw = File.CreateText("LiveScoreKeybinds.txt"))
                {
                    sw.Write("ADDLEFT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("SUBLEFT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("TOLEFT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("YLEFT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("RLEFT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("WLEFT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("ADDRIGHT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("SUBRIGHT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("TORIGHT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("YRIGHT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("RRIGHT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("WRIGHT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("NEXT ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("SWITCH ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.WriteLine("null ");

                    sw.Write("SERVICE ");
                    sw.Write("null ");
                    sw.Write("null ");
                    sw.Write("null ");
                }
            }
                using (StreamReader sr = File.OpenText("LiveScoreKeybinds.txt"))
                {
                    KeysConverter converter = new KeysConverter();
                    String s;
                    String[] aux;
                    int i = 0;
                    while(!sr.EndOfStream) 
                    {
                        int j = 1;
                        s = sr.ReadLine();
                        aux = s.Split(' ');
                        
                        if (aux[j] == "null")
                            keyMapping[i] = Keys.None;
                        else
                            keyMapping[i] = (Keys)converter.ConvertFromString(aux[j]);
                        i++;
                        j++;
                        
                        if (aux[j] == "null")
                            keyMapping[i] = Keys.None;
                        else
                            keyMapping[i] = (Keys)converter.ConvertFromString(aux[j]);
                        i++;
                        j++;

                        if (aux[j] == "null")
                            keyMapping[i] = Keys.None;
                        else
                            keyMapping[i] = (Keys)converter.ConvertFromString(aux[j]);
                        i++;
                    }
                }
                Array.Copy(keyMapping, tempKeyMapping, keyMapping.Length);
            for(int i = 0; i < 45; i++)
                if(keyMapping[i] == Keys.None)
                    keyPressed[i] = true;
            for (int i = 0; i < 15; i++) 
                if(keyMapping[i*3] != Keys.None)
                    keybindOn[i] = true;
        }

        #endregion

        #region Keybinds

        private void frmLiveScore_KeyDown(object sender, KeyEventArgs e)
        {
            if (listen && e.KeyCode != keybuffer[0] && e.KeyCode != keybuffer[1])
            {
                switch (counter)
                {
                    case 0:
                        {
                            keybuffer[0] = e.KeyCode;
                            tempKeyMapping[index] = e.KeyCode;
                            final = e.KeyCode.ToString();
                            updateKeyText();
                            counter++;
                            break;
                        }
                    case 1:
                        {
                            keybuffer[1] = e.KeyCode;
                            tempKeyMapping[index + 1] = e.KeyCode;
                            final = final + "+" + e.KeyCode.ToString();
                            updateKeyText();
                            counter++;
                            break;
                        }
                    case 2:
                        {
                            listen = false;
                            tempKeyMapping[index + 2] = e.KeyCode;
                            final = final + "+" + e.KeyCode.ToString();
                            updateKeyText();
                            break;
                        }
                }
            }
            else
            {
                for (int i = 0; i < 45; i++)
                {
                    if (e.KeyCode == keyMapping[i])
                    {
                        keyPressed[i] = true;
                        int group = (i / 3) * 3;
                        if (keybindOn[group / 3] && keyPressed[group] && keyPressed[group + 1] && keyPressed[group + 2])
                            keyDone[group / 3] = true;
                    }
                }

                if (keyDone[0])
                {
                    keyDone[0] = false;
                    btnPointScorePlayer1_Click(sender, e);
                }
                if (keyDone[1])
                {
                    keyDone[1] = false;
                    btnSubtractPointPlayer1_Click(sender, e);
                }
                if (keyDone[2])
                {
                    keyDone[2] = false;
                    //TODO
                }
                if(keyDone[3])
                {
                    keyDone[3] = false;
                    //TODO
                }
                if (keyDone[4])
                {
                    keyDone[4] = false;
                    //TODO
                }
                if (keyDone[6])
                {
                    keyDone[6] = false;
                    btnPointScorePlayer2_Click(sender, e);
                }
                if (keyDone[7])
                {
                    keyDone[7] = false;
                    btnSubtractPointPlayer2_Click(sender, e);
                }
                if (keyDone[8])
                {
                    keyDone[8] = false;
                    //TODO
                }
                if (keyDone[9])
                {
                    keyDone[9] = false;
                    //TODO
                }
                if (keyDone[10])
                {
                    keyDone[10] = false;
                    //TODO
                }
                if (keyDone[11])
                {
                    keyDone[11] = false;
                    //TODO
                }
                if (keyDone[12])
                {
                    keyDone[12] = false;
                    if(btnNext.Enabled)
                        btnNext_Click(sender, e);
                }
                if (keyDone[13])
                {
                    keyDone[13] = false;
                    ckbSwitch.Checked = !ckbSwitch.Checked;
                }
                if (keyDone[14])
                {
                    keyDone[14] = false;
                    ckbService.Checked = !ckbService.Checked;
                }
            }
        }
        private void updateKeyText()
        {
            switch (index)
            {
                case 0:
                    {
                        txtAddS.Text = final;
                        break;
                    }
                case 3:
                    {
                        txtSubS.Text = final;
                        break;
                    }
                case 6:
                    {
                        txtTOS.Text = final;
                        break;
                    }
                case 9:
                    {
                        txtYS.Text = final;
                        break;
                    }
                case 12:
                    {
                        txtRS.Text = final;
                        break;
                    }
                case 15:
                    {
                        txtWS.Text = final;
                        break;
                    }
                case 18:
                    {
                        txtAddR.Text = final;
                        break;
                    }
                case 21:
                    {
                        txtSubR.Text = final;
                        break;
                    }
                case 24:
                    {
                        txtTOR.Text = final;
                        break;
                    }
                case 27:
                    {
                        txtYR.Text = final;
                        break;
                    }
                case 30:
                    {
                        txtRR.Text = final;
                        break;
                    }
                case 33:
                    {
                        txtWR.Text = final;
                        break;
                    }
                case 36:
                    {
                        txtNext.Text = final;
                        break;
                    }
                case 39:
                    {
                        txtSwitch.Text = final;
                        break;
                    }
                case 42:
                    {
                        txtService.Text = final;
                        break;
                    }
            }
        }
        private void frmLiveScore_KeyUp(object sender, KeyEventArgs e)
        {
            if (listen)
            {
                listen = false;
                updateKeyText();
            }
            else
            {
                for(int i = 0; i < 45; i++)
                {
                    if (e.KeyCode == keyMapping[i])
                        keyPressed[i] = false;
                }
            }
        }

        #region Keybind Setting

        #region Click Events

        private void btnSetAddS_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtAddS.Text = "";
            index = 0;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetSubS_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtSubS.Text = "";
            index = 3;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetTOS_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtTOS.Text = "";
            index = 6;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetYS_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtYS.Text = "";
            index = 9;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetRS_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtRS.Text = "";
            index = 12;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetWS_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtWS.Text = "";
            index = 15;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetAddR_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtAddR.Text = "";
            index = 18;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetSubR_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtSubR.Text = "";
            index = 21;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetTOR_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtTOR.Text = "";
            index = 24;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetYR_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtYR.Text = "";
            index = 27;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetRR_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtRR.Text = "";
            index = 30;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetWR_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtWR.Text = "";
            index = 33;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetNext_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtNext.Text = "";
            index = 36;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetSwitch_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtSwitch.Text = "";
            index = 39;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnSetService_Click(object sender, EventArgs e)
        {
            keybuffer = new Keys[2];
            txtService.Text = "";
            index = 42;
            counter = 0;
            final = "";
            tempKeyMapping[index] = tempKeyMapping[index + 1] = tempKeyMapping[index + 2] = Keys.None;
            modDone = true;
            listen = true;
        }
        private void btnDelAddS_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtAddS.Text = "";
                int bau = 0;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelSubS_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtSubS.Text = "";
                int bau = 3;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelTOS_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtTOS.Text = "";
                int bau = 6;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelYS_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtYS.Text = "";
                int bau = 9;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelRS_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtRS.Text = "";
                int bau = 12;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelWS_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtWS.Text = "";
                int bau = 15;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelAddR_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtAddR.Text = "";
                int bau = 18;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelSubR_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtSubR.Text = "";
                int bau = 21;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelTOR_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtTOR.Text = "";
                int bau = 24;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelYR_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtYR.Text = "";
                int bau = 27;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelRR_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtRR.Text = "";
                int bau = 30;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelWR_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtWR.Text = "";
                int bau = 33;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelNext_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtNext.Text = "";
                int bau = 36;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelSwitch_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtSwitch.Text = "";
                int bau = 39;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }
        private void btnDelService_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Cancellare l'elemento selezionato?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                modDone = true;
                txtService.Text = "";
                int bau = 42;
                tempKeyMapping[bau] = tempKeyMapping[bau + 1] = tempKeyMapping[bau + 2] = Keys.None;
            }
        }

        #endregion

        #region Update Settings

        private void btnKeybinds_Click(object sender, EventArgs e)
        {
            if (!pnlKeybinds.Visible)
            {
                modDone = false;
                String s = "";
                Boolean first = true;

                if (keyMapping[0] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[0];
                    first = false;
                }
                if (keyMapping[1] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[1];
                    first = false;
                }
                if (keyMapping[2] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[2];
                    first = false;
                }
                txtAddS.Text = s;
                s = "";
                first = true;


                if (keyMapping[3] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[3];
                    first = false;
                }
                if (keyMapping[4] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[4];
                    first = false;
                }
                if (keyMapping[5] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[5];
                    first = false;
                }
                txtSubS.Text = s;
                s = "";
                first = true;


                if (keyMapping[6] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[6];
                    first = false;
                }
                if (keyMapping[7] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[7];
                    first = false;
                }
                if (keyMapping[8] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[8];
                    first = false;
                }
                txtTOS.Text = s;
                s = "";
                first = true;


                if (keyMapping[9] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[9];
                    first = false;
                }
                if (keyMapping[10] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[10];
                    first = false;
                }
                if (keyMapping[11] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[11];
                    first = false;
                }
                txtYS.Text = s;
                s = "";
                first = true;


                if (keyMapping[12] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[12];
                    first = false;
                }
                if (keyMapping[13] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[13];
                    first = false;
                }
                if (keyMapping[14] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[14];
                    first = false;
                }
                txtRS.Text = s;
                s = "";
                first = true;


                if (keyMapping[15] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[15];
                    first = false;
                }
                if (keyMapping[16] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[16];
                    first = false;
                }
                if (keyMapping[17] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[17];
                    first = false;
                }
                txtWS.Text = s;
                s = "";
                first = true;


                if (keyMapping[18] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[18];
                    first = false;
                }
                if (keyMapping[19] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[19];
                    first = false;
                }
                if (keyMapping[20] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[20];
                    first = false;
                }
                txtAddR.Text = s;
                s = "";
                first = true;


                if (keyMapping[21] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[21];
                    first = false;
                }
                if (keyMapping[22] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[22];
                    first = false;
                }
                if (keyMapping[23] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[23];
                    first = false;
                }
                txtSubR.Text = s;
                s = "";
                first = true;


                if (keyMapping[24] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[24];
                    first = false;
                }
                if (keyMapping[25] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[25];
                    first = false;
                }
                if (keyMapping[26] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[26];
                    first = false;
                }
                txtTOR.Text = s;
                s = "";
                first = true;


                if (keyMapping[27] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[27];
                    first = false;
                }
                if (keyMapping[28] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[28];
                    first = false;
                }
                if (keyMapping[29] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[29];
                    first = false;
                }
                txtYR.Text = s;
                s = "";
                first = true;


                if (keyMapping[30] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[30];
                    first = false;
                }
                if (keyMapping[31] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[31];
                    first = false;
                }
                if (keyMapping[32] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[32];
                    first = false;
                }
                txtRR.Text = s;
                s = "";
                first = true;


                if (keyMapping[33] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[33];
                    first = false;
                }
                if (keyMapping[34] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[34];
                    first = false;
                }
                if (keyMapping[35] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[35];
                    first = false;
                }
                txtWR.Text = s;
                s = "";
                first = true;


                if (keyMapping[36] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[36];
                    first = false;
                }
                if (keyMapping[37] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[37];
                    first = false;
                }
                if (keyMapping[38] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[38];
                    first = false;
                }
                txtNext.Text = s;
                s = "";
                first = true;


                if (keyMapping[39] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[39];
                    first = false;
                }
                if (keyMapping[40] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[40];
                    first = false;
                }
                if (keyMapping[41] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[41];
                    first = false;
                }
                txtSwitch.Text = s;
                s = "";
                first = true;


                if (keyMapping[42] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[42];
                    first = false;
                }
                if (keyMapping[43] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[43];
                    first = false;
                }
                if (keyMapping[44] != Keys.None)
                {
                    if (!first)
                        s = s + " + ";
                    s = s + keyMapping[44];
                    first = false;
                }
                txtService.Text = s;
                pnlKeybinds.Visible = true;
            }
            else if (modDone)
            {
                DialogResult dialogResult = MessageBox.Show("Salvare le modifiche fatte?", "Salvare?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.No)
                {
                    Array.Copy(keyMapping, tempKeyMapping, keyMapping.Length);
                    pnlKeybinds.Visible = false;
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    pnlKeybinds.Visible = false;
                    updateKeyBinds();
                }
            }
            else
                pnlKeybinds.Visible = false;
        }
        private void updateKeyBinds()
        {
            Array.Copy(tempKeyMapping, keyMapping, tempKeyMapping.Length);
            using (StreamWriter sw = File.CreateText("LiveScoreKeybinds.txt"))
            {
                sw.Write("ADDLEFT ");
                sw.Write(keyMapping[0].ToString() + " ");
                sw.Write(keyMapping[1].ToString() + " ");
                sw.WriteLine(keyMapping[2].ToString() + " ");

                sw.Write("SUBLEFT ");
                sw.Write(keyMapping[3].ToString() + " ");
                sw.Write(keyMapping[4].ToString() + " ");
                sw.WriteLine(keyMapping[5].ToString() + " ");

                sw.Write("TOLEFT ");
                sw.Write(keyMapping[6].ToString() + " ");
                sw.Write(keyMapping[7].ToString() + " ");
                sw.WriteLine(keyMapping[8].ToString() + " ");

                sw.Write("YLEFT ");
                sw.Write(keyMapping[9].ToString() + " ");
                sw.Write(keyMapping[10].ToString() + " ");
                sw.WriteLine(keyMapping[11].ToString() + " ");

                sw.Write("RLEFT ");
                sw.Write(keyMapping[12].ToString() + " ");
                sw.Write(keyMapping[13].ToString() + " ");
                sw.WriteLine(keyMapping[14].ToString() + " ");

                sw.Write("WLEFT ");
                sw.Write(keyMapping[15].ToString() + " ");
                sw.Write(keyMapping[16].ToString() + " ");
                sw.WriteLine(keyMapping[17].ToString() + " ");

                sw.Write("ADDRIGHT ");
                sw.Write(keyMapping[18].ToString() + " ");
                sw.Write(keyMapping[19].ToString() + " ");
                sw.WriteLine(keyMapping[20].ToString() + " ");

                sw.Write("SUBRIGHT ");
                sw.Write(keyMapping[21].ToString() + " ");
                sw.Write(keyMapping[22].ToString() + " ");
                sw.WriteLine(keyMapping[23].ToString() + " ");

                sw.Write("TORIGHT ");
                sw.Write(keyMapping[24].ToString() + " ");
                sw.Write(keyMapping[25].ToString() + " ");
                sw.WriteLine(keyMapping[26].ToString() + " ");

                sw.Write("YRIGHT ");
                sw.Write(keyMapping[27].ToString() + " ");
                sw.Write(keyMapping[28].ToString() + " ");
                sw.WriteLine(keyMapping[29].ToString() + " ");

                sw.Write("RRIGHT ");
                sw.Write(keyMapping[30].ToString() + " ");
                sw.Write(keyMapping[31].ToString() + " ");
                sw.WriteLine(keyMapping[32].ToString() + " ");

                sw.Write("WRIGHT ");
                sw.Write(keyMapping[33].ToString() + " ");
                sw.Write(keyMapping[34].ToString() + " ");
                sw.WriteLine(keyMapping[35].ToString() + " ");

                sw.Write("NEXT ");
                sw.Write(keyMapping[36].ToString() + " ");
                sw.Write(keyMapping[37].ToString() + " ");
                sw.WriteLine(keyMapping[38].ToString() + " ");

                sw.Write("SWITCH ");
                sw.Write(keyMapping[39].ToString() + " ");
                sw.Write(keyMapping[40].ToString() + " ");
                sw.WriteLine(keyMapping[41].ToString() + " ");

                sw.Write("SERVICE ");
                sw.Write(keyMapping[42].ToString() + " ");
                sw.Write(keyMapping[43].ToString() + " ");
                sw.Write(keyMapping[44].ToString() + " ");
            }
            for (int i = 0; i < 45; i++)
                if (keyMapping[i] == Keys.None)
                    keyPressed[i] = true;
            for (int i = 0; i < 15; i++)
                if (keyMapping[i * 3] != Keys.None)
                    keybindOn[i] = true;
        }

        #endregion

        #endregion

        #endregion

        #region Event Handlers

        #region Settings
        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Sei sicuro di voler cancellare tutto?", "Cancellare?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
                reset();
        }
        private void btnUpdateGraphics_Click(object sender, EventArgs e)
        {
            updateGraphics();
        }
        private void ckbTeams_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbTeams.Checked)
            {
                grbTeams.Visible = true;
                grbMatch.Visible = true;
                btnUpdateTeams.Enabled = true;
            }
            else
            {
                grbTeams.Visible = false;
                grbMatch.Visible = false;
                btnUpdateTeams.Enabled = false;
            }
        }
        private void ckbDouble_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbDouble.Checked)
            {
                pnlDoubles.Visible = true;
                pnlSingles.Visible = false;
            }
            else
            {
                pnlDoubles.Visible = false;
                pnlSingles.Visible = true;
            }
        }
        private void ckbShowPreview_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbShowPreview.Checked)
            {
                grbTeamPreview.Visible = true;
            }
            else
            {
                grbTeamPreview.Visible = false;
                this.Height = 900;
            }
        }

        #endregion

        #region Game

        private void btnUpdatePlayers_Click(object sender, EventArgs e)
        {
            if (ckbDouble.Checked)
                currentMatch = new Match(txtDouble1Player1.Text + " / " + txtDouble1Player2.Text, txtDouble2Player1.Text + " / " + txtDouble2Player2.Text, "", "", Int32.Parse(cmbSets.SelectedItem.ToString()), true, false);
            else
                currentMatch = new Match(txtSurname1.Text, txtSurname2.Text, txtName1.Text, txtName2.Text, Int32.Parse(cmbSets.SelectedItem.ToString()), false, false);
            ckbService.Checked = ckbSwitch.Checked = false;
            ckbTimeOutPlayer1.Checked = ckbTimeOutPlayer2.Checked = ckbWhitePlayer1.Checked = ckbWhitePlayer2.Checked = ckbYellowPlayer1.Checked = ckbYellowPlayer2.Checked = ckbRedPlayer1.Checked = ckbRedPlayer2.Checked = false;
            txtSet1Player1.Enabled = txtSet1Player2.Enabled = txtSet2Player1.Enabled = txtSet2Player2.Enabled = txtSet3Player1.Enabled = txtSet3Player2.Enabled = txtSet4Player1.Enabled = txtSet4Player2.Enabled = txtSet5Player1.Enabled = txtSet5Player2.Enabled = txtSet6Player1.Enabled = txtSet6Player2.Enabled = txtSet7Player1.Enabled = txtSet7Player2.Enabled = false;
            txtSet1Player1.Text = txtSet1Player2.Text = txtSet2Player1.Text = txtSet2Player2.Text = txtSet3Player1.Text = txtSet3Player2.Text = txtSet4Player1.Text = txtSet4Player2.Text = txtSet5Player1.Text = txtSet5Player2.Text = txtSet6Player1.Text = txtSet6Player2.Text = txtSet7Player1.Text = txtSet7Player2.Text = "";
            btnPointScorePlayer1.Text = btnPointScorePlayer2.Text = btnSetScorePlayer1.Text = btnSetScorePlayer2.Text = "0";
            lblSurrender1.Visible = lblSurrender2.Visible = false;
            btnPointScorePlayer1.BackColor = Color.Yellow;
            btnPointScorePlayer2.BackColor = Color.FromArgb(224, 224, 224);

            btnPointScorePlayer1.Enabled = btnPointScorePlayer2.Enabled = btnSubtractPointPlayer1.Enabled = btnSubtractPointPlayer2.Enabled = true;

            if (ckbDouble.Checked)
            {
                lblShowNamePlayer1.Text = txtDouble1Player1.Text.ToUpper() + " / " + txtDouble1Player2.Text.ToUpper();
                lblShowNamePlayer2.Text = txtDouble2Player1.Text.ToUpper() + " / " + txtDouble2Player2.Text.ToUpper();
            }
            else
            {
                lblShowNamePlayer1.Text = txtSurname1.Text.ToUpper();
                lblShowNamePlayer2.Text = txtSurname2.Text.ToUpper();
            }

            int currentSets = Int32.Parse(cmbSets.SelectedItem.ToString());
            switch (currentSets)
            {
                case 3:
                    {
                        txtSet4Player1.Visible = txtSet4Player2.Visible = txtSet5Player1.Visible = txtSet5Player2.Visible = txtSet6Player1.Visible = txtSet6Player2.Visible = txtSet7Player1.Visible = txtSet7Player2.Visible = false;
                        break;
                    }
                case 5:
                    {
                        txtSet4Player1.Visible = txtSet4Player2.Visible = txtSet5Player1.Visible = txtSet5Player2.Visible = true;
                        txtSet6Player1.Visible = txtSet6Player2.Visible = txtSet7Player1.Visible = txtSet7Player2.Visible = false;
                        break;
                    }
                case 7:
                    {
                        txtSet4Player1.Visible = txtSet4Player2.Visible = txtSet5Player1.Visible = txtSet5Player2.Visible = txtSet6Player1.Visible = txtSet6Player2.Visible = txtSet7Player1.Visible = txtSet7Player2.Visible = true;
                        break;
                    }
            }
        }
        private void btnClearPlayerNames_Click(object sender, EventArgs e)
        {
            txtSurname1.Text = txtSurname2.Text = txtName1.Text = txtName2.Text = "";
        }
        private void btnClearDoubleNames_Click(object sender, EventArgs e)
        {
            txtDouble1Player1.Text = txtDouble1Player2.Text = txtDouble2Player1.Text = txtDouble2Player2.Text = "";
        }

        private void btnPointScorePlayer1_Click(object sender, EventArgs e)
        {
            if (!ckbSwitch.Checked)
            {
                updatePoint(currentMatch.addPoint1());
                btnPointScorePlayer1.Text = currentMatch.CurrentScore1.ToString();
            }
            else
            {
                updatePoint(currentMatch.addPoint2());
                btnPointScorePlayer1.Text = currentMatch.CurrentScore2.ToString();
            }
        }
        private void btnPointScorePlayer2_Click(object sender, EventArgs e)
        {
            if (!ckbSwitch.Checked)
            {
                updatePoint(currentMatch.addPoint2());
                btnPointScorePlayer2.Text = currentMatch.CurrentScore2.ToString();
            }
            else
            {
                updatePoint(currentMatch.addPoint1());
                btnPointScorePlayer2.Text = currentMatch.CurrentScore1.ToString();
            }
        }
        private void btnSubtractPointPlayer1_Click(object sender, EventArgs e)
        {
            if (!ckbSwitch.Checked)
            {
                updatePoint(currentMatch.subPoint1());
                btnPointScorePlayer1.Text = currentMatch.CurrentScore1.ToString();
            }
            else
            {
                updatePoint(currentMatch.subPoint2());
                btnPointScorePlayer1.Text = currentMatch.CurrentScore2.ToString();
            }
        }
        private void btnSubtractPointPlayer2_Click(object sender, EventArgs e)
        {
            if (!ckbSwitch.Checked)
            {
                updatePoint(currentMatch.subPoint2());
                btnPointScorePlayer2.Text = currentMatch.CurrentScore2.ToString();
            }
            else
            {
                updatePoint(currentMatch.subPoint1());
                btnPointScorePlayer2.Text = currentMatch.CurrentScore1.ToString();
            }
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            btnPointScorePlayer1.Text = btnPointScorePlayer2.Text = "0";
            btnNext.Enabled = false;
            RECAP_WIDTH = RECAP_SCORE_START + SCORE_WIDTH * (currentMatch.CurrentSet + 2);
            nextGame();
            pnlGamePreview.Invalidate(new Rectangle(SET_START, 0, SCORE_WIDTH * 2, pnlGamePreview.ClientRectangle.Height));
            pnlGameRecap.Invalidate(new Rectangle(RECAP_SCORE_START + LATERAL_WIDTH, 0, RECAP_WIDTH - RECAP_SCORE_START + LATERAL_WIDTH + SCORE_WIDTH, LATERAL_HEIGHT));
            if (!ckbSwitch.Checked)
            {
                btnSetScorePlayer1.Text = currentMatch.FinalScore1.ToString();
                btnSetScorePlayer2.Text = currentMatch.FinalScore2.ToString();
            }
            else
            {
                btnSetScorePlayer2.Text = currentMatch.FinalScore1.ToString();
                btnSetScorePlayer1.Text = currentMatch.FinalScore2.ToString();
            }

            print1();
            print2();

            if (currentMatch.GameWon)
            {
                btnPointScorePlayer1.Enabled = btnPointScorePlayer2.Enabled = btnSubtractPointPlayer1.Enabled = btnSubtractPointPlayer2.Enabled = false;
                btnSurrenderPlayer1.Enabled = btnSurrenderPlayer2.Enabled = false;
                if (ckbTeams.Checked)
                {
                    nextMatch();
                    currentTeamMatch.nextMatch(currentMatch);
                    btnScoreTeam1.Text = currentTeamMatch.Final1.ToString();
                    btnScoreTeam2.Text = currentTeamMatch.Final2.ToString();
                    pnlTeamPreview.Invalidate();
                    pnlTeamRecap.Invalidate();
                    print3();
                    print4();
                }
            }
            if(!currentMatch.GameWon)
                ckbSwitch.Checked = !ckbSwitch.Checked;
            checkService();
        }
        private void ckbService_CheckedChanged(object sender, EventArgs e)
        {
            Color aux = btnPointScorePlayer1.BackColor;
            btnPointScorePlayer1.BackColor = btnPointScorePlayer2.BackColor;
            btnPointScorePlayer2.BackColor = aux;
            currentMatch.Service = !currentMatch.Service;
            updatePoint(false);
        }
        private void ckbSwitch_CheckedChanged(object sender, EventArgs e)
        {
            String aux;
            aux = lblShowNamePlayer1.Text;
            lblShowNamePlayer1.Text = lblShowNamePlayer2.Text;
            lblShowNamePlayer2.Text = aux;
            aux = btnPointScorePlayer1.Text;
            btnPointScorePlayer1.Text = btnPointScorePlayer2.Text;
            btnPointScorePlayer2.Text = aux;
            aux = btnSetScorePlayer1.Text;
            btnSetScorePlayer1.Text = btnSetScorePlayer2.Text;
            btnSetScorePlayer2.Text = aux;
            currentMatch.Service = !currentMatch.Service;
            ckbService.Checked = !ckbService.Checked;
        }

        private void btnUpdateScore_Click(object sender, EventArgs e)
        {
            updateScores();
        }
        
        #endregion

        #region Match

        private void btnUpdateTeams_Click(object sender, EventArgs e)
        {
            lblScoreTeam1.Text = txtTeamName1.Text;
            lblScoreTeam2.Text = txtTeamName2.Text;
            currentTeamMatch = new TeamMatch(txtTeamName1.Text, txtTeamName2.Text, 9);

            txtPlayerTeam1Game1.Text = txtPlayerTeam1Game2.Text = txtPlayerTeam1Game3.Text = txtPlayerTeam1Game4.Text = txtPlayerTeam1Game5.Text = txtPlayerTeam1Game6.Text = txtPlayerTeam1Game7.Text = txtPlayerTeam1Game8.Text = txtPlayerTeam1Game9.Text = "";
            txtPlayerTeam2Game1.Text = txtPlayerTeam2Game2.Text = txtPlayerTeam2Game3.Text = txtPlayerTeam2Game4.Text = txtPlayerTeam2Game5.Text = txtPlayerTeam2Game6.Text = txtPlayerTeam2Game7.Text = txtPlayerTeam2Game8.Text = txtPlayerTeam2Game9.Text = "";
            txtScoreTeam1Game1.Text = txtScoreTeam1Game2.Text = txtScoreTeam1Game3.Text = txtScoreTeam1Game4.Text = txtScoreTeam1Game5.Text = txtScoreTeam1Game6.Text = txtScoreTeam1Game7.Text = txtScoreTeam1Game8.Text = txtScoreTeam1Game9.Text = "";
            txtScoreTeam2Game1.Text = txtScoreTeam2Game2.Text = txtScoreTeam2Game3.Text = txtScoreTeam2Game4.Text = txtScoreTeam2Game5.Text = txtScoreTeam2Game6.Text = txtScoreTeam2Game7.Text = txtScoreTeam2Game8.Text = txtScoreTeam2Game9.Text = "";

            txtPlayerTeam1Game1.Enabled = txtPlayerTeam1Game2.Enabled = txtPlayerTeam1Game3.Enabled = txtPlayerTeam1Game4.Enabled = txtPlayerTeam1Game5.Enabled = txtPlayerTeam1Game6.Enabled = txtPlayerTeam1Game7.Enabled = txtPlayerTeam1Game8.Enabled = txtPlayerTeam1Game9.Enabled = false;
            txtPlayerTeam2Game1.Enabled = txtPlayerTeam2Game2.Enabled = txtPlayerTeam2Game3.Enabled = txtPlayerTeam2Game4.Enabled = txtPlayerTeam2Game5.Enabled = txtPlayerTeam2Game6.Enabled = txtPlayerTeam2Game7.Enabled = txtPlayerTeam2Game8.Enabled = txtPlayerTeam2Game9.Enabled = false;
            txtScoreTeam1Game1.Enabled = txtScoreTeam1Game2.Enabled = txtScoreTeam1Game3.Enabled = txtScoreTeam1Game4.Enabled = txtScoreTeam1Game5.Enabled = txtScoreTeam1Game6.Enabled = txtScoreTeam1Game7.Enabled = txtScoreTeam1Game8.Enabled = txtScoreTeam1Game9.Enabled = false;
            txtScoreTeam2Game1.Enabled = txtScoreTeam2Game2.Enabled = txtScoreTeam2Game3.Enabled = txtScoreTeam2Game4.Enabled = txtScoreTeam2Game5.Enabled = txtScoreTeam2Game6.Enabled = txtScoreTeam2Game7.Enabled = txtScoreTeam2Game8.Enabled = txtScoreTeam2Game9.Enabled = false;

            btnScoreTeam1.Text = btnScoreTeam2.Text = "0";
        }
        private void btnClearTNames_Click(object sender, EventArgs e)
        {
            txtTeamName1.Text = txtTeamName2.Text = "";
        }
        private void btnScoreTeam1_Click(object sender, EventArgs e)
        {
            if ((Int32.Parse(btnScoreTeam1.Text) + Int32.Parse(btnScoreTeam2.Text)) < currentTeamMatch.MatchLength)
            {
                nextMatch();
                currentTeamMatch.CurrentMatch++;
                btnScoreTeam1.Text = (Int32.Parse(btnScoreTeam1.Text) + 1).ToString();
            }
        }
        private void btnScoreTeam2_Click(object sender, EventArgs e)
        {
            if ((Int32.Parse(btnScoreTeam1.Text) + Int32.Parse(btnScoreTeam2.Text)) < currentTeamMatch.MatchLength)
            {
                nextMatch();
                currentTeamMatch.CurrentMatch++;
                btnScoreTeam2.Text = (Int32.Parse(btnScoreTeam1.Text) + 1).ToString();
            }
        }

        #endregion

        #endregion

        #region Methods

        private void reset()
        {
            init();
            initGComponents();
            updateGraphics();
            txtSurname1.Text = txtSurname2.Text = txtName1.Text = txtName2.Text = "";
            txtDouble1Player1.Text = txtDouble1Player2.Text = txtDouble2Player1.Text = txtDouble2Player2.Text = "";
            txtTeamName1.Text = txtTeamName2.Text = "";
            txtPlayerTag.Text = txtTeamTag.Text = "";
            ckbTeams.Checked = ckbDouble.Checked = ckbService.Checked = ckbSwitch.Checked = false;
            ckbTimeOutPlayer1.Checked = ckbTimeOutPlayer2.Checked = ckbWhitePlayer1.Checked = ckbWhitePlayer2.Checked = ckbYellowPlayer1.Checked = ckbYellowPlayer2.Checked = ckbRedPlayer1.Checked = ckbRedPlayer2.Checked = false;
            btnPointScorePlayer1.Text = btnPointScorePlayer2.Text = btnSetScorePlayer1.Text = btnSetScorePlayer2.Text = btnScoreTeam1.Text = btnScoreTeam2.Text = "0";
            lblShowNamePlayer1.Text = "Player 1";
            lblShowNamePlayer2.Text = "Player 2";
            lblScoreTeam1.Text = "Team 1";
            lblScoreTeam2.Text = "Team 2";
            txtSet4Player1.Visible = txtSet4Player2.Visible = txtSet5Player1.Visible = txtSet5Player2.Visible = txtSet6Player1.Visible = txtSet6Player2.Visible = txtSet7Player1.Visible = txtSet7Player2.Visible = true;

            txtSet1Player1.Text = txtSet1Player2.Text = txtSet2Player1.Text = txtSet2Player2.Text = txtSet3Player1.Text = txtSet3Player2.Text = txtSet4Player1.Text = txtSet4Player2.Text = txtSet5Player1.Text = txtSet5Player2.Text = txtSet6Player1.Text = txtSet6Player2.Text = txtSet7Player1.Text = txtSet7Player2.Text = "";
            txtPlayerTeam1Game1.Text = txtPlayerTeam1Game2.Text = txtPlayerTeam1Game3.Text = txtPlayerTeam1Game4.Text = txtPlayerTeam1Game5.Text = txtPlayerTeam1Game6.Text = txtPlayerTeam1Game7.Text = txtPlayerTeam1Game8.Text = txtPlayerTeam1Game9.Text = "";
            txtPlayerTeam2Game1.Text = txtPlayerTeam2Game2.Text = txtPlayerTeam2Game3.Text = txtPlayerTeam2Game4.Text = txtPlayerTeam2Game5.Text = txtPlayerTeam2Game6.Text = txtPlayerTeam2Game7.Text = txtPlayerTeam2Game8.Text = txtPlayerTeam2Game9.Text = "";
            txtScoreTeam1Game1.Text = txtScoreTeam1Game2.Text = txtScoreTeam1Game3.Text = txtScoreTeam1Game4.Text = txtScoreTeam1Game5.Text = txtScoreTeam1Game6.Text = txtScoreTeam1Game7.Text = txtScoreTeam1Game8.Text = txtScoreTeam1Game9.Text = "";
            txtScoreTeam2Game1.Text = txtScoreTeam2Game2.Text = txtScoreTeam2Game3.Text = txtScoreTeam2Game4.Text = txtScoreTeam2Game5.Text = txtScoreTeam2Game6.Text = txtScoreTeam2Game7.Text = txtScoreTeam2Game8.Text = txtScoreTeam2Game9.Text = "";
            btnPointScorePlayer1.BackColor = Color.Yellow;
            btnPointScorePlayer2.BackColor = Color.FromArgb(224, 224, 224);
        }
        private void checkService()
        {
            if (!ckbSwitch.Checked)
            {
                if (currentMatch.isService())
                {

                    btnPointScorePlayer2.BackColor = Color.FromArgb(224, 224, 224);
                    btnPointScorePlayer1.BackColor = Color.Yellow;
                }
                else
                {
                    btnPointScorePlayer1.BackColor = Color.FromArgb(224, 224, 224);
                    btnPointScorePlayer2.BackColor = Color.Yellow;
                }
            }
            else
            {
                if (!currentMatch.isService())
                {

                    btnPointScorePlayer2.BackColor = Color.FromArgb(224, 224, 224);
                    btnPointScorePlayer1.BackColor = Color.Yellow;
                }
                else
                {
                    btnPointScorePlayer1.BackColor = Color.FromArgb(224, 224, 224);
                    btnPointScorePlayer2.BackColor = Color.Yellow;
                }
            }
        }
        private void nextGame()
        {
            switch (currentMatch.CurrentSet)
            {
                case 0:
                    {
                        txtSet1Player1.Enabled = true;
                        txtSet1Player2.Enabled = true;
                        txtSet1Player1.Text = currentMatch.CurrentScore1.ToString();
                        txtSet1Player2.Text = currentMatch.CurrentScore2.ToString();
                        currentMatch.nextGame();
                        break;
                    }
                case 1:
                    {
                        txtSet2Player1.Enabled = true;
                        txtSet2Player2.Enabled = true;
                        txtSet2Player1.Text = currentMatch.CurrentScore1.ToString();
                        txtSet2Player2.Text = currentMatch.CurrentScore2.ToString();
                        currentMatch.nextGame();
                        break;
                    }
                case 2:
                    {
                        txtSet3Player1.Enabled = true;
                        txtSet3Player2.Enabled = true;
                        txtSet3Player1.Text = currentMatch.CurrentScore1.ToString();
                        txtSet3Player2.Text = currentMatch.CurrentScore2.ToString();
                        currentMatch.nextGame();
                        break;
                    }
                case 3:
                    {
                        txtSet4Player1.Enabled = true;
                        txtSet4Player2.Enabled = true;
                        txtSet4Player1.Text = currentMatch.CurrentScore1.ToString();
                        txtSet4Player2.Text = currentMatch.CurrentScore2.ToString();
                        currentMatch.nextGame();
                        break;
                    }
                case 4:
                    {
                        txtSet5Player1.Enabled = true;
                        txtSet5Player2.Enabled = true;
                        txtSet5Player1.Text = currentMatch.CurrentScore1.ToString();
                        txtSet5Player2.Text = currentMatch.CurrentScore2.ToString();
                        currentMatch.nextGame();
                        break;
                    }
                case 5:
                    {
                        txtSet6Player1.Enabled = true;
                        txtSet6Player2.Enabled = true;
                        txtSet6Player1.Text = currentMatch.CurrentScore1.ToString();
                        txtSet6Player2.Text = currentMatch.CurrentScore2.ToString();
                        currentMatch.nextGame();
                        break;
                    }
                case 6:
                    {
                        txtSet7Player1.Enabled = true;
                        txtSet7Player2.Enabled = true;
                        txtSet7Player1.Text = currentMatch.CurrentScore1.ToString();
                        txtSet7Player2.Text = currentMatch.CurrentScore2.ToString();
                        currentMatch.nextGame();
                        break;
                    }
            }
        }
        private void nextMatch()
        {
            switch (currentTeamMatch.CurrentMatch)
            {
                case 0:
                    {
                        txtPlayerTeam1Game1.Enabled = true;
                        txtPlayerTeam2Game1.Enabled = true;
                        txtScoreTeam1Game1.Enabled = true;
                        txtScoreTeam2Game1.Enabled = true;
                        txtPlayerTeam1Game1.Text = currentMatch.Player1;
                        txtPlayerTeam2Game1.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game1.Text = "R";
                        else
                            txtScoreTeam1Game1.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game1.Text = "R";
                        else
                            txtScoreTeam2Game1.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 1:
                    {
                        txtPlayerTeam1Game2.Enabled = true;
                        txtPlayerTeam2Game2.Enabled = true;
                        txtScoreTeam1Game2.Enabled = true;
                        txtScoreTeam2Game2.Enabled = true;
                        txtPlayerTeam1Game2.Text = currentMatch.Player1;
                        txtPlayerTeam2Game2.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game2.Text = "/";
                        else
                            txtScoreTeam1Game2.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game2.Text = "/";
                        else
                            txtScoreTeam2Game2.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 2:
                    {
                        txtPlayerTeam1Game3.Enabled = true;
                        txtPlayerTeam2Game3.Enabled = true;
                        txtScoreTeam1Game3.Enabled = true;
                        txtScoreTeam2Game3.Enabled = true;
                        txtPlayerTeam1Game3.Text = currentMatch.Player1;
                        txtPlayerTeam2Game3.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game3.Text = "/";
                        else
                            txtScoreTeam1Game3.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game3.Text = "/";
                        else
                            txtScoreTeam2Game3.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 3:
                    {
                        txtPlayerTeam1Game4.Enabled = true;
                        txtPlayerTeam2Game4.Enabled = true;
                        txtScoreTeam1Game4.Enabled = true;
                        txtScoreTeam2Game4.Enabled = true;
                        txtPlayerTeam1Game4.Text = currentMatch.Player1;
                        txtPlayerTeam2Game4.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game4.Text = "/";
                        else
                            txtScoreTeam1Game4.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game4.Text = "/";
                        else
                            txtScoreTeam2Game4.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 4:
                    {
                        txtPlayerTeam1Game5.Enabled = true;
                        txtPlayerTeam2Game5.Enabled = true;
                        txtScoreTeam1Game5.Enabled = true;
                        txtScoreTeam2Game5.Enabled = true;
                        txtPlayerTeam1Game5.Text = currentMatch.Player1;
                        txtPlayerTeam2Game5.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game5.Text = "/";
                        else
                            txtScoreTeam1Game5.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game5.Text = "/";
                        else
                            txtScoreTeam2Game5.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 5:
                    {
                        txtPlayerTeam1Game6.Enabled = true;
                        txtPlayerTeam2Game6.Enabled = true;
                        txtScoreTeam1Game6.Enabled = true;
                        txtScoreTeam2Game6.Enabled = true;
                        txtPlayerTeam1Game6.Text = currentMatch.Player1;
                        txtPlayerTeam2Game6.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game6.Text = "/";
                        else
                            txtScoreTeam1Game6.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game6.Text = "/";
                        else
                            txtScoreTeam2Game6.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 6:
                    {
                        txtPlayerTeam1Game7.Enabled = true;
                        txtPlayerTeam2Game7.Enabled = true;
                        txtScoreTeam1Game7.Enabled = true;
                        txtScoreTeam2Game7.Enabled = true;
                        txtPlayerTeam1Game7.Text = currentMatch.Player1;
                        txtPlayerTeam2Game7.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game7.Text = "/";
                        else
                            txtScoreTeam1Game7.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game7.Text = "/";
                        else
                            txtScoreTeam2Game7.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 7:
                    {
                        txtPlayerTeam1Game8.Enabled = true;
                        txtPlayerTeam2Game8.Enabled = true;
                        txtScoreTeam1Game8.Enabled = true;
                        txtScoreTeam2Game8.Enabled = true;
                        txtPlayerTeam1Game8.Text = currentMatch.Player1;
                        txtPlayerTeam2Game8.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game8.Text = "/";
                        else
                            txtScoreTeam1Game8.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game8.Text = "/";
                        else
                            txtScoreTeam2Game8.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
                case 8:
                    {
                        txtPlayerTeam1Game9.Enabled = true;
                        txtPlayerTeam2Game9.Enabled = true;
                        txtScoreTeam1Game9.Enabled = true;
                        txtScoreTeam2Game9.Enabled = true;
                        txtPlayerTeam1Game9.Text = currentMatch.Player1;
                        txtPlayerTeam2Game9.Text = currentMatch.Player2;
                        if (currentMatch.Surrender1)
                            txtScoreTeam1Game9.Text = "/";
                        else
                            txtScoreTeam1Game9.Text = currentMatch.FinalScore1.ToString();
                        if (currentMatch.Surrender2)
                            txtScoreTeam2Game9.Text = "/";
                        else
                            txtScoreTeam2Game9.Text = currentMatch.FinalScore2.ToString();
                        break;
                    }
            }
        }
        private void updateScores()
        {
            currentTeamMatch.Final1 = currentTeamMatch.Final2 = 0;
            try
            {
                currentTeamMatch.MatchResults[0] = new Match();
                currentTeamMatch.MatchResults[0].Player1 = txtPlayerTeam1Game1.Text;
                currentTeamMatch.MatchResults[0].Player2 = txtPlayerTeam2Game1.Text;
                currentTeamMatch.MatchResults[0].FinalScore1 = Int32.Parse(txtScoreTeam1Game1.Text);
                currentTeamMatch.MatchResults[0].FinalScore2 = Int32.Parse(txtScoreTeam2Game1.Text);
                if (currentTeamMatch.MatchResults[0].FinalScore1 > currentTeamMatch.MatchResults[0].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[1] = new Match();
                currentTeamMatch.MatchResults[1].Player1 = txtPlayerTeam1Game2.Text;
                currentTeamMatch.MatchResults[1].Player2 = txtPlayerTeam2Game2.Text;
                currentTeamMatch.MatchResults[1].FinalScore1 = Int32.Parse(txtScoreTeam1Game2.Text);
                currentTeamMatch.MatchResults[1].FinalScore2 = Int32.Parse(txtScoreTeam2Game2.Text);
                if (currentTeamMatch.MatchResults[1].FinalScore1 > currentTeamMatch.MatchResults[1].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[2] = new Match();
                currentTeamMatch.MatchResults[2].Player1 = txtPlayerTeam1Game3.Text;
                currentTeamMatch.MatchResults[2].Player2 = txtPlayerTeam2Game3.Text;
                currentTeamMatch.MatchResults[2].FinalScore1 = Int32.Parse(txtScoreTeam1Game3.Text);
                currentTeamMatch.MatchResults[2].FinalScore2 = Int32.Parse(txtScoreTeam2Game3.Text);
                if (currentTeamMatch.MatchResults[2].FinalScore1 > currentTeamMatch.MatchResults[2].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[3] = new Match();
                currentTeamMatch.MatchResults[3].Player1 = txtPlayerTeam1Game4.Text;
                currentTeamMatch.MatchResults[3].Player2 = txtPlayerTeam2Game4.Text;
                currentTeamMatch.MatchResults[3].FinalScore1 = Int32.Parse(txtScoreTeam1Game4.Text);
                currentTeamMatch.MatchResults[3].FinalScore2 = Int32.Parse(txtScoreTeam2Game4.Text);
                if (currentTeamMatch.MatchResults[3].FinalScore1 > currentTeamMatch.MatchResults[3].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[4] = new Match();
                currentTeamMatch.MatchResults[4].Player1 = txtPlayerTeam1Game5.Text;
                currentTeamMatch.MatchResults[4].Player2 = txtPlayerTeam2Game5.Text;
                currentTeamMatch.MatchResults[4].FinalScore1 = Int32.Parse(txtScoreTeam1Game5.Text);
                currentTeamMatch.MatchResults[4].FinalScore2 = Int32.Parse(txtScoreTeam2Game5.Text);
                if (currentTeamMatch.MatchResults[4].FinalScore1 > currentTeamMatch.MatchResults[4].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[5] = new Match();
                currentTeamMatch.MatchResults[5].Player1 = txtPlayerTeam1Game6.Text;
                currentTeamMatch.MatchResults[5].Player2 = txtPlayerTeam2Game6.Text;
                currentTeamMatch.MatchResults[5].FinalScore1 = Int32.Parse(txtScoreTeam1Game6.Text);
                currentTeamMatch.MatchResults[5].FinalScore2 = Int32.Parse(txtScoreTeam2Game6.Text);
                if (currentTeamMatch.MatchResults[5].FinalScore1 > currentTeamMatch.MatchResults[5].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[6] = new Match();
                currentTeamMatch.MatchResults[6].Player1 = txtPlayerTeam1Game7.Text;
                currentTeamMatch.MatchResults[6].Player2 = txtPlayerTeam2Game7.Text;
                currentTeamMatch.MatchResults[6].FinalScore1 = Int32.Parse(txtScoreTeam1Game7.Text);
                currentTeamMatch.MatchResults[6].FinalScore2 = Int32.Parse(txtScoreTeam2Game7.Text);
                if (currentTeamMatch.MatchResults[6].FinalScore1 > currentTeamMatch.MatchResults[6].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[7] = new Match();
                currentTeamMatch.MatchResults[7].Player1 = txtPlayerTeam1Game8.Text;
                currentTeamMatch.MatchResults[7].Player2 = txtPlayerTeam2Game8.Text;
                currentTeamMatch.MatchResults[7].FinalScore1 = Int32.Parse(txtScoreTeam1Game8.Text);
                currentTeamMatch.MatchResults[7].FinalScore2 = Int32.Parse(txtScoreTeam2Game8.Text);
                if (currentTeamMatch.MatchResults[7].FinalScore1 > currentTeamMatch.MatchResults[7].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            try
            {
                currentTeamMatch.MatchResults[8] = new Match();
                currentTeamMatch.MatchResults[8].Player1 = txtPlayerTeam1Game9.Text;
                currentTeamMatch.MatchResults[8].Player2 = txtPlayerTeam2Game9.Text;
                currentTeamMatch.MatchResults[8].FinalScore1 = Int32.Parse(txtScoreTeam1Game9.Text);
                currentTeamMatch.MatchResults[8].FinalScore2 = Int32.Parse(txtScoreTeam2Game9.Text);
                if (currentTeamMatch.MatchResults[8].FinalScore1 > currentTeamMatch.MatchResults[8].FinalScore2)
                    currentTeamMatch.Final1++;
                else
                    currentTeamMatch.Final2++;
            }
            catch (Exception ex) { }
            btnScoreTeam1.Text = currentTeamMatch.Final1.ToString();
            btnScoreTeam2.Text = currentTeamMatch.Final2.ToString();
        }
        private void updatePoint(bool update)
        {
            checkService();
            if (update)
            {
                pnlGamePreview.Invalidate();
                //pnlGameRecap.Invalidate();
            }
            else
            {
                pnlGamePreview.Invalidate(new Rectangle(SET_START + SCORE_WIDTH, 0, SCORE_WIDTH, pnlGamePreview.ClientRectangle.Height));
                pnlGamePreview.Invalidate(new Rectangle(SET_START - ADD_OFFSET + MAIN_X_OFF / 2, 0, SCORE_WIDTH, pnlGamePreview.ClientRectangle.Height));
            }
            if (currentMatch.SetWon || currentMatch.GameWon)
                btnNext.Enabled = true;
            else
                btnNext.Enabled = false;
            print1();
        }
        private void updateGraphics()
        {
            Bitmap auxMap = new Bitmap(pnlGamePreview.ClientRectangle.Width, pnlGamePreview.ClientRectangle.Height);
            Graphics g;
            g = Graphics.FromImage(auxMap);


            updateScores();


            //Game Preview 
            try
            {
                ADD_OFFSET = BASE_OFFSET + Int32.Parse(txtOffset.Text);
            }
            catch (Exception ex) { }
            NAME_1_OFF = (int)g.MeasureString(currentMatch.Player1.ToUpper(), mainFont).Width;
            NAME_2_OFF = (int)g.MeasureString(currentMatch.Player2.ToUpper(), mainFont).Width;
            SET_START = ADD_OFFSET + 2 * MAIN_X_OFF + maxValue(NAME_1_OFF + (int)g.MeasureString(currentMatch.Player1_2, regularFont).Width, NAME_2_OFF + (int)g.MeasureString(currentMatch.Player2_2, regularFont).Width);
            NAME_1_OFF += MAIN_X_OFF;
            NAME_2_OFF += MAIN_X_OFF;
            MAIN_WIDTH = SET_START + 2 * SCORE_WIDTH;
            TAG_WIDTH = 2 * TAG_X_OFF + (int)g.MeasureString(txtPlayerTag.Text, auxFont).Width;
            SP_WIDTH = 2 * SP_X_OFF + (int)g.MeasureString("SET POINT", auxFont).Width;
            GP_WIDTH = 2 * GP_X_OFF + (int)g.MeasureString("GAME POINT", auxFont).Width;
            MAIN_Y_OFF = (MAIN_HEIGHT - (int)g.MeasureString("BANANA", mainFont).Height) / 2;
            REGULAR_Y_OFF = (MAIN_HEIGHT - (int)g.MeasureString(currentMatch.Player1_2, regularFont).Height) - MAIN_Y_OFF;
            AUX_Y_OFF = (AUX_HEIGHT - (int)g.MeasureString("BANANA", auxFont).Height) / 2;

            if(currentMatch.FinalScore1 == 0 && currentMatch.FinalScore2 == 0 && currentMatch.CurrentScore1 == 0 && currentMatch.CurrentScore2 == 0)
                first = true;
            else
                first = false;

            //Game Recap
            RECAP_SCORE_START = SET_START - ADD_OFFSET + BASE_OFFSET / 2;
            if (RECAP_SCORE_START < TAG_WIDTH)
                RECAP_SCORE_START = TAG_WIDTH;
            RECAP_WIDTH = RECAP_SCORE_START + SCORE_WIDTH * (currentMatch.CurrentSet + 1);


            //Team Preview
            TEAM_TAG_WIDTH = 2 * TAG_X_OFF + (int)g.MeasureString(txtTeamTag.Text, auxFont).Width;
            TEAM_WIDTH = MAIN_X_OFF * 2 + maxValue((int)g.MeasureString(currentTeamMatch.Team1.ToUpper(), mainFont).Width, (int)g.MeasureString(currentTeamMatch.Team2.ToUpper(), mainFont).Width);
            if (ckbTeamTag.Checked)
            {
                TEAM_WIDTH = maxValue(TEAM_WIDTH, TEAM_TAG_WIDTH - SCORE_WIDTH);
                TEAM_X_OFF = (TEAM_WIDTH + SCORE_WIDTH - (int)g.MeasureString(txtTeamTag.Text, auxFont).Width) / 2;
            }
            TEAM_PREVIEW_START_POINT = pnlTeamPreview.ClientRectangle.Width - TEAM_WIDTH - SCORE_WIDTH;


            //Team Recap
            TEAM_RECAP_WIDTH1 = (int)g.MeasureString(currentTeamMatch.Team1.ToUpper(), mainFont).Width + 2 * MAIN_X_OFF;
            for (int i = 0; i < currentTeamMatch.CurrentMatch; i++) 
            {
                TEAM_RECAP_WIDTH1 = maxValue(TEAM_RECAP_WIDTH1, (int)g.MeasureString(currentTeamMatch.MatchResults[i].Player1, mainFont).Width + 2 * MAIN_X_OFF);
            }
            TEAM_RECAP_WIDTH2 = (int)g.MeasureString(currentTeamMatch.Team2.ToUpper(), mainFont).Width + 2 * MAIN_X_OFF;
            for (int i = 0; i < currentTeamMatch.CurrentMatch; i++)
            {
                TEAM_RECAP_WIDTH2 = maxValue(TEAM_RECAP_WIDTH2, (int)g.MeasureString(currentTeamMatch.MatchResults[i].Player2, mainFont).Width + 2 * MAIN_X_OFF);
            }
            TEAM_RECAP_SCORE_START = TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2;
            LATERAL_RECAP_HEIGTH = AUX_HEIGHT + MAIN_HEIGHT * currentTeamMatch.CurrentMatch + MAIN_HEIGHT;
            RECAP_TOTAL_WIDTH = TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + 2 * SCORE_WIDTH;
            RECAP_TOTAL_WIDTH = maxValue(RECAP_TOTAL_WIDTH, (int)g.MeasureString(txtTeamTag.Text, auxFont).Width + 2 * TAG_X_OFF);
            TEAM_RECAP_WIDTH2 = RECAP_TOTAL_WIDTH - TEAM_RECAP_WIDTH1 - 2 * SCORE_WIDTH;
            RECAP_AUX_X_OFF = (RECAP_TOTAL_WIDTH - (int)g.MeasureString(txtTeamTag.Text, auxFont).Width) / 2;

            g.Dispose();

            pnlGamePreview.Invalidate();
            pnlGameRecap.Invalidate();
            pnlTeamPreview.Invalidate();
            pnlTeamRecap.Invalidate();
            print1();
            print2();
            print3();
            print4();
        }
        private int maxValue(int a, int b)
        {
            if (a > b)
                return a;
            return b;
        }

        #endregion

        #region Paint Graphics

        private void pnlGamePreview_Paint(object sender, PaintEventArgs e)
        {
            if (currentMatch.GameWon)
                MAIN_WIDTH -= SCORE_WIDTH;
            int score_x_off = 0;
            int currentHeight = 0;
            Graphics g = e.Graphics;
            if (!ckbPlayerTag.Checked)
                currentHeight += AUX_HEIGHT;
            if (currentMatch.GamePoint)
            {
                g.FillRectangle(sBrush, new Rectangle(0, currentHeight, SP_WIDTH, AUX_HEIGHT));
                currentHeight += AUX_Y_OFF;
                g.DrawString("SET POINT", auxFont, auxBrush, SP_X_OFF, currentHeight);
                currentHeight += AUX_HEIGHT - AUX_Y_OFF;
            }
            else if (currentMatch.MatchPoint)
            {
                g.FillRectangle(sBrush, new Rectangle(0, currentHeight, GP_WIDTH, AUX_HEIGHT));
                currentHeight += AUX_Y_OFF;
                g.DrawString("GAME POINT", auxFont, auxBrush, GP_X_OFF, currentHeight);
                currentHeight += AUX_HEIGHT - AUX_Y_OFF;
            }
            else
                currentHeight += AUX_HEIGHT;
            if (ckbPlayerTag.Checked)
            {
                g.FillRectangle(sBrush, new Rectangle(0, currentHeight, TAG_WIDTH, AUX_HEIGHT));
                currentHeight += AUX_Y_OFF;
                g.DrawString(txtPlayerTag.Text, auxFont, auxBrush, TAG_X_OFF, currentHeight);
                currentHeight += AUX_HEIGHT - AUX_Y_OFF;
            }

            //Starting Score Graphics

            //Creating LinearGradientBrush
            lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);

            //First Player
            g.FillRectangle(lBrush, new Rectangle(0, currentHeight, MAIN_WIDTH, MAIN_HEIGHT));
            //Set Space
            g.FillRectangle(sBrush, new Rectangle(SET_START, currentHeight, SCORE_WIDTH, MAIN_HEIGHT));
            //Strings in place
            currentHeight += MAIN_Y_OFF;
            g.DrawString(currentMatch.Player1.ToUpper(), mainFont, textBrush, MAIN_X_OFF, currentHeight);
            if (!currentMatch.IsDouble)
            {
                currentHeight -= MAIN_Y_OFF;
                currentHeight += REGULAR_Y_OFF;
                g.DrawString(currentMatch.Player1_2, regularFont, textBrush, NAME_1_OFF, currentHeight);
                currentHeight += MAIN_Y_OFF;
                currentHeight -= REGULAR_Y_OFF;
            }
            if(first)
                first = first;
            else if (currentMatch.isService() && (!currentMatch.SetWon || currentMatch.GameWon))
            {
                g.DrawString("⮜", mainFont, textBrush, SET_START - ADD_OFFSET + MAIN_X_OFF / 2, currentHeight);
            }
            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.FinalScore1.ToString(), mainFont).Width) / 2;
            g.DrawString(currentMatch.FinalScore1.ToString(), mainFont, auxBrush, SET_START + score_x_off, currentHeight);
            if (!currentMatch.GameWon)
            {
                score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.CurrentScore1.ToString(), mainFont).Width) / 2;
                g.DrawString(currentMatch.CurrentScore1.ToString(), mainFont, textBrush, SET_START + SCORE_WIDTH + score_x_off, currentHeight);
            }
            currentHeight += MAIN_HEIGHT - MAIN_Y_OFF;

            //Separating Line
            /*g.DrawLine(mainPen, 0, currentHeight, MAIN_WIDTH, currentHeight);
            g.FillRectangle(sBrush, new Rectangle(SET_START, currentHeight, SCORE_WIDTH, 1));
            currentHeight++;*/
            lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);

            //Second Player
            g.FillRectangle(lBrush, new Rectangle(0, currentHeight, MAIN_WIDTH, MAIN_HEIGHT));
            //Set Space
            g.FillRectangle(sBrush, new Rectangle(SET_START, currentHeight, SCORE_WIDTH, MAIN_HEIGHT));
            //Strings in place
            currentHeight += MAIN_Y_OFF;
            g.DrawString(currentMatch.Player2.ToUpper(), mainFont, textBrush, MAIN_X_OFF, currentHeight);
            if (!currentMatch.IsDouble)
            {
                currentHeight -= MAIN_Y_OFF;
                currentHeight += REGULAR_Y_OFF;
                g.DrawString(currentMatch.Player2_2, regularFont, textBrush, NAME_2_OFF, currentHeight);
                currentHeight += MAIN_Y_OFF;
                currentHeight -= REGULAR_Y_OFF;
            }
            if (first)
                first = false;
            else if (!currentMatch.isService() && (!currentMatch.SetWon || currentMatch.GameWon))
            {
                g.DrawString("⮜", mainFont, textBrush, SET_START - ADD_OFFSET + MAIN_X_OFF / 2, currentHeight);
            }
            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.FinalScore2.ToString(), mainFont).Width) / 2;
            g.DrawString(currentMatch.FinalScore2.ToString(), mainFont, auxBrush, SET_START + score_x_off, currentHeight);
            if (!currentMatch.GameWon)
            {
                score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.CurrentScore2.ToString(), mainFont).Width) / 2;
                g.DrawString(currentMatch.CurrentScore2.ToString(), mainFont, textBrush, SET_START + SCORE_WIDTH + score_x_off, currentHeight);
            }
            currentHeight += MAIN_HEIGHT - MAIN_Y_OFF;
            g.Dispose();
        }
        private void pnlGameRecap_Paint(object sender, PaintEventArgs e)
        {
            int score_x_off = 0;
            int currentHeight = 0;
            int currentWidth = 0;
            Graphics g = e.Graphics;

            //Lateral Bars
            g.FillRectangle(sBrush, new Rectangle(0, 0, LATERAL_WIDTH, LATERAL_HEIGHT));
            g.FillRectangle(sBrush, new Rectangle(RECAP_WIDTH + LATERAL_WIDTH, 0, LATERAL_WIDTH, LATERAL_HEIGHT));

            //Tag Space
            g.FillRectangle(auxBrush, new Rectangle(LATERAL_WIDTH, currentHeight, RECAP_WIDTH, AUX_HEIGHT));
            currentHeight += AUX_Y_OFF;
            g.DrawString(txtPlayerTag.Text, auxFont, textBrush, TAG_X_OFF + LATERAL_WIDTH, currentHeight);
            currentWidth = RECAP_SCORE_START + SCORE_WIDTH + LATERAL_WIDTH;
            for (int i = 0; i < currentMatch.CurrentSet; i++)
            {
                score_x_off = (SCORE_WIDTH - (int)g.MeasureString((i + 1).ToString(), auxFont).Width) / 2;
                g.DrawString((i + 1).ToString(), auxFont, textBrush, currentWidth + score_x_off, currentHeight);

                currentWidth += SCORE_WIDTH;
            }
            currentHeight += AUX_HEIGHT - AUX_Y_OFF;
            currentWidth = 0;

            //Starting Score Graphics

            //Creating LinearGradientBrush
            lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);

            //First Player
            g.FillRectangle(lBrush, new Rectangle(LATERAL_WIDTH, currentHeight, RECAP_WIDTH, MAIN_HEIGHT));
            //Set Space
            g.FillRectangle(sBrush, new Rectangle(RECAP_SCORE_START + LATERAL_WIDTH, currentHeight, SCORE_WIDTH, MAIN_HEIGHT));
            //Strings in place
            currentHeight += MAIN_Y_OFF;
            g.DrawString(currentMatch.Player1.ToUpper(), mainFont, textBrush, MAIN_X_OFF + LATERAL_WIDTH, currentHeight);
            if (!currentMatch.IsDouble)
            {
                currentHeight -= MAIN_Y_OFF;
                currentHeight += REGULAR_Y_OFF;
                g.DrawString(currentMatch.Player1_2, regularFont, textBrush, NAME_1_OFF + LATERAL_WIDTH, currentHeight);
                currentHeight += MAIN_Y_OFF;
                currentHeight -= REGULAR_Y_OFF;
            }
            //Set Score
            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.FinalScore1.ToString(), mainFont).Width) / 2 + LATERAL_WIDTH;
            g.DrawString(currentMatch.FinalScore1.ToString(), mainFont, auxBrush, RECAP_SCORE_START + score_x_off, currentHeight);
            //Recap
            currentWidth = RECAP_SCORE_START + SCORE_WIDTH + LATERAL_WIDTH;
            for (int i = 0; i < currentMatch.CurrentSet; i++)
            {
                if (i == currentMatch.CurrentSet && !currentMatch.GameWon)
                {
                    score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.CurrentScore1.ToString(), mainFont).Width) / 2;
                    g.DrawString(currentMatch.CurrentScore1.ToString(), mainFont, textBrush, currentWidth + score_x_off, currentHeight);
                }
                else
                {
                    if (currentMatch.Results1[i] < currentMatch.Results2[i])
                    {
                        score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.Results1[i].ToString(), mainFont).Width) / 2;
                        g.DrawString(currentMatch.Results1[i].ToString(), mainFont, loserBrush, currentWidth + score_x_off, currentHeight);
                    }
                    else
                    {
                        score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.Results1[i].ToString(), mainFont).Width) / 2;
                        g.DrawString(currentMatch.Results1[i].ToString(), mainFont, textBrush, currentWidth + score_x_off, currentHeight);
                    }
                }
                currentWidth += SCORE_WIDTH;
            }
            currentWidth = 0;
            currentHeight += MAIN_HEIGHT - MAIN_Y_OFF;

            //Separating Line
            /*g.DrawLine(mainPen, 0, currentHeight, MAIN_WIDTH, currentHeight);
            g.FillRectangle(sBrush, new Rectangle(SET_START, currentHeight, SCORE_WIDTH, 1));
            currentHeight++;*/
            lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);

            //Second Player
            g.FillRectangle(lBrush, new Rectangle(LATERAL_WIDTH, currentHeight, RECAP_WIDTH, MAIN_HEIGHT));
            //Set Space
            g.FillRectangle(sBrush, new Rectangle(RECAP_SCORE_START + LATERAL_WIDTH, currentHeight, SCORE_WIDTH, MAIN_HEIGHT));
            //Strings in place
            currentHeight += MAIN_Y_OFF;
            g.DrawString(currentMatch.Player2.ToUpper(), mainFont, textBrush, MAIN_X_OFF + LATERAL_WIDTH, currentHeight);
            if (!currentMatch.IsDouble)
            {
                currentHeight -= MAIN_Y_OFF;
                currentHeight += REGULAR_Y_OFF;
                g.DrawString(currentMatch.Player2_2, regularFont, textBrush, NAME_2_OFF + LATERAL_WIDTH, currentHeight);
                currentHeight += MAIN_Y_OFF;
                currentHeight -= REGULAR_Y_OFF;
            }
            //Set Score
            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.FinalScore2.ToString(), mainFont).Width) / 2 + LATERAL_WIDTH;
            g.DrawString(currentMatch.FinalScore2.ToString(), mainFont, auxBrush, RECAP_SCORE_START + score_x_off, currentHeight);
            //Recap
            currentWidth = RECAP_SCORE_START + SCORE_WIDTH + LATERAL_WIDTH;
            for (int i = 0; i < currentMatch.CurrentSet; i++)
            {
                if (i == currentMatch.CurrentSet && !currentMatch.GameWon)
                {
                    score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.CurrentScore2.ToString(), mainFont).Width) / 2;
                    g.DrawString(currentMatch.CurrentScore2.ToString(), mainFont, textBrush, currentWidth + score_x_off, currentHeight);
                }
                else
                {
                    if (currentMatch.Results2[i] < currentMatch.Results1[i])
                    {
                        score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.Results2[i].ToString(), mainFont).Width) / 2;
                        g.DrawString(currentMatch.Results2[i].ToString(), mainFont, loserBrush, currentWidth + score_x_off, currentHeight);
                    }
                    else
                    {
                        score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentMatch.Results2[i].ToString(), mainFont).Width) / 2;
                        g.DrawString(currentMatch.Results2[i].ToString(), mainFont, textBrush, currentWidth + score_x_off, currentHeight);
                    }
                }
                currentWidth += SCORE_WIDTH;
            }
            currentWidth = 0;
            currentHeight += MAIN_HEIGHT - MAIN_Y_OFF;
            g.Dispose();
        }
        private void pnlTeamPreview_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int currentHeight = AUX_HEIGHT;
            int score_x_off = 0;

            if (ckbTeamTag.Checked)
            {
                g.FillRectangle(sBrush, new Rectangle(TEAM_PREVIEW_START_POINT, currentHeight, TEAM_WIDTH + SCORE_WIDTH, AUX_HEIGHT));
                currentHeight += AUX_Y_OFF;
                g.DrawString(txtTeamTag.Text, auxFont, auxBrush, TEAM_PREVIEW_START_POINT + TEAM_X_OFF, currentHeight);
                currentHeight += AUX_HEIGHT - AUX_Y_OFF;
            }
            else
                currentHeight += AUX_HEIGHT;

            
            //First Team
           
            lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);
            
            g.FillRectangle(lBrush, new Rectangle(TEAM_PREVIEW_START_POINT, currentHeight, TEAM_WIDTH, MAIN_HEIGHT));
            //Set Space
            g.FillRectangle(sBrush, new Rectangle(TEAM_PREVIEW_START_POINT + TEAM_WIDTH, currentHeight, SCORE_WIDTH, MAIN_HEIGHT));
            //Strings in place
            currentHeight += MAIN_Y_OFF;
            g.DrawString(currentTeamMatch.Team1.ToUpper(), mainFont, textBrush, TEAM_PREVIEW_START_POINT + MAIN_X_OFF, currentHeight);

            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.Final1.ToString(), mainFont).Width) / 2;
            g.DrawString(currentTeamMatch.Final1.ToString(), mainFont, auxBrush, TEAM_PREVIEW_START_POINT + TEAM_WIDTH + score_x_off, currentHeight);

            currentHeight += (MAIN_HEIGHT - MAIN_Y_OFF);

            
            //Second Team

            lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);

            g.FillRectangle(lBrush, new Rectangle(TEAM_PREVIEW_START_POINT, currentHeight, TEAM_WIDTH, MAIN_HEIGHT));
            //Set Space
            g.FillRectangle(sBrush, new Rectangle(TEAM_PREVIEW_START_POINT + TEAM_WIDTH, currentHeight, SCORE_WIDTH, MAIN_HEIGHT));
            //Strings in place
            currentHeight += MAIN_Y_OFF;
            g.DrawString(currentTeamMatch.Team2.ToUpper(), mainFont, textBrush, TEAM_PREVIEW_START_POINT + MAIN_X_OFF, currentHeight);

            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.Final2.ToString(), mainFont).Width) / 2;
            g.DrawString(currentTeamMatch.Final2.ToString(), mainFont, auxBrush, TEAM_PREVIEW_START_POINT + TEAM_WIDTH + score_x_off, currentHeight);

            currentHeight += (MAIN_HEIGHT - MAIN_Y_OFF);
        }
        private void pnlTeamRecap_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int currentHeight = 0;
            int score_x_off = 0;

            //Lateral Bars
            g.FillRectangle(sBrush, new Rectangle(0, 0, LATERAL_WIDTH, LATERAL_RECAP_HEIGTH));
            g.FillRectangle(sBrush, new Rectangle(RECAP_TOTAL_WIDTH + LATERAL_WIDTH, 0, LATERAL_WIDTH, LATERAL_RECAP_HEIGTH));

            //Tag in place
            g.FillRectangle(auxBrush, new Rectangle(LATERAL_WIDTH, currentHeight, RECAP_TOTAL_WIDTH, AUX_HEIGHT));
            currentHeight += AUX_Y_OFF;
            g.DrawString(txtTeamTag.Text, auxFont, textBrush, RECAP_AUX_X_OFF + LATERAL_WIDTH, currentHeight);
            currentHeight += AUX_HEIGHT - AUX_Y_OFF;

            //Teams
            lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);
            g.FillRectangle(lBrush, new Rectangle(LATERAL_WIDTH, currentHeight, TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2, MAIN_HEIGHT));
            g.FillRectangle(sBrush, new Rectangle(TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + LATERAL_WIDTH, currentHeight, SCORE_WIDTH * 2, MAIN_HEIGHT));
            //Team Strings
            currentHeight += MAIN_Y_OFF;

            g.DrawString(currentTeamMatch.Team1.ToUpper(), mainFont, textBrush, LATERAL_WIDTH + MAIN_X_OFF, currentHeight);
            g.DrawString(currentTeamMatch.Team2.ToUpper(), mainFont, textBrush, TEAM_RECAP_WIDTH1 + LATERAL_WIDTH + MAIN_X_OFF, currentHeight);
            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.Final1.ToString(), mainFont).Width) / 2;
            g.DrawString(currentTeamMatch.Final1.ToString(), mainFont, auxBrush, TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + LATERAL_WIDTH + score_x_off, currentHeight);
            score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.Final2.ToString(), mainFont).Width) / 2;
            g.DrawString(currentTeamMatch.Final2.ToString(), mainFont, auxBrush, TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + LATERAL_WIDTH + score_x_off + SCORE_WIDTH, currentHeight);

            currentHeight += MAIN_HEIGHT - MAIN_Y_OFF;

            //Starting Recap
            for (int i = 0; i < currentTeamMatch.CurrentMatch; i++)
            {
                //Background
                lBrush = new LinearGradientBrush(new Rectangle(RECTANGLE_X, RECTANGLE_Y + currentHeight - 1, RECTANGLE_WIDTH, RECTANGLE_HEIGHT), startingColor, endColor, gradientMode);
                g.FillRectangle(lBrush, new Rectangle(LATERAL_WIDTH, currentHeight, RECAP_TOTAL_WIDTH, MAIN_HEIGHT));
                //Strings in place
                currentHeight += MAIN_Y_OFF;
                if(currentTeamMatch.MatchResults[i].FinalScore1 > currentTeamMatch.MatchResults[i].FinalScore2)
                {
                    g.DrawString(currentTeamMatch.MatchResults[i].Player1, mainFont, textBrush, LATERAL_WIDTH + MAIN_X_OFF, currentHeight);
                    score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.MatchResults[i].FinalScore1.ToString(), mainFont).Width) / 2;
                    g.DrawString(currentTeamMatch.MatchResults[i].FinalScore1.ToString(), mainFont, textBrush, LATERAL_WIDTH + TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + score_x_off, currentHeight);

                    g.DrawString(currentTeamMatch.MatchResults[i].Player2, mainFont, loserBrush, LATERAL_WIDTH + MAIN_X_OFF + TEAM_RECAP_WIDTH1, currentHeight);
                    score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.MatchResults[i].FinalScore2.ToString(), mainFont).Width) / 2;
                    g.DrawString(currentTeamMatch.MatchResults[i].FinalScore2.ToString(), mainFont, loserBrush, LATERAL_WIDTH + TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + score_x_off + SCORE_WIDTH, currentHeight);
                }
                else
                {
                    g.DrawString(currentTeamMatch.MatchResults[i].Player1, mainFont, loserBrush, LATERAL_WIDTH + MAIN_X_OFF, currentHeight);
                    score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.MatchResults[i].FinalScore1.ToString(), mainFont).Width) / 2;
                    g.DrawString(currentTeamMatch.MatchResults[i].FinalScore1.ToString(), mainFont, loserBrush, LATERAL_WIDTH + TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + score_x_off, currentHeight);

                    g.DrawString(currentTeamMatch.MatchResults[i].Player2, mainFont, textBrush, LATERAL_WIDTH + MAIN_X_OFF + TEAM_RECAP_WIDTH1, currentHeight);
                    score_x_off = (SCORE_WIDTH - (int)g.MeasureString(currentTeamMatch.MatchResults[i].FinalScore2.ToString(), mainFont).Width) / 2;
                    g.DrawString(currentTeamMatch.MatchResults[i].FinalScore2.ToString(), mainFont, textBrush, LATERAL_WIDTH + TEAM_RECAP_WIDTH1 + TEAM_RECAP_WIDTH2 + score_x_off + SCORE_WIDTH, currentHeight);
                }
                

                currentHeight += MAIN_HEIGHT - MAIN_Y_OFF;
            }

        }

        #endregion

        #region Print Graphics to File
        private void print1()
        {            
            Bitmap auxMap = new Bitmap(MAIN_WIDTH, pnlGamePreview.ClientRectangle.Height);
            pnlGamePreview.DrawToBitmap(auxMap, new Rectangle(0, 0, MAIN_WIDTH, pnlGamePreview.ClientRectangle.Height));
            auxMap.MakeTransparent(Color.FromArgb(224, 224, 224));
            auxMap.Save("GamePreview.png", ImageFormat.Png);
            auxMap.Dispose();
        }
        private void print2()
        {
            int width = 2 * LATERAL_WIDTH + RECAP_WIDTH;
            Bitmap auxMap = new Bitmap(width, LATERAL_HEIGHT);
            pnlGameRecap.DrawToBitmap(auxMap, new Rectangle(0, 0, width, LATERAL_HEIGHT));
            auxMap.Save("GameRecap.png", ImageFormat.Png);
            auxMap.Dispose();
        }
        private void print3()
        {
            int width = 2 * LATERAL_WIDTH + RECAP_TOTAL_WIDTH;
            Bitmap auxMap = new Bitmap(width, LATERAL_RECAP_HEIGTH);
            pnlTeamRecap.DrawToBitmap(auxMap, new Rectangle(0, 0, width, LATERAL_RECAP_HEIGTH));
            auxMap.Save("TeamRecap.png", ImageFormat.Png);
            auxMap.Dispose();
        }
        private void print4()
        {
            Bitmap auxMap = new Bitmap(pnlTeamPreview.ClientRectangle.Width, pnlTeamPreview.ClientRectangle.Height);
            pnlTeamPreview.DrawToBitmap(auxMap, new Rectangle(0, 0, pnlTeamPreview.ClientRectangle.Width, pnlTeamPreview.ClientRectangle.Height));
            auxMap.MakeTransparent(Color.FromArgb(224, 224, 224));
            auxMap.Save("TeamPreview.png", ImageFormat.Png);
            auxMap.Dispose();
        }

        #endregion
    }
}
