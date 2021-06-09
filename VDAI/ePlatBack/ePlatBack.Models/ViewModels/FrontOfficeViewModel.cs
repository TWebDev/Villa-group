using System;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;
using System.Globalization;
using System.Collections.Generic;
using ePlatBack.Models.DataModels;
using ePlatBack.Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace ePlatBack.Models.ViewModels
{
    public class FrontOfficeViewModel
    {
        public class HistorialReservacionesResult
        {
            public decimal? idhuesped { get; set; }
            public decimal idreservacion { get; set; }
            public string numconfirmacion { get; set; }
            public string namepais { get; set; }
            public string nameestado { get; set; }
            public int? numadultos { get; set; }
            public int? numchilds { get; set; }
            public string nameagencia { get; set; }
            public string numdereservacioncrs { get; set; }
            public string numcuenta { get; set; }
            public string mercado { get; set; }
            public string nametipodehabitacion { get; set; }
            public DateTime? llegada { get; set; }
            public DateTime? salida { get; set; }
            public string TipoPlan { get; set; }
            public decimal? Renta { get; set; }
            public decimal? Paquete { get; set; }
            public decimal? Otros { get; set; }
            public decimal? SPA { get; set; }
            public decimal? ConsumoPOS { get; set; }
        }

        public class LlegadasResult
        {
            public decimal? idReservacion { get; set; }
            public int? CuartosNoche { get; set; }
            public string TipoHab { get; set; }
            public DateTime? llegada { get; set; }
            public DateTime? salida { get; set; }
            public string NumHab { get; set; }
            public string numconfirmacion { get; set; }
            public string Procedencia { get; set; }
            public string CodigoMerc { get; set; }
            public decimal? idresort { get; set; }
            public bool? Split { get; set; }
            public string CRS { get; set; }
            public string codeagencia { get; set; }
            public string nameagencia { get; set; }
            public string Huesped { get; set; }
            public int? cuartos { get; set; }
            public int? Adultos { get; set; }
            public int? Ninos { get; set; }
            public string apellidopaterno { get; set; }
            public string apellidomaterno { get; set; }
            public string nombres { get; set; }
            public string codepais { get; set; }
            public string namepais { get; set; }
            public string codigostatusreservacion { get; set; }
            public int X { get; set; }
            public string Titulo { get; set; }
            public int? Infantes { get; set; }
            public string HLlegada { get; set; }
            public string HSalida { get; set; }
            public decimal? idhuesped { get; set; }
            public string DistintivoPrecheckin { get; set; }
            public DateTime? FechaHoraCheckin { get; set; }
            public string Contrato { get; set; }
            public string TipoPlan { get; set; }
            public string Comentario { get; set; }
            public int? idroomlist { get; set; }
            public double? Tarifa { get; set; }
            public string codetipodemoneda { get; set; }
            public string email { get; set; }
            public string telefono { get; set; }
        }

        public class ContactInfo
        {
            public decimal? GuestID { get; set; }
            public string Email { get; set; }
            public string UnformattedEmail { get; set; }
            public string Phone { get; set; }
            public string UnformattedPhone { get; set; }
        }

        public class CorreoHuesped
        {
            public string email { get; set; }
            public string idmember { get; set; }
            public decimal idhuesped { get; set; }
            public string telefono { get; set; }
        }

        public class Distintivo
        {
            public string idDistintivos { get; set; }
            public string NombreDeDistintivos { get; set; }
            public string CodigoDeDistintivos { get; set; }
            public string fechaalta { get; set; }
            public string llegada { get; set; }
            public string salida { get; set; }
            public string numconfirmacion { get; set; }
        }
    }
}
