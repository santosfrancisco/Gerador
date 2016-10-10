using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gerador.Models;
using IdentitySample.Models;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using PagedList;
using System.Text;

namespace Gerador.Controllers
{
	public class UnidadesController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: Unidades
		public async Task<ActionResult> Index()
		{
			var unidades = db.Unidades.Include(u => u.Empreendimentos);
			return View(await unidades.ToListAsync());
		}

		// GET: Unidades download planilha exemplo
		public FileResult PlanilhaModelo(int id)
		{
			ViewBag.Empreendimento = db.Empreendimentos.Find(id).Nome.ToString();

			byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/App_Data/planilhas/Planilha_modelo.csv"));
			string fileName = (id + " - " + ViewBag.Empreendimento + ".csv");

			return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
		}

		// GET: Unidades/Details/5
		public async Task<ActionResult> Details(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Unidades unidades = await db.Unidades.FindAsync(id);
			if (unidades == null)
			{
				return HttpNotFound();
			}
			return View(unidades);
		}

		// GET: Unidades/Upload
		public ActionResult Upload(int? id)
		{
			ViewBag.FeedBack = "Aviso";
			// Se carregar a página de upload sem o ID do empreendimento, direciona para a view de erro.
			if (id == null)
			{
				return View("Error");
			}

			ViewBag.Empreendimento = db.Empreendimentos.Find(id).Nome.ToString();
			return View();
		}
		// POST/Upload
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Upload(HttpPostedFileBase FileUpload, int id, bool OK)
		{
			if (!OK)
			{
				return View();
			}
			ViewBag.FeedBack = "Aviso";
			DataTable dt = new DataTable();


			if (FileUpload.ContentLength > 0)
			{
				if (FileUpload.FileName.EndsWith(".csv") && FileUpload.FileName.StartsWith(id.ToString()))
				{

					string fileName = Path.GetFileName(FileUpload.FileName);
					string path = Path.Combine(Server.MapPath("~/App_Data/planilhas"), fileName);

					try
					{
						FileUpload.SaveAs(path);

						dt = ProcessCSV(path, id);


						ViewBag.FeedBack = ProcessBulkCopy(dt);
					}
					catch (Exception ex)
					{

						ViewBag.FeedBack = ex.Message;
						dt.Dispose();
					}
				}
				else
				{
					ViewBag.FeedBack = "Código ou extensão inválidos. Por favor verifique se o nome do seu arquivo inicia com o ID do empreendimento ou se formato é *.CSV.";
				}
			}
			else
			{

				ViewBag.FeedBack = "Please select a file";
			}


			dt.Dispose();

			//return View("Upload", ViewData["Feedback"]);

			return View();
		}

		private static DataTable ProcessCSV(string fileName, int? id)
		{

			string Feedback = string.Empty;
			string line = string.Empty;
			string[] strArray;
			//DataTable dt = new DataTable();

			//-------------------------
			string tmpTable = "create table #Unidades (IDEmpreendimento int, IDUnidade int, Numero nvarchar(10), UnidadeStatus int, Tipo int, UnidadeObservacao nvarchar(100))";

			//Create a datatable that matches the temp table exactly. (WARNING: order of columns must match the order in the table)
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("IDEmpreendimento", typeof(Int32)));
			dt.Columns.Add(new DataColumn("IDUnidade", typeof(Int32)));
			dt.Columns.Add(new DataColumn("Numero", typeof(string)));
			dt.Columns.Add(new DataColumn("UnidadeStatus", typeof(Unidades.Status)));
			dt.Columns.Add(new DataColumn("Tipo", typeof(Unidades.Tipos)));
			dt.Columns.Add(new DataColumn("UnidadeObservacao", typeof(string)));
			//-------------------------
			//DataRow row;


			Regex r = new Regex(";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");


			StreamReader sr = new StreamReader(fileName);

			//line = "IDEmpreendimento;IDUnidade;" + sr.ReadLine();
			line = "IDEmpreendimento;" + sr.ReadLine();
			strArray = r.Split(line);


			//Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));



			while ((line = sr.ReadLine()) != null)
			{
				// Adiciono o ID do empreendimento no início da linha
				//line = id + ";;" + line;
				line = id + ";" + line;

				//row = dt.NewRow();

				var unidade = r.Split(line);

				DataRow row = dt.NewRow();
				if(unidade[1] == "")
				{
					row["IDEmpreendimento"] = id;
					//row["IDUnidade"] = (unidade[1] == "") ? : Convert.ToInt32(unidade[1]);
					row["Numero"] = unidade[2];
					row["UnidadeStatus"] = unidade[3];
					row["Tipo"] = unidade[4];
					row["UnidadeObservacao"] = unidade[5];
					dt.Rows.Add(row);
				} else
				{
					row["IDEmpreendimento"] = id;
					row["IDUnidade"] = Convert.ToInt32(unidade[1]);
					row["Numero"] = unidade[2];
					row["UnidadeStatus"] = unidade[3];
					row["Tipo"] = unidade[4];
					row["UnidadeObservacao"] = unidade[5];
					dt.Rows.Add(row);
				}
				


				//data[3] = "1";

				//row.ItemArray = data;
				//dt.Rows.Add(row);
			}


			sr.Dispose();


			return dt;


		}


		private static String ProcessBulkCopy(DataTable dt)
		{
			string Feedback = string.Empty;

			//string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


			List<Unidades> unidades = new List<Unidades>();
			//Make a temp table in sql server that matches our production table
			string tmpTable = "create table #Unidades (IDEmpreendimento int, IDUnidade int, Numero nvarchar(10), UnidadeStatus int, Tipo int, UnidadeObservacao nvarchar(100))";

			//Create a datatable that matches the temp table exactly. (WARNING: order of columns must match the order in the table)
			//DataTable table = new DataTable();
			//table.Columns.Add(new DataColumn("IDEmpreendimento", typeof(int)));
			//table.Columns.Add(new DataColumn("IDUnidade", typeof(int)));
			//table.Columns.Add(new DataColumn("Numero", typeof(string)));
			//table.Columns.Add(new DataColumn("UnidadeStatus", typeof(Unidades.Status)));
			//table.Columns.Add(new DataColumn("Tipo", typeof(Unidades.Tipos)));
			//table.Columns.Add(new DataColumn("UnidadeObservacao", typeof(string)));

			//Add prices in our list to our DataTable
			//foreach (Unidades unidade in dt)
			//{
			//	DataRow row = table.NewRow();
			//	row["IDEmpreendimento"] = id;
			//	row["IDUnidade"] = unidade.IDUnidade;
			//	row["Timestamp"] = price.Timestamp;
			//	row["Symbol"] = price.Symbol;
			//	row["Price"] = price.Value;
			//	row["Timestamp"] = price.Timestamp;
			//	table.Rows.Add(row);
			//}

			//Connect to DB
			string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
			using (SqlConnection con = new SqlConnection(conString))
			{
				con.Open();

				//Execute the command to make a temp table
				SqlCommand cmd = new SqlCommand(tmpTable, con);
				cmd.ExecuteNonQuery();

				//BulkCopy the data in the DataTable to the temp table
				using (SqlBulkCopy bulk = new SqlBulkCopy(con))
				{
					bulk.DestinationTableName = "#Unidades";
					bulk.WriteToServer(dt);
				}

				//Now use the merge command to upsert from the temp table to the production table
				string mergeSql =

				"SET IDENTITY_INSERT Unidades OFF " +
				"MERGE Unidades AS TARGET " +
				"USING #Unidades AS SOURCE " +
				"ON TARGET.IDUnidade = SOURCE.IDUnidade " +
				"WHEN MATCHED THEN " +
				"UPDATE SET TARGET.Numero = SOURCE.Numero, " +
				"TARGET.UnidadeStatus = SOURCE.UnidadeStatus " +
				"WHEN NOT MATCHED BY TARGET THEN " +
				"INSERT (IDEmpreendimento,Numero,UnidadeStatus,Tipo,UnidadeObservacao) " +
				"VALUES (Source.IDEmpreendimento,Source.Numero,Source.UnidadeStatus,Source.Tipo,Source.UnidadeObservacao);";

				cmd.CommandText = mergeSql;
				cmd.ExecuteNonQuery();

				//Clean up the temp table
				cmd.CommandText = "drop table #Unidades";
				cmd.ExecuteNonQuery();

				Feedback = "Sucesso";
			}
			//}
 

			//using (SqlConnection conn = new SqlConnection(connString))
			//{

			//	using (var copy = new SqlBulkCopy(conn))
			//	{


			//		conn.Open();


			//		copy.DestinationTableName = "Unidades";
			//		copy.BatchSize = dt.Rows.Count;
			//		try
			//		{

			//			copy.WriteToServer(dt);
			//			Feedback = "Sucesso";
			//		}
			//		catch (Exception ex)
			//		{
			//			Feedback = ex.Message;
			//		}
			//	}
			//}

			return Feedback;
		}

	// GET: Unidades/Consulta/5
	public async Task<ActionResult> Consulta(int? id, int? page, string sortOrder, string currentFilter, string searchString)
	{
		ViewBag.CurrentSort = sortOrder;
		ViewBag.NumeroParam = String.IsNullOrEmpty(sortOrder) ? "Numero_Desc" : "";
		ViewBag.StatusParam = sortOrder == "Status" ? "Status_Desc" : "Status";
		List<Unidades> unidades;

		if (id == null)
		{
			return RedirectToAction("Index", "Home", null);
		}

		unidades = await db.Unidades.Where(u => u.IDEmpreendimento == id).ToListAsync();

		//var tipoUsuario = RepositorioUsuarios.VerificaTipoUsuario();
		//var idUsuario = RepositorioUsuarios.RecuperaIDUsuario();
		//if (tipoUsuario == 0)
		//{
		//	unidades = await db.Unidades.Where(u => u.IDEmpreendimento == id).ToListAsync();
		//}
		//else if (tipoUsuario == 1)
		//{
		//	unidades = await db.Unidades.Where(u => u.IDEmpreendimento == id).ToListAsync();
		//}
		//else
		//{
		//	unidades = await db.Unidades.Where(u => u.IDEmpreendimento == id && (u.UnidadeStatus == 0 || u.Analises.FirstOrDefault().Clientes.IDUsuario == idUsuario)).ToListAsync();
		//}

		if (searchString != null)
		{
			page = 1;
		}
		else
		{
			searchString = currentFilter;
		}

		ViewBag.CurrentFilter = searchString;

		if (!String.IsNullOrEmpty(searchString))
		{
			unidades = unidades.Where(u => u.Numero.ToUpper().Contains(searchString.ToUpper())).ToList();
		}

		switch (sortOrder)
		{
			case "Numero_Desc":
				unidades = unidades.OrderByDescending(u => u.Numero).ToList();
				break;
			case "Status":
				unidades = unidades.OrderBy(u => u.UnidadeStatus).ToList();
				break;
			case "Status_Desc":
				unidades = unidades.OrderByDescending(u => u.UnidadeStatus).ToList();
				break;
			default:
				unidades = unidades.OrderBy(u => u.Numero).ToList();
				break;
		}
		ViewBag.IdEmpreendimento = id;
		int pageSize = 10;
		int pageNumber = (page ?? 1);
		return View(unidades.ToPagedList(pageNumber, pageSize));
	}

	// GET: Unidades/Create
	public ActionResult Create(int? id)
	{
		if (id == null)
		{
			return View("Error");
		}

		ViewBag.Empreendimento = db.Empreendimentos.Find(id).Nome.ToString();
		//ViewBag.UnidadeStatus = Unidades.StatusUnidade();
		//ViewBag.Tipo = Unidades.TipoUnidade();
		ViewBag.IDEmpreendimento = new SelectList(db.Empreendimentos, "IDEmpreendimento", "Nome", id);
		return View();
	}

	// POST: Unidades/Create
	// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
	// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	//public async Task<ActionResult> Create([Bind(Include = "IDUnidade,Numero,IDEmpreendimento,UnidadeStatus,Tipo,UnidadeObservacao")] Unidades unidades)
	public async Task<ActionResult> Create(FormCollection form, int id)
	{
		//if (ModelState.IsValid)
		//{
		//    db.Unidades.Add(unidades);
		//    await db.SaveChangesAsync();
		//    return RedirectToAction("Index");
		//}

		string[] novasUnidades = form["unidades"].Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

		if (novasUnidades != null)
		{
			if (ModelState.IsValid)
			{
				foreach (string u in novasUnidades)
				{
					Unidades unidade = new Unidades();
					unidade.Numero = u;
					unidade.IDEmpreendimento = id;
					unidade.Tipo = (Unidades.Tipos)Enum.Parse(typeof(Unidades.Tipos), form["Tipo"]);
					unidade.UnidadeStatus = (Unidades.Status)Enum.Parse(typeof(Unidades.Status), form["UnidadeStatus"]);
					unidade.UnidadeObservacao = form["UnidadeObservacao"];
					db.Unidades.Add(unidade);
				}


				await db.SaveChangesAsync();
				return RedirectToAction("Consulta", "Unidades", new { id = id });
			}
		}

		//ViewBag.IDEmpreendimento = new SelectList(db.Empreendimentos, "IDEmpreendimento", "Nome", unidades.IDEmpreendimento);
		ViewBag.IDEmpreendimento = new SelectList(db.Empreendimentos, "IDEmpreendimento", "Nome", id);
		return View();
	}

	// GET: Unidades/Edit/5
	public async Task<ActionResult> Edit(int? id)
	{
		if (id == null)
		{
			return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		}
		Unidades unidades = await db.Unidades.FindAsync(id);
		if (unidades == null)
		{
			return HttpNotFound();
		}
		ViewBag.IDEmpreendimento = new SelectList(db.Empreendimentos, "IDEmpreendimento", "Nome", unidades.IDEmpreendimento);
		return View(unidades);
	}

	// POST: Unidades/Edit/5
	// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
	// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> Edit([Bind(Include = "IDUnidade,Numero,IDEmpreendimento,UnidadeStatus,Tipo,UnidadeObservacao")] Unidades unidades)
	{
		if (ModelState.IsValid)
		{
			db.Entry(unidades).State = EntityState.Modified;
			await db.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		ViewBag.IDEmpreendimento = new SelectList(db.Empreendimentos, "IDEmpreendimento", "Nome", unidades.IDEmpreendimento);
		return View(unidades);
	}

	// GET: Unidades/Delete/5
	public async Task<ActionResult> Delete(int? id)
	{
		if (id == null)
		{
			return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		}
		Unidades unidades = await db.Unidades.FindAsync(id);
		if (unidades == null)
		{
			return HttpNotFound();
		}
		return View(unidades);
	}

	// POST: Unidades/Delete/5
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<ActionResult> DeleteConfirmed(int id)
	{
		Unidades unidades = await db.Unidades.FindAsync(id);
		db.Unidades.Remove(unidades);
		await db.SaveChangesAsync();
		return RedirectToAction("Index");
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			db.Dispose();
		}
		base.Dispose(disposing);
	}
}
}
