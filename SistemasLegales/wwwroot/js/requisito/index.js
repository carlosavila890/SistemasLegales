﻿MiApp.Requisito = function () {
    var semaforoVerde = false;
    var semaforoAmarillo = false;
    var semaforoRojo = true;

    this.eventoOrganismoControl = function () {
        $("#IdOrganismoControl").on("change", function (e) {
            partialViewListadoTabla();
        });
    };

    this.eventoResponsableGestSeg = function () {
        $("#IdActorResponsableGestSeg").on("change", function (e) {
            partialViewListadoTabla();
        });
    };

    this.eventoFechaAnno = function () {
        MiApp.DatePicker.fnCallbackChangeDatepicker = function () {
            partialViewListadoTabla();
        };
    };

    this.checkUnCheckSemaforo = function (idSemaforo, checkUncheck) {
        if (checkUncheck == true) {
            $("#semaforo" + idSemaforo).html("check");
        }
        else {
            $("#semaforo" + idSemaforo).html("");
        }
    }

    this.eventoSemaforoEstado = function () {
        $(".semaforo").on("click", function (e) {
            var semaforo = $(e.currentTarget);
            var idSemaforo = semaforo.data("semaforo");

            switch (idSemaforo) {
                case 1: {
                    semaforoVerde = !semaforoVerde;
                    checkUnCheckSemaforo(idSemaforo, semaforoVerde);
                    break;
                }
                case 2: {
                    semaforoAmarillo = !semaforoAmarillo;
                    checkUnCheckSemaforo(idSemaforo, semaforoAmarillo);
                    break;
                }
                case 3: {
                    semaforoRojo = !semaforoRojo;
                    checkUnCheckSemaforo(idSemaforo, semaforoRojo);
                    break;
                }
            }
            partialViewListadoTabla();
        });
    };

    this.partialViewListadoTabla = function () {
        MiApp.LoadingPanel.mostrarNotificacion("bodyTemplate", "Cargando datos...");
        $.ajax({
            url: urlListadoResult,
            method: "POST",
            data: {
                requisito: {
                    Documento: {
                        RequisitoLegal: {
                            IdOrganismoControl: $("#IdOrganismoControl").val(),
                        }
                    },
                    IdActorResponsableGestSeg: $("#IdActorResponsableGestSeg").val(),
                    Anno: $("#Anno").val(),
                    SemaforoVerde: semaforoVerde,
                    SemaforoAmarillo: semaforoAmarillo,
                    SemaforoRojo: semaforoRojo
                }
            },
            success: function (data) {
                $("#divTablaListado").html(data);
                MiApp.Datatables.inicializarDatatable();
            },
            error: function (errorMessage) {
                MiApp.Mensajes.mostrarNotificacion("error", "Ocurrió un error al cargar los datos, inténtelo nuevamente.");
            },
            complete: function () {
                $("#bodyTemplate").waitMe("hide");
            }
        });
    };

    return {
        init: function () {
            eventoOrganismoControl();
            eventoResponsableGestSeg();
            eventoFechaAnno();
            eventoSemaforoEstado();
            partialViewListadoTabla();
        }
    }
}();
MiApp.Requisito.init();