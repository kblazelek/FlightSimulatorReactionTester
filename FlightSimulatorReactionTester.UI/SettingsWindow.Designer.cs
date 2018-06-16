namespace FlightSimulatorReactionTester.UI
{
    partial class SettingsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonStart = new System.Windows.Forms.Button();
            this.richTextBoxReactionTimes = new System.Windows.Forms.RichTextBox();
            this.buttonChangeOutputDirectory = new System.Windows.Forms.Button();
            this.labelOutputDirectory = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxArrowScreen = new System.Windows.Forms.ComboBox();
            this.comboBoxSquareScreen = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxFutureEventSets = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(33, 392);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // richTextBoxReactionTimes
            // 
            this.richTextBoxReactionTimes.Location = new System.Drawing.Point(33, 194);
            this.richTextBoxReactionTimes.Name = "richTextBoxReactionTimes";
            this.richTextBoxReactionTimes.Size = new System.Drawing.Size(434, 178);
            this.richTextBoxReactionTimes.TabIndex = 1;
            this.richTextBoxReactionTimes.Text = "";
            // 
            // buttonChangeOutputDirectory
            // 
            this.buttonChangeOutputDirectory.Location = new System.Drawing.Point(33, 48);
            this.buttonChangeOutputDirectory.Name = "buttonChangeOutputDirectory";
            this.buttonChangeOutputDirectory.Size = new System.Drawing.Size(140, 23);
            this.buttonChangeOutputDirectory.TabIndex = 5;
            this.buttonChangeOutputDirectory.Text = "Change output directory";
            this.buttonChangeOutputDirectory.UseVisualStyleBackColor = true;
            this.buttonChangeOutputDirectory.Click += new System.EventHandler(this.buttonChangeOutputDirectory_Click);
            // 
            // labelOutputDirectory
            // 
            this.labelOutputDirectory.AutoSize = true;
            this.labelOutputDirectory.Location = new System.Drawing.Point(188, 53);
            this.labelOutputDirectory.Name = "labelOutputDirectory";
            this.labelOutputDirectory.Size = new System.Drawing.Size(0, 13);
            this.labelOutputDirectory.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Show arrows on screen";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Show square on screen";
            // 
            // comboBoxArrowScreen
            // 
            this.comboBoxArrowScreen.FormattingEnabled = true;
            this.comboBoxArrowScreen.Location = new System.Drawing.Point(177, 83);
            this.comboBoxArrowScreen.Name = "comboBoxArrowScreen";
            this.comboBoxArrowScreen.Size = new System.Drawing.Size(121, 21);
            this.comboBoxArrowScreen.TabIndex = 9;
            // 
            // comboBoxSquareScreen
            // 
            this.comboBoxSquareScreen.FormattingEnabled = true;
            this.comboBoxSquareScreen.Location = new System.Drawing.Point(177, 114);
            this.comboBoxSquareScreen.Name = "comboBoxSquareScreen";
            this.comboBoxSquareScreen.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSquareScreen.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Choose Future Event Set";
            // 
            // comboBoxFutureEventSets
            // 
            this.comboBoxFutureEventSets.FormattingEnabled = true;
            this.comboBoxFutureEventSets.Location = new System.Drawing.Point(177, 15);
            this.comboBoxFutureEventSets.Name = "comboBoxFutureEventSets";
            this.comboBoxFutureEventSets.Size = new System.Drawing.Size(121, 21);
            this.comboBoxFutureEventSets.TabIndex = 12;
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 511);
            this.Controls.Add(this.comboBoxFutureEventSets);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxSquareScreen);
            this.Controls.Add(this.comboBoxArrowScreen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelOutputDirectory);
            this.Controls.Add(this.buttonChangeOutputDirectory);
            this.Controls.Add(this.richTextBoxReactionTimes);
            this.Controls.Add(this.buttonStart);
            this.Name = "SettingsWindow";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.SettingsWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.RichTextBox richTextBoxReactionTimes;
        private System.Windows.Forms.Button buttonChangeOutputDirectory;
        private System.Windows.Forms.Label labelOutputDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxArrowScreen;
        private System.Windows.Forms.ComboBox comboBoxSquareScreen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxFutureEventSets;
    }
}

