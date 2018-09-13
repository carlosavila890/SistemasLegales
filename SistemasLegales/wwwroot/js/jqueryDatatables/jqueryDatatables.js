MiApp.Datatables = function () {
    return {
        init: function (idTabla) {
            this.eventoBtnEliminar();
            this.inicializarDatatable(idTabla);            
        },

        inicializarDatatable: function (idTabla) {
            var idTable = idTabla && idTabla != null ? idTabla : 'datatable-responsive';
            $('#' + idTable).DataTable();
        },

        eventoBtnEliminar: function () {
            $(".btnEliminar").on("click", function (e) {
                var btnEliminar = $(e.currentTarget);
                var descripcion = btnEliminar.data("descripcion");
                var id = btnEliminar.prop("id");
                MiApp.Bootbox.init("Eliminar", descripcion, null, [], {
                    isGuardar: true, hideAlGuardar: true, callbackGuardar: function () {
                        alert("Aceptar");
                    }
                });
            });
        }
    }
}();