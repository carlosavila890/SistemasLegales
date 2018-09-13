using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemasLegales.Models.Entidades;
using SistemasLegales.Models.Utiles;

namespace SistemasLegales.Controllers
{
    public class OrganismoControlController : Controller
    {
        private readonly SistemasLegalesContext db;

        public OrganismoControlController(SistemasLegalesContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index()
        {
            var lista = new List<OrganismoControl>();
            try
            {
                lista = await db.OrganismoControl.OrderBy(c=> c.Nombre).ToListAsync();
            }
            catch (Exception)
            {
                TempData["Mensaje"] = $"{Mensaje.Error}|{Mensaje.ErrorListado}";
            }
            return View(lista);
        }
    }
}