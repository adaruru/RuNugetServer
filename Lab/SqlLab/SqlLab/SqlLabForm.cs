using RuLib.SqlConn;

namespace SqlLab
{
    public partial class SqlLabForm : Form
    {
        public IConn Conn { get; set; }
        public SqlLabForm(IConn conn)
        {
            Conn = conn;
            InitializeComponent();
        }

        private void ConnTestBtnClick(object sender, EventArgs e)
        {
            var err = string.Empty;
            var isSuccess = Conn.OpenConn(out err);
            if (isSuccess)
            {
                MessageBox.Show($"連線成功,Err:{err}");
            }
            else
            {
                MessageBox.Show(err);
            }
        }

        private void RandomInsertClick(object sender, EventArgs e)
        {


        }
    }
}
