namespace EasyPattern
{
    partial class Form1
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
            this.choiceMeasuresSet = new System.Windows.Forms.ComboBox();
            this.header = new System.Windows.Forms.Label();
            this.welcomeText = new System.Windows.Forms.Label();
            this.updateMeasures = new System.Windows.Forms.RadioButton();
            this.noUpdate = new System.Windows.Forms.RadioButton();
            this.welcome = new System.Windows.Forms.Panel();
            this.next1 = new System.Windows.Forms.Button();
            this.measures = new System.Windows.Forms.Panel();
            this.height = new System.Windows.Forms.NumericUpDown();
            this.nameAndNoteMeasures = new System.Windows.Forms.Panel();
            this.noteDataSet = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.nameOfDataSet = new System.Windows.Forms.Label();
            this.labelNewMeasures = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.next2 = new System.Windows.Forms.Button();
            this.toWlecome = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.circ_bust = new System.Windows.Forms.NumericUpDown();
            this.circ_waist = new System.Windows.Forms.NumericUpDown();
            this.circ_hips = new System.Windows.Forms.NumericUpDown();
            this.len_back = new System.Windows.Forms.NumericUpDown();
            this.wid_back = new System.Windows.Forms.NumericUpDown();
            this.len_shoulder = new System.Windows.Forms.NumericUpDown();
            this.len_sleeve = new System.Windows.Forms.NumericUpDown();
            this.len_front = new System.Windows.Forms.NumericUpDown();
            this.len_breast = new System.Windows.Forms.NumericUpDown();
            this.circ_neck = new System.Windows.Forms.NumericUpDown();
            this.circ_sleeve = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.len_knee = new System.Windows.Forms.NumericUpDown();
            this.welcome.SuspendLayout();
            this.measures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.height)).BeginInit();
            this.nameAndNoteMeasures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.circ_bust)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_waist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_hips)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_back)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wid_back)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_shoulder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_sleeve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_front)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_breast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_neck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_sleeve)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_knee)).BeginInit();
            this.SuspendLayout();
            // 
            // choiceMeasuresSet
            // 
            this.choiceMeasuresSet.FormattingEnabled = true;
            this.choiceMeasuresSet.Location = new System.Drawing.Point(406, 193);
            this.choiceMeasuresSet.Name = "choiceMeasuresSet";
            this.choiceMeasuresSet.Size = new System.Drawing.Size(121, 21);
            this.choiceMeasuresSet.TabIndex = 2;
            // 
            // header
            // 
            this.header.AutoSize = true;
            this.header.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.header.Location = new System.Drawing.Point(341, 9);
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(275, 55);
            this.header.TabIndex = 0;
            this.header.Text = "Střihovátko";
            // 
            // welcomeText
            // 
            this.welcomeText.AutoSize = true;
            this.welcomeText.Location = new System.Drawing.Point(231, 164);
            this.welcomeText.Name = "welcomeText";
            this.welcomeText.Size = new System.Drawing.Size(506, 13);
            this.welcomeText.TabIndex = 1;
            this.welcomeText.Text = "Jako první je třeba zadat míry. Vyberte uložené míry a upravte si je podle sebe n" +
    "ebo zadejte vlastní sadu.";
            // 
            // updateMeasures
            // 
            this.updateMeasures.AutoSize = true;
            this.updateMeasures.Checked = true;
            this.updateMeasures.Location = new System.Drawing.Point(406, 220);
            this.updateMeasures.Name = "updateMeasures";
            this.updateMeasures.Size = new System.Drawing.Size(128, 17);
            this.updateMeasures.TabIndex = 4;
            this.updateMeasures.TabStop = true;
            this.updateMeasures.Text = "chci míry ještě upravit";
            this.updateMeasures.UseVisualStyleBackColor = true;
            // 
            // noUpdate
            // 
            this.noUpdate.AutoSize = true;
            this.noUpdate.Location = new System.Drawing.Point(406, 243);
            this.noUpdate.Name = "noUpdate";
            this.noUpdate.Size = new System.Drawing.Size(159, 17);
            this.noUpdate.TabIndex = 5;
            this.noUpdate.TabStop = true;
            this.noUpdate.Text = "přejít rovnou k výběru střihu";
            this.noUpdate.UseVisualStyleBackColor = true;
            // 
            // welcome
            // 
            this.welcome.Controls.Add(this.next1);
            this.welcome.Controls.Add(this.noUpdate);
            this.welcome.Controls.Add(this.updateMeasures);
            this.welcome.Controls.Add(this.choiceMeasuresSet);
            this.welcome.Controls.Add(this.welcomeText);
            this.welcome.Location = new System.Drawing.Point(12, 78);
            this.welcome.Name = "welcome";
            this.welcome.Size = new System.Drawing.Size(963, 530);
            this.welcome.TabIndex = 6;
            // 
            // next1
            // 
            this.next1.Location = new System.Drawing.Point(885, 504);
            this.next1.Name = "next1";
            this.next1.Size = new System.Drawing.Size(75, 23);
            this.next1.TabIndex = 6;
            this.next1.Text = "Další";
            this.next1.UseVisualStyleBackColor = true;
            this.next1.Click += new System.EventHandler(this.next1_Click);
            // 
            // measures
            // 
            this.measures.Controls.Add(this.len_knee);
            this.measures.Controls.Add(this.label13);
            this.measures.Controls.Add(this.circ_sleeve);
            this.measures.Controls.Add(this.circ_neck);
            this.measures.Controls.Add(this.len_breast);
            this.measures.Controls.Add(this.len_front);
            this.measures.Controls.Add(this.len_sleeve);
            this.measures.Controls.Add(this.len_shoulder);
            this.measures.Controls.Add(this.wid_back);
            this.measures.Controls.Add(this.len_back);
            this.measures.Controls.Add(this.circ_hips);
            this.measures.Controls.Add(this.circ_waist);
            this.measures.Controls.Add(this.circ_bust);
            this.measures.Controls.Add(this.height);
            this.measures.Controls.Add(this.nameAndNoteMeasures);
            this.measures.Controls.Add(this.labelNewMeasures);
            this.measures.Controls.Add(this.save);
            this.measures.Controls.Add(this.next2);
            this.measures.Controls.Add(this.toWlecome);
            this.measures.Controls.Add(this.label12);
            this.measures.Controls.Add(this.label11);
            this.measures.Controls.Add(this.label10);
            this.measures.Controls.Add(this.label9);
            this.measures.Controls.Add(this.label8);
            this.measures.Controls.Add(this.label7);
            this.measures.Controls.Add(this.label6);
            this.measures.Controls.Add(this.label5);
            this.measures.Controls.Add(this.label4);
            this.measures.Controls.Add(this.label3);
            this.measures.Controls.Add(this.label2);
            this.measures.Controls.Add(this.label1);
            this.measures.Location = new System.Drawing.Point(12, 78);
            this.measures.Name = "measures";
            this.measures.Size = new System.Drawing.Size(963, 530);
            this.measures.TabIndex = 7;
            // 
            // height
            // 
            this.height.Location = new System.Drawing.Point(187, 158);
            this.height.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.height.Name = "height";
            this.height.Size = new System.Drawing.Size(100, 20);
            this.height.TabIndex = 1;
            // 
            // nameAndNoteMeasures
            // 
            this.nameAndNoteMeasures.Controls.Add(this.noteDataSet);
            this.nameAndNoteMeasures.Controls.Add(this.label14);
            this.nameAndNoteMeasures.Controls.Add(this.nameOfDataSet);
            this.nameAndNoteMeasures.Location = new System.Drawing.Point(30, 53);
            this.nameAndNoteMeasures.Name = "nameAndNoteMeasures";
            this.nameAndNoteMeasures.Size = new System.Drawing.Size(362, 99);
            this.nameAndNoteMeasures.TabIndex = 30;
            this.nameAndNoteMeasures.Visible = false;
            // 
            // noteDataSet
            // 
            this.noteDataSet.AutoSize = true;
            this.noteDataSet.Location = new System.Drawing.Point(16, 28);
            this.noteDataSet.Name = "noteDataSet";
            this.noteDataSet.Size = new System.Drawing.Size(38, 13);
            this.noteDataSet.TabIndex = 29;
            this.noteDataSet.Text = "\"note\"";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(35, 77);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(285, 13);
            this.label14.TabIndex = 28;
            this.label14.Text = "Nyní můžete míry upravit, hodnoty zadávejte v milimetrech:";
            // 
            // nameOfDataSet
            // 
            this.nameOfDataSet.AutoSize = true;
            this.nameOfDataSet.Location = new System.Drawing.Point(16, 0);
            this.nameOfDataSet.Name = "nameOfDataSet";
            this.nameOfDataSet.Size = new System.Drawing.Size(45, 13);
            this.nameOfDataSet.TabIndex = 27;
            this.nameOfDataSet.Text = "\"Name\"";
            // 
            // labelNewMeasures
            // 
            this.labelNewMeasures.AutoSize = true;
            this.labelNewMeasures.Location = new System.Drawing.Point(105, 131);
            this.labelNewMeasures.Name = "labelNewMeasures";
            this.labelNewMeasures.Size = new System.Drawing.Size(153, 13);
            this.labelNewMeasures.TabIndex = 29;
            this.labelNewMeasures.Text = "Zadejte své míry v milimetrech:";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(807, 507);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 26;
            this.save.Text = "Uložit míry";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // next2
            // 
            this.next2.Location = new System.Drawing.Point(888, 507);
            this.next2.Name = "next2";
            this.next2.Size = new System.Drawing.Size(75, 23);
            this.next2.TabIndex = 25;
            this.next2.Text = "Vybrat střih";
            this.next2.UseVisualStyleBackColor = true;
            // 
            // toWlecome
            // 
            this.toWlecome.Location = new System.Drawing.Point(0, 507);
            this.toWlecome.Name = "toWlecome";
            this.toWlecome.Size = new System.Drawing.Size(75, 23);
            this.toWlecome.TabIndex = 24;
            this.toWlecome.Text = "Zpět";
            this.toWlecome.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(84, 451);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(97, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "Dolní šířka rukávu";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(118, 425);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Obvod krku";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(105, 399);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Hloubka prsou";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(112, 373);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Přední délka";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(110, 347);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Délka rukávu";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(109, 321);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Šířka ramene";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(127, 295);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Šířka zad";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(126, 269);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Délka zad";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(116, 243);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Obvod sedu";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(116, 217);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Obvod pasu";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 191);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Obvod hrudníku";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Výška postavy";
            // 
            // circ_bust
            // 
            this.circ_bust.Location = new System.Drawing.Point(187, 184);
            this.circ_bust.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.circ_bust.Name = "circ_bust";
            this.circ_bust.Size = new System.Drawing.Size(100, 20);
            this.circ_bust.TabIndex = 2;
            // 
            // circ_waist
            // 
            this.circ_waist.Location = new System.Drawing.Point(187, 210);
            this.circ_waist.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.circ_waist.Name = "circ_waist";
            this.circ_waist.Size = new System.Drawing.Size(100, 20);
            this.circ_waist.TabIndex = 3;
            // 
            // circ_hips
            // 
            this.circ_hips.Location = new System.Drawing.Point(187, 236);
            this.circ_hips.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.circ_hips.Name = "circ_hips";
            this.circ_hips.Size = new System.Drawing.Size(100, 20);
            this.circ_hips.TabIndex = 4;
            // 
            // len_back
            // 
            this.len_back.Location = new System.Drawing.Point(187, 262);
            this.len_back.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.len_back.Name = "len_back";
            this.len_back.Size = new System.Drawing.Size(100, 20);
            this.len_back.TabIndex = 5;
            // 
            // wid_back
            // 
            this.wid_back.Location = new System.Drawing.Point(187, 288);
            this.wid_back.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.wid_back.Name = "wid_back";
            this.wid_back.Size = new System.Drawing.Size(100, 20);
            this.wid_back.TabIndex = 6;
            // 
            // len_shoulder
            // 
            this.len_shoulder.Location = new System.Drawing.Point(187, 314);
            this.len_shoulder.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.len_shoulder.Name = "len_shoulder";
            this.len_shoulder.Size = new System.Drawing.Size(100, 20);
            this.len_shoulder.TabIndex = 7;
            // 
            // len_sleeve
            // 
            this.len_sleeve.Location = new System.Drawing.Point(187, 340);
            this.len_sleeve.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.len_sleeve.Name = "len_sleeve";
            this.len_sleeve.Size = new System.Drawing.Size(100, 20);
            this.len_sleeve.TabIndex = 8;
            // 
            // len_front
            // 
            this.len_front.Location = new System.Drawing.Point(187, 366);
            this.len_front.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.len_front.Name = "len_front";
            this.len_front.Size = new System.Drawing.Size(100, 20);
            this.len_front.TabIndex = 9;
            // 
            // len_breast
            // 
            this.len_breast.Location = new System.Drawing.Point(187, 392);
            this.len_breast.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.len_breast.Name = "len_breast";
            this.len_breast.Size = new System.Drawing.Size(100, 20);
            this.len_breast.TabIndex = 10;
            // 
            // circ_neck
            // 
            this.circ_neck.Location = new System.Drawing.Point(187, 418);
            this.circ_neck.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.circ_neck.Name = "circ_neck";
            this.circ_neck.Size = new System.Drawing.Size(100, 20);
            this.circ_neck.TabIndex = 11;
            // 
            // circ_sleeve
            // 
            this.circ_sleeve.Location = new System.Drawing.Point(187, 444);
            this.circ_sleeve.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.circ_sleeve.Name = "circ_sleeve";
            this.circ_sleeve.Size = new System.Drawing.Size(100, 20);
            this.circ_sleeve.TabIndex = 12;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(59, 477);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(122, 13);
            this.label13.TabIndex = 31;
            this.label13.Text = "Délka od pasu ke koleni";
            // 
            // len_knee
            // 
            this.len_knee.Location = new System.Drawing.Point(187, 470);
            this.len_knee.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.len_knee.Name = "len_knee";
            this.len_knee.Size = new System.Drawing.Size(100, 20);
            this.len_knee.TabIndex = 32;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 620);
            this.Controls.Add(this.measures);
            this.Controls.Add(this.welcome);
            this.Controls.Add(this.header);
            this.Name = "Form1";
            this.Text = "Střihovátko";
            this.welcome.ResumeLayout(false);
            this.welcome.PerformLayout();
            this.measures.ResumeLayout(false);
            this.measures.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.height)).EndInit();
            this.nameAndNoteMeasures.ResumeLayout(false);
            this.nameAndNoteMeasures.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.circ_bust)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_waist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_hips)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_back)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wid_back)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_shoulder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_sleeve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_front)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_breast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_neck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.circ_sleeve)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.len_knee)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label header;
        private System.Windows.Forms.Label welcomeText;
        private System.Windows.Forms.RadioButton updateMeasures;
        private System.Windows.Forms.RadioButton noUpdate;
        private System.Windows.Forms.Panel welcome;
        private System.Windows.Forms.Panel measures;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button next2;
        private System.Windows.Forms.Button toWlecome;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button next1;
        private System.Windows.Forms.ComboBox choiceMeasuresSet;
        private System.Windows.Forms.Label labelNewMeasures;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label nameOfDataSet;
        private System.Windows.Forms.Panel nameAndNoteMeasures;
        private System.Windows.Forms.Label noteDataSet;
        private System.Windows.Forms.NumericUpDown height;
        private System.Windows.Forms.NumericUpDown circ_sleeve;
        private System.Windows.Forms.NumericUpDown circ_neck;
        private System.Windows.Forms.NumericUpDown len_breast;
        private System.Windows.Forms.NumericUpDown len_front;
        private System.Windows.Forms.NumericUpDown len_sleeve;
        private System.Windows.Forms.NumericUpDown len_shoulder;
        private System.Windows.Forms.NumericUpDown wid_back;
        private System.Windows.Forms.NumericUpDown len_back;
        private System.Windows.Forms.NumericUpDown circ_hips;
        private System.Windows.Forms.NumericUpDown circ_waist;
        private System.Windows.Forms.NumericUpDown circ_bust;
        private System.Windows.Forms.NumericUpDown len_knee;
        private System.Windows.Forms.Label label13;
    }
}

