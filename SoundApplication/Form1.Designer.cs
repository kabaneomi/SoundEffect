namespace SoundApplication
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.MediaOpen = new System.Windows.Forms.Button();
            this.MediaPlay = new System.Windows.Forms.Button();
            this.MediaStop = new System.Windows.Forms.Button();
            this.MediaRepeat = new System.Windows.Forms.CheckBox();
            this.Gensui = new System.Windows.Forms.TextBox();
            this.LateTime = new System.Windows.Forms.TextBox();
            this.Repeat = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Reverve = new System.Windows.Forms.Button();
            this.PlayReverve = new System.Windows.Forms.Button();
            this.wavewrite = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MediaOpen
            // 
            this.MediaOpen.Location = new System.Drawing.Point(64, 26);
            this.MediaOpen.Name = "MediaOpen";
            this.MediaOpen.Size = new System.Drawing.Size(152, 74);
            this.MediaOpen.TabIndex = 3;
            this.MediaOpen.Text = "ファイル読み込み";
            this.MediaOpen.UseVisualStyleBackColor = true;
            this.MediaOpen.Click += new System.EventHandler(this.MediaOpen_Click);
            // 
            // MediaPlay
            // 
            this.MediaPlay.Location = new System.Drawing.Point(64, 139);
            this.MediaPlay.Name = "MediaPlay";
            this.MediaPlay.Size = new System.Drawing.Size(152, 76);
            this.MediaPlay.TabIndex = 4;
            this.MediaPlay.Text = "再生";
            this.MediaPlay.UseVisualStyleBackColor = true;
            this.MediaPlay.Click += new System.EventHandler(this.MediaPlay_Click);
            // 
            // MediaStop
            // 
            this.MediaStop.Location = new System.Drawing.Point(64, 257);
            this.MediaStop.Name = "MediaStop";
            this.MediaStop.Size = new System.Drawing.Size(152, 70);
            this.MediaStop.TabIndex = 5;
            this.MediaStop.Text = "停止";
            this.MediaStop.UseVisualStyleBackColor = true;
            this.MediaStop.Click += new System.EventHandler(this.MediaStop_Click);
            // 
            // MediaRepeat
            // 
            this.MediaRepeat.AutoSize = true;
            this.MediaRepeat.Location = new System.Drawing.Point(64, 370);
            this.MediaRepeat.Name = "MediaRepeat";
            this.MediaRepeat.Size = new System.Drawing.Size(72, 19);
            this.MediaRepeat.TabIndex = 6;
            this.MediaRepeat.Text = "リピート";
            this.MediaRepeat.UseVisualStyleBackColor = true;
            this.MediaRepeat.CheckedChanged += new System.EventHandler(this.MediaRepeat_CheckedChanged);
            // 
            // Gensui
            // 
            this.Gensui.Location = new System.Drawing.Point(306, 39);
            this.Gensui.Name = "Gensui";
            this.Gensui.Size = new System.Drawing.Size(100, 22);
            this.Gensui.TabIndex = 7;
            this.Gensui.TextChanged += new System.EventHandler(this.Gensui_TextChanged);
            // 
            // LateTime
            // 
            this.LateTime.Location = new System.Drawing.Point(306, 113);
            this.LateTime.Name = "LateTime";
            this.LateTime.Size = new System.Drawing.Size(100, 22);
            this.LateTime.TabIndex = 8;
            this.LateTime.TextChanged += new System.EventHandler(this.LateTime_TextChanged);
            // 
            // Repeat
            // 
            this.Repeat.Location = new System.Drawing.Point(306, 192);
            this.Repeat.Name = "Repeat";
            this.Repeat.Size = new System.Drawing.Size(100, 22);
            this.Repeat.TabIndex = 9;
            this.Repeat.TextChanged += new System.EventHandler(this.Repeat_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(303, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "減衰率";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(306, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "遅延時間";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(306, 171);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 12;
            this.label3.Text = "繰り返し回数";
            // 
            // Reverve
            // 
            this.Reverve.Location = new System.Drawing.Point(306, 247);
            this.Reverve.Name = "Reverve";
            this.Reverve.Size = new System.Drawing.Size(100, 23);
            this.Reverve.TabIndex = 13;
            this.Reverve.Text = "リバーブ";
            this.Reverve.UseVisualStyleBackColor = true;
            this.Reverve.Click += new System.EventHandler(this.Reverve_Click);
            // 
            // PlayReverve
            // 
            this.PlayReverve.Location = new System.Drawing.Point(309, 303);
            this.PlayReverve.Name = "PlayReverve";
            this.PlayReverve.Size = new System.Drawing.Size(97, 23);
            this.PlayReverve.TabIndex = 14;
            this.PlayReverve.Text = "リバーブ再生";
            this.PlayReverve.UseVisualStyleBackColor = true;
            this.PlayReverve.Click += new System.EventHandler(this.PlayReverve_Click);
            // 
            // wavewrite
            // 
            this.wavewrite.Location = new System.Drawing.Point(309, 357);
            this.wavewrite.Name = "wavewrite";
            this.wavewrite.Size = new System.Drawing.Size(97, 23);
            this.wavewrite.TabIndex = 15;
            this.wavewrite.Text = "test";
            this.wavewrite.UseVisualStyleBackColor = true;
            this.wavewrite.Click += new System.EventHandler(this.wavewrite_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 450);
            this.Controls.Add(this.wavewrite);
            this.Controls.Add(this.PlayReverve);
            this.Controls.Add(this.Reverve);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Repeat);
            this.Controls.Add(this.LateTime);
            this.Controls.Add(this.Gensui);
            this.Controls.Add(this.MediaRepeat);
            this.Controls.Add(this.MediaStop);
            this.Controls.Add(this.MediaPlay);
            this.Controls.Add(this.MediaOpen);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button MediaOpen;
        private System.Windows.Forms.Button MediaPlay;
        private System.Windows.Forms.Button MediaStop;
        private System.Windows.Forms.CheckBox MediaRepeat;
        private System.Windows.Forms.TextBox Gensui;
        private System.Windows.Forms.TextBox LateTime;
        private System.Windows.Forms.TextBox Repeat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Reverve;
        private System.Windows.Forms.Button PlayReverve;
        private System.Windows.Forms.Button wavewrite;
    }
}

