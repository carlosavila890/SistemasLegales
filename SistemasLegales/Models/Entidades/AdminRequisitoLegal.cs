using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemasLegales.Models.Entidades
{
    public partial class AdminRequisitoLegal
    {
        public int IdAdminRequisitoLegal { get; set; }

        [Display(Name = "Organismo de control")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdOrganismoControl { get; set; }
        public virtual OrganismoControl OrganismoControl { get; set; }

        [Display(Name = "Requisito legal")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdRequisitoLegal { get; set; }
        public virtual RequisitoLegal RequisitoLegal { get; set; }

        [Display(Name = "Documento")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdDocumento { get; set; }
        public virtual Documento Documento { get; set; }

        [Display(Name = "Ciudad")]
        [Required(ErrorMessage = "Debe seleccionar la {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar la {0}.")]
        public int IdCiudad { get; set; }
        public virtual Ciudad Ciudad { get; set; }

        [Display(Name = "Proceso")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdProceso { get; set; }
        public virtual Proceso Proceso { get; set; }

        [Display(Name = "Dueño del proceso")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdActorDuennoProceso { get; set; }
        public virtual Actor ActorDuennoProceso { get; set; }

        [Display(Name = "Responsable de gestión y seguimiento")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdActorResponsableGestSeg { get; set; }
        public virtual Actor ActorResponsableGestSeg { get; set; }

        [Display(Name = "Custodio de documento")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdActorCustodioDocumento { get; set; }
        public virtual Actor ActorCustodioDocumento { get; set; }

        [Required(ErrorMessage = "Debe introducir la {0}")]
        [Display(Name = "Fecha de cumplimiento")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCumplimiento { get; set; }

        [Display(Name = "Fecha de caducidad")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCaducidad { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Debe seleccionar el {0}.")]
        [Range(1, double.MaxValue, ErrorMessage = "Debe seleccionar el {0}.")]
        public int IdStatus { get; set; }
        public virtual Status Status { get; set; }

        [Display(Name = "Duración del trámite (Días)")]
        [Required(ErrorMessage = "Debe introducir la {0}.")]
        public int DuracionTramite { get; set; }

        [Display(Name = "Nro. días para notificación")]
        [Required(ErrorMessage = "Debe introducir el {0}.")]
        public int DiasNotificacion { get; set; }

        [Required(ErrorMessage = "Debe introducir el {0}.")]
        [EmailAddress(ErrorMessage = "El {0} es inválido.")]
        [Display(Name = "Correo notificación 1")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El {0} no puede tener más de {1} y menos de {2} caracteres.")]
        public string EmailNotificacion1 { get; set; }

        [Required(ErrorMessage = "Debe introducir el {0}.")]
        [EmailAddress(ErrorMessage = "El {0} es inválido.")]
        [Display(Name = "Correo notificación 2")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El {0} no puede tener más de {1} y menos de {2} caracteres.")]
        public string EmailNotificacion2 { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Las {0} no pueden tener más de {1} y menos de {2} caracteres.")]
        public string Observaciones { get; set; }
    }
}
