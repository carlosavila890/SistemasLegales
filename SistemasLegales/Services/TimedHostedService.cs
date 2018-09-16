using Microsoft.EntityFrameworkCore;
using SistemasLegales.Models.Entidades;
using SistemasLegales.Models.Utiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemasLegales.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly SistemasLegalesContext db;
        private Timer _timer;
        private readonly IEmailSender emailSender;

        public TimedHostedService(SistemasLegalesContext db, IEmailSender emailSender)
        {
            this.db = db;
            this.emailSender = emailSender;
        }

        public Task StartAsync()
        {
            DateTime fechaEjecucionTimer = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, ConstantesTimerEnvioNotificacion.Hora, ConstantesTimerEnvioNotificacion.Minutos, ConstantesTimerEnvioNotificacion.Segundos);
            TimeSpan tiempoEspera = new TimeSpan();

            if (DateTime.Now > fechaEjecucionTimer)
            {
                var fechaMannana = fechaEjecucionTimer.AddDays(1);
                tiempoEspera = fechaMannana - DateTime.Now;
            }
            else
                tiempoEspera = fechaEjecucionTimer - DateTime.Now;

            bool isEjecutarTiempoEspera = true;
            _timer = new Timer(async (state) => {
                await EnviarNotificacionRequisitos(state);

                if (isEjecutarTiempoEspera)
                {
                    _timer.Change(tiempoEspera, TimeSpan.Zero);
                    isEjecutarTiempoEspera = false;
                }
                else
                    _timer.Change(TimeSpan.FromDays(1), TimeSpan.Zero);
            }, null, TimeSpan.Zero, TimeSpan.Zero);
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task EnviarNotificacionRequisitos(object state)
        {
            var listadoRequisitos = await db.Requisito.Where(c => c.FechaCaducidad != null && !c.NotificacionEnviada).ToListAsync();
            foreach (var item in listadoRequisitos)
                await item.EnviarEmailNotificaion(emailSender, db);
        }
    }
}
