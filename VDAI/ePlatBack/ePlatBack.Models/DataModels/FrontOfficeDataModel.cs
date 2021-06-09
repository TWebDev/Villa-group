using System;
using System.IO;
using System.Web;
using System.Net;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Mvc;
using System.Reflection;
using System.Data.Common;
using System.Data.Objects;
using System.Web.Routing;
using System.Web.Security;
using System.Linq.Dynamic;
using System.Globalization;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils.Custom;
using ePlatBack.Models.Utils.Custom.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;

namespace ePlatBack.Models.DataModels
{
    public class FrontOfficeDataModel
    {
        public static List<FrontOfficeViewModel.HistorialReservacionesResult> GetReservationsHistory(int frontOfficeResortID, int? GuestID = null, string Email = null)
        {
            List<FrontOfficeViewModel.HistorialReservacionesResult> result = new List<FrontOfficeViewModel.HistorialReservacionesResult>();

            string body = "";

            try
            {
                var client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/HistorialReservaciones/" + frontOfficeResortID + "/" + (Email ?? "") + "/" + (GuestID ?? 0).ToString());
                str = str.Replace("{\"result\":[", "")
        .Replace("]]}", "]");
                var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.HistorialReservacionesResult>>(str);
                result = (from a in _result
                          select new FrontOfficeViewModel.HistorialReservacionesResult()
                          {
                              idhuesped = a.idhuesped,
                              idreservacion = a.idreservacion,
                              numconfirmacion = a.numconfirmacion,
                              namepais = a.namepais,
                              nameestado = a.nameestado,
                              numadultos = a.numadultos,
                              numchilds = a.numchilds,
                              nameagencia = a.nameagencia,
                              numdereservacioncrs = "",
                              numcuenta = a.numcuenta,
                              mercado = a.mercado,
                              nametipodehabitacion = a.nametipodehabitacion,
                              llegada = a.llegada,
                              salida = a.salida,
                              TipoPlan = a.TipoPlan,
                              Renta = a.Renta,
                              Paquete = a.Paquete,
                              Otros = a.Otros,
                              SPA = a.SPA,
                              ConsumoPOS = (decimal?)a.ConsumoPOS
                          }).ToList();


                //switch (frontOfficeResortID)
                //{
                //case 9://Garza Blanca PV
                //    FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBPV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
                //    frontGBPV.CommandTimeout = 120;
                //    result = (from a in frontGBPV.spHistorialReservaciones(GuestID, Email)
                //              select new FrontOfficeViewModel.HistorialReservacionesResult()
                //              {
                //                  idhuesped = a.idhuesped,
                //                  idreservacion = a.idreservacion,
                //                  numconfirmacion = a.numconfirmacion,
                //                  namepais = a.namepais,
                //                  nameestado = a.nameestado,
                //                  numadultos = a.numadultos,
                //                  numchilds = a.numchilds,
                //                  nameagencia = a.nameagencia,
                //                  numdereservacioncrs = "",
                //                  numcuenta = a.numcuenta,
                //                  mercado = a.mercado,
                //                  nametipodehabitacion = a.nametipodehabitacion,
                //                  llegada = a.llegada,
                //                  salida = a.salida,
                //                  TipoPlan = a.TipoPlan,
                //                  Renta = a.Renta,
                //                  Paquete = a.Paquete,
                //                  Otros = a.Otros,
                //                  SPA = a.SPA,
                //                  ConsumoPOS = (decimal?)a.ConsumoPOS
                //              }).ToList();
                //    break;
                //case 11://VDP Cancun
                //    FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities frontVDPN = new FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities();
                //    frontVDPN.CommandTimeout = 120;
                //    result = (from a in frontVDPN.spHistorialReservaciones(GuestID, Email)
                //              select new FrontOfficeViewModel.HistorialReservacionesResult()
                //              {
                //                  idhuesped = a.idhuesped,
                //                  idreservacion = a.idreservacion,
                //                  numconfirmacion = a.numconfirmacion,
                //                  namepais = a.namepais,
                //                  nameestado = a.nameestado,
                //                  numadultos = a.numadultos,
                //                  numchilds = a.numchilds,
                //                  nameagencia = a.nameagencia,
                //                  numdereservacioncrs = "",
                //                  numcuenta = "",
                //                  mercado = a.mercado,
                //                  nametipodehabitacion = a.nametipodehabitacion,
                //                  llegada = a.llegada,
                //                  salida = a.salida,
                //                  TipoPlan = a.TipoPlan,
                //                  Renta = a.Renta,
                //                  Paquete = a.Paquete,
                //                  Otros = a.Otros,
                //                  SPA = a.SPA,
                //                  ConsumoPOS = (decimal?)a.ConsumoPOS
                //              }).ToList();
                //    break;
                //case 13://Mousai PV
                //    FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities frontHMPV = new FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities();
                //    frontHMPV.CommandTimeout = 120;
                //    result = (from a in frontHMPV.spHistorialReservaciones(GuestID, Email)
                //              select new FrontOfficeViewModel.HistorialReservacionesResult()
                //              {
                //                  idhuesped = a.idhuesped,
                //                  idreservacion = a.idreservacion,
                //                  numconfirmacion = a.numconfirmacion,
                //                  namepais = a.namepais,
                //                  nameestado = a.nameestado,
                //                  numadultos = a.numadultos,
                //                  numchilds = a.numchilds,
                //                  nameagencia = a.nameagencia,
                //                  numdereservacioncrs = a.numdereservacioncrs,
                //                  numcuenta = a.numcuenta,
                //                  mercado = a.mercado,
                //                  nametipodehabitacion = a.nametipodehabitacion,
                //                  llegada = a.llegada,
                //                  salida = a.salida,
                //                  TipoPlan = a.TipoPlan,
                //                  Renta = a.Renta,
                //                  Paquete = a.Paquete,
                //                  Otros = a.Otros,
                //                  SPA = a.SPA,
                //                  ConsumoPOS = (decimal?)a.ConsumoPOS
                //              }).ToList();
                //    break;
                //case 15://Garza Blanca Cabo

                //    FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities frontGBC = new FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities();
                //    frontGBC.CommandTimeout = 60;
                //    result = (from a in frontGBC.spHistorialReservaciones(GuestID, Email)
                //              select new FrontOfficeViewModel.HistorialReservacionesResult()
                //              {
                //                  idhuesped = a.idhuesped,
                //                  idreservacion = a.idreservacion,
                //                  numconfirmacion = a.numconfirmacion,
                //                  namepais = a.namepais,
                //                  nameestado = a.nameestado,
                //                  numadultos = a.numadultos,
                //                  numchilds = a.numchilds,
                //                  nameagencia = a.nameagencia,
                //                  numdereservacioncrs = "",
                //                  numcuenta = a.numcuenta,
                //                  mercado = a.mercado,
                //                  nametipodehabitacion = a.nametipodehabitacion,
                //                  llegada = a.llegada,
                //                  salida = a.salida,
                //                  TipoPlan = a.TipoPlan,
                //                  Renta = a.Renta,
                //                  Paquete = a.Paquete,
                //                  Otros = a.Otros,
                //                  SPA = a.SPA,
                //                  ConsumoPOS = (decimal?)a.ConsumoPOS
                //              }).ToList();
                //    break;
                //case 1://Villa del Palmar Vallarta
                //case 3://Villa del Palmar Flamingos
                //case 4://Villa del Palmar Cabo
                //case 5://Villa la Estancia Cabo
                //case 6://Villa del Arco Cabo
                //case 7://Villa la Estancia Nuevo Vallarta
                //case 12://Villa del Palmar Loreto
                //case 9:
                //case 11:
                //case 13:
                //case 15:
                //        var client = new WebClient();
                //        client.UseDefaultCredentials = true;
                //        client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                //        var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/HistorialReservaciones/" + frontOfficeResortID + "/" + (Email ?? "") + "/" + (GuestID ?? 0).ToString());
                //        str = str.Replace("{\"result\":[", "")
                //.Replace("]]}", "]");
                //        var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.HistorialReservacionesResult>>(str);
                //        result = (from a in _result
                //                  select new FrontOfficeViewModel.HistorialReservacionesResult()
                //                  {
                //                      idhuesped = a.idhuesped,
                //                      idreservacion = a.idreservacion,
                //                      numconfirmacion = a.numconfirmacion,
                //                      namepais = a.namepais,
                //                      nameestado = a.nameestado,
                //                      numadultos = a.numadultos,
                //                      numchilds = a.numchilds,
                //                      nameagencia = a.nameagencia,
                //                      numdereservacioncrs = "",
                //                      numcuenta = a.numcuenta,
                //                      mercado = a.mercado,
                //                      nametipodehabitacion = a.nametipodehabitacion,
                //                      llegada = a.llegada,
                //                      salida = a.salida,
                //                      TipoPlan = a.TipoPlan,
                //                      Renta = a.Renta,
                //                      Paquete = a.Paquete,
                //                      Otros = a.Otros,
                //                      SPA = a.SPA,
                //                      ConsumoPOS = (decimal?)a.ConsumoPOS
                //                  }).ToList();
                //break;
                //}
            }
            catch (Exception x)
            {
                body += "<p>Method: GetReservationsHistory<br>Email: " + Email + "<br> Resort: " + frontOfficeResortID + "<br>Error: " + x.InnerException + "</p>";
            }

            if (body != "")
            {
                System.Net.Mail.MailMessage message = Utils.EmailNotifications.GetSystemEmail(body, "gguerrap@villagroup.com");
                //Utils.EmailNotifications.Send(message);
            }

            return result;
        }
        public static List<FrontOfficeViewModel.HistorialReservacionesResult> _GetReservationsHistory(int frontOfficeResortID, int? GuestID = null, string Email = null)
        {
            List<FrontOfficeViewModel.HistorialReservacionesResult> result = new List<FrontOfficeViewModel.HistorialReservacionesResult>();

            string body = "";

            try
            {
                switch (frontOfficeResortID)
                {
                    case 9://Garza Blanca PV
                        FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBPV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
                        frontGBPV.CommandTimeout = 120;
                        result = (from a in frontGBPV.spHistorialReservaciones(GuestID, Email)
                                  select new FrontOfficeViewModel.HistorialReservacionesResult()
                                  {
                                      idhuesped = a.idhuesped,
                                      idreservacion = a.idreservacion,
                                      numconfirmacion = a.numconfirmacion,
                                      namepais = a.namepais,
                                      nameestado = a.nameestado,
                                      numadultos = a.numadultos,
                                      numchilds = a.numchilds,
                                      nameagencia = a.nameagencia,
                                      numdereservacioncrs = "",
                                      numcuenta = a.numcuenta,
                                      mercado = a.mercado,
                                      nametipodehabitacion = a.nametipodehabitacion,
                                      llegada = a.llegada,
                                      salida = a.salida,
                                      TipoPlan = a.TipoPlan,
                                      Renta = a.Renta,
                                      Paquete = a.Paquete,
                                      Otros = a.Otros,
                                      SPA = a.SPA,
                                      ConsumoPOS = (decimal?)a.ConsumoPOS
                                  }).ToList();
                        break;
                    case 11://VDP Cancun
                        FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities frontVDPN = new FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities();
                        frontVDPN.CommandTimeout = 120;
                        result = (from a in frontVDPN.spHistorialReservaciones(GuestID, Email)
                                  select new FrontOfficeViewModel.HistorialReservacionesResult()
                                  {
                                      idhuesped = a.idhuesped,
                                      idreservacion = a.idreservacion,
                                      numconfirmacion = a.numconfirmacion,
                                      namepais = a.namepais,
                                      nameestado = a.nameestado,
                                      numadultos = a.numadultos,
                                      numchilds = a.numchilds,
                                      nameagencia = a.nameagencia,
                                      numdereservacioncrs = "",
                                      numcuenta = "",
                                      mercado = a.mercado,
                                      nametipodehabitacion = a.nametipodehabitacion,
                                      llegada = a.llegada,
                                      salida = a.salida,
                                      TipoPlan = a.TipoPlan,
                                      Renta = a.Renta,
                                      Paquete = a.Paquete,
                                      Otros = a.Otros,
                                      SPA = a.SPA,
                                      ConsumoPOS = (decimal?)a.ConsumoPOS
                                  }).ToList();
                        break;
                    case 13://Mousai PV
                        FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities frontHMPV = new FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities();
                        frontHMPV.CommandTimeout = 120;
                        result = (from a in frontHMPV.spHistorialReservaciones(GuestID, Email)
                                  select new FrontOfficeViewModel.HistorialReservacionesResult()
                                  {
                                      idhuesped = a.idhuesped,
                                      idreservacion = a.idreservacion,
                                      numconfirmacion = a.numconfirmacion,
                                      namepais = a.namepais,
                                      nameestado = a.nameestado,
                                      numadultos = a.numadultos,
                                      numchilds = a.numchilds,
                                      nameagencia = a.nameagencia,
                                      numdereservacioncrs = a.numdereservacioncrs,
                                      numcuenta = a.numcuenta,
                                      mercado = a.mercado,
                                      nametipodehabitacion = a.nametipodehabitacion,
                                      llegada = a.llegada,
                                      salida = a.salida,
                                      TipoPlan = a.TipoPlan,
                                      Renta = a.Renta,
                                      Paquete = a.Paquete,
                                      Otros = a.Otros,
                                      SPA = a.SPA,
                                      ConsumoPOS = (decimal?)a.ConsumoPOS
                                  }).ToList();
                        break;
                    case 15://Garza Blanca Cabo

                        FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities frontGBC = new FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities();
                        frontGBC.CommandTimeout = 60;
                        result = (from a in frontGBC.spHistorialReservaciones(GuestID, Email)
                                  select new FrontOfficeViewModel.HistorialReservacionesResult()
                                  {
                                      idhuesped = a.idhuesped,
                                      idreservacion = a.idreservacion,
                                      numconfirmacion = a.numconfirmacion,
                                      namepais = a.namepais,
                                      nameestado = a.nameestado,
                                      numadultos = a.numadultos,
                                      numchilds = a.numchilds,
                                      nameagencia = a.nameagencia,
                                      numdereservacioncrs = "",
                                      numcuenta = a.numcuenta,
                                      mercado = a.mercado,
                                      nametipodehabitacion = a.nametipodehabitacion,
                                      llegada = a.llegada,
                                      salida = a.salida,
                                      TipoPlan = a.TipoPlan,
                                      Renta = a.Renta,
                                      Paquete = a.Paquete,
                                      Otros = a.Otros,
                                      SPA = a.SPA,
                                      ConsumoPOS = (decimal?)a.ConsumoPOS
                                  }).ToList();
                        break;
                    case 1://Villa del Palmar Vallarta
                    case 3://Villa del Palmar Flamingos
                    case 4://Villa del Palmar Cabo
                    case 5://Villa la Estancia Cabo
                    case 6://Villa del Arco Cabo
                    case 7://Villa la Estancia Nuevo Vallarta
                    case 12://Villa del Palmar Loreto
                        var client = new WebClient();
                        client.UseDefaultCredentials = true;
                        client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                        var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/HistorialReservaciones/" + frontOfficeResortID + "/" + (Email ?? "") + "/" + (GuestID ?? 0).ToString());
                        var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.HistorialReservacionesResult>>(str);
                        result = (from a in _result
                                  select new FrontOfficeViewModel.HistorialReservacionesResult()
                                  {
                                      idhuesped = a.idhuesped,
                                      idreservacion = a.idreservacion,
                                      numconfirmacion = a.numconfirmacion,
                                      namepais = a.namepais,
                                      nameestado = a.nameestado,
                                      numadultos = a.numadultos,
                                      numchilds = a.numchilds,
                                      nameagencia = a.nameagencia,
                                      numdereservacioncrs = "",
                                      numcuenta = a.numcuenta,
                                      mercado = a.mercado,
                                      nametipodehabitacion = a.nametipodehabitacion,
                                      llegada = a.llegada,
                                      salida = a.salida,
                                      TipoPlan = a.TipoPlan,
                                      Renta = a.Renta,
                                      Paquete = a.Paquete,
                                      Otros = a.Otros,
                                      SPA = a.SPA,
                                      ConsumoPOS = (decimal?)a.ConsumoPOS
                                  }).ToList();
                        break;
                }
            }
            catch (Exception x)
            {
                body += "<p>Method: GetReservationsHistory<br>Email: " + Email + "<br> Resort: " + frontOfficeResortID + "<br>Error: " + x.InnerException + "</p>";
            }

            if (body != "")
            {
                System.Net.Mail.MailMessage message = Utils.EmailNotifications.GetSystemEmail(body, "gguerrap@villagroup.com");
                //Utils.EmailNotifications.Send(message);
            }

            return result;
        }

