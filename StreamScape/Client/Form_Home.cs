﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Client
{
    public partial class Form_Home : Form
    {
        //3 biến này sử dụng cho chức năng panelHeader
        private bool dragging = false;
        private Point dragCursor;
        private Point dragForm;

        private string[] titleofFile = { "", "", "", "", "", "" }; //biến này dùng cho chức năng hiện phim và ảnh trong form
        private string textconnect;//biến này dùng để truyền dữ liệu tên người dùng từ form hiện tại đến các form khác
        private byte[] Avatarconnect;//biến này dùng để truyền dữ liệu ảnh từ form hiện tại đến các form khác
        private string serverIP;
        public Form_Home(string _serverIP, string username, byte[] avatarconnect)
        {
            InitializeComponent();
            serverIP = _serverIP;
            LoadDataFromServer(); //lấy dữ liệu từ DB đổ về cho form home
            
            Avatarconnect = avatarconnect;
            lbUsername.Text = username; //gán dữ liệu vừa được truyền từ form signin cho label của form home
            textconnect = username; //gán dữ liệu bắc cầu form signin -> form home -> form tiếp theo
            if (avatarconnect != null && avatarconnect.Length > 0)
            {
                using (MemoryStream ms = new MemoryStream(avatarconnect))
                {
                    btAvatar.Image = Image.FromStream(ms); //load avatar vừa được truyền lên giao diện
                }
            }
            
            this.pnHeader.MouseDown += new MouseEventHandler(panelHeader_MouseDown);
            this.pnHeader.MouseMove += new MouseEventHandler(panelHeader_MouseMove);
            this.pnHeader.MouseUp += new MouseEventHandler(panelHeader_MouseUp);
        }

        #region Chức năng load data từ DB của server về cho form home và hiển thị trên form home...
        List<MovieNMusic> movieandmusic;
        public void LoadDataFromServer()
        {
            try
            {
                using (TcpClient client = new TcpClient(serverIP, 5000)) // Sửa địa chỉ IP và port nếu cần
                using (NetworkStream stream = client.GetStream())
                using (BinaryWriter writer = new BinaryWriter(stream))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    writer.Write("getdata");
                    // Đọc dữ liệu từ server
                    movieandmusic = ReceiveMoviesFromServer(stream);

                    // Phân loại dữ liệu và hiển thị trên client form
                    DisplayData(movieandmusic);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to server: " + ex.Message);
            }
        }

        private List<MovieNMusic> ReceiveMoviesFromServer(NetworkStream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                return DeserializeMovieNMusic(ms.ToArray());
            }
        }

        public List<MovieNMusic> DeserializeMovieNMusic(byte[] data)
        {
            List<MovieNMusic> movieandmusic = new List<MovieNMusic>();
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        MovieNMusic mandm = new MovieNMusic
                        {
                            Title = reader.ReadString(),
                            Description = reader.ReadString(),
                            Tag = reader.ReadString(),
                            Poster = reader.ReadBytes(reader.ReadInt32())
                        };
                        movieandmusic.Add(mandm);
                    }
                }
            }
            return movieandmusic;
        }

        List<MovieNMusic> movieList = new List<MovieNMusic>();
        List<MovieNMusic> musicList = new List<MovieNMusic>();
        private void DisplayData(List<MovieNMusic> movies)
        {
            // Phân loại dữ liệu
            foreach (var item in movies)
            {
                if (item.Tag == "Movie")
                    movieList.Add(item);
                else if (item.Tag == "Music")
                    musicList.Add(item);
            }

            // Hiển thị danh sách phim và nhạc
            DisplayMovies(movieList);
            DisplayMusics(musicList);
        }

        int somemovies = 0;
        private void DisplayMovies(List<MovieNMusic> movies)
        {
            int seq_movie = 0;
            foreach (var movie in movies.Skip(somemovies))
            {
                titleofFile[seq_movie] = movie.Title;
                seq_movie++;
                // Tìm Label cho Title
                Label match_movieTitle = this.Controls.Find("lbTitleMV" + seq_movie, true).FirstOrDefault() as Label;
                Label match_movieDes = this.Controls.Find("lbDesMV" + seq_movie, true).FirstOrDefault() as Label;
                PictureBox match_moviePoster = this.Controls.Find("pbMV" + seq_movie, true).FirstOrDefault() as PictureBox;
                match_movieTitle.Text = movie.Title;
                match_movieDes.Text = movie.Description;
                // Chuyển đổi byte array thành hình ảnh cho PictureBox
                using (MemoryStream ms = new MemoryStream(movie.Poster))
                {
                    match_moviePoster.Image = Image.FromStream(ms);
                }
                match_movieTitle.Visible = true;
                match_movieDes.Visible = true;
                match_moviePoster.Visible = true;
                if (seq_movie == 2) break;
            }
        }

        int somemusics = 0;
        private void DisplayMusics(List<MovieNMusic> musics)
        {
            int seq_music = 0;

            foreach (var music in musics.Skip(somemusics))
            {
                titleofFile[seq_music + 2] = music.Title;
                seq_music++;

                // Tìm Label cho Title
                Label match_musicTitle = this.Controls.Find("lbTitleS" + seq_music, true).FirstOrDefault() as Label;
                Label match_musicDes = this.Controls.Find("lbDesS" + seq_music, true).FirstOrDefault() as Label;
                PictureBox match_musicPoster = this.Controls.Find("pbS" + seq_music, true).FirstOrDefault() as PictureBox;
                match_musicTitle.Text = music.Title;
                match_musicDes.Text = music.Description;
                // Chuyển đổi byte array thành hình ảnh cho PictureBox
                using (MemoryStream ms = new MemoryStream(music.Poster))
                {
                    match_musicPoster.Image = Image.FromStream(ms);
                }
                match_musicTitle.Visible = true;
                match_musicDes.Visible = true;
                match_musicPoster.Visible = true;
                if (seq_music == 4) break;
            }
        }

        #endregion...

        private void Form_Home_Load(object sender, EventArgs e)
        {
            //tạo background trong suốt
            btExit.Parent = pbBackgroundHome;
            btMaximized.Parent = pbBackgroundHome;
            btMinimized.Parent = pbBackgroundHome;
            pnHeader.Parent = pbBackgroundHome;
            btSetting.Parent = pbBackgroundHome;
            lbTitleMV1.Parent = pbBackgroundHome;
            lbDesMV1.Parent = pbBackgroundHome;
            lbTitleMV2.Parent = pbBackgroundHome;
            lbDesMV2.Parent = pbBackgroundHome;
            lbTitleS1.Parent = pbBackgroundHome;
            lbDesS1.Parent = pbBackgroundHome;
            lbTitleS2.Parent = pbBackgroundHome;
            lbDesS2.Parent = pbBackgroundHome;
            lbTitleS3.Parent = pbBackgroundHome;
            lbDesS3.Parent = pbBackgroundHome;
            lbTitleS4.Parent = pbBackgroundHome;
            lbDesS4.Parent = pbBackgroundHome;
        }

        #region Chức năng có thể di chuyển cửa sổ...
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
        #endregion

        #region 3 button công cụ...
        private void btExit_Click(object sender, EventArgs e)
        {
            var formsToClose = Application.OpenForms.Cast<Form>().ToList();
            foreach (var form in formsToClose)
            {
                form.Close();
            }
        }

        //Chức năng thu nhỏ cửa sổ xuống tab
        private void btMinimized_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        #endregion

        #region Chức năng mở các form liên kết...
        //Mở form setting và đóng form hiện tại
        private void btSetting_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_Setting formSetting = new Form_Setting(serverIP, textconnect, Avatarconnect);//truyền cho form setting username và avatar
            formSetting.Show();
            formSetting.Location = new Point(this.Location.X, this.Location.Y);
        }

        //Mở form profile và đóng form hiện tại
        private void btProfile_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_Profile formProfile = new Form_Profile(serverIP, textconnect, Avatarconnect);//truyền cho form profile username và avatar
            formProfile.Show();
            formProfile.Location = new Point(this.Location.X, this.Location.Y);
        }

        //Mở form create và đóng form hiện tại
        private void btCreate_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_Create formCreate = new Form_Create(serverIP, textconnect, Avatarconnect);//truyền cho form create username và avatar
            formCreate.Show();
            formCreate.Location = new Point(this.Location.X, this.Location.Y);
        }

        //Mở form join và đóng form hiện tại
        private void btJoin_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_Join formJoin = new Form_Join(serverIP, textconnect, Avatarconnect);//truyền cho form join username và avatar
            formJoin.Show();
            formJoin.Location = new Point(this.Location.X, this.Location.Y);
        }
        #endregion

        #region Chức năng cho phép chuyển video và chuyển nhạc...
        private void btBackofVideo_Click(object sender, EventArgs e)
        {
            somemovies -= 2;
            if (somemovies >= 0)
                DisplayMovies(movieList);
            else somemovies += 2;
        }

        private void btNextofVideo_Click(object sender, EventArgs e)
        {
            if (somemovies + 2 < movieList.Count)
            {
                somemovies += 2;
                pbMV1.Visible = false;
                lbTitleMV1.Visible = false;
                lbDesMV1.Visible = false;
                pbMV2.Visible = false;
                lbTitleMV2.Visible = false;
                lbDesMV2.Visible = false;
                DisplayMovies(movieList);
            }
        }

        private void btBackofSong_Click(object sender, EventArgs e)
        {
            somemusics -= 4;
            if (somemusics >= 0)
                DisplayMusics(musicList);
            else somemusics += 4;
        }

        private void btNextofSong_Click(object sender, EventArgs e)
        {
            if (somemusics + 4 < musicList.Count)
            {
                somemusics += 4;
                pbS1.Visible = false;
                lbTitleS1.Visible = false;
                lbDesS1.Visible = false;
                pbS2.Visible = false;
                lbTitleS2.Visible = false;
                lbDesS2.Visible = false;
                pbS3.Visible = false;
                lbTitleS3.Visible = false;
                lbDesS3.Visible = false;
                pbS4.Visible = false;
                lbTitleS4.Visible = false;
                lbDesS4.Visible = false;
                DisplayMusics(musicList);
            }
        }
        #endregion

        #region Chức năng mở video và nhạc tương ứng...
        private void pbMV1_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_room formRoom = new Form_room(serverIP, textconnect, Avatarconnect, titleofFile[0]);
            formRoom.Show();
            formRoom.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void pbMV2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_room formRoom = new Form_room(serverIP, textconnect, Avatarconnect, titleofFile[1]);
            formRoom.Show();
            formRoom.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void pbS1_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_room formRoom = new Form_room(serverIP, textconnect, Avatarconnect, titleofFile[2]);
            formRoom.Show();
            formRoom.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void pbS2_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_room formRoom = new Form_room(serverIP, textconnect, Avatarconnect, titleofFile[3]);
            formRoom.Show();
            formRoom.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void pbS3_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_room formRoom = new Form_room(serverIP, textconnect, Avatarconnect, titleofFile[4]);
            formRoom.Show();
            formRoom.Location = new Point(this.Location.X, this.Location.Y);
        }

        private void pbS4_Click(object sender, EventArgs e)
        {
            this.Close();
            Form_room formRoom = new Form_room(serverIP, textconnect, Avatarconnect, titleofFile[5]);
            formRoom.Show();
            formRoom.Location = new Point(this.Location.X, this.Location.Y);
        }
        #endregion

        #region Công cụ tìm kiếm...
        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            searchResult.Visible = true;
            // Kiểm tra từ khóa tìm kiếm có trống hay không
            if (string.IsNullOrEmpty(tbSearch.Text))
            {
                return;
            }
            else
            {
                // Lọc danh sách dựa trên từ khóa tìm kiếm (không phân biệt chữ hoa chữ thường)
                var filteredList = movieandmusic
                    .Where(m => m.Title.IndexOf(tbSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(m => new { Title = m.Title })
                    .ToList();

                // Cập nhật DataSource của DataGridView với danh sách đã lọc
                searchResult.DataSource = filteredList;
            }
        }

        private void searchResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra xem chỉ số hàng có hợp lệ không
            if (e.RowIndex >= 0)
            {
                // Lấy hàng được chọn
                DataGridViewRow selectedRow = searchResult.Rows[e.RowIndex];

                // Lấy giá trị của cột "Title" từ hàng được chọn
                string title = selectedRow.Cells["Title"].Value.ToString();
                this.Close();
                Form_room formRoom = new Form_room(serverIP, textconnect, Avatarconnect, title);
                formRoom.Show();
                formRoom.Location = new Point(this.Location.X, this.Location.Y);

            }
        }

        private void pbBackgroundHome_MouseEnter(object sender, EventArgs e)
        {
            searchResult.Visible = false;
        }

        private void tbSearch_MouseEnter(object sender, EventArgs e)
        {
            searchResult.Visible = true;
        }
        #endregion

        public class MovieNMusic
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Tag { get; set; }
            public byte[] Poster { get; set; }
        }
    }
}
