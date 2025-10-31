namespace SystemMonitoring
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView lvLogs;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colMessage;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lvLogs = new System.Windows.Forms.ListView();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.colMessage = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // lvLogs
            // 
            this.lvLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colType,
            this.colMessage});
            this.lvLogs.FullRowSelect = true;
            this.lvLogs.GridLines = true;
            this.lvLogs.HideSelection = false;
            this.lvLogs.Location = new System.Drawing.Point(0, 0);
            this.lvLogs.Name = "lvLogs";
            this.lvLogs.Size = new System.Drawing.Size(800, 500);
            this.lvLogs.TabIndex = 0;
            this.lvLogs.UseCompatibleStateImageBehavior = false;
            this.lvLogs.View = System.Windows.Forms.View.Details;
            this.lvLogs.Font = new System.Drawing.Font("Segoe UI", 10);
            this.lvLogs.OwnerDraw = true;
            this.lvLogs.DrawItem += LvLogs_DrawItem;
            this.lvLogs.DrawSubItem += LvLogs_DrawSubItem;
            this.lvLogs.DrawColumnHeader += LvLogs_DrawColumnHeader;
            // 
            // colType
            // 
            this.colType.Text = "Type";
            this.colType.Width = 200;
            // 
            // colMessage
            // 
            this.colMessage.Text = "Message";
            this.colMessage.Width = 580;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.lvLogs);
            this.Name = "Form1";
            this.Text = "System Monitoring Logs";
            this.ResumeLayout(false);
        }

        #endregion

        // Custom drawing for ListView
        private void LvLogs_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.DarkBlue, e.Bounds);
            e.Graphics.DrawString(e.Header.Text, new Font("Segoe UI", 10, FontStyle.Bold), Brushes.White, e.Bounds);
        }

        private void LvLogs_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void LvLogs_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Color bgColor = e.ItemIndex % 2 == 0 ? Color.LightGray : Color.White;
            e.Graphics.FillRectangle(new SolidBrush(bgColor), e.Bounds);
            e.Graphics.DrawString(e.SubItem.Text, e.Item.Font, Brushes.Black, e.Bounds);
        }

        // Helper to add logs with color coding
        private void AddLog(string type, string message)
        {
            var item = new ListViewItem(type);
            item.SubItems.Add(message);
            // Optional: set color based on type
            if (type.Contains("not found"))
                item.ForeColor = Color.Red;
            else if (type.Contains("Connected"))
                item.ForeColor = Color.Green;
            else
                item.ForeColor = Color.Black;

            lvLogs.Items.Add(item);
        }
    }
}
