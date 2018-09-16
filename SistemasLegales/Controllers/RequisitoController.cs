using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemasLegales.Models.Entidades;
using SistemasLegales.Models.Extensores;
using SistemasLegales.Models.Utiles;
using SistemasLegales.Services;

namespace SistemasLegales.Controllers
{
    [Authorize]
    public class RequisitoController : Controller
    {
        private readonly SistemasLegalesContext db;
        private readonly IEmailSender emailSender;
        private readonly IUploadFileService uploadFileService;

        public RequisitoController(SistemasLegalesContext context, IEmailSender emailSender, IUploadFileService uploadFileService)
        {
            db = context;
            this.emailSender = emailSender;
            this.uploadFileService = uploadFileService;
        }

        private async Task<List<Requisito>> ListarRequisitos()
        {
            return await db.Requisito
                    .Include(c => c.OrganismoControl)
                    .Include(c => c.RequisitoLegal)
                    .Include(c => c.Documento)
                    .Include(c => c.Ciudad)
                    .Include(c => c.Proceso)
                    .OrderBy(c => c.IdOrganismoControl).ThenBy(c => c.IdRequisitoLegal).ThenBy(c => c.IdDocumento).ThenBy(c => c.IdCiudad).ThenBy(c => c.IdProceso).ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            var lista = new List<Requisito>();
            try
            {
                var listadoOrganismoControl = await db.OrganismoControl.OrderBy(c => c.Nombre).ToListAsync();
                listadoOrganismoControl.Insert(0, new OrganismoControl { IdOrganismoControl = -1, Nombre = "Todos" });
                ViewData["OrganismoControl"] = new SelectList(listadoOrganismoControl, "IdOrganismoControl", "Nombre");

                var listadoActores = await db.Actor.OrderBy(c => c.Nombres).ToListAsync();
                listadoActores.Insert(0, new Actor { IdActor = -1, Nombres = "Todos" });
                ViewData["Actor"] = new SelectList(listadoActores, "IdActor", "Nombres");

                lista = await ListarRequisitos();
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
            }
            return View(lista);
        }

