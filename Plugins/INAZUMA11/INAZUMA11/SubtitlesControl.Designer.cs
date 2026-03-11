// ----------------------------------------------------------------------
// <copyright file="SubtitlesControl.Designer.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>28/04/2012 1:19:53</date>
// -----------------------------------------------------------------------
namespace INAZUMA11
{
    partial class SubtitlesControl
    {
        /// <summary> 
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar 
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtOld = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numStartTime = new System.Windows.Forms.NumericUpDown();
            this.numEndTime = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtNew = new System.Windows.Forms.TextBox();
            this.numSub = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnWrite = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numStartTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEndTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSub)).BeginInit();
            this.SuspendLayout();
            // 
            // txtOld
            // 
            this.txtOld.Location = new System.Drawing.Point(3, 48);
            this.txtOld.Multiline = true;
            this.txtOld.Name = "txtOld";
            this.txtOld.ReadOnly = true;
            this.txtOld.Size = new System.Drawing.Size(250, 60);
            this.txtOld.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Original text:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Start time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(177, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "3/100 seconds";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "End time:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(177, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "3/100 seconds";
            // 
            // numStartTime
            // 
            this.numStartTime.Location = new System.Drawing.Point(91, 143);
            this.numStartTime.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numStartTime.Name = "numStartTime";
            this.numStartTime.Size = new System.Drawing.Size(80, 21);
            this.numStartTime.TabIndex = 6;
            this.numStartTime.ValueChanged += new System.EventHandler(this.numStartTime_ValueChanged);
            // 
            // numEndTime
            // 
            this.numEndTime.Location = new System.Drawing.Point(91, 167);
            this.numEndTime.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numEndTime.Name = "numEndTime";
            this.numEndTime.Size = new System.Drawing.Size(80, 21);
            this.numEndTime.TabIndex = 7;
            this.numEndTime.ValueChanged += new System.EventHandler(this.numEndTime_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(253, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "New text:";
            // 
            // txtNew
            // 
            this.txtNew.Location = new System.Drawing.Point(256, 48);
            this.txtNew.Multiline = true;
            this.txtNew.Name = "txtNew";
            this.txtNew.Size = new System.Drawing.Size(250, 60);
            this.txtNew.TabIndex = 10;
            this.txtNew.TextChanged += new System.EventHandler(this.txtNew_TextChanged);
            // 
            // numSub
            // 
            this.numSub.Location = new System.Drawing.Point(81, 3);
            this.numSub.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.numSub.Name = "numSub";
            this.numSub.Size = new System.Drawing.Size(90, 21);
            this.numSub.TabIndex = 13;
            this.numSub.ValueChanged += new System.EventHandler(this.numSub_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "Num. sub:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(177, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "of ";
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(426, 156);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(80, 37);
            this.btnWrite.TabIndex = 8;
            this.btnWrite.Text = "Write file";
            this.btnWrite.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // SubtitlesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numSub);
            this.Controls.Add(this.txtNew);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnWrite);
            this.Controls.Add(this.numEndTime);
            this.Controls.Add(this.numStartTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOld);
            this.Name = "SubtitlesControl";
            this.Size = new System.Drawing.Size(512, 473);
            ((System.ComponentModel.ISupportInitialize)(this.numStartTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEndTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSub)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOld;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numStartTime;
        private System.Windows.Forms.NumericUpDown numEndTime;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtNew;
        private System.Windows.Forms.NumericUpDown numSub;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}
