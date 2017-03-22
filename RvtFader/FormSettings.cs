using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace RvtFader
{
  public partial class FormSettings : Form
  {
    Settings _settings;

    public FormSettings()
    {
      InitializeComponent();

      _settings = Settings.Load();

      txtAttenuationAir.Validating += TxtAttenuationAir_Validating;
      txtAttenuationAir.Validated += TxtAttenuationAir_Validated;
      txtAttenuationWall.Validating += TxtAttenuationWall_Validating;
      txtAttenuationWall.Validated += TxtAttenuationWall_Validated;
    }

    private void DecibelValidating( 
      TextBox t,
      CancelEventArgs e )
    {
      string s = t.Text;
      double d;
      try
      {
        d = double.Parse( s );
        if( 0>= d ) { throw new FormatException( 
          "expected positive dB" ); }
      }
      catch( System.FormatException )
      {
        // Cancel the event.
        e.Cancel = true;
        // Select the text to be corrected by the user.
        t.Select( 0, t.Text.Length );
        // Report error.
        this.errorProvider1.SetError( t,
          "Invalid attenuation in dB: " + s );
      }
    }

    private void TxtAttenuationAir_Validating( 
      object sender, 
      CancelEventArgs e )
    {
      DecibelValidating( txtAttenuationAir, e );
    }

    private void TxtAttenuationAir_Validated( object sender, EventArgs e )
    {
      errorProvider1.SetError( txtAttenuationAir, "" );
    }

    private void TxtAttenuationWall_Validating( object sender, CancelEventArgs e )
    {
      DecibelValidating( txtAttenuationWall, e );
    }

    private void TxtAttenuationWall_Validated( object sender, EventArgs e )
    {
      errorProvider1.SetError( txtAttenuationWall, "" );
    }

    private void btnSave_Click( object sender, EventArgs e )
    {
      _settings.AttenuationAirPerMetreInDb 
        = double.Parse(txtAttenuationAir.Text);

      _settings.AttenuationWallInDb 
        = double.Parse( txtAttenuationWall.Text );

      _settings.Save();
    }
  }
}
