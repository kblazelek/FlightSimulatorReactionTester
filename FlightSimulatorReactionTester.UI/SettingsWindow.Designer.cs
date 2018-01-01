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
            this.buttonLoadFutureEventSet = new System.Windows.Forms.Button();
            this.labelLoadedFutureEventSet = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(33, 253);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // richTextBoxReactionTimes
            // 
            this.richTextBoxReactionTimes.Location = new System.Drawing.Point(33, 55);
            this.richTextBoxReactionTimes.Name = "richTextBoxReactionTimes";
            this.richTextBoxReactionTimes.Size = new System.Drawing.Size(434, 178);
            this.richTextBoxReactionTimes.TabIndex = 1;
            this.richTextBoxReactionTimes.Text = "";
            // 
            // buttonLoadFutureEventSet
            // 
            this.buttonLoadFutureEventSet.Location = new System.Drawing.Point(33, 13);
            this.buttonLoadFutureEventSet.Name = "buttonLoadFutureEventSet";
            this.buttonLoadFutureEventSet.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadFutureEventSet.TabIndex = 3;
            this.buttonLoadFutureEventSet.Text = "Load FES";
            this.buttonLoadFutureEventSet.UseVisualStyleBackColor = true;
            this.buttonLoadFutureEventSet.Click += new System.EventHandler(this.buttonLoadFutureEventSet_Click);
            // 
            // labelLoadedFutureEventSet
            // 
            this.labelLoadedFutureEventSet.AutoSize = true;
            this.labelLoadedFutureEventSet.Location = new System.Drawing.Point(123, 18);
            this.labelLoadedFutureEventSet.Name = "labelLoadedFutureEventSet";
            this.labelLoadedFutureEventSet.Size = new System.Drawing.Size(0, 13);
            this.labelLoadedFutureEventSet.TabIndex = 4;
            // 
            // ClientWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 343);
            this.Controls.Add(this.labelLoadedFutureEventSet);
            this.Controls.Add(this.buttonLoadFutureEventSet);
            this.Controls.Add(this.richTextBoxReactionTimes);
            this.Controls.Add(this.buttonStart);
            this.Name = "ClientWindow";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.ClientWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.RichTextBox richTextBoxReactionTimes;
        private System.Windows.Forms.Button buttonLoadFutureEventSet;
        private System.Windows.Forms.Label labelLoadedFutureEventSet;
    }
}