        [Authorize(Policy = "Gestion")]
        public async Task<IActionResult> Gestionar(int? id)
        {
            try
            {
                ViewBag.accion = id == null ? "Crear" : "Editar";
                ViewData["OrganismoControl"] = new SelectList(await db.OrganismoControl.OrderBy(c => c.Nombre).ToListAsync(), "IdOrganismoControl", "Nombre");
                ViewData["RequisitoLegal"] = new SelectList(await db.RequisitoLegal.OrderBy(c => c.Nombre).ToListAsync(), "IdRequisitoLegal", "Nombre");
                ViewData["Documento"] = new SelectList(await db.Documento.OrderBy(c => c.Nombre).ToListAsync(), "IdDocumento", "Nombre");
                ViewData["Ciudad"] = new SelectList(await db.Ciudad.OrderBy(c => c.Nombre).ToListAsync(), "IdCiudad", "Nombre");
                ViewData["Proceso"] = new SelectList(await db.Proceso.OrderBy(c => c.Nombre).ToListAsync(), "IdProceso", "Nombre");
                ViewData["Actor"] = new SelectList(await db.Actor.OrderBy(c => c.Nombres).ToListAsync(), "IdActor", "Nombres");
                ViewData["Status"] = new SelectList(await db.Status.ToListAsync(), "IdStatus", "Nombre");

                if (id != null)
                {
                    var requisito = await db.Requisito.FirstOrDefaultAsync(c => c.IdRequisito == id);
                    if (requisito == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(requisito);
                }
                return View();
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.ErrorCargarDatos}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Gestion")]
        public async Task<IActionResult> Gestionar(Requisito requisito, IFormFile file)
        {
            try
            {
                ViewBag.accion = requisito.IdRequisito == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    if (requisito.IdRequisito == 0)
                        db.Add(requisito);
                    else
                        db.Update(requisito);

                    await db.SaveChangesAsync();

                    var responseFile = true;
                    if (file != null)
                    {
                        byte[] data;
                        using (var br = new BinaryReader(file.OpenReadStream()))
                            data = br.ReadBytes((int)file.OpenReadStream().Length);

                        if (data.Length > 0)
                        {
                            var activoFijoDocumentoTransfer = new DocumentoRequisitoTransfer { Nombre = file.FileName, Fichero = data, IdRequisito = requisito.IdRequisito };
                            responseFile = await uploadFileService.UploadFiles(activoFijoDocumentoTransfer);
                        }
                    }
                    await requisito.EnviarEmailNotificaion(emailSender, db);
                    return this.Redireccionar(responseFile ? $"{Mensaje.Informacion}|{Mensaje.Satisfactorio}" : $"{Mensaje.Aviso}|{Mensaje.ErrorUploadFiles}");
                }
                ViewData["OrganismoControl"] = new SelectList(await db.OrganismoControl.OrderBy(c => c.Nombre).ToListAsync(), "IdOrganismoControl", "Nombre");
                ViewData["RequisitoLegal"] = new SelectList(await db.RequisitoLegal.OrderBy(c => c.Nombre).ToListAsync(), "IdRequisitoLegal", "Nombre");
                ViewData["Documento"] = new SelectList(await db.Documento.OrderBy(c => c.Nombre).ToListAsync(), "IdDocumento", "Nombre");
                ViewData["Ciudad"] = new SelectList(await db.Ciudad.OrderBy(c => c.Nombre).ToListAsync(), "IdCiudad", "Nombre");
                ViewData["Proceso"] = new SelectList(await db.Proceso.OrderBy(c => c.Nombre).ToListAsync(), "IdProceso", "Nombre");
                ViewData["Actor"] = new SelectList(await db.Actor.OrderBy(c => c.Nombres).ToListAsync(), "IdActor", "Nombres");
                ViewData["Status"] = new SelectList(await db.Status.ToListAsync(), "IdStatus", "Nombre");
                return this.VistaError(requisito, $"{Mensaje.Error}|{Mensaje.ModeloInvalido}");
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.Excepcion}");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Gestion")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var requisito = await db.Requisito.FirstOrDefaultAsync(m => m.IdRequisito == id);
                if (requisito != null)
                {
                    db.Requisito.Remove(requisito);
                    await db.SaveChangesAsync();
                    return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                }
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");
            }
            catch (Exception)
            {
                return this.Redireccionar($"{Mensaje.Error}|{Mensaje.BorradoNoSatisfactorio}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ListadoResult(Requisito requisito)
        {
            var listaRequisitos = new List<Requisito>();
            try
            {
                var lista = await ListarRequisitos();

                if (requisito.IdOrganismoControl != -1)
                    lista = lista.Where(c => c.IdOrganismoControl == requisito.IdOrganismoControl).ToList();

                if (requisito.IdActorResponsableGestSeg != -1)
                    lista = lista.Where(c => c.IdActorResponsableGestSeg == requisito.IdActorResponsableGestSeg).ToList();

                if (requisito.Anno != null)
                    lista = lista.Where(c => c.FechaCumplimiento.Year == requisito.Anno).ToList();

                foreach (var item in lista)
                {
                    int semaforo = item.ObtenerSemaforo();
                    var validarSemaforo = false;

                    if (requisito.SemaforoVerde)
                    {
                        if (semaforo == 1)
                            validarSemaforo = true;
                    }
                    if (requisito.SemaforoAmarillo)
                    {
                        if (semaforo == 2)
                            validarSemaforo = true;
                    }
                    if (requisito.SemaforoRojo)
                    {
                        if (semaforo == 3)
                            validarSemaforo = true;
                    }

                    if (validarSemaforo)
                        listaRequisitos.Add(item);
                }
                return PartialView("_Listado", listaRequisitos);

            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}