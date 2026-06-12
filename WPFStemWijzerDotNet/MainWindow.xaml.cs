using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


namespace WPFStemWijzerDotNet;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

	Database db = new Database();
	int geselecteerdeId = -1;
	int selectedCommentId = -1;
	int selectedGebruikerId = -1;
	public MainWindow()
	{
		InitializeComponent();
		LoadNieuws();
		LoadPartijen();
		LoadComments();
		LoadGebruikers();
		LoadVerkiezingen();
        LoadNieuwsCombo();
		LoadGebruikerCombo();
		LoadVragen();
		LoadVraagCombo();

	}
	private void LoadPartijen()
	{
		PartijenGrid.ItemsSource = db.GetPartijen().DefaultView;
	}

	private void LoadNieuws()
	{
		NieuwsGrid.ItemsSource = db.GetNieuws().DefaultView;
	}

	private void LoadComments()
	{
		ReactiesGrid.ItemsSource = db.GetComments().DefaultView;
	}

    private void LoadNieuwsCombo()
    {
        ReactieNieuws.ItemsSource = db.GetNieuws().DefaultView;
    }
	private void LoadGebruikerCombo()
	{
		ReactieGebruiker.ItemsSource = db.GetGebruikers().DefaultView;
	}

    private void LoadGebruikers()
	{
		GebruikersGrid.ItemsSource = db.GetGebruikers().DefaultView;
	}

	private void LoadVerkiezingen()
	{
		VerkiezingenGrid.ItemsSource = db.GetVerkiezingen().DefaultView;
	}

	private void LoadVragen() //
	{
		VragenGrid.ItemsSource = db.GetVragen().DefaultView;
	}


	//----------------------- NIEUWS ------------------//

	private void BtnNieuwsToevoegen(object sender, RoutedEventArgs e)
	{
		db.AddNieuws(NieuwsTitel.Text, NieuwsImage.Text, NieuwsContent.Text);
		LoadNieuws();
		ClearNieuwsFields();
    }


	private void BtnNieuwsWijzigen(object sender, RoutedEventArgs e)
	{
		if (geselecteerdeId != -1)
		{
			db.Update(geselecteerdeId, NieuwsTitel.Text, NieuwsImage.Text, NieuwsContent.Text, NieuwsCreatedAt.Text);
			LoadNieuws();
            ClearNieuwsFields();
        }
	}


	private void BtnNieuwsVerwijderen(object sender, RoutedEventArgs e)
	{
		if (geselecteerdeId != -1)
		{
			db.Delete(geselecteerdeId);
			LoadNieuws();
            ClearNieuwsFields();
        }
	}
	private void NieuwsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (NieuwsGrid.SelectedItem != null)
		{
			DataRowView row = (DataRowView)NieuwsGrid.SelectedItem;
			geselecteerdeId = (int)row["id"];
			NieuwsTitel.Text = row["title"].ToString(); // 
			NieuwsImage.Text = row["image"].ToString(); //
			NieuwsContent.Text = row["content"].ToString(); //
			NieuwsCreatedAt.Text = row["created_at"].ToString(); //
		}
	}

    private void ClearNieuwsFields()
    {
        NieuwsTitel.Text = "";
        NieuwsImage.Text = "";
        NieuwsContent.Text = "";
        NieuwsCreatedAt.Text = "";

        geselecteerdeId = -1;
        NieuwsGrid.SelectedItem = null;
    }

    //----------------------- PARTIJEN ------------------//
    private void BtnPartijToevoegen_Click(object sender, RoutedEventArgs e)
    {
        byte[] logoBytes = LeesLogo(PartijLogo.Text);
        string logoType = PartijLogo.Text.EndsWith(".png") ? "image/png" : "image/jpeg";

        byte[] leiderFotoBytes = LeesLogo(PartijleiderFoto.Text);
        string leiderFotoType = PartijleiderFoto.Text.EndsWith(".png") ? "image/png" : "image/jpeg";

        db.AddPartij(PartijNaam.Text, logoBytes, logoType, Partijleider.Text, leiderFotoBytes, leiderFotoType, PartijBeschrijving.Text);
        LoadPartijen();
        ClearFields();
    }

    private void PartijenGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PartijenGrid.SelectedItem != null)
        {
            DataRowView row = (DataRowView)PartijenGrid.SelectedItem;
            geselecteerdeId = Convert.ToInt32(row["id"]);
            PartijNaam.Text = row["name"].ToString();
            PartijLogo.Text = "";
            Partijleider.Text = row["partijleider"].ToString();
            PartijleiderFoto.Text = "";
            PartijBeschrijving.Text = row["description"].ToString();
        }
    }

    private void BtnPartijWijzigen_Click(object sender, RoutedEventArgs e)
    {
        if (geselecteerdeId != -1)
        {
            byte[] logoBytes = LeesLogo(PartijLogo.Text);
            string logoType = PartijLogo.Text.EndsWith(".png") ? "image/png" : "image/jpeg";

            byte[] leiderFotoBytes = LeesLogo(PartijleiderFoto.Text);
            string leiderFotoType = PartijleiderFoto.Text.EndsWith(".png") ? "image/png" : "image/jpeg";

            db.UpdatePartij(geselecteerdeId, PartijNaam.Text, logoBytes, logoType, Partijleider.Text, leiderFotoBytes, leiderFotoType, PartijBeschrijving.Text);
            LoadPartijen();
            ClearFields();
        }
    }

    private byte[] LeesLogo(string bestandsnaam)
    {
        if (string.IsNullOrEmpty(bestandsnaam)) return null;
        string logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", bestandsnaam);
        if (File.Exists(logoPath))
            return File.ReadAllBytes(logoPath);
        return null;
    }

    private void BtnPartijVerwijderen_Click(object sender, RoutedEventArgs e)
    {
        if (geselecteerdeId != -1)
        {
            db.DeletePartij(geselecteerdeId);
            LoadPartijen();
            ClearFields();
        }
    }

    private void ClearFields()
    {
        PartijNaam.Text = "";
        PartijLogo.Text = "";
        Partijleider.Text = "";
        PartijleiderFoto.Text = "";
        PartijBeschrijving.Text = "";
        geselecteerdeId = -1;
    }



    //------------------ REACTIES ------------------//

    private void ReactieNieuws_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (ReactieNieuws.SelectedValue != null)
		{
			int nieuwsId = Convert.ToInt32(ReactieNieuws.SelectedValue);

			ReactieGebruiker.ItemsSource = db.GetGebruikersByNieuws(nieuwsId).DefaultView;
		}
	}

	private void ReactieGebruiker_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (ReactieNieuws.SelectedValue != null &&
			ReactieGebruiker.SelectedValue != null)
		{
			int nieuwsId = Convert.ToInt32(ReactieNieuws.SelectedValue);
			int gebruikerId = Convert.ToInt32(ReactieGebruiker.SelectedValue);

			ReactieTekst.Text = db.GetReactieTekst(nieuwsId, gebruikerId);
		}
	}

	private void ReactiesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (ReactiesGrid.SelectedItem is DataRowView row)
		{
			selectedCommentId = Convert.ToInt32(row["id"]);

			ReactieTekst.Text = row["reactie_text"].ToString();
			ReactieCreatedAt.Text = row["created_at"].ToString();
		}
	}

	private void AddReactie_Click(object sender, RoutedEventArgs e)
	{
		db.AddComment(
			Convert.ToInt32(ReactieNieuws.SelectedValue),
			Convert.ToInt32(ReactieGebruiker.SelectedValue),
			ReactieTekst.Text);

		LoadComments();
		ClearReactieFields();

    }

	private void UpdateReactie_Click(object sender, RoutedEventArgs e)
	{
		db.UpdateComment(selectedCommentId, ReactieTekst.Text);

		LoadComments();
		ClearReactieFields();

    }

	private void DeleteReactie_Click(object sender, RoutedEventArgs e)
	{
		db.DeleteComment(selectedCommentId);

		LoadComments();
		ClearReactieFields();

    }

    private void ClearReactieFields()
    {
        ReactieNieuws.SelectedIndex = -1;
        ReactieGebruiker.SelectedIndex = -1;

        ReactieTekst.Text = "";
        ReactieCreatedAt.Text = "";

        selectedCommentId = -1;
        ReactiesGrid.SelectedItem = null;
    }

    //------------------ GEBRUIKERS ------------------//
    private void Gebruikers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GebruikersGrid.SelectedItem is DataRowView row)
        {
            selectedGebruikerId = Convert.ToInt32(row["id"]);

            GebruikerNaam.Text = row["naam"].ToString();
            GebruikerEmail.Text = row["email"].ToString();
            GebruikerPassword.Password = row["password_hash"].ToString();
            GebruikerCreatedAt.Text = row["created_at"].ToString();
        }
    }
	private void Addgebruiker_click(object sender, RoutedEventArgs e)
		{
        db.AddGebruiker(
            GebruikerNaam.Text,
            GebruikerEmail.Text,
            GebruikerPassword.Password);

        LoadGebruikers();
        ClearGebruikerFields();
    }

    private void UpdateGebruiker_Click(object sender, RoutedEventArgs e)
    {
        db.UpdateGebruiker(
            selectedGebruikerId,
            GebruikerNaam.Text,
            GebruikerEmail.Text,
            GebruikerPassword.Password);

        LoadGebruikers();
        ClearGebruikerFields();
    }

    private void DeleteGebruiker_Click(object sender, RoutedEventArgs e)
	{
		db.DeleteGebruiker(selectedGebruikerId);

		LoadGebruikers();
		ClearGebruikerFields();

    }

    private void ClearGebruikerFields()
    {
        GebruikerNaam.Text = "";
        GebruikerEmail.Text = "";
        GebruikerPassword.Password = "";
        GebruikerCreatedAt.Text = "";

        selectedGebruikerId = -1;
        GebruikersGrid.SelectedItem = null;
    }


    //------------------ VERKIEZINGEN ------------------//

    private void BtnVerkiezingToevoegen(object sender, RoutedEventArgs e) // toevoegen
	{
		if (VerkiezingDatum.SelectedDate == null)
		{
			return;
		}

		db.AddVerkiezingen(VerkiezingNaam.Text, VerkiezingDatum.SelectedDate.Value, VerkiezingActief.IsChecked ?? false);
		LoadVerkiezingen();
		ClearVerkiezingFields();

    }

	private void BtnVerkiezingWijzigen(object sender, RoutedEventArgs e) // wijzigen
	{
		if (geselecteerdeId != -1)
		{
			db.UpdateVerkiezingen(geselecteerdeId, VerkiezingNaam.Text, VerkiezingDatum.SelectedDate.Value, VerkiezingActief.IsChecked ?? false);
			LoadVerkiezingen();
			ClearVerkiezingFields();

        }
	}

	private void BtnVerkiezingVerwijderen(object sender, RoutedEventArgs e) // verwijderen
	{
		if (geselecteerdeId != -1)
		{
			db.DeleteVerkiezingen(geselecteerdeId);
			LoadVerkiezingen();
			ClearVerkiezingFields();

        }
	}


	private void VerkiezingenGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
	{
		if (VerkiezingenGrid.SelectedItem is not DataRowView row)
			return;

		geselecteerdeId = Convert.ToInt32(row["id"]);


		VerkiezingNaam.Text = row["name"].ToString();


		VerkiezingDatum.SelectedDate =
			row["election_date"] == DBNull.Value
				? null
				: Convert.ToDateTime(row["election_date"]);


		VerkiezingActief.IsChecked =
			row["active"] != DBNull.Value &&
			Convert.ToBoolean(row["active"]);
	}

    private void ClearVerkiezingFields()
    {
        VerkiezingNaam.Text = "";
        VerkiezingDatum.SelectedDate = null;
        VerkiezingActief.IsChecked = false;

        geselecteerdeId = -1;
        VerkiezingenGrid.SelectedItem = null;
    }

	//------------------ VRAGEN ------------------//


	private void BtnVragenToevoegen(object sender, RoutedEventArgs e)
	{
		int verkiezingid = Convert.ToInt32(VraagVerkiezing.SelectedValue);
		string questionText = VraagTekst.Text;

		db.AddVragen(verkiezingid, questionText);

		LoadVragen();
	}

	private void BtnVragenWijzigen(object sender, RoutedEventArgs e)
	{
		if (geselecteerdeId != -1)
		{
			int verkiezingenId = Convert.ToInt32(VraagVerkiezing.SelectedValue);
			string questionText = VraagTekst.Text;

			db.UpdateVragen(geselecteerdeId, verkiezingenId, questionText);

			LoadVragen();
		}
	}
	private void BtnVragenVerwijderen(object sender, RoutedEventArgs e)
	{
		if (geselecteerdeId != -1)
		{

			db.DeleteVragen(geselecteerdeId);
			LoadVragen();
		}
	}



	private void VragenGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (VragenGrid.SelectedItem != null)
		{
			DataRowView row = (DataRowView)VragenGrid.SelectedItem;
			geselecteerdeId = (int)row["id"];
			VraagTekst.Text = row["question_text"].ToString();
			VraagVerkiezing.SelectedValue = row["verkiezingen_id"];
		}
	}

	private void LoadVraagCombo()
	{
		VraagVerkiezing.ItemsSource = db.GetVerkiezingen().DefaultView;
		VraagVerkiezing.DisplayMemberPath = "name";
		VraagVerkiezing.SelectedValuePath = "id";
	}
}
//

