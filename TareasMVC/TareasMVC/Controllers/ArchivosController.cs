﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/archivos")]
    public class ArchivosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "archivosAdjuntos";

        public ArchivosController(ApplicationDbContext context, IServicioUsuarios servicioUsuarios, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpPost("{tareaId:int}")]
        public async Task<ActionResult<IEnumerable<ArchivoAdjunto>>> Post(int tareaId, [FromBody] IEnumerable<IFormFile> archivos)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId);


            if (tarea is null)
            {
                return NotFound();
            }

            if (tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            var existenArchivosAdjuntos = await context.ArchivosAdjuntos.AnyAsync(a => a.TareaId == tareaId);


            var ordenMayor = 0;
            if (existenArchivosAdjuntos)
            {
                ordenMayor = await context.ArchivosAdjuntos.Where(a => a.TareaId == tareaId)
                    .Select(a => a.Orden).MaxAsync();
            }

            var resultados = await almacenadorArchivos.Almacenar(contenedor, archivos);

            var archivosAdjuntos = resultados.Select((resultado, indice) => new ArchivoAdjunto
            {
                TareaId = tareaId,
                FechaCreacion = DateTime.UtcNow,
                Url = resultado.URL,
                Titulo = resultado.Titulo,
                Orden = indice + ordenMayor + 1,
            }).ToList();
           

            context.AddRange(archivosAdjuntos);
            await context.SaveChangesAsync();

            return archivosAdjuntos.ToList();
        }
    }
}
