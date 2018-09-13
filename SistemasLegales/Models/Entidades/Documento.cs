﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemasLegales.Models.Entidades
{
    public partial class Documento
    {
        public Documento()
        {
            AdminRequisitoLegal = new HashSet<AdminRequisitoLegal>();
        }

        public int IdDocumento { get; set; }

        [Required(ErrorMessage = "Debe introducir el {0}.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El {0} no puede tener más de {1} y menos de {2} caracteres.")]
        [Display(Name = "Documento")]
        public string Nombre { get; set; }

        public virtual ICollection<AdminRequisitoLegal> AdminRequisitoLegal { get; set; }
    }
}