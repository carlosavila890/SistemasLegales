﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemasLegales.Models.Entidades
{
    public partial class OrganismoControl
    {
        public OrganismoControl()
        {
            AdminRequisitoLegal = new HashSet<Requisito>();
        }

        public int IdOrganismoControl { get; set; }

        [Required(ErrorMessage = "Debe introducir el {0}.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El {0} no puede tener más de {1} y menos de {2} caracteres.")]
        [Display(Name = "Organismo de control")]
        public string Nombre { get; set; }

        public virtual ICollection<Requisito> AdminRequisitoLegal { get; set; }
    }
}