        public static List<FrontOfficeViewModel.LlegadasResult> GetArrivals(int frontOfficeResortID, DateTime fromDate, DateTime toDate)
        {
            List<FrontOfficeViewModel.LlegadasResult> result = new List<FrontOfficeViewModel.LlegadasResult>();
            var serializer = new JavaScriptSerializer();
            var client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
            serializer.MaxJsonLength = Int32.MaxValue;
            //("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss.fffffffK")
            var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/Llegadas/" + frontOfficeResortID + "/" + fromDate.ToString("yyyy-MM-ddTHH’:’mm’:’ss") + "/" + toDate.ToString("yyyy-MM-ddTHH’:’mm’:’ss"));
            //var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/Llegadas/" + frontOfficeResortID + "/" + fromDate.ToString("yyyy-MM-dd") + "T00:00:00/" + toDate.ToString("yyyy-MM-dd") + "T00:00:00");
            str = str.Replace("{\"result\":[", "")
                .Replace("]]}", "]");
            var _result = serializer.Deserialize<List<FrontOfficeViewModel.LlegadasResult>>(str);
            result = (from a in _result
                      select new FrontOfficeViewModel.LlegadasResult()
                      {
                          idReservacion = a.idReservacion,
                          CuartosNoche = a.CuartosNoche,
                          TipoHab = a.TipoHab,
                          llegada = a.llegada,
                          salida = a.salida,
                          NumHab = a.NumHab,
                          numconfirmacion = a.numconfirmacion,
                          Procedencia = a.Procedencia,
                          CodigoMerc = a.CodigoMerc,
                          idresort = a.idresort,
                          Split = a.Split,
                          CRS = a.CRS,
                          codeagencia = a.codeagencia,
                          nameagencia = a.nameagencia,
                          Huesped = a.Huesped,
                          cuartos = a.cuartos,
                          Adultos = a.Adultos,
                          Ninos = a.Ninos,
                          apellidopaterno = a.apellidopaterno,
                          apellidomaterno = a.apellidomaterno,
                          nombres = a.nombres,
                          codepais = a.codepais,
                          namepais = a.namepais,
                          codigostatusreservacion = a.codigostatusreservacion,
                          X = a.X,
                          Titulo = a.Titulo,
                          Infantes = a.Infantes,
                          HLlegada = a.HLlegada,
                          HSalida = a.HSalida,
                          idhuesped = a.idhuesped,
                          DistintivoPrecheckin = a.DistintivoPrecheckin,
                          FechaHoraCheckin = a.FechaHoraCheckin,
                          Contrato = a.Contrato,
                          TipoPlan = a.TipoPlan,
                          Comentario = a.Comentario,
                          idroomlist = a.idroomlist,
                          Tarifa = a.Tarifa,
                          codetipodemoneda = a.codetipodemoneda,
                          telefono = a.telefono,
                          email = a.email
                      }).ToList();

            return result;
        }

