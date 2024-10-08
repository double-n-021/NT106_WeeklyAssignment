﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form_Home : Form
    {
        private bool dragging = false;
        private Point dragCursor;
        private Point dragForm;
        public Form_Home()
        {
            InitializeComponent();
            this.pnHeader.MouseDown += new MouseEventHandler(panelHeader_MouseDown);
            this.pnHeader.MouseMove += new MouseEventHandler(panelHeader_MouseMove);
            this.pnHeader.MouseUp += new MouseEventHandler(panelHeader_MouseUp);
        }

        private void Form_Home_Load(object sender, EventArgs e)
        {
            btExit.Parent = pbBackgroundHome;
            btMaximized.Parent = pbBackgroundHome;
            btMinimized.Parent = pbBackgroundHome;
            pnHeader.Parent = pbBackgroundHome;
            btSetting.Parent = pbBackgroundHome;
        }

        private void btExit_Click(object sender, EventArgs e)
        {
            var formsToClose = Application.OpenForms.Cast<Form>().ToList();
            foreach (var form in formsToClose)
            {
                form.Close();
            }
        }

        private void btMinimized_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void btSetting_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form_Setting formSetting = new Form_Setting();
            formSetting.Show();
            formSetting.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void panelHeader_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursor = Cursor.Position;
            dragForm = this.Location;
        }

        private void panelHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point delta = new Point(Cursor.Position.X - dragCursor.X, Cursor.Position.Y - dragCursor.Y);
                this.Location = new Point(dragForm.X + delta.X, dragForm.Y + delta.Y);
            }
        }

        private void panelHeader_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void btProfile_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form_Profile formProfile = new Form_Profile();
            formProfile.Show();
            formProfile.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form_Create formCreate = new Form_Create();
            formCreate.Show();
            formCreate.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void btJoin_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form_Join formJoin = new Form_Join();
            formJoin.Show();
            formJoin.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void btBackofVideo_Click(object sender, EventArgs e)
        {
            pbBackgroundNextMV.Visible = false;
        }

        private void btNextofVideo_Click(object sender, EventArgs e)
        {
            pbBackgroundNextMV.Visible = !pbBackgroundNextMV.Visible;
        }

        private void btBackofSong_Click(object sender, EventArgs e)
        {
            pbBackgroundNS.Visible = false;
        }

        private void btNextofSong_Click(object sender, EventArgs e)
        {
            pbBackgroundNS.Visible = !pbBackgroundNS.Visible;
        }

        private void btFirstMovie_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form_Offlineroom formOfflineroom = new Form_Offlineroom();
            formOfflineroom.Show();
            formOfflineroom.Location = new Point(this.Location.X, this.Location.Y);
        }
    }
}
