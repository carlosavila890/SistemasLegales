using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemasLegales.Models.Entidades
{
    public partial class RequisitoLegal
    {
        public RequisitoLegal()
        {
            AdminRequisitoLegal = new HashSet<AdminRequisitoLegal>();
        }

        public int IdRequisitoLegal { get; set; }

        [Required(ErrorMessage = "Debe introducir el {0}.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El {0} no puede tener más de {1} y menos de {2} caracteres.")]
        [Display(Name = "Requisito legal")]
        public string Nombre { get; set; }

        public virtual ICollection<AdminRequisitoLegal> AdminRequisitoLegal { get; set; }
    }
}
