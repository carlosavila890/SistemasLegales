using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemasLegales.Models.Utiles
{
    public static class Mensaje
    {
        public static string Satisfactorio { get { return "La acción se ha realizado satisfactoriamente."; } }
        public static string RegistroNoExiste { get { return "El registro que desea editar no existe."; } }
        public static string ErrorCrear { get { return "Ha ocurrido un error al crear el registro."; } }
        public static string ErrorEditar { get { return "Ha ocurrido un error al editar el registro."; } }
        public static string ErrorListado { get { return "Ha ocurrido un error al cargar el listado."; } }
        public static string Excepcion { get { return "Ha ocurrido una excepción."; } }
        public static string ErrorCargarDatos { get { return "Ha ocurrido un error al cargar los datos."; } }
        public static string ErrorUploadFiles { get { return "Ha ocurrido un error al subir la documentación adicional."; } }
        public static string ErrorReporte { get { return "Ha ocurrido un error al generar el reporte."; } }
        public static string ModeloInvalido { get { return "El modelo es inválido."; } }
        public static string Informacion { get { return "success"; } }
        public static string Error { get { return "error"; } }
        public static string Aviso { get { return "warning"; } }
        public static string ErrorRecursoSolicitado { get { return "No puede acceder al recurso solicitado."; } }
    }
}
