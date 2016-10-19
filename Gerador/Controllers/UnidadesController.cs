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
	public class UnidadesController : BaseController
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

		//---------------------------------------------------------------------------------------------------------------------
		// Baixar planilha das unidades em CSV
		public void DownloadCsv(int? id)
		{
			// ViewBag com o nome do empreendimento
			ViewBag.Empreendimento = db.Empreendimentos.Find(id).Nome.ToString();

			string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
			// Select na tabela trazendo as colunas IDUnidade, Numero, Tipo e UnidadeObservacao Where ID seja igual ao ID do empreendimento
			var selectQuery = "select IDUnidade, Numero, Tipo, UnidadeObservacao from Unidades where IDEmpreendimento = " + id + ";";

			DataTable table = ReadTable(connectionString, selectQuery);
			// Chama o download da planilha CSV com o ID e o NOME do empreendimento
			ToCsv(table, id + " - " + ViewBag.Empreendimento + ".csv", ";");
		}

		/// <param name="dt"></param>
		/// <param name="fileName"></param>
		/// <param name="delimiter"></param>
		private void ToCsv(DataTable dt, string fileName, string delimiter)
		{
			//Output
			Response.Clear();
			Response.ContentType = "text/csv";
			Response.AppendHeader("Content-Disposition",
				string.Format("attachment; filename={0}", fileName));

			//Cabeçalhos da tabela
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				Response.Write(dt.Columns[i].ColumnName);
				Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
			}

			//write the data
			foreach (DataRow row in dt.Rows)
			{
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					Response.Write(row[i].ToString());
					Response.Write((i < dt.Columns.Count - 1) ? delimiter : Environment.NewLine);
				}
			}

			Response.End();
		}
		public static DataTable ReadTable(string connectionString, string selectQuery)
		{
			var tmpUnidades = new DataTable();

			var conn = new SqlConnection(connectionString);

			try
			{
				conn.Open();
				var command = new SqlCommand(selectQuery, conn);

				using (var adapter = new SqlDataAdapter(command))
				{
					adapter.Fill(tmpUnidades);
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (conn.State == ConnectionState.Open)
					conn.Close();
			}

			return tmpUnidades;
		}

		// Fim código download planilha CSV
		//---------------------------------------------------------------------------------------------------------------------

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

			ViewBag.OK = true;
			ViewBag.FeedBack = "Aviso";
			ViewBag.Empreendimento = db.Empreendimentos.Find(id).Nome.ToString();
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

						if (ViewBag.Ok)
						{
							ViewBag.FeedBack = ProcessBulkCopy(dt);
						}
					}
					catch (Exception ex)
					{

						ViewBag.FeedBack = ex.Message;
					}
				}
				else
				{
					ViewBag.FeedBack = "Código ou extensão inválidos. Por favor verifique se o nome do seu arquivo inicia com o ID do empreendimento ou se formato é *.CSV.";
				}
			}
			else
			{

				ViewBag.FeedBack = "Escolha um arquivo";
			}


			dt.Dispose();

			return View();
		}

		private  DataTable ProcessCSV(string fileName, int? id)
		{
			//string Feedback = string.Empty;
			string line = string.Empty;
			string[] strArray;

			//-------------------------
			//Create a datatable that matches the temp table exactly. (WARNING: order of columns must match the order in the table)
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("IDEmpreendimento", typeof(Int32)));
			dt.Columns.Add(new DataColumn("IDUnidade", typeof(Int32)));
			dt.Columns.Add(new DataColumn("Numero", typeof(string)));
			dt.Columns.Add(new DataColumn("Tipo", typeof(Unidades.Tipos)));
			dt.Columns.Add(new DataColumn("UnidadeObservacao", typeof(string)));
			dt.Columns.Add(new DataColumn("UnidadeStatus", typeof(Unidades.Status)));
			//-------------------------


			Regex r = new Regex(";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");


			StreamReader sr = new StreamReader(fileName);
			
			line = "IDEmpreendimento;" + sr.ReadLine() + ";UnidadeStatus";
			strArray = r.Split(line);

			try
			{
				while ((line = sr.ReadLine()) != null)
				{
					// Adiciono o ID do empreendimento no início da linha
					line = id + ";" + line + ";";

					var unidade = r.Split(line);

					DataRow row = dt.NewRow();
					if (unidade[1] == "")
					{
						row["IDEmpreendimento"] = id;
						row["Numero"] = unidade[2];
						row["UnidadeStatus"] = 0;// 0 = status Livre
						row["Tipo"] = unidade[3];
						row["UnidadeObservacao"] = unidade[4];
						dt.Rows.Add(row);
					}
					else
					{
						row["IDEmpreendimento"] = id;
						row["IDUnidade"] = Convert.ToInt32(unidade[1]);
						row["Numero"] = unidade[2];
						row["Tipo"] = unidade[3];
						row["UnidadeObservacao"] = unidade[4];
						dt.Rows.Add(row);
					}

				}
			}
			catch(Exception ex)
			{
				ViewBag.OK = false;
				ViewBag.FeedBack = ex.Message;
				dt.Dispose();
			}
			


			sr.Dispose();


			return dt;


		}


		private static String ProcessBulkCopy(DataTable dt)
		{
			string Feedback = string.Empty;

			List<Unidades> unidades = new List<Unidades>();
			//Cria a tabela temporária que receberá as unidades da planilha
			string tmpTable = "create table #Unidades (IDEmpreendimento int, IDUnidade int, Numero nvarchar(10), Tipo int, UnidadeObservacao nvarchar(100), UnidadeStatus int)";

			//ConnectionString
			string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
			using (SqlConnection con = new SqlConnection(conString))
			{
				con.Open();
				try
				{
					//Executa o cmd para criar a tabela temporária
					SqlCommand cmd = new SqlCommand(tmpTable, con);
					cmd.ExecuteNonQuery();

					//Insere os dados da Dt na tabela temporaria #unidades
					using (SqlBulkCopy bulk = new SqlBulkCopy(con))
					{
						bulk.DestinationTableName = "#Unidades";
						bulk.WriteToServer(dt);
					}

					//Merge para atualizar as unidades que existem e inserir as que nao existem
					string mergeSql =

					"SET IDENTITY_INSERT Unidades OFF " +
					"MERGE Unidades AS TARGET " +
					"USING #Unidades AS SOURCE " +
					"ON TARGET.IDUnidade = SOURCE.IDUnidade " +
					"WHEN MATCHED THEN " +
					"UPDATE SET TARGET.Numero = SOURCE.Numero, " +
					"TARGET.Tipo = SOURCE.Tipo, " +
					"TARGET.UnidadeObservacao = SOURCE.UnidadeObservacao " +
					"WHEN NOT MATCHED BY TARGET THEN " +
					"INSERT (IDEmpreendimento,Numero,UnidadeStatus,Tipo,UnidadeObservacao) " +
					"VALUES (Source.IDEmpreendimento,Source.Numero,Source.UnidadeStatus,Source.Tipo,Source.UnidadeObservacao);";

					cmd.CommandText = mergeSql;
					cmd.ExecuteNonQuery();

					//Drop na tabela temporária
					cmd.CommandText = "drop table #Unidades";
					cmd.ExecuteNonQuery();

					Feedback = "Sucesso";
				}
				catch (Exception ex)
				{

					Feedback = ex.Message;
				}
			}
			
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
