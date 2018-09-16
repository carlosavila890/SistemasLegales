using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemasLegales.Models.Entidades;
using SistemasLegales.Models.Extensores;
using SistemasLegales.Models.Utiles;
namespace SistemasLegales.Controllers
{
    [Authorize]
    public class DocumentoController : Controller
    {
        private readonly SistemasLegalesContext db;

        public DocumentoController(SistemasLegalesContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = new List<Documento>();
            try
            {
                lista = await db.Documento.OrderBy(c => c.Nombre).ToListAsync();
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
                if (id != null)
                {
                    var documento = await db.Documento.FirstOrDefaultAsync(c => c.IdDocumento == id);
                    if (documento == null)
                        return this.Redireccionar($"{Mensaje.Error}|{Mensaje.RegistroNoEncontrado}");

                    return View(documento);
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
        public async Task<IActionResult> Gestionar(Documento documento)
        {
            try
            {
                ViewBag.accion = documento.IdDocumento == 0 ? "Crear" : "Editar";
                if (ModelState.IsValid)
                {
                    var existeRegistro = false;
                    if (documento.IdDocumento == 0)
                    {
                        if (!await db.Documento.AnyAsync(c => c.Nombre.ToUpper().Trim() == documento.Nombre.ToUpper().Trim()))
                            db.Add(documento);
                        else
                            existeRegistro = true;
                    }
                    else
                    {
                        if (!await db.Documento.Where(c => c.Nombre.ToUpper().Trim() == documento.Nombre.ToUpper().Trim()).AnyAsync(c => c.IdDocumento != documento.IdDocumento))
                            db.Update(documento);
                        else
                            existeRegistro = true;
                    }
                    if (!existeRegistro)
                    {
                        await db.SaveChangesAsync();
                        return this.Redireccionar($"{Mensaje.Informacion}|{Mensaje.Satisfactorio}");
                    }
                    else
                        return this.VistaError(documento, $"{Mensaje.Error}|{Mensaje.ExisteRegistro}");
                }
                return this.VistaError(documento, $"{Mensaje.Error}|{Mensaje.ModeloInvalido}");
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
                var documento = await db.Documento.FirstOrDefaultAsync(m => m.IdDocumento == id);
                if (documento != null)
                {
                    db.Documento.Remove(documento);
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
    }
}