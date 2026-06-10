using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Packaging;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;

namespace WPFStemWijzerDotNet
{
	public class Database
	{
		string connectie = @"Server=localhost;Database=stemwijzer;uid=root;pwd=;";


		//---NIEUWS---//
		public DataTable GetNieuws()
		{
			DataTable table = new DataTable();

			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM nieuws", conn);
				adapter.Fill(table);
			}
			return table;
		}

		public void AddNieuws(string title, string image, string content)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("INSERT INTO nieuws (title, image, content) VALUES (@title, @image, @content)", conn);
				cmd.Parameters.AddWithValue("@title", title);
				cmd.Parameters.AddWithValue("@image", image);
				cmd.Parameters.AddWithValue("@content", content);

				cmd.ExecuteNonQuery();
			}
		}

		public void Update(int id, string title, string image, string content, string created_at)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				MySqlCommand cmd = new MySqlCommand(@"UPDATE nieuws SET title=@title, image=@image, content=@content, created_at=@created_at WHERE id=@id", conn);

				cmd.Parameters.AddWithValue("@id", id);
				cmd.Parameters.AddWithValue("@title", title);
				cmd.Parameters.AddWithValue("@image", image);
				cmd.Parameters.AddWithValue("@content", content);
				cmd.Parameters.AddWithValue("@created_at", created_at);

				cmd.ExecuteNonQuery();
			}
		}

		public void Delete(int id)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM nieuws WHERE id=@id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.ExecuteNonQuery();
			}
		}




		//---Partijen---//
		public DataTable GetPartijen()
		{
			DataTable table = new DataTable();

			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM partijen", conn);
				adapter.Fill(table);
			}

			return table;
		}

		public void AddPartij(string name, string logo, string description)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				MySqlCommand cmd = new MySqlCommand(@"INSERT INTO partijen (name, logo, description)VALUES(@name, @logo, @description)", conn);
				cmd.Parameters.AddWithValue("@name", name);
				cmd.Parameters.AddWithValue("@logo", logo);
				cmd.Parameters.AddWithValue("@description", description);

				cmd.ExecuteNonQuery();
			}
		}

		public void UpdatePartij(int id, string name, string logo, string description)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand(@"UPDATE partijen SET name = @name, logo = @logo, description = @description WHERE id = @id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.Parameters.AddWithValue("@name", name);
				cmd.Parameters.AddWithValue("@logo", logo);
				cmd.Parameters.AddWithValue("@description", description);
				cmd.ExecuteNonQuery();
			}
		}

		public void DeletePartij(int id)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("DELETE FROM partijen WHERE id=@id", conn);
				cmd.Parameters.AddWithValue("@id", id);
				cmd.ExecuteNonQuery();
			}
		}




		//---REACTIES---//
		public DataTable GetComments()
		{
			DataTable table = new DataTable();

			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"SELECT comments.id, nieuws.title AS nieuwsTitel, gebruikers.username, comments.reactie_text, comments.created_at FROM comments JOIN nieuws ON comments.nieuws_id = nieuws.id JOIN gebruikers ON comments.gebruikers_id = gebruikers.id";

				MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
				adapter.Fill(table);
			}

			return table;
		}

		public DataTable GetGebruikersByNieuws(int nieuwsId)
		{
			DataTable table = new DataTable();

			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"SELECT DISTINCT gebruikers.id, gebruikers.username FROM comments JOIN gebruikers ON comments.gebruikers_id = gebruikers.id WHERE comments.nieuws_id = @nieuwsId";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@nieuwsId", nieuwsId);

				MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

				adapter.Fill(table);
			}

			return table;
		}

		public string GetReactieTekst(int nieuwsId, int gebruikerId)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"SELECT reactie_text FROM comments WHERE nieuws_id = @nieuwsId AND gebruikers_id = @gebruikerId LIMIT 1";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@nieuwsId", nieuwsId);
				cmd.Parameters.AddWithValue("@gebruikerId", gebruikerId);

				object result = cmd.ExecuteScalar();

				return result?.ToString() ?? "";
			}
		}

		public void AddComment(int nieuwsId, int gebruikerId, string reactie)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"INSERT INTO comments(nieuws_id, gebruikers_id, reactie_text, created_at) VALUES(@nieuwsId, @gebruikerId, @reactie, NOW())";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@nieuwsId", nieuwsId);
				cmd.Parameters.AddWithValue("@gebruikerId", gebruikerId);
				cmd.Parameters.AddWithValue("@reactie", reactie);

				cmd.ExecuteNonQuery();
			}
		}
		public void UpdateComment(int id, string reactie)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"UPDATE comments SET reactie_text = @reactie WHERE id = @id";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@id", id);
				cmd.Parameters.AddWithValue("@reactie", reactie);

				cmd.ExecuteNonQuery();
			}
		}
		public void DeleteComment(int id)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("DELETE FROM comments WHERE id=@id", conn);

				cmd.Parameters.AddWithValue("@id", id);

				cmd.ExecuteNonQuery();
			}
		}


		//---GEBRUIKERS---//
		public DataTable GetGebruikers()
		{
			DataTable table = new DataTable();

			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlDataAdapter adapter = new MySqlDataAdapter("SELECT * FROM gebruikers", conn);
				adapter.Fill(table);
			}
			return table;
		}

		public void AddGebruiker(string username, string email, string password)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"INSERT INTO gebruikers(username, email, password_hash, created_at) VALUES(@username, @email, @password, NOW())";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@username", username);
				cmd.Parameters.AddWithValue("@email", email);
				cmd.Parameters.AddWithValue("@password", password);

				cmd.ExecuteNonQuery();
			}
		}

		public void UpdateGebruiker(int id, string username, string email, string password)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"UPDATE gebruikers SET username = @username, email = @email, password_hash = @password WHERE id = @id";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@id", id);
				cmd.Parameters.AddWithValue("@username", username);
				cmd.Parameters.AddWithValue("@email", email);
				cmd.Parameters.AddWithValue("@password", password);

				cmd.ExecuteNonQuery();
			}
		}

		public void DeleteGebruiker(int id)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				MySqlCommand cmd = new MySqlCommand("DELETE FROM gebruikers WHERE id=@id", conn);

				cmd.Parameters.AddWithValue("@id", id);

				cmd.ExecuteNonQuery();
			}
		}




		//---VERKIEZINGEN---//

		public DataTable GetVerkiezingen()
		{
			DataTable table = new DataTable();

			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();
				MySqlDataAdapter adapter =
					new MySqlDataAdapter("SELECT * FROM verkiezingen", conn);

				adapter.Fill(table);
			}

			return table;
		}

		public void AddVerkiezingen(string name, DateTime election_date, bool active)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"INSERT INTO verkiezingen 
                        (name, election_date, active)
                        VALUES (@name, @election_date, @active)";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@name", name);
				cmd.Parameters.AddWithValue("@election_date", election_date);
				cmd.Parameters.AddWithValue("@active", active ? 1 : 0);

				cmd.ExecuteNonQuery();
			}
		}

		public void UpdateVerkiezingen(int id, string name, DateTime? election_date, bool active)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				string query = @"UPDATE verkiezingen 
                         SET name=@name,
                             election_date=@election_date,
                             active=@active
                         WHERE id=@id";

				MySqlCommand cmd = new MySqlCommand(query, conn);

				cmd.Parameters.AddWithValue("@id", id);
				cmd.Parameters.AddWithValue("@name", name);
				cmd.Parameters.AddWithValue("@election_date", election_date);
				cmd.Parameters.AddWithValue("@active", active ? 1 : 0);

				cmd.ExecuteNonQuery();
			}
		}

		public void DeleteVerkiezingen(int id)
		{
			using (MySqlConnection conn = new MySqlConnection(connectie))
			{
				conn.Open();

				MySqlCommand cmd =
					new MySqlCommand("DELETE FROM verkiezingen WHERE id=@id", conn);

				cmd.Parameters.AddWithValue("@id", id);

				cmd.ExecuteNonQuery();
			}
		}
	}
}