        public static List<FrontOfficeViewModel.LlegadasResult> _GetArrivals(int frontOfficeResortID, DateTime fromDate, DateTime toDate)
        {
            List<FrontOfficeViewModel.LlegadasResult> result = new List<FrontOfficeViewModel.LlegadasResult>();

            switch (frontOfficeResortID)
            {
                case 1: //Villa del Palmar Vallarta
                    FrontOfficeVDPVEntities frontVDPV = new FrontOfficeVDPVEntities();
                    result = (from a in frontVDPV.spLlegadas(fromDate, toDate)
                              select new FrontOfficeViewModel.LlegadasResult()
                              {
                                  idReservacion = a.idReservacion,
                                  CuartosNoche = a.CuartosNoche,
                                  TipoHab = a.TipoHab,
                                  llegada = a.llegada,
                                  salida = a.salida,
                                  NumHab = a.NumHab,
                                  numconfirmacion = a.numconfirmacion,
                                  Procedencia = a.Procedencia,
                                  CodigoMerc = a.CodigoMerc,
                                  idresort = a.idresort,
                                  Split = a.Split,
                                  CRS = a.CRS,
                                  codeagencia = a.codeagencia,
                                  nameagencia = a.nameagencia,
                                  Huesped = a.Huesped,
                                  cuartos = a.cuartos,
                                  Adultos = a.Adultos,
                                  Ninos = a.Ninos,
                                  apellidopaterno = a.apellidopaterno,
                                  apellidomaterno = a.apellidomaterno,
                                  nombres = a.nombres,
                                  codepais = a.codepais,
                                  namepais = a.namepais,
                                  codigostatusreservacion = a.codigostatusreservacion,
                                  X = a.X,
                                  Titulo = a.Titulo,
                                  Infantes = a.Infantes,
                                  HLlegada = a.HLlegada,
                                  HSalida = a.HSalida,
                                  idhuesped = a.idhuesped,
                                  DistintivoPrecheckin = a.DistintivoPrecheckin,
                                  FechaHoraCheckin = a.FechaHoraCheckin,
                                  Contrato = a.Contrato,
                                  TipoPlan = a.TipoPlan
                              }).ToList();

                    break;
                case 4: //Villa del Palmar Cabo
                case 3: //Villa del Palmar Flamingos
                case 5: //Villa La Estancia Cabo
                case 6: //Villa del Arco Cabo
                case 7: //Villa La Estancia Nuevo Vallarta
                case 12: //Villa del Palmar Loreto
                    var client = new WebClient();
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                    var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/Llegadas/" + frontOfficeResortID + "/" + fromDate.ToString("yyyy-MM-dd") + "T00:00:00/" + toDate.ToString("yyyy-MM-dd") + "T00:00:00");
                    str = str.Replace("{\"result\":[", "")
                        .Replace("]]}", "]");
                    var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.LlegadasResult>>(str);
                    result = (from a in _result
                              select new FrontOfficeViewModel.LlegadasResult()
                              {
                                  idReservacion = a.idReservacion,
                                  CuartosNoche = a.CuartosNoche,
                                  TipoHab = a.TipoHab,
                                  llegada = a.llegada,
                                  salida = a.salida,
                                  NumHab = a.NumHab,
                                  numconfirmacion = a.numconfirmacion,
                                  Procedencia = a.Procedencia,
                                  CodigoMerc = a.CodigoMerc,
                                  idresort = a.idresort,
                                  Split = a.Split,
                                  CRS = a.CRS,
                                  codeagencia = a.codeagencia,
                                  nameagencia = a.nameagencia,
                                  Huesped = a.Huesped,
                                  cuartos = a.cuartos,
                                  Adultos = a.Adultos,
                                  Ninos = a.Ninos,
                                  apellidopaterno = a.apellidopaterno,
                                  apellidomaterno = a.apellidomaterno,
                                  nombres = a.nombres,
                                  codepais = a.codepais,
                                  namepais = a.namepais,
                                  codigostatusreservacion = a.codigostatusreservacion,
                                  X = a.X,
                                  Titulo = a.Titulo,
                                  Infantes = a.Infantes,
                                  HLlegada = a.HLlegada,
                                  HSalida = a.HSalida,
                                  idhuesped = a.idhuesped,
                                  DistintivoPrecheckin = a.DistintivoPrecheckin,
                                  FechaHoraCheckin = a.FechaHoraCheckin,
                                  Contrato = a.Contrato,
                                  TipoPlan = a.TipoPlan,
                                  Comentario = a.Comentario,
                                  idroomlist = a.idroomlist,
                                  Tarifa = a.Tarifa,
                                  codetipodemoneda = a.codetipodemoneda
                              }).ToList();

                    break;
                case 9: //Garza Blanca Vallarta
                    FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBPV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
                    frontGBPV.CommandTimeout = int.MaxValue;
                    result = (from a in frontGBPV.spLlegadas(fromDate, toDate)
                              select new FrontOfficeViewModel.LlegadasResult()
                              {
                                  idReservacion = a.idReservacion,
                                  CuartosNoche = a.CuartosNoche,
                                  TipoHab = a.TipoHab,
                                  llegada = a.llegada,
                                  salida = a.salida,
                                  NumHab = a.NumHab,
                                  numconfirmacion = a.numconfirmacion,
                                  Procedencia = a.Procedencia,
                                  CodigoMerc = a.CodigoMerc,
                                  idresort = a.idresort,
                                  Split = a.Split,
                                  CRS = a.CRS,
                                  codeagencia = a.codeagencia,
                                  nameagencia = a.nameagencia,
                                  Huesped = a.Huesped,
                                  cuartos = a.cuartos,
                                  Adultos = a.Adultos,
                                  Ninos = a.Ninos,
                                  apellidopaterno = a.apellidopaterno,
                                  apellidomaterno = a.apellidomaterno,
                                  nombres = a.nombres,
                                  codepais = a.codepais,
                                  namepais = a.namepais,
                                  codigostatusreservacion = a.codigostatusreservacion,
                                  X = a.X,
                                  Titulo = a.Titulo,
                                  Infantes = a.Infantes,
                                  HLlegada = a.HLlegada,
                                  HSalida = a.HSalida,
                                  idhuesped = a.IdHuesped,
                                  DistintivoPrecheckin = a.DistintivoPrecheckin,
                                  FechaHoraCheckin = a.FechaHoraCheckin,
                                  Contrato = a.Contrato,
                                  TipoPlan = a.TipoPlan,
                                  Comentario = a.Comentario,
                                  idroomlist = a.idroomlist,
                                  Tarifa = a.Tarifa,
                                  codetipodemoneda = a.codetipodemoneda
                              }).ToList();

                    break;
                case 11: //Villa del Palmar Cancun
                    FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities frontVDPN = new FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities();
                    result = (from a in frontVDPN.spLlegadas(fromDate, toDate)
                              select new FrontOfficeViewModel.LlegadasResult()
                              {
                                  idReservacion = a.idReservacion,
                                  CuartosNoche = a.CuartosNoche,
                                  TipoHab = a.TipoHab,
                                  llegada = a.llegada,
                                  salida = a.salida,
                                  NumHab = a.NumHab,
                                  numconfirmacion = a.numconfirmacion,
                                  Procedencia = a.Procedencia,
                                  CodigoMerc = a.CodigoMerc,
                                  idresort = a.idresort,
                                  Split = a.Split,
                                  CRS = a.CRS,
                                  codeagencia = a.codeagencia,
                                  nameagencia = a.nameagencia,
                                  Huesped = a.Huesped,
                                  cuartos = a.cuartos,
                                  Adultos = a.Adultos,
                                  Ninos = a.Ninos,
                                  apellidopaterno = a.apellidopaterno,
                                  apellidomaterno = a.apellidomaterno,
                                  nombres = a.nombres,
                                  codepais = a.codepais,
                                  namepais = a.namepais,
                                  codigostatusreservacion = a.codigostatusreservacion,
                                  X = a.X,
                                  Titulo = a.Titulo,
                                  Infantes = a.Infantes,
                                  HLlegada = a.HLlegada,
                                  HSalida = a.HSalida,
                                  idhuesped = a.idhuesped,
                                  DistintivoPrecheckin = a.DistintivoPrecheckin,
                                  FechaHoraCheckin = a.FechaHoraCheckin,
                                  Contrato = a.Contrato,
                                  TipoPlan = a.TipoPlan,
                                  Comentario = a.Comentario,
                                  idroomlist = a.idroomlist,
                                  Tarifa = a.Tarifa,
                                  codetipodemoneda = a.codetipodemoneda
                              }).ToList();


                    break;
                case 13: //Mousai PV
                    FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities frontHMPV = new FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities();
                    result = (from a in frontHMPV.spLlegadas(fromDate, toDate)
                              select new FrontOfficeViewModel.LlegadasResult()
                              {
                                  idReservacion = a.idReservacion,
                                  CuartosNoche = a.CuartosNoche,
                                  TipoHab = a.TipoHab,
                                  llegada = a.llegada,
                                  salida = a.salida,
                                  NumHab = a.NumHab,
                                  numconfirmacion = a.numconfirmacion,
                                  Procedencia = a.Procedencia,
                                  CodigoMerc = a.CodigoMerc,
                                  idresort = a.idresort,
                                  Split = a.Split,
                                  CRS = a.CRS,
                                  codeagencia = a.codeagencia,
                                  nameagencia = a.nameagencia,
                                  Huesped = a.Huesped,
                                  cuartos = a.cuartos,
                                  Adultos = a.Adultos,
                                  Ninos = a.Ninos,
                                  apellidopaterno = a.apellidopaterno,
                                  apellidomaterno = a.apellidomaterno,
                                  nombres = a.nombres,
                                  codepais = a.codepais,
                                  namepais = a.namepais,
                                  codigostatusreservacion = a.codigostatusreservacion,
                                  X = a.X,
                                  Titulo = a.Titulo,
                                  Infantes = a.Infantes,
                                  HLlegada = a.HLlegada,
                                  HSalida = a.HSalida,
                                  idhuesped = a.IdHuesped,
                                  DistintivoPrecheckin = a.DistintivoPrecheckin,
                                  FechaHoraCheckin = a.FechaHoraCheckin,
                                  Contrato = a.Contrato,
                                  TipoPlan = a.TipoPlan,
                                  Comentario = a.Comentario,
                                  idroomlist = a.idroomlist,
                                  Tarifa = a.Tarifa,
                                  codetipodemoneda = a.codetipodemoneda
                              }).ToList();

                    break;
                case 15: //Garza Blanca Cabo
                    FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities frontGBC = new FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities();
                    result = (from a in frontGBC.spLlegadas(fromDate, toDate)
                              select new FrontOfficeViewModel.LlegadasResult()
                              {
                                  idReservacion = a.idReservacion,
                                  CuartosNoche = a.CuartosNoche,
                                  TipoHab = a.TipoHab,
                                  llegada = a.llegada,
                                  salida = a.salida,
                                  NumHab = a.NumHab,
                                  numconfirmacion = a.numconfirmacion,
                                  Procedencia = a.Procedencia,
                                  CodigoMerc = a.CodigoMerc,
                                  idresort = a.idresort,
                                  Split = a.Split,
                                  CRS = a.CRS,
                                  codeagencia = a.codeagencia,
                                  nameagencia = a.nameagencia,
                                  Huesped = a.Huesped,
                                  cuartos = a.cuartos,
                                  Adultos = a.Adultos,
                                  Ninos = a.Ninos,
                                  apellidopaterno = a.apellidopaterno,
                                  apellidomaterno = a.apellidomaterno,
                                  nombres = a.nombres,
                                  codepais = a.codepais,
                                  namepais = a.namepais,
                                  codigostatusreservacion = a.codigostatusreservacion,
                                  X = a.X,
                                  Titulo = a.Titulo,
                                  Infantes = a.Infantes,
                                  HLlegada = a.HLlegada,
                                  HSalida = a.HSalida,
                                  idhuesped = a.IdHuesped,
                                  DistintivoPrecheckin = a.DistintivoPrecheckin,
                                  FechaHoraCheckin = a.FechaHoraCheckin,
                                  Contrato = a.Contrato,
                                  TipoPlan = a.TipoPlan,
                                  Comentario = a.Comentario,
                                  idroomlist = a.idroomlist,
                                  Tarifa = a.Tarifa,
                                  codetipodemoneda = a.codetipodemoneda
                              }).ToList();

                    break;
            }

            return result;
        }

        //public static List<FrontOfficeViewModel.LlegadasResult> GetArrivals(int frontOfficeResortID, DateTime fromDate, DateTime toDate)
        //{
        //    List<FrontOfficeViewModel.LlegadasResult> result = new List<FrontOfficeViewModel.LlegadasResult>();
        //    try
        //    {
        //        var client = new WebClient();
        //        client.UseDefaultCredentials = true;
        //        client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
        //        var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/Llegadas/" + frontOfficeResortID + "/" + fromDate.ToString("yyyy-MM-dd") + "T00:00:00/" + toDate.ToString("yyyy-MM-dd") + "T00:00:00");
        //        str = str.Replace("{\"result\":[", "")
        //            .Replace("]]}", "]");
        //        var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.LlegadasResult>>(str);
        //        result = (from a in _result
        //                  select new FrontOfficeViewModel.LlegadasResult()
        //                  {
        //                      idReservacion = a.idReservacion,
        //                      CuartosNoche = a.CuartosNoche,
        //                      TipoHab = a.TipoHab,
        //                      llegada = a.llegada,
        //                      salida = a.salida,
        //                      NumHab = a.NumHab,
        //                      numconfirmacion = a.numconfirmacion,
        //                      Procedencia = a.Procedencia,
        //                      CodigoMerc = a.CodigoMerc,
        //                      idresort = a.idresort,
        //                      Split = a.Split,
        //                      CRS = a.CRS,
        //                      codeagencia = a.codeagencia,
        //                      nameagencia = a.nameagencia,
        //                      Huesped = a.Huesped,
        //                      cuartos = a.cuartos,
        //                      Adultos = a.Adultos,
        //                      Ninos = a.Ninos,
        //                      apellidopaterno = a.apellidopaterno,
        //                      apellidomaterno = a.apellidomaterno,
        //                      nombres = a.nombres,
        //                      codepais = a.codepais,
        //                      namepais = a.namepais,
        //                      codigostatusreservacion = a.codigostatusreservacion,
        //                      X = a.X,
        //                      Titulo = a.Titulo,
        //                      Infantes = a.Infantes,
        //                      HLlegada = a.HLlegada,
        //                      HSalida = a.HSalida,
        //                      idhuesped = a.idhuesped,
        //                      DistintivoPrecheckin = a.DistintivoPrecheckin,
        //                      FechaHoraCheckin = a.FechaHoraCheckin,
        //                      Contrato = a.Contrato,
        //                      TipoPlan = a.TipoPlan,
        //                      Comentario = a.Comentario,
        //                      idroomlist = a.idroomlist,
        //                      Tarifa = a.Tarifa,
        //                      codetipodemoneda = a.codetipodemoneda
        //                  }).ToList();
        //    }
        //}

        public static string FormatContactInfo(string item, string itemType)
        {
            var response = "";
            switch (itemType)
            {
                case "email":
                    {

                        if (Regex.IsMatch(item, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                        {
                            response = item;
                        }
                        break;
                    }
                case "phone":
                    {

                        if (item.Trim().Length >= 10)
                        {
                            if (item.IndexOf(" ") == 10)
                            {
                                response = item.Split(' ')[0];
                            }
                            else
                            {
                                item = Regex.Replace(item, @"[^\d]", "");
                                if (response.Length >= 10)
                                {
                                    response = item.Substring(item.Length - 10);
                                }
                            }
                        }
                        break;
                    }
            }
            return response;
        }

        public static FrontOfficeViewModel.ContactInfo GetContactInfo(int frontOfficeResortID, decimal? frontOfficeGuestID = null, string email = null)
        {
            FrontOfficeViewModel.ContactInfo contactInfo = new FrontOfficeViewModel.ContactInfo();
            FrontOfficeViewModel.ContactInfo ContactInfoQ = new FrontOfficeViewModel.ContactInfo();
            try
            {
                var client = new WebClient();
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/correoHuesped/" + frontOfficeResortID + "/" + (email ?? "") + "/" + (frontOfficeGuestID ?? 0).ToString());
                str = str.Replace("{\"result\":[", "")
                    .Replace("]]}", "]");
                var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.CorreoHuesped>>(str);
                ContactInfoQ = (from a in _result
                                select new FrontOfficeViewModel.ContactInfo()
                                {
                                    GuestID = a.idhuesped,
                                    Email = a.email,
                                    Phone = a.telefono
                                }).FirstOrDefault();

                if (ContactInfoQ.Email != null && Regex.IsMatch(ContactInfoQ.Email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                {
                    contactInfo.Email = ContactInfoQ.Email;
                }
                else if (ContactInfoQ.Email != null && ContactInfoQ.Email != "")
                {
                    contactInfo.UnformattedEmail = ContactInfoQ.Email;
                }

                if (ContactInfoQ.Phone != null && ContactInfoQ.Phone.Trim().Length >= 10)
                {
                    if (ContactInfoQ.Phone.IndexOf(" ") == 10)
                    {
                        contactInfo.Phone = ContactInfoQ.Phone.Split(' ')[0];
                    }
                    else
                    {
                        ContactInfoQ.Phone = Regex.Replace(ContactInfoQ.Phone, @"[^\d]", "");
                        //new
                        //var prefix = ContactInfoQ.Phone.Substring(0, 3);
                        //var sufix = ContactInfoQ.Phone.Substring(ContactInfoQ.Phone.Length - 3);
                        //if(prefix == sufix)
                        //{

                        //}
                        //end new
                        if (ContactInfoQ.Phone.Length >= 10)
                        {
                            contactInfo.Phone = ContactInfoQ.Phone.Substring(ContactInfoQ.Phone.Length - 10);
                        }
                    }
                }
                else if (ContactInfoQ.Phone != null && ContactInfoQ.Phone != "")
                {
                    contactInfo.UnformattedPhone = ContactInfoQ.Phone;
                }
                if (ContactInfoQ != null && ContactInfoQ.GuestID != null)
                {
                    contactInfo.GuestID = ContactInfoQ.GuestID;
                }
            }
            catch (Exception ex) { }
            return contactInfo;
        }

        public static FrontOfficeViewModel.ContactInfo _GetContactInfo(int frontOfficeResortID, decimal? frontOfficeGuestID = null, string email = null)
        {
            FrontOfficeViewModel.ContactInfo contactInfo = new FrontOfficeViewModel.ContactInfo();
            FrontOfficeViewModel.ContactInfo ContactInfoQ = new FrontOfficeViewModel.ContactInfo();

            switch (frontOfficeResortID)
            {
                case 1: //Villa del Palmar Vallarta
                    FrontOfficeVDPVEntities frontVDPV = new FrontOfficeVDPVEntities();
                    contactInfo.GuestID = null;
                    contactInfo.Email = "";
                    contactInfo.Phone = "";
                    break;
                case 4: //Villa del Palmar Cabo
                case 3: //Villa del Palmar Flamingos
                case 5: //Villa La Estancia Cabo
                case 6: //Villa del Arco Cabo
                case 7: //Villa La Estancia Nuevo Vallarta
                case 12: //Villa del Palmar Loreto
                    var client = new WebClient();
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
                    var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/correoHuesped/" + frontOfficeResortID + "/" + (email ?? "") + "/" + (frontOfficeGuestID ?? 0).ToString());
                    str = str.Replace("{\"result\":[", "")
                        .Replace("]]}", "]");
                    var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.CorreoHuesped>>(str);
                    ContactInfoQ = (from a in _result
                                    select new FrontOfficeViewModel.ContactInfo()
                                    {
                                        GuestID = a.idhuesped,
                                        Email = a.email,
                                        Phone = a.telefono
                                    }).FirstOrDefault();
                    break;
                case 9: //Garza Blanca Vallarta
                    FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBPV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
                    ContactInfoQ = (from a in frontGBPV.spCorreoHuesped((int)frontOfficeGuestID, email)
                                    select new FrontOfficeViewModel.ContactInfo()
                                    {
                                        GuestID = a.idhuesped,
                                        Email = a.email,
                                        Phone = a.telefono
                                    }).FirstOrDefault();
                    break;
                case 11: //Villa del Palmar Cancun
                    FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities frontVDPN = new FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities();
                    ContactInfoQ = (from a in frontVDPN.spCorreoHuesped((int)frontOfficeGuestID, email)
                                    select new FrontOfficeViewModel.ContactInfo()
                                    {
                                        GuestID = a.idhuesped,
                                        Email = a.email,
                                        Phone = a.telefono
                                    }).FirstOrDefault();
                    break;
                case 13: //Mousai PV
                    FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities frontHMPV = new FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities();
                    ContactInfoQ = (from a in frontHMPV.spCorreoHuesped((int)frontOfficeGuestID, email)
                                    select new FrontOfficeViewModel.ContactInfo()
                                    {
                                        GuestID = a.idhuesped,
                                        Email = a.email,
                                        Phone = a.telefono
                                    }).FirstOrDefault();

                    break;
                case 15: //Garza Blanca Cabo
                    FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities frontGBC = new FrontOfficeModels.GarzaBlancaResortCabo.FrontofficeGBCEntities();
                    ContactInfoQ = (from a in frontGBC.spCorreoHuesped((int)frontOfficeGuestID, email)
                                    select new FrontOfficeViewModel.ContactInfo()
                                    {
                                        GuestID = a.idhuesped,
                                        Email = a.email,
                                        Phone = a.telefono
                                    }).FirstOrDefault();
                    break;
            }

            if (ContactInfoQ.Email != null && Regex.IsMatch(ContactInfoQ.Email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
            {
                contactInfo.Email = ContactInfoQ.Email;
            }
            else if (ContactInfoQ.Email != null)
            {
                contactInfo.UnformattedEmail = ContactInfoQ.Email;
            }

            if (ContactInfoQ.Phone != null && ContactInfoQ.Phone.Trim().Length >= 10)
            {
                ContactInfoQ.Phone = ContactInfoQ.Phone.Trim();
                ContactInfoQ.Phone = ContactInfoQ.Phone.Replace("-", "");
                ContactInfoQ.Phone = ContactInfoQ.Phone.Replace(" ", "");
                ContactInfoQ.Phone = ContactInfoQ.Phone.Replace("(", "");
                ContactInfoQ.Phone = ContactInfoQ.Phone.Replace(")", "");
                if (ContactInfoQ.Phone.Length >= 17)
                {
                    contactInfo.Phone = ContactInfoQ.Phone.Substring(0, 10);
                }
                else if (ContactInfoQ.Phone.Length >= 10)
                {
                    contactInfo.Phone = ContactInfoQ.Phone.Substring(ContactInfoQ.Phone.Length - 10);
                }
            }
            else if (ContactInfoQ.Phone != null)
            {
                contactInfo.UnformattedPhone = ContactInfoQ.Phone;
            }
            if (ContactInfoQ != null && ContactInfoQ.GuestID != null)
            {
                contactInfo.GuestID = ContactInfoQ.GuestID;
            }

            return contactInfo;
        }

        public static List<FrontOfficeViewModel.Distintivo> GetDistinctives(int? frontOfficeResortID, long? frontOfficeReservationID)
        {
            List<FrontOfficeViewModel.Distintivo> list = new List<FrontOfficeViewModel.Distintivo>();

            var client = new WebClient();
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("ePlat", "3Pl@tV1ll@gr0p");
            try
            {
                var str = client.DownloadString("http://187.174.136.139:8081/DataSnap/rest/TdmClients/Distintivos/" + frontOfficeResortID + "/" + frontOfficeReservationID);
                str = str.Replace("{\"result\":[", "")
                            .Replace("]]}", "]");
                var _result = new JavaScriptSerializer().Deserialize<List<FrontOfficeViewModel.Distintivo>>(str);

                list = (from a in _result
                        select new FrontOfficeViewModel.Distintivo()
                        {
                            idDistintivos = a.idDistintivos,
                            NombreDeDistintivos = a.NombreDeDistintivos,
                            CodigoDeDistintivos = a.CodigoDeDistintivos,
                            fechaalta = a.fechaalta,
                            llegada = a.llegada,
                            salida = a.salida,
                            numconfirmacion = a.numconfirmacion
                        }).ToList();

                return list;
            }
            catch (Exception ex)
            {
                list.Add(new FrontOfficeViewModel.Distintivo()
                {
                    NombreDeDistintivos = "Error. Contact System Administrator<br />" + ex.Message
                });
                return list;
            }
        }
    }
}
