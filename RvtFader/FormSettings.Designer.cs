namespace RvtFader
{
  partial class FormSettings
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if( disposing && ( components != null ) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.txtAttenuationWall = new System.Windows.Forms.TextBox();
      this.txtAttenuationAir = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(112, 75);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 11;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnSave
      // 
      this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnSave.Location = new System.Drawing.Point(15, 75);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(75, 23);
      this.btnSave.TabIndex = 10;
      this.btnSave.Text = "Save";
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // txtAttenuationWall
      // 
      this.txtAttenuationWall.Location = new System.Drawing.Point(113, 37);
      this.txtAttenuationWall.Name = "txtAttenuationWall";
      this.txtAttenuationWall.Size = new System.Drawing.Size(100, 20);
      this.txtAttenuationWall.TabIndex = 9;
      // 
      // txtAttenuationAir
      // 
      this.txtAttenuationAir.Location = new System.Drawing.Point(113, 9);
      this.txtAttenuationAir.Name = "txtAttenuationAir";
      this.txtAttenuationAir.Size = new System.Drawing.Size(100, 20);
      this.txtAttenuationAir.TabIndex = 8;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(11, 40);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(92, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "dB through a wall:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 17);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(84, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "dB per metre air:";
      // 
      // errorProvider1
      // 
      this.errorProvider1.ContainerControl = this;
      // 
      // FormSettings
      // 
      this.AcceptButton = this.btnSave;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(227, 114);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnSave);
      this.Controls.Add(this.txtAttenuationWall);
      this.Controls.Add(this.txtAttenuationAir);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "FormSettings";
      this.Text = "Attenuation";
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.TextBox txtAttenuationWall;
    private System.Windows.Forms.TextBox txtAttenuationAir;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ErrorProvider errorProvider1;
  }
}