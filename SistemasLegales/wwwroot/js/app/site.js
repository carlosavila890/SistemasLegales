MiApp.Generales = function () {
    this.gestionarMsg = function() {
        var mensaje = $("#spanMensaje").html();
        if (mensaje != "" && mensaje != null) {
            var arr_msg = mensaje.split('|');
            MiApp.Mensajes.mostrarNotificacion(arr_msg[0], arr_msg[1]);
        }
    };

    return {
        init: function () {
            gestionarMsg();
        }
    }
}().init();