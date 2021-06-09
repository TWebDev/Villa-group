using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ePlatBack.Models.ViewModels;
using ePlatBack.Models.Utils;
using Excel = Microsoft.Office.Interop.Excel;
using System.Web.UI;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;



namespace ePlatBack.Models.DataModels
{
    public class HubDataModel
    {
        public AttemptResponse MigrateGuests(GuestSearchModel gsm)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            DateTime date = DateTime.Parse(gsm.SearchDateI);
            DateTime date2 = DateTime.Parse(gsm.SearchDatef);
            tblGuestHub_FrontOffice tblguesthub_frontoffice = new tblGuestHub_FrontOffice();
            Guid arrivalId;
            Guid paymentID;




            try
            {
                //**********************Mousai********************
                if (gsm.Search_Resort[0] == 13)
                {

                    FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities frontHM = new FrontOfficeModels.HotelMousaiVallarta.FrontOfficeHMPVEntities();
                    List<FrontOfficeModels.HotelMousaiVallarta.spLlegadas_Result> guests = frontHM.spLlegadas(date, date2).ToList();

                    List<FrontOfficeModels.HotelMousaiVallarta.spHistorialReservaciones_Result> reservaciones;

                    tblGuestsHub tblguesthub = new tblGuestsHub();

                    tblGuestsPayments tblguestpayments = new tblGuestsPayments();
                    tblArrivals tblarrivals = new tblArrivals();

                    foreach (var guest in guests)
                    {
                        if (db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion && (m.roomNumber == null || m.roomNumber == "" || m.roomNumber == "-") && guest.NumHab != null) != null)
                        {
                            var guestrepetido = db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion);
                            guestrepetido.roomNumber = guest.NumHab;
                            db.SaveChanges();
                        }

                        if (db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion) == null)
                        {
                            int idHuesped;
                            idHuesped = Int32.Parse(guest.IdHuesped.ToString());
                            Guid guestHubId;
                            List<string> correo = new List<string>();
                            correo = frontHM.spCorreoHuesped(Int32.Parse(guest.IdHuesped.ToString()), null).Select(x => x.email).ToList();
                            string email1 = "";
                            //////////////////----------------------------
                            if (correo.Count() > 0 && correo[0] != null && Regex.IsMatch(correo[0], "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                            {
                                email1 = correo[0];
                            }
                                
                            var guestRepetido = new tblGuestsHub();
                            var huespedNuevo = db.tblGuestHub_FrontOffice.FirstOrDefault(k => k.frontOfficeGuestID == guest.IdHuesped);
                            if (((email1 != null) && (email1.Length > 2) && (email1.Contains("tiene"))) || (huespedNuevo != null))
                            {

                                var guestRepetidoo = from g in db.tblGuestsHub
                                                     join fo in db.tblGuestHub_FrontOffice on g.guestHubID equals fo.guestHubID
                                                     where fo.guestHubID == g.guestHubID
                                                     && fo.frontOfficeGuestID == guest.IdHuesped
                                                     || g.email1 == email1
                                                     select g;
                                guestRepetido = guestRepetidoo.FirstOrDefault();
                            }

                            //**************************************FALTA QUERY QUE RETORNE INFO GUEST Y VERIFIQUE RELACION CON EL GUESTHUB y juntar con el if de abajo

                            //*********************GUESTREGISTRADO?????????????
                            Guid compara = new Guid("00000000-0000-0000-0000-000000000000");
                            if (guestRepetido.guestHubID != compara)
                            {
                                //**********SI**********
                                if (guestRepetido.email1 != null)
                                {
                                    if (guestRepetido.email1 == "@" || guestRepetido.email1.Contains("tiene"))
                                        guestRepetido.email1 = correo[0];
                                    else if (guestRepetido.email1 != correo[0])
                                    {
                                        guestRepetido.email2 = correo[0];
                                    }
                                    db.SaveChanges();
                                }
                                if (huespedNuevo == null)
                                {
                                    tblguesthub_frontoffice = new tblGuestHub_FrontOffice();
                                    tblguesthub_frontoffice.guestHubID = guestRepetido.guestHubID;
                                    tblguesthub_frontoffice.frontOfficeGuestID = (long)guest.IdHuesped;
                                    tblguesthub_frontoffice.frontOfficeResortID = (int)guest.idresort;
                                    db.tblGuestHub_FrontOffice.AddObject(tblguesthub_frontoffice);

                                    db.SaveChanges();
                                }
                                tblarrivals = new tblArrivals();
                                guestHubId = guestRepetido.guestHubID;

                                var arrivalRepetido = db.tblArrivals.FirstOrDefault(arrival => arrival.frontOfficeReservationID == guest.idReservacion);
                                //el arrival nuevo ya esta registrado????????????
                                if (arrivalRepetido != null)
                                {
                                    //******SI******

                                    reservaciones = new List<FrontOfficeModels.HotelMousaiVallarta.spHistorialReservaciones_Result>();
                                    reservaciones = frontHM.spHistorialReservaciones(Int32.Parse(guest.IdHuesped.ToString()), null).ToList();
                                    foreach (var reservacion in reservaciones)
                                    {
                                        var llegadaHistory = db.tblArrivals.FirstOrDefault(arrival => arrival.frontOfficeReservationID == reservacion.idreservacion);
                                        //Historial de llegadas
                                        if ((llegadaHistory == null))
                                        {

                                            arrivalId = Guid.NewGuid();
                                            tblarrivals = new tblArrivals();
                                            tblarrivals.guestHubID = guestRepetido.guestHubID;
                                            tblarrivals.arrivalID = arrivalId;
                                            tblarrivals.country = guest.namepais;
                                            if (guest.NumHab != null)
                                                tblarrivals.roomNumber = guest.NumHab;
                                            else
                                                tblarrivals.roomNumber = "-";
                                            tblarrivals.terminalID = 33;
                                            tblarrivals.frontOfficeReservationID = long.Parse(reservacion.idreservacion.ToString());
                                            tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                            tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                            tblarrivals.arrivalDate = (DateTime)reservacion.llegada;
                                            tblarrivals.marketCode = reservacion.mercado;
                                            tblarrivals.firstName = guest.nombres;
                                            tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                            tblarrivals.infants = 0;
                                            tblarrivals.countryCode = guest.codepais;
                                            tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                            tblarrivals.agencyCode = guest.codeagencia;
                                            tblarrivals.source = guest.Procedencia;
                                            tblarrivals.crs = guest.CRS;
                                            tblarrivals.reservationStatus = "OUT";
                                            tblarrivals.dateSaved = DateTime.Today;
                                            tblarrivals.checkOut = (DateTime)reservacion.salida;
                                            tblarrivals.agencyName = reservacion.nameagencia;
                                            tblarrivals.roomType = reservacion.nametipodehabitacion;
                                            tblarrivals.adults = (int)reservacion.numadultos;
                                            tblarrivals.children = (int)reservacion.numchilds;
                                            tblarrivals.nights = (int)((DateTime)reservacion.salida - (DateTime)reservacion.llegada).Days;
                                            tblarrivals.confirmationNumber = reservacion.numconfirmacion;

                                            tblarrivals.guestHubID = guestHubId;
                                            tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                            db.tblArrivals.AddObject(tblarrivals);
                                            db.SaveChanges();


                                            //********************payments**************
                                            tblguestpayments = new tblGuestsPayments();
                                            paymentID = Guid.NewGuid();
                                            tblguestpayments.paymentID = paymentID;
                                            tblguestpayments.arrivalID = arrivalId;
                                            tblguestpayments.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                            if (reservacion.Renta != null)
                                                tblguestpayments.rent = reservacion.Renta.ToString();
                                            if (reservacion.Otros != null)
                                            {
                                                tblguestpayments.otherIncome = reservacion.Otros.ToString();
                                            }
                                            if (reservacion.SPA != null)
                                                tblguestpayments.spa = reservacion.SPA.ToString();
                                            if (reservacion.ConsumoPOS != null)
                                                tblguestpayments.pos = reservacion.ConsumoPOS.ToString();
                                            if (reservacion.TipoPlan != null)
                                                tblguestpayments.planType = reservacion.TipoPlan.ToString();
                                            tblguestpayments.dateSaved = DateTime.Now.Date;
                                            db.tblGuestsPayments.AddObject(tblguestpayments);
                                            db.SaveChanges();
                                        }

                                    }
                                }
                                else
                                {
                                    //el arrival reciente nuevo no esta registrado
                                    tblarrivals = new tblArrivals();
                                    arrivalId = Guid.NewGuid();
                                    tblarrivals.arrivalID = arrivalId;
                                    tblarrivals.terminalID = 33;
                                    tblarrivals.frontOfficeReservationID = long.Parse(guest.idReservacion.ToString());
                                    tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                    tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                    tblarrivals.arrivalDate = (DateTime)guest.llegada;
                                    tblarrivals.marketCode = guest.CodigoMerc;
                                    tblarrivals.checkOut = guest.salida;
                                    tblarrivals.agencyName = guest.nameagencia;
                                    tblarrivals.agencyCode = guest.codeagencia;
                                    tblarrivals.source = guest.Procedencia;
                                    tblarrivals.roomType = guest.TipoHab;
                                    tblarrivals.adults = (int)guest.Adultos;
                                    tblarrivals.children = (int)guest.Ninos;
                                    tblarrivals.infants = (int)guest.Infantes;
                                    tblarrivals.crs = guest.CRS;
                                    tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                    tblarrivals.firstName = guest.nombres;
                                    tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (guest.NumHab != null)
                                        tblarrivals.roomNumber = guest.NumHab;
                                    else
                                        tblarrivals.roomNumber = "-";
                                    tblarrivals.nights = (int)guest.CuartosNoche;
                                    tblarrivals.confirmationNumber = guest.numconfirmacion;
                                    tblarrivals.dateSaved = DateTime.Today;
                                    tblarrivals.reservationStatus = "OUT";
                                    tblarrivals.country = guest.namepais;
                                    tblarrivals.countryCode = guest.codepais;

                                    tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                    tblarrivals.guestHubID = guestHubId;
                                    db.tblArrivals.AddObject(tblarrivals);
                                    db.SaveChanges();
                                }


                            }
                            else
                            {
                                //guest aun no registrado
                                //***********************************UPDATE*************************************
                                //if((db.tblGuestsHub.FirstOrDefault(guestrepetido => guestrepetido.firstName == guest.nombres && guestrepetido.lastName == (guest.apellidopaterno + " " + guest.apellidomaterno))==null))
                                long idhuesped = (long)guest.IdHuesped;
                                if (db.tblGuestHub_FrontOffice.FirstOrDefault(w => w.frontOfficeGuestID == idhuesped) == null)
                                {
                                    tblguesthub = new tblGuestsHub();
                                    guestHubId = Guid.NewGuid();
                                    List<string> emails = new List<string>();
                                    correo = frontHM.spCorreoHuesped(Int32.Parse(guest.IdHuesped.ToString()), null).Select(x => x.email).ToList();
                                    foreach (var email in correo)
                                    {
                                        if (email != null && Regex.IsMatch(email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                                        {
                                            emails.Add(email.ToString());
                                        }
                                    }

                                    tblguesthub.guestHubID = guestHubId;
                                    tblguesthub.firstName = guest.nombres;
                                    tblguesthub.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (emails.Count != 0)
                                    {
                                        tblguesthub.email1 = emails[0];
                                    }
                                    if (guest.namepais == "Estados Unidos")
                                    {
                                        tblguesthub.countryID = 1;
                                    }
                                    if (guest.namepais == "Canada")
                                    {
                                        tblguesthub.countryID = 2;
                                    }
                                    if (guest.namepais == "MEXICO")
                                    {
                                        tblguesthub.countryID = 3;
                                    }
                                    db.tblGuestsHub.AddObject(tblguesthub);
                                    db.SaveChanges();

                                    tblguesthub_frontoffice = new tblGuestHub_FrontOffice();
                                    tblguesthub_frontoffice.guestHubID = tblguesthub.guestHubID;
                                    tblguesthub_frontoffice.frontOfficeGuestID = (long)guest.IdHuesped;
                                    tblguesthub_frontoffice.frontOfficeResortID = (int)guest.idresort;
                                    db.tblGuestHub_FrontOffice.AddObject(tblguesthub_frontoffice);
                                    db.SaveChanges();

                                    tblarrivals = new tblArrivals();
                                    arrivalId = Guid.NewGuid();
                                    tblarrivals.arrivalID = arrivalId;
                                    tblarrivals.terminalID = 33;
                                    tblarrivals.frontOfficeReservationID = long.Parse(guest.idReservacion.ToString());
                                    tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                    tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                    tblarrivals.arrivalDate = (DateTime)guest.llegada;
                                    tblarrivals.marketCode = guest.CodigoMerc;
                                    tblarrivals.checkOut = guest.salida;
                                    tblarrivals.agencyName = guest.nameagencia;
                                    tblarrivals.agencyCode = guest.codeagencia;
                                    tblarrivals.source = guest.Procedencia;
                                    tblarrivals.roomType = guest.TipoHab;
                                    tblarrivals.adults = (int)guest.Adultos;
                                    tblarrivals.children = (int)guest.Ninos;
                                    tblarrivals.infants = (int)guest.Infantes;
                                    tblarrivals.crs = guest.CRS;
                                    tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                    tblarrivals.firstName = guest.nombres;
                                    tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (guest.NumHab != null)
                                        tblarrivals.roomNumber = guest.NumHab;
                                    else
                                        tblarrivals.roomNumber = "-";
                                    tblarrivals.nights = (int)guest.CuartosNoche;
                                    tblarrivals.confirmationNumber = guest.numconfirmacion;
                                    tblarrivals.dateSaved = DateTime.Today;
                                    tblarrivals.reservationStatus = "OUT";
                                    tblarrivals.country = guest.namepais;
                                    tblarrivals.countryCode = guest.codepais;

                                    tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                    tblarrivals.guestHubID = guestHubId;
                                    db.tblArrivals.AddObject(tblarrivals);
                                    db.SaveChanges();
                                    reservaciones = new List<FrontOfficeModels.HotelMousaiVallarta.spHistorialReservaciones_Result>();
                                    reservaciones = frontHM.spHistorialReservaciones(int.Parse(guest.IdHuesped.ToString()), null).ToList();
                                    foreach (var reservacion in reservaciones)
                                    {
                                        arrivalId = Guid.NewGuid();
                                        tblarrivals = new tblArrivals();
                                        tblarrivals.country = guest.namepais;
                                        if (guest.NumHab != null)
                                            tblarrivals.roomNumber = guest.NumHab;
                                        else
                                            tblarrivals.roomNumber = "-";
                                        tblarrivals.terminalID = 33;
                                        tblarrivals.frontOfficeReservationID = long.Parse(reservacion.idreservacion.ToString());
                                        tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                        tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                        tblarrivals.arrivalDate = (DateTime)reservacion.llegada;
                                        tblarrivals.marketCode = reservacion.mercado;
                                        tblarrivals.firstName = guest.nombres;
                                        tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                        tblarrivals.infants = 0;
                                        tblarrivals.countryCode = guest.codepais;
                                        tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        tblarrivals.agencyCode = guest.codeagencia;
                                        tblarrivals.source = guest.Procedencia;
                                        tblarrivals.crs = guest.CRS;
                                        tblarrivals.arrivalID = arrivalId;
                                        tblarrivals.reservationStatus = "OUT";
                                        tblarrivals.dateSaved = DateTime.Today;
                                        tblarrivals.guestHubID = guestHubId;
                                        tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;

                                        tblarrivals.checkOut = reservacion.salida;
                                        tblarrivals.agencyName = reservacion.nameagencia;
                                        tblarrivals.roomType = reservacion.nametipodehabitacion;
                                        tblarrivals.adults = (int)reservacion.numadultos;
                                        tblarrivals.children = (int)reservacion.numchilds;
                                        tblarrivals.nights = (int)((DateTime)reservacion.salida - (DateTime)reservacion.llegada).Days;
                                        tblarrivals.confirmationNumber = reservacion.numconfirmacion;
                                        db.tblArrivals.AddObject(tblarrivals);
                                        db.SaveChanges();
                                        //********************payments**************
                                        tblguestpayments = new tblGuestsPayments();
                                        paymentID = Guid.NewGuid();
                                        tblguestpayments.paymentID = paymentID;
                                        tblguestpayments.arrivalID = arrivalId;
                                        tblguestpayments.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        if (reservacion.Renta != null)
                                            tblguestpayments.rent = reservacion.Renta.ToString();
                                        if (reservacion.Otros != null)
                                        {
                                            tblguestpayments.otherIncome = reservacion.Otros.ToString();
                                        }
                                        if (reservacion.SPA != null)
                                            tblguestpayments.spa = reservacion.SPA.ToString();
                                        if (reservacion.ConsumoPOS != null)
                                            tblguestpayments.pos = reservacion.ConsumoPOS.ToString();
                                        if (reservacion.TipoPlan != null)
                                            tblguestpayments.planType = reservacion.TipoPlan.ToString();
                                        tblguestpayments.dateSaved = DateTime.Now.Date;
                                        db.tblGuestsPayments.AddObject(tblguestpayments);
                                        db.SaveChanges();


                                    }
                                }
                            }

                        }
                    }


                }

                //**********************Garza Blanca Vallarta********************
                if (gsm.Search_Resort[0] == 9)
                {

                    FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBRV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
                    List<FrontOfficeModels.GarzaBlancaResortVallarta.spLlegadas_Result> guests = frontGBRV.spLlegadas(date, date2).ToList();
                    List<FrontOfficeModels.GarzaBlancaResortVallarta.spHistorialReservaciones_Result> reservaciones;

                    tblGuestsHub tblguesthub = new tblGuestsHub();

                    tblGuestsPayments tblguestpayments = new tblGuestsPayments();
                    tblArrivals tblarrivals = new tblArrivals();

                    foreach (var guest in guests)
                    {
                        if (db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion && (m.roomNumber == null || m.roomNumber == "" || m.roomNumber == "-") && guest.NumHab != null) != null)
                        {
                            var guestrepetido = db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion);
                            guestrepetido.roomNumber = guest.NumHab;
                            db.SaveChanges();
                        }

                        if (db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion) == null)
                        {
                            int idHuesped;
                            idHuesped = Int32.Parse(guest.IdHuesped.ToString());
                            Guid guestHubId;
                            List<string> correo = new List<string>();
                            correo = frontGBRV.spCorreoHuesped(Int32.Parse(guest.IdHuesped.ToString()), null).Select(x => x.email).ToList();
                            string email1 = "";
                            //////////////////----------------------------
                            if (correo.Count() > 0 && correo[0]!= null && Regex.IsMatch(correo[0], "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$")){
                                email1 = correo[0];
                            }
                                
                            var guestRepetido = new tblGuestsHub();
                            var huespedNuevo = db.tblGuestHub_FrontOffice.FirstOrDefault(k => k.frontOfficeGuestID == guest.IdHuesped);
                            if (((email1 != null) && (email1.Length > 2) && (email1.Contains("tiene"))) || (huespedNuevo != null))
                            {

                                var guestRepetidoo = from g in db.tblGuestsHub
                                                     join fo in db.tblGuestHub_FrontOffice on g.guestHubID equals fo.guestHubID
                                                     where fo.guestHubID == g.guestHubID
                                                     && fo.frontOfficeGuestID == guest.IdHuesped
                                                     || g.email1 == email1
                                                     select g;
                                guestRepetido = guestRepetidoo.FirstOrDefault();
                            }

                            //**************************************FALTA QUERY QUE RETORNE INFO GUEST Y VERIFIQUE RELACION CON EL GUESTHUB y juntar con el if de abajo

                            //*********************GUESTREGISTRADO?????????????
                            Guid compara = new Guid("00000000-0000-0000-0000-000000000000");
                            if (guestRepetido.guestHubID != compara)
                            {
                                //**********SI**********
                                if (guestRepetido.email1 != null)
                                {
                                    if (guestRepetido.email1 == "@" || guestRepetido.email1.Contains("tiene"))
                                        guestRepetido.email1 = correo[0];
                                    else if (guestRepetido.email1 != correo[0])
                                    {
                                        guestRepetido.email2 = correo[0];
                                    }
                                    db.SaveChanges();
                                }
                                if (huespedNuevo == null)
                                {
                                    tblguesthub_frontoffice = new tblGuestHub_FrontOffice();
                                    tblguesthub_frontoffice.guestHubID = guestRepetido.guestHubID;
                                    tblguesthub_frontoffice.frontOfficeGuestID = (long)guest.IdHuesped;
                                    tblguesthub_frontoffice.frontOfficeResortID = (int)guest.idresort;
                                    db.tblGuestHub_FrontOffice.AddObject(tblguesthub_frontoffice);

                                    db.SaveChanges();
                                }
                                tblarrivals = new tblArrivals();
                                guestHubId = guestRepetido.guestHubID;

                                var arrivalRepetido = db.tblArrivals.FirstOrDefault(arrival => arrival.frontOfficeReservationID == guest.idReservacion);
                                //el arrival nuevo ya esta registrado????????????
                                if (arrivalRepetido != null)
                                {
                                    //******SI******




                                    reservaciones = new List<FrontOfficeModels.GarzaBlancaResortVallarta.spHistorialReservaciones_Result>();
                                    reservaciones = frontGBRV.spHistorialReservaciones(Int32.Parse(guest.IdHuesped.ToString()), null).ToList();
                                    foreach (var reservacion in reservaciones)
                                    {
                                        var llegadaHistory = db.tblArrivals.FirstOrDefault(arrival => arrival.frontOfficeReservationID == reservacion.idreservacion);
                                        //Historial de llegadas
                                        if ((llegadaHistory == null))
                                        {

                                            arrivalId = Guid.NewGuid();
                                            tblarrivals = new tblArrivals();
                                            tblarrivals.guestHubID = guestRepetido.guestHubID;
                                            tblarrivals.arrivalID = arrivalId;
                                            tblarrivals.country = guest.namepais;
                                            if (guest.NumHab != null)
                                                tblarrivals.roomNumber = guest.NumHab;
                                            else
                                                tblarrivals.roomNumber = "-";
                                            tblarrivals.terminalID = 33;
                                            tblarrivals.frontOfficeReservationID = long.Parse(reservacion.idreservacion.ToString());
                                            tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                            tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                            tblarrivals.arrivalDate = (DateTime)reservacion.llegada;
                                            tblarrivals.marketCode = reservacion.mercado;
                                            tblarrivals.firstName = guest.nombres;
                                            tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                            tblarrivals.infants = 0;
                                            tblarrivals.countryCode = guest.codepais;
                                            tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                            tblarrivals.agencyCode = guest.codeagencia;
                                            tblarrivals.source = guest.Procedencia;
                                            tblarrivals.crs = guest.CRS;
                                            tblarrivals.reservationStatus = "OUT";
                                            tblarrivals.dateSaved = DateTime.Today;
                                            tblarrivals.checkOut = (DateTime)reservacion.salida;
                                            tblarrivals.agencyName = reservacion.nameagencia;
                                            tblarrivals.roomType = reservacion.nametipodehabitacion;
                                            tblarrivals.adults = (int)reservacion.numadultos;
                                            tblarrivals.children = (int)reservacion.numchilds;
                                            tblarrivals.nights = (int)((DateTime)reservacion.salida - (DateTime)reservacion.llegada).Days;
                                            tblarrivals.confirmationNumber = reservacion.numconfirmacion;

                                            tblarrivals.guestHubID = guestHubId;
                                            tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                            db.tblArrivals.AddObject(tblarrivals);
                                            db.SaveChanges();


                                            //********************payments**************
                                            tblguestpayments = new tblGuestsPayments();
                                            paymentID = Guid.NewGuid();
                                            tblguestpayments.paymentID = paymentID;
                                            tblguestpayments.arrivalID = arrivalId;
                                            tblguestpayments.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                            if (reservacion.Renta != null)
                                                tblguestpayments.rent = reservacion.Renta.ToString();
                                            if (reservacion.Otros != null)
                                            {
                                                tblguestpayments.otherIncome = reservacion.Otros.ToString();
                                            }
                                            if (reservacion.SPA != null)
                                                tblguestpayments.spa = reservacion.SPA.ToString();
                                            if (reservacion.ConsumoPOS != null)
                                                tblguestpayments.pos = reservacion.ConsumoPOS.ToString();
                                            if (reservacion.TipoPlan != null)
                                                tblguestpayments.planType = reservacion.TipoPlan.ToString();
                                            tblguestpayments.dateSaved = DateTime.Now.Date;
                                            db.tblGuestsPayments.AddObject(tblguestpayments);
                                            db.SaveChanges();
                                        }

                                    }
                                }
                                else
                                {
                                    //el arrival reciente nuevo no esta registrado
                                    tblarrivals = new tblArrivals();
                                    arrivalId = Guid.NewGuid();
                                    tblarrivals.arrivalID = arrivalId;
                                    tblarrivals.terminalID = 33;
                                    tblarrivals.frontOfficeReservationID = long.Parse(guest.idReservacion.ToString());
                                    tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                    tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                    tblarrivals.arrivalDate = (DateTime)guest.llegada;
                                    tblarrivals.marketCode = guest.CodigoMerc;
                                    tblarrivals.checkOut = guest.salida;
                                    tblarrivals.agencyName = guest.nameagencia;
                                    tblarrivals.agencyCode = guest.codeagencia;
                                    tblarrivals.source = guest.Procedencia;
                                    tblarrivals.roomType = guest.TipoHab;
                                    tblarrivals.adults = (int)guest.Adultos;
                                    tblarrivals.children = (int)guest.Ninos;
                                    tblarrivals.infants = (int)guest.Infantes;
                                    tblarrivals.crs = guest.CRS;
                                    tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                    tblarrivals.firstName = guest.nombres;
                                    tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (guest.NumHab != null)
                                        tblarrivals.roomNumber = guest.NumHab;
                                    else
                                        tblarrivals.roomNumber = "-";
                                    tblarrivals.nights = (int)guest.CuartosNoche;
                                    tblarrivals.confirmationNumber = guest.numconfirmacion;
                                    tblarrivals.dateSaved = DateTime.Today;
                                    tblarrivals.reservationStatus = "OUT";
                                    tblarrivals.country = guest.namepais;
                                    tblarrivals.countryCode = guest.codepais;

                                    tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                    tblarrivals.guestHubID = guestHubId;
                                    db.tblArrivals.AddObject(tblarrivals);
                                    db.SaveChanges();
                                }


                            }
                            else
                            {
                                //guest aun no registrado
                                //***********************************UPDATE*************************************
                                //if((db.tblGuestsHub.FirstOrDefault(guestrepetido => guestrepetido.firstName == guest.nombres && guestrepetido.lastName == (guest.apellidopaterno + " " + guest.apellidomaterno))==null))
                                long idhuesped = (long)guest.IdHuesped;
                                if (db.tblGuestHub_FrontOffice.FirstOrDefault(w => w.frontOfficeGuestID == idhuesped) == null)
                                {
                                    tblguesthub = new tblGuestsHub();
                                    guestHubId = Guid.NewGuid();
                                    List<string> emails = new List<string>();
                                    correo = frontGBRV.spCorreoHuesped(Int32.Parse(guest.IdHuesped.ToString()), null).Select(x => x.email).ToList();
                                    foreach (var email in correo)
                                    {
                                        if (email != null && Regex.IsMatch(email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                                        {
                                            emails.Add(email.ToString());
                                        }
                                    }
                                        
                                    tblguesthub.guestHubID = guestHubId;
                                    tblguesthub.firstName = guest.nombres;
                                    tblguesthub.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (emails.Count != 0)
                                    {
                                        tblguesthub.email1 = emails[0];
                                    }
                                    if (guest.namepais == "Estados Unidos")
                                    {
                                        tblguesthub.countryID = 1;
                                    }
                                    if (guest.namepais == "Canada")
                                    {
                                        tblguesthub.countryID = 2;
                                    }
                                    if (guest.namepais == "MEXICO")
                                    {
                                        tblguesthub.countryID = 3;
                                    }
                                    db.tblGuestsHub.AddObject(tblguesthub);
                                    db.SaveChanges();

                                    tblguesthub_frontoffice = new tblGuestHub_FrontOffice();
                                    tblguesthub_frontoffice.guestHubID = tblguesthub.guestHubID;
                                    tblguesthub_frontoffice.frontOfficeGuestID = (long)guest.IdHuesped;
                                    tblguesthub_frontoffice.frontOfficeResortID = (int)guest.idresort;
                                    db.tblGuestHub_FrontOffice.AddObject(tblguesthub_frontoffice);
                                    db.SaveChanges();

                                    tblarrivals = new tblArrivals();
                                    arrivalId = Guid.NewGuid();
                                    tblarrivals.arrivalID = arrivalId;
                                    tblarrivals.terminalID = 33;
                                    tblarrivals.frontOfficeReservationID = long.Parse(guest.idReservacion.ToString());
                                    tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                    tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                    tblarrivals.arrivalDate = (DateTime)guest.llegada;
                                    tblarrivals.marketCode = guest.CodigoMerc;
                                    tblarrivals.checkOut = guest.salida;
                                    tblarrivals.agencyName = guest.nameagencia;
                                    tblarrivals.agencyCode = guest.codeagencia;
                                    tblarrivals.source = guest.Procedencia;
                                    tblarrivals.roomType = guest.TipoHab;
                                    tblarrivals.adults = (int)guest.Adultos;
                                    tblarrivals.children = (int)guest.Ninos;
                                    tblarrivals.infants = (int)guest.Infantes;
                                    tblarrivals.crs = guest.CRS;
                                    tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                    tblarrivals.firstName = guest.nombres;
                                    tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (guest.NumHab != null)
                                        tblarrivals.roomNumber = guest.NumHab;
                                    else
                                        tblarrivals.roomNumber = "-";
                                    tblarrivals.nights = (int)guest.CuartosNoche;
                                    tblarrivals.confirmationNumber = guest.numconfirmacion;
                                    tblarrivals.dateSaved = DateTime.Today;
                                    tblarrivals.reservationStatus = "OUT";
                                    tblarrivals.country = guest.namepais;
                                    tblarrivals.countryCode = guest.codepais;

                                    tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                    tblarrivals.guestHubID = guestHubId;
                                    db.tblArrivals.AddObject(tblarrivals);
                                    db.SaveChanges();
                                    reservaciones = new List<FrontOfficeModels.GarzaBlancaResortVallarta.spHistorialReservaciones_Result>();
                                    reservaciones = frontGBRV.spHistorialReservaciones(Int32.Parse(guest.IdHuesped.ToString()), null).ToList();
                                    foreach (var reservacion in reservaciones)
                                    {
                                        arrivalId = Guid.NewGuid();
                                        tblarrivals = new tblArrivals();
                                        tblarrivals.country = guest.namepais;
                                        if (guest.NumHab != null)
                                            tblarrivals.roomNumber = guest.NumHab;
                                        else
                                            tblarrivals.roomNumber = "-";
                                        tblarrivals.terminalID = 33;
                                        tblarrivals.frontOfficeReservationID = long.Parse(reservacion.idreservacion.ToString());
                                        tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                        tblarrivals.frontOfficeGuestID = long.Parse(guest.IdHuesped.ToString());
                                        tblarrivals.arrivalDate = (DateTime)reservacion.llegada;
                                        tblarrivals.marketCode = reservacion.mercado;
                                        tblarrivals.firstName = guest.nombres;
                                        tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                        tblarrivals.infants = 0;
                                        tblarrivals.countryCode = guest.codepais;
                                        tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        tblarrivals.agencyCode = guest.codeagencia;
                                        tblarrivals.source = guest.Procedencia;
                                        tblarrivals.crs = guest.CRS;
                                        tblarrivals.arrivalID = arrivalId;
                                        tblarrivals.reservationStatus = "OUT";
                                        tblarrivals.dateSaved = DateTime.Today;
                                        tblarrivals.guestHubID = guestHubId;
                                        tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;

                                        tblarrivals.checkOut = reservacion.salida;
                                        tblarrivals.agencyName = reservacion.nameagencia;
                                        tblarrivals.roomType = reservacion.nametipodehabitacion;
                                        tblarrivals.adults = (int)reservacion.numadultos;
                                        tblarrivals.children = (int)reservacion.numchilds;
                                        tblarrivals.nights = (int)((DateTime)reservacion.salida - (DateTime)reservacion.llegada).Days;
                                        tblarrivals.confirmationNumber = reservacion.numconfirmacion;
                                        db.tblArrivals.AddObject(tblarrivals);
                                        db.SaveChanges();
                                        //********************payments**************
                                        tblguestpayments = new tblGuestsPayments();
                                        paymentID = Guid.NewGuid();
                                        tblguestpayments.paymentID = paymentID;
                                        tblguestpayments.arrivalID = arrivalId;
                                        tblguestpayments.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        if (reservacion.Renta != null)
                                            tblguestpayments.rent = reservacion.Renta.ToString();
                                        if (reservacion.Otros != null)
                                        {
                                            tblguestpayments.otherIncome = reservacion.Otros.ToString();
                                        }
                                        if (reservacion.SPA != null)
                                            tblguestpayments.spa = reservacion.SPA.ToString();
                                        if (reservacion.ConsumoPOS != null)
                                            tblguestpayments.pos = reservacion.ConsumoPOS.ToString();
                                        if (reservacion.TipoPlan != null)
                                            tblguestpayments.planType = reservacion.TipoPlan.ToString();
                                        tblguestpayments.dateSaved = DateTime.Now.Date;
                                        db.tblGuestsPayments.AddObject(tblguestpayments);
                                        db.SaveChanges();


                                    }
                                }
                            }

                        }
                    }


                }
                //**********Villa del palmar cancun**********
                if (gsm.Search_Resort[0] == 11)
                {

                    FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities frontGBRC = new FrontOfficeModels.VillaDelPalmarCancun.FrontOfficeVDPVCancunEntities();
                    List<FrontOfficeModels.VillaDelPalmarCancun.spLlegadas_Result> guests = frontGBRC.spLlegadas(date, date2).ToList();
                    List<FrontOfficeModels.VillaDelPalmarCancun.spHistorialReservaciones_Result> reservaciones;




                    tblGuestsHub tblguesthub = new tblGuestsHub();
                    tblGuestsPayments tblguestpayments = new tblGuestsPayments();
                    tblArrivals tblarrivals = new tblArrivals();

                    foreach (var guest in guests)
                    {
                        if (db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion && (m.roomNumber == null || m.roomNumber == "" || m.roomNumber == "-") && guest.NumHab != null) != null)
                        {
                            var guestrepetido = db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion);
                            guestrepetido.roomNumber = guest.NumHab;
                            db.SaveChanges();
                        }
                        if (db.tblArrivals.FirstOrDefault(m => m.frontOfficeReservationID == guest.idReservacion) == null)
                        {
                            Guid guestHubId;
                            int idHuesped;
                            idHuesped = Int32.Parse(guest.idhuesped.ToString());
                            List<string> correo = new List<string>();
                            List<FrontOfficeModels.VillaDelPalmarCancun.spCorreoHuesped_Result> correos = frontGBRC.spCorreoHuesped(idHuesped, null).ToList();
                            foreach (var email in correos)
                                if (email.email != null && Regex.IsMatch(email.email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                                {
                                    correo.Add(email.email.ToString());
                                }
                            string email1 = "";
                            //////////////////----------------------------
                            if (correo.Count() > 0)
                                email1 = correo[0];
                            var guestRepetido = new tblGuestsHub();
                            var huespedNuevo = db.tblGuestHub_FrontOffice.FirstOrDefault(k => k.frontOfficeGuestID == guest.idhuesped);
                            if (((email1 != null) && (email1.Length > 2) && (email1.Contains("tiene"))) || (huespedNuevo != null))
                            {

                                var guestRepetidoo = from g in db.tblGuestsHub
                                                     join fo in db.tblGuestHub_FrontOffice on g.guestHubID equals fo.guestHubID
                                                     where fo.guestHubID == g.guestHubID
                                                     && fo.frontOfficeGuestID == guest.idhuesped
                                                     || g.email1 == email1
                                                     select g;
                                guestRepetido = guestRepetidoo.FirstOrDefault();
                            }

                            //**************************************FALTA QUERY QUE RETORNE INFO GUEST Y VERIFIQUE RELACION CON EL GUESTHUB y juntar con el if de abajo

                            //*********************GUESTREGISTRADO?????????????
                            Guid compara = new Guid("00000000-0000-0000-0000-000000000000");
                            if (guestRepetido.guestHubID != compara)
                            {
                                //**********SI**********
                                if (guestRepetido.email1 != null)
                                {
                                    if (guestRepetido.email1 == "@" || guestRepetido.email1.Contains("tiene"))
                                        guestRepetido.email1 = correo[0];
                                    else if (guestRepetido.email1 != correo[0])
                                    {
                                        guestRepetido.email2 = correo[0];
                                    }
                                    db.SaveChanges();
                                }
                                if (huespedNuevo == null)
                                {
                                    tblguesthub_frontoffice = new tblGuestHub_FrontOffice();
                                    tblguesthub_frontoffice.guestHubID = guestRepetido.guestHubID;
                                    tblguesthub_frontoffice.frontOfficeGuestID = (long)guest.idhuesped;
                                    tblguesthub_frontoffice.frontOfficeResortID = (int)guest.idresort;
                                    db.tblGuestHub_FrontOffice.AddObject(tblguesthub_frontoffice);

                                    db.SaveChanges();
                                }
                                tblarrivals = new tblArrivals();
                                guestHubId = guestRepetido.guestHubID;

                                var arrivalRepetido = db.tblArrivals.FirstOrDefault(arrival => arrival.frontOfficeReservationID == guest.idReservacion);
                                //el arrival nuevo ya esta registrado????????????
                                if (arrivalRepetido != null)
                                {
                                    //******SI******




                                    reservaciones = new List<FrontOfficeModels.VillaDelPalmarCancun.spHistorialReservaciones_Result>();
                                    reservaciones = frontGBRC.spHistorialReservaciones(Int32.Parse(guest.idhuesped.ToString()), null).ToList();
                                    foreach (var reservacion in reservaciones)
                                    {
                                        var llegadaHistory = db.tblArrivals.FirstOrDefault(arrival => arrival.frontOfficeReservationID == reservacion.idreservacion);
                                        //Historial de llegadas
                                        if ((llegadaHistory == null))
                                        {

                                            arrivalId = Guid.NewGuid();
                                            tblarrivals = new tblArrivals();
                                            tblarrivals.guestHubID = guestRepetido.guestHubID;
                                            tblarrivals.arrivalID = arrivalId;
                                            tblarrivals.country = guest.namepais;
                                            if (guest.NumHab != null)
                                                tblarrivals.roomNumber = guest.NumHab;
                                            else
                                                tblarrivals.roomNumber = "-";
                                            tblarrivals.terminalID = 33;
                                            tblarrivals.frontOfficeReservationID = long.Parse(reservacion.idreservacion.ToString());
                                            tblarrivals.frontOfficeGuestID = long.Parse(reservacion.idhuesped.ToString());
                                            tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                            tblarrivals.arrivalDate = (DateTime)reservacion.llegada;
                                            tblarrivals.marketCode = reservacion.mercado;
                                            tblarrivals.firstName = guest.nombres;
                                            tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                            tblarrivals.infants = 0;
                                            tblarrivals.countryCode = guest.codepais;
                                            tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                            tblarrivals.agencyCode = guest.codeagencia;
                                            tblarrivals.source = guest.Procedencia;
                                            tblarrivals.crs = guest.CRS;
                                            tblarrivals.reservationStatus = "OUT";
                                            tblarrivals.dateSaved = DateTime.Today;
                                            tblarrivals.checkOut = (DateTime)reservacion.salida;
                                            tblarrivals.agencyName = reservacion.nameagencia;
                                            tblarrivals.roomType = reservacion.nametipodehabitacion;
                                            tblarrivals.adults = (int)reservacion.numadultos;
                                            tblarrivals.children = (int)reservacion.numchilds;
                                            tblarrivals.nights = (int)((DateTime)reservacion.salida - (DateTime)reservacion.llegada).Days;
                                            tblarrivals.confirmationNumber = reservacion.numconfirmacion;

                                            tblarrivals.guestHubID = guestHubId;
                                            tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                            db.tblArrivals.AddObject(tblarrivals);
                                            db.SaveChanges();


                                            //********************payments**************
                                            tblguestpayments = new tblGuestsPayments();
                                            paymentID = Guid.NewGuid();
                                            tblguestpayments.paymentID = paymentID;
                                            tblguestpayments.arrivalID = arrivalId;
                                            tblguestpayments.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                            if (reservacion.Renta != null)
                                                tblguestpayments.rent = reservacion.Renta.ToString();
                                            if (reservacion.Otros != null)
                                            {
                                                tblguestpayments.otherIncome = reservacion.Otros.ToString();
                                            }
                                            if (reservacion.SPA != null)
                                                tblguestpayments.spa = reservacion.SPA.ToString();
                                            if (reservacion.ConsumoPOS != null)
                                                tblguestpayments.pos = reservacion.ConsumoPOS.ToString();
                                            if (reservacion.TipoPlan != null)
                                                tblguestpayments.planType = reservacion.TipoPlan.ToString();
                                            tblguestpayments.dateSaved = DateTime.Now.Date;
                                            db.tblGuestsPayments.AddObject(tblguestpayments);
                                            db.SaveChanges();
                                        }

                                    }
                                }
                                else
                                {
                                    //el arrival reciente nuevo no esta registrado
                                    tblarrivals = new tblArrivals();
                                    arrivalId = Guid.NewGuid();
                                    tblarrivals.arrivalID = arrivalId;
                                    tblarrivals.terminalID = 33;
                                    tblarrivals.frontOfficeReservationID = long.Parse(guest.idReservacion.ToString());
                                    tblarrivals.frontOfficeReservationID = long.Parse(guest.idhuesped.ToString());
                                    tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                    tblarrivals.arrivalDate = (DateTime)guest.llegada;
                                    tblarrivals.marketCode = guest.CodigoMerc;
                                    tblarrivals.checkOut = guest.salida;
                                    tblarrivals.agencyName = guest.nameagencia;
                                    tblarrivals.agencyCode = guest.codeagencia;
                                    tblarrivals.source = guest.Procedencia;
                                    tblarrivals.roomType = guest.TipoHab;
                                    tblarrivals.adults = (int)guest.Adultos;
                                    tblarrivals.children = (int)guest.Ninos;
                                    tblarrivals.infants = (int)guest.Infantes;
                                    tblarrivals.crs = guest.CRS;
                                    tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                    tblarrivals.firstName = guest.nombres;
                                    tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (guest.NumHab != null)
                                        tblarrivals.roomNumber = guest.NumHab;
                                    else
                                        tblarrivals.roomNumber = "-";
                                    tblarrivals.nights = (int)guest.CuartosNoche;
                                    tblarrivals.confirmationNumber = guest.numconfirmacion;
                                    tblarrivals.dateSaved = DateTime.Today;
                                    tblarrivals.reservationStatus = "OUT";
                                    tblarrivals.country = guest.namepais;
                                    tblarrivals.countryCode = guest.codepais;

                                    tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                    tblarrivals.guestHubID = guestHubId;
                                    db.tblArrivals.AddObject(tblarrivals);
                                    db.SaveChanges();
                                }


                            }
                            else
                            {
                                //guest aun no registrado
                                //***********************************UPDATE*************************************
                                //if((db.tblGuestsHub.FirstOrDefault(guestrepetido => guestrepetido.firstName == guest.nombres && guestrepetido.lastName == (guest.apellidopaterno + " " + guest.apellidomaterno))==null))
                                long idhuesped = (long)guest.idhuesped;
                                if (db.tblGuestHub_FrontOffice.FirstOrDefault(w => w.frontOfficeGuestID == idhuesped) == null)
                                {
                                    tblguesthub = new tblGuestsHub();
                                    guestHubId = Guid.NewGuid();
                                    correos = frontGBRC.spCorreoHuesped(Int32.Parse(guest.idhuesped.ToString()), null).ToList();
                                    foreach (var email in correos)
                                    {
                                        if (email.email != null && Regex.IsMatch(email.email, "^[_A-Za-z0-9-]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$"))
                                        {
                                            correo.Add(email.email.ToString());
                                        }
                                    }

                                    tblguesthub.guestHubID = guestHubId;
                                    tblguesthub.firstName = guest.nombres;
                                    tblguesthub.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (correo.Count != 0)
                                    {
                                        tblguesthub.email1 = correo[0];
                                    }
                                    if (guest.namepais == "Estados Unidos")
                                    {
                                        tblguesthub.countryID = 1;
                                    }
                                    if (guest.namepais == "Canada")
                                    {
                                        tblguesthub.countryID = 2;
                                    }
                                    if (guest.namepais == "MEXICO")
                                    {
                                        tblguesthub.countryID = 3;
                                    }
                                    db.tblGuestsHub.AddObject(tblguesthub);
                                    db.SaveChanges();

                                    tblguesthub_frontoffice = new tblGuestHub_FrontOffice();
                                    tblguesthub_frontoffice.guestHubID = tblguesthub.guestHubID;
                                    tblguesthub_frontoffice.frontOfficeGuestID = (long)guest.idhuesped;
                                    tblguesthub_frontoffice.frontOfficeResortID = (int)guest.idresort;
                                    db.tblGuestHub_FrontOffice.AddObject(tblguesthub_frontoffice);
                                    db.SaveChanges();

                                    tblarrivals = new tblArrivals();
                                    arrivalId = Guid.NewGuid();
                                    tblarrivals.arrivalID = arrivalId;
                                    tblarrivals.terminalID = 33;
                                    tblarrivals.frontOfficeReservationID = long.Parse(guest.idReservacion.ToString());
                                    tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                    tblarrivals.frontOfficeGuestID = long.Parse(guest.idhuesped.ToString());
                                    tblarrivals.arrivalDate = (DateTime)guest.llegada;
                                    tblarrivals.marketCode = guest.CodigoMerc;
                                    tblarrivals.checkOut = guest.salida;
                                    tblarrivals.agencyName = guest.nameagencia;
                                    tblarrivals.agencyCode = guest.codeagencia;
                                    tblarrivals.source = guest.Procedencia;
                                    tblarrivals.roomType = guest.TipoHab;
                                    tblarrivals.adults = (int)guest.Adultos;
                                    tblarrivals.children = (int)guest.Ninos;
                                    tblarrivals.infants = (int)guest.Infantes;
                                    tblarrivals.crs = guest.CRS;
                                    tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;
                                    tblarrivals.firstName = guest.nombres;
                                    tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                    if (guest.NumHab != null)
                                        tblarrivals.roomNumber = guest.NumHab;
                                    else
                                        tblarrivals.roomNumber = "-";
                                    tblarrivals.nights = (int)guest.CuartosNoche;
                                    tblarrivals.confirmationNumber = guest.numconfirmacion;
                                    tblarrivals.dateSaved = DateTime.Today;
                                    tblarrivals.reservationStatus = "OUT";
                                    tblarrivals.country = guest.namepais;
                                    tblarrivals.countryCode = guest.codepais;

                                    tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                    tblarrivals.guestHubID = guestHubId;
                                    db.tblArrivals.AddObject(tblarrivals);
                                    db.SaveChanges();
                                    reservaciones = new List<FrontOfficeModels.VillaDelPalmarCancun.spHistorialReservaciones_Result>();
                                    reservaciones = frontGBRC.spHistorialReservaciones(Int32.Parse(guest.idhuesped.ToString()), null).ToList();
                                    foreach (var reservacion in reservaciones)
                                    {
                                        arrivalId = Guid.NewGuid();
                                        tblarrivals = new tblArrivals();
                                        tblarrivals.country = guest.namepais;
                                        if (guest.NumHab != null)
                                            tblarrivals.roomNumber = guest.NumHab;
                                        else
                                            tblarrivals.roomNumber = "-";
                                        tblarrivals.terminalID = 33;
                                        tblarrivals.frontOfficeReservationID = long.Parse(reservacion.idreservacion.ToString());
                                        tblarrivals.frontOfficeResortID = Int32.Parse(guest.idresort.ToString());
                                        tblarrivals.frontOfficeGuestID = long.Parse(guest.idhuesped.ToString());
                                        tblarrivals.arrivalDate = (DateTime)reservacion.llegada;
                                        tblarrivals.marketCode = reservacion.mercado;
                                        tblarrivals.firstName = guest.nombres;
                                        tblarrivals.lastName = guest.apellidopaterno + " " + guest.apellidomaterno;
                                        tblarrivals.infants = 0;
                                        tblarrivals.countryCode = guest.codepais;
                                        tblarrivals.lastUpdateUserID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        tblarrivals.agencyCode = guest.codeagencia;
                                        tblarrivals.source = guest.Procedencia;
                                        tblarrivals.crs = guest.CRS;
                                        tblarrivals.arrivalID = arrivalId;
                                        tblarrivals.reservationStatus = "OUT";
                                        tblarrivals.dateSaved = DateTime.Today;
                                        tblarrivals.guestHubID = guestHubId;
                                        tblarrivals.guest = guest.nombres + " " + guest.apellidopaterno + " " + guest.apellidomaterno;

                                        tblarrivals.checkOut = reservacion.salida;
                                        tblarrivals.agencyName = reservacion.nameagencia;
                                        tblarrivals.roomType = reservacion.nametipodehabitacion;
                                        tblarrivals.adults = (int)reservacion.numadultos;
                                        tblarrivals.children = (int)reservacion.numchilds;
                                        tblarrivals.nights = (int)((DateTime)reservacion.salida - (DateTime)reservacion.llegada).Days;
                                        tblarrivals.confirmationNumber = reservacion.numconfirmacion;
                                        db.tblArrivals.AddObject(tblarrivals);
                                        db.SaveChanges();
                                        //********************payments**************
                                        tblguestpayments = new tblGuestsPayments();
                                        paymentID = Guid.NewGuid();
                                        tblguestpayments.paymentID = paymentID;
                                        tblguestpayments.arrivalID = arrivalId;
                                        tblguestpayments.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        if (reservacion.Renta != null)
                                            tblguestpayments.rent = reservacion.Renta.ToString();
                                        if (reservacion.Otros != null)
                                        {
                                            tblguestpayments.otherIncome = reservacion.Otros.ToString();
                                        }
                                        if (reservacion.SPA != null)
                                            tblguestpayments.spa = reservacion.SPA.ToString();
                                        if (reservacion.ConsumoPOS != null)
                                            tblguestpayments.pos = reservacion.ConsumoPOS.ToString();
                                        if (reservacion.TipoPlan != null)
                                            tblguestpayments.planType = reservacion.TipoPlan.ToString();
                                        tblguestpayments.dateSaved = DateTime.Now.Date;
                                        db.tblGuestsPayments.AddObject(tblguestpayments);
                                        db.SaveChanges();


                                    }
                                }
                            }
                        }

                    }


                }

                //Response
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Migration Complete";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                //was an error not saved
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "There was an error";
                response.ObjectID = -1;
                response.Exception = ex;
                return response;
            }

        }
        public GuestInfoModel GetGuest(String ReservationGuestID)
        {
            ePlatEntities db = new ePlatEntities();
            GuestInfoModel gim = new GuestInfoModel();
            Guid guesthubID = new Guid(ReservationGuestID);
            var query = from g in db.tblGuestsHub
                        join a in db.tblArrivals on g.guestHubID equals a.guestHubID
                        join c in db.tblCountries on g.countryID equals c.countryID
                        where g.guestHubID == guesthubID

                        select new
                        {
                            FirstName = g.firstName,
                            LastName = g.lastName,
                            Country = c.country,
                            State = g.state,
                            City = g.city,
                            Email = g.email1
                        };
            foreach (var w in query)
            {
                gim.GuestHubID = guesthubID;
                gim.FirstName = w.FirstName;
                gim.LastName = w.LastName;
                gim.Country = w.Country;
                gim.State = w.State;
                gim.City = w.City;
                gim.Email = w.Email;
            }






            return gim;
        }

        public List<GuestMemberInfo> GetMemberInfo(String ID)
        {
            HubDataModel hdm = new HubDataModel();
            ePlatEntities db = new ePlatEntities();
            List<GuestMemberInfo> lista = new List<GuestMemberInfo>();
            Guid GuestHubID = new Guid(ID);
            var llegadas = db.tblArrivals.Where(e => e.guestHubID == GuestHubID);
            GuestMemberInfo gmi = new GuestMemberInfo();
            foreach (var llegada in llegadas)
            {
                llegada.crs = "12A0ABC";
                var query = db.tblGuestContractReservartions.FirstOrDefault(m => m.confirmationNumber == llegada.crs);
                if (query != null)
                {

                    var reservacion = db.tblGuestHubMemberInfo.FirstOrDefault(m => m.ContractNumber == query.contractNumber);
                    gmi.AccountNumber = reservacion.AccountNumber;
                    gmi.ClubType = reservacion.ClubType;
                    gmi.ContractNumber = reservacion.ContractNumber;
                    gmi.ContractStatus = reservacion.ContractStatus;
                    gmi.ResortContractNumber = reservacion.ResortContractNumber;
                    gmi.SourceOfSale = reservacion.SourceOfSale;
                }
            }
            if (gmi.ContractNumber != 0)
            {
                lista.Add(gmi);
            }
            return lista;
        }

        public List<GuestReservationModel> GetHistoryReservations(string GuestHubID)
        {
            ePlatEntities db = new ePlatEntities();
            FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBRV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
            Guid guesthubID;
            guesthubID = new Guid(GuestHubID);

            List<GuestReservationModel> reservations = new List<GuestReservationModel>();
            var arrivals = (from a in db.tblArrivals
                            where a.guestHubID == guesthubID &&
                            a.arrivalDate < DateTime.Today
                            select a);
            var guest = arrivals.FirstOrDefault();
            //var guest=db.tblArrivals.FirstOrDefault(a=>a.guestHubID==guesthubID);
            if (guest != null)
            {
                var destination = db.tblPlaces.FirstOrDefault(p => p.frontOfficeResortID == guest.frontOfficeResortID);
                long IDReservacion = guest.frontOfficeReservationID;
                //List<FrontOfficeModels.GarzaBlancaResortVallarta.spHistorialReservaciones_Result> reservaciones;
                //reservaciones = new List<FrontOfficeModels.GarzaBlancaResortVallarta.spHistorialReservaciones_Result>();
                //reservaciones = frontGBRV.spHistorialReservaciones((int)IDReservacion, null).ToList();


                float total = 0;
                foreach (var a in arrivals)
                {
                    var reservacionesP = db.tblGuestsPayments.FirstOrDefault(m => m.arrivalID == a.arrivalID);
                    if (reservacionesP != null)
                    {
                        total = 0;
                        if (reservacionesP.rent != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.rent);
                        // if (reservacionesP.p != null)
                        //   total = total + (float)a.Paquete;
                        if (reservacionesP.otherIncome != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.otherIncome);
                        if (reservacionesP.spa != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.spa);
                        if (reservacionesP.otherIncome != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.otherIncome);




                        reservations.Add(new GuestReservationModel
                        {
                            Destination = destination.place,
                            Nights = (int)((DateTime)a.checkOut - (DateTime)a.arrivalDate).Days,
                            ArrivalDate = (DateTime)a.arrivalDate,
                            RoomType = a.roomType,
                            RoomNumber = "Unknown",
                            Adults = (int)a.adults,
                            Childrens = (int)a.children,
                            AgencyName = a.agencyName,
                            Source = a.source,
                            Total = total.ToString(),
                            //PlanType = a.,

                        });
                    }
                    else
                    {
                        reservations.Add(new GuestReservationModel
                        {
                            Destination = destination.place,
                            Nights = (int)((DateTime)a.checkOut - (DateTime)a.arrivalDate).Days,
                            ArrivalDate = (DateTime)a.arrivalDate,
                            RoomType = a.roomType,
                            RoomNumber = a.roomNumber,
                            Adults = (int)a.adults,
                            Childrens = (int)a.children,
                            AgencyName = a.agencyName,
                            Source = a.source,
                            Total = total.ToString(),
                            //PlanType = a.,

                        });

                    }
                }
            }
            return reservations;
        }

        public List<GuestReservationModel> GetReservations(string GuestHubID)
        {
            ePlatEntities db = new ePlatEntities();
            FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBRV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
            Guid guesthubID;
            guesthubID = new Guid(GuestHubID);

            List<GuestReservationModel> reservations = new List<GuestReservationModel>();
            var arrivals = (from a in db.tblArrivals
                            where a.guestHubID == guesthubID &&
                            a.arrivalDate >= DateTime.Today
                            select a);
            var guest = arrivals.FirstOrDefault();
            //var guest=db.tblArrivals.FirstOrDefault(a=>a.guestHubID==guesthubID);
            if (guest != null)
            {
                var destination = db.tblPlaces.FirstOrDefault(p => p.frontOfficeResortID == guest.frontOfficeResortID);
                long IDReservacion = guest.frontOfficeReservationID;
                //List<FrontOfficeModels.GarzaBlancaResortVallarta.spHistorialReservaciones_Result> reservaciones;
                //reservaciones = new List<FrontOfficeModels.GarzaBlancaResortVallarta.spHistorialReservaciones_Result>();
                //reservaciones = frontGBRV.spHistorialReservaciones((int)IDReservacion, null).ToList();


                float total = 0;
                foreach (var a in arrivals)
                {
                    var reservacionesP = db.tblGuestsPayments.FirstOrDefault(m => m.arrivalID == a.arrivalID);
                    if (reservacionesP != null)
                    {
                        total = 0;
                        if (reservacionesP.rent != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.rent);
                        // if (reservacionesP.p != null)
                        //   total = total + (float)a.Paquete;
                        if (reservacionesP.otherIncome != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.otherIncome);
                        if (reservacionesP.spa != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.spa);
                        if (reservacionesP.otherIncome != null)
                            total = total + (float)Convert.ToDouble(reservacionesP.otherIncome);




                        reservations.Add(new GuestReservationModel
                        {
                            Destination = destination.place,
                            Nights = (int)((DateTime)a.checkOut - (DateTime)a.arrivalDate).Days,
                            ArrivalDate = (DateTime)a.arrivalDate,
                            RoomType = a.roomType,
                            RoomNumber = "Unknown",
                            Adults = (int)a.adults,
                            Childrens = (int)a.children,
                            AgencyName = a.agencyName,
                            Source = a.source,
                            Total = total.ToString(),
                            //PlanType = a.,

                        });
                    }
                    else
                    {
                        reservations.Add(new GuestReservationModel
                        {
                            Destination = destination.place,
                            Nights = (int)((DateTime)a.checkOut - (DateTime)a.arrivalDate).Days,
                            ArrivalDate = (DateTime)a.arrivalDate,
                            RoomType = a.roomType,
                            RoomNumber = a.roomNumber,
                            Adults = (int)a.adults,
                            Childrens = (int)a.children,
                            AgencyName = a.agencyName,
                            Source = a.source,
                            Total = total.ToString(),
                            //PlanType = a.,

                        });

                    }
                }
            }
            return reservations;
        }

        public List<GuestSearchModel> GetArrivals(GuestSearchModel gsm)
        {

            ePlatEntities db = new ePlatEntities();
            DateTime datei;
            DateTime datef;
            List<GuestSearchModel> list = new List<GuestSearchModel>();
            DateTime hoy = DateTime.Today;
            List<tblGuestsHub> guests = new List<tblGuestsHub>();
            List<GuestSearchModel> nextArrivals = new List<GuestSearchModel>();
            int resortID = 0;
            int z = Convert.ToInt32(gsm.Search_Resort[0]);




            var migrated = db.tblArrivals.FirstOrDefault(m => m.guestHubID != null && m.dateSaved == hoy && m.frontOfficeResortID == z);

            /*if (migrated == null)
            {
                MigrateGuests(gsm);
            }*/





            if (gsm.SearchDatef == null)
            {
                datef = DateTime.Today;
            }
            else
            {
                datef = DateTime.Parse(gsm.SearchDatef);
            }



            if (gsm.SearchDateI == null)
            {
                datei = DateTime.Today;
            }
            else
            {
                datei = DateTime.Parse(gsm.SearchDateI);
            }

            gsm.SearchDatef = datef.ToString();
            gsm.SearchDateI = datei.ToString();


            MigrateGuests(gsm);
            /*
            //next dates
            if (datef > DateTime.Today || datei > DateTime.Today)
            {
                if (datei > DateTime.Today && datef > DateTime.Today)
                {
                    nextArrivals=SearchNextArrivals(datei, datef);

                }
                if (datei < DateTime.Today && datef > DateTime.Today)
                {
                    nextArrivals=SearchNextArrivals(DateTime.Today, datef);
                }
                if (datei > DateTime.Today && datef == null)
                {
                    nextArrivals=SearchNextArrivals(datei, datei);
                }
                if (datei == null && datef > DateTime.Today)
                {
                    nextArrivals=SearchNextArrivals(datef, datef);
                }
            }*/


            //search when preferences is not selected
            int frontOfficeResortID = z;
            //****************Search By CRS****************
            if (gsm.crs != null || gsm.FirstName != null)
            {
                if (gsm.crs != null)
                {
                    var query = (from g in db.tblGuestsHub
                                 join a in db.tblArrivals on g.guestHubID equals a.guestHubID
                                 join c in db.tblCountries on g.countryID equals c.countryID
                                 where a.crs == gsm.crs
                                 where a.frontOfficeResortID == frontOfficeResortID
                                 select new
                                 {
                                     GuestHubID = g.guestHubID,
                                     FirstName = g.firstName,
                                     LastName = g.lastName,
                                     Country = c.country,
                                     RoomNumber = a.roomNumber,
                                 }).Distinct();
                    foreach (var u in query)
                    {
                        list.Add(new GuestSearchModel()
                        {

                            GuestHubID = u.GuestHubID,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Country = u.Country,
                            RoomNumber = u.RoomNumber

                        });
                    }
                    foreach (var t in nextArrivals)
                        list.Add(t);
                    return list;
                }
                //**********Search By name*********
                if (gsm.FirstName != null)
                {
                    var query = (from g in db.tblGuestsHub
                                 join a in db.tblArrivals on g.guestHubID equals a.guestHubID
                                 join c in db.tblCountries on g.countryID equals c.countryID
                                 where a.firstName.Contains(gsm.FirstName)
                                 where a.frontOfficeResortID == frontOfficeResortID
                                 select new
                                 {
                                     GuestHubID = g.guestHubID,
                                     FirstName = g.firstName,
                                     LastName = g.lastName,
                                     Country = c.country,
                                     RoomNumber = a.roomNumber,
                                 }).Distinct();
                    foreach (var u in query)
                    {
                        list.Add(new GuestSearchModel()
                        {

                            GuestHubID = u.GuestHubID,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Country = u.Country,
                            RoomNumber = u.RoomNumber

                        });
                    }
                    foreach (var t in nextArrivals)
                        list.Add(t);
                    return list;
                }
            }
            if ((gsm.Preference == "-1") || (gsm.Preference == null))
            {
                //preferencia nula 
                var query = (from g in db.tblGuestsHub
                             join a in db.tblArrivals on g.guestHubID equals a.guestHubID
                             join c in db.tblCountries on g.countryID equals c.countryID
                             where a.arrivalDate >= datei
                             where a.arrivalDate <= datef
                             where a.frontOfficeResortID == frontOfficeResortID
                             select new
                             {
                                 GuestHubID = g.guestHubID,
                                 FirstName = g.firstName,
                                 LastName = g.lastName,
                                 Country = c.country,
                                 RoomNumber = a.roomNumber,
                             }).Distinct();
                foreach (var u in query)
                {
                    list.Add(new GuestSearchModel()
                    {

                        GuestHubID = u.GuestHubID,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Country = u.Country,
                        RoomNumber = u.RoomNumber

                    });
                }
                foreach (var t in nextArrivals)
                    list.Add(t);
                return list;
            }//**********hay una preferencia selecionada*********
            else
            {

                int preferenceType = Int32.Parse(gsm.Preference);
                //Search by date es una fecha
                if (preferenceType == 61)
                {
                    if (gsm.SearchSpecialDate != null)
                    {
                        DateTime date = DateTime.Parse(gsm.SearchSpecialDate);
                        var query = (from g in db.tblGuestsHub
                                     join a in db.tblArrivals on g.guestHubID equals a.guestHubID
                                     join c in db.tblCountries on g.countryID equals c.countryID
                                     join p in db.tblGuestPreferences on g.guestHubID equals p.guestHubID
                                     join d in db.tblImportantDates on p.guestPreferenceID equals d.guestPreferenceID
                                     where a.arrivalDate >= datei
                                     where a.arrivalDate <= datef
                                     where a.frontOfficeResortID == frontOfficeResortID
                                     where p.preferenceID == preferenceType
                                     where d.date == date
                                     select new
                                     {
                                         GuestHubID = g.guestHubID,
                                         FirstName = g.firstName,
                                         LastName = g.lastName,
                                         Country = c.country,
                                         RoomNumber = a.roomNumber,

                                     }).Distinct();

                        foreach (var u in query)
                        {
                            list.Add(new GuestSearchModel()
                            {

                                GuestHubID = u.GuestHubID,
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                Country = u.Country,
                                RoomNumber = u.RoomNumber


                            });
                        }
                        return list;
                    }
                }
                else
                {
                    //preferencia normal
                    var query = (from g in db.tblGuestsHub
                                 join a in db.tblArrivals on g.guestHubID equals a.guestHubID
                                 join c in db.tblCountries on g.countryID equals c.countryID
                                 join p in db.tblGuestPreferences on g.guestHubID equals p.guestHubID
                                 where a.arrivalDate >= datei
                                 where a.arrivalDate <= datef
                                 where a.frontOfficeResortID == frontOfficeResortID
                                 where p.preferenceID == preferenceType
                                 select new
                                 {
                                     GuestHubID = g.guestHubID,
                                     FirstName = g.firstName,
                                     LastName = g.lastName,
                                     Country = c.country,
                                     RoomNumber = a.roomNumber,

                                 }).Distinct();

                    foreach (var u in query)
                    {
                        list.Add(new GuestSearchModel()
                        {

                            GuestHubID = u.GuestHubID,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Country = u.Country,
                            RoomNumber = u.RoomNumber,

                        });
                    }
                    return list;
                }
            }
            return list;

        }
        public List<GuestSearchModel> SearchNextArrivals(DateTime datei, DateTime datef)
        {
            FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities frontGBRV = new FrontOfficeModels.GarzaBlancaResortVallarta.FrontOfficeGBRVEntities();
            List<FrontOfficeModels.GarzaBlancaResortVallarta.spLlegadas_Result> guests = frontGBRV.spLlegadas(datei, datef).ToList();
            List<GuestSearchModel> list = new List<GuestSearchModel>();
            foreach (var u in guests)
            {

                list.Add(new GuestSearchModel()
                {


                    FirstName = u.nombres,
                    LastName = u.apellidopaterno + " " + u.apellidomaterno,
                    Country = u.codepais,
                    RoomNumber = "UNASIGNED",

                });
            }
            return list;

        }
        public static List<SelectListItem> GetResortList()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> rl = new List<SelectListItem>();

            var query = (from u in db.tblPlaces
                         where u.placeTypeID == 1 && u.vg == true && u.frontOfficeResortID != null
                         select new
                         {
                             Id = u.frontOfficeResortID,
                             Name = u.place,
                             Name1 = u.oldPlaceName
                         }).Distinct();

            foreach (var u in query)
            {
                if (u.Name1 == null)
                {
                    rl.Add(new SelectListItem()
                    {
                        Value = ((int)u.Id).ToString(),
                        Text = u.Name,
                    });
                }
                else
                {
                    rl.Add(new SelectListItem()
                    {
                        Value = ((int)u.Id).ToString(),
                        Text = u.Name1.ToString(),
                    });
                }
            }
            return rl;
        }

        public static List<SelectListItem> GetTypePreferencesList()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> tpl = new List<SelectListItem>();

            tpl.Add(new SelectListItem()
            {
                Value = "-1",
                Text = "---Select---",
            });

            foreach (var u in db.tblPreferenceTypes.OrderBy(m => m.preferenceTypeID))
            {
                tpl.Add(new SelectListItem()
                {
                    Value = ((int)u.preferenceTypeID).ToString(),
                    Text = u.name
                });
            }
            return tpl;
        }
        public List<SelectListItem> GetPreferencesList(String Type)
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> tpl = new List<SelectListItem>();


            int x = Int32.Parse(Type);
            var query = (from u in db.tblPreferences
                         where u.preferenceTypeID == x
                         select new
                         {
                             Id = u.preferenceID,
                             Name = u.name
                         }).Distinct();

            foreach (var u in query)
            {
                tpl.Add(new SelectListItem()
                {
                    Value = ((int)u.Id).ToString(),
                    Text = u.Name,
                });
            }
            return tpl;
        }

        public static List<SelectListItem> GetPreferencesListDefault()
        {
            ePlatEntities db = new ePlatEntities();
            List<SelectListItem> tpl = new List<SelectListItem>();
            var query = (from u in db.tblPreferences
                         where u.preferenceTypeID == 1
                         select new
                         {
                             Id = u.preferenceID,
                             Name = u.name
                         }).Distinct();
            tpl.Add(new SelectListItem()
            {
                Value = "-1",
                Text = "---Select---",
            });
            foreach (var u in query)
            {
                tpl.Add(new SelectListItem()
                {
                    Value = ((int)u.Id).ToString(),
                    Text = u.Name,
                });
            }
            return tpl;
        }

        public AttemptResponse SaveGuestHub(GuestInfoModel gim)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {

                tblGuestsHub tblGuest = new tblGuestsHub();
                tblGuest.firstName = gim.FirstName;
                tblGuest.lastName = gim.LastName;
                tblGuest.email1 = gim.Email;
                tblGuest.email2 = gim.Email2;
                tblGuest.email3 = gim.Email3;
                tblGuest.city = gim.City;
                tblGuest.state = gim.State;
                tblGuest.guestHubID = gim.GuestHubID;
                tblGuest.countryID = gim.CountryID;
                db.tblGuestsHub.AddObject(tblGuest);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "User Saved";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "User NOT Saved";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse SaveGuestPreference(PreferenceRegistrationModel prm)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            DateTime date = DateTime.Parse(prm.YearValue + "-" + prm.MonthValue + "-" + prm.DayValue);
            if (prm.PreferenceType_Value == "8")
            {

                var fecharepetida = (from gp in db.tblGuestPreferences
                                     join id in db.tblImportantDates on gp.guestPreferenceID equals id.guestPreferenceID
                                     where gp.guestPreferenceID == id.guestPreferenceID
                                     where gp.guestHubID == new Guid(prm.GuestHubID)
                                     where id.name == prm.SpecialDateName
                                     where id.date == date
                                     select gp.preferenceID).Count();
                int z = Int32.Parse(prm.PreferenceValue);
                if (fecharepetida == 0)
                {
                    try
                    {//special date saved
                        Guid guestPreferenceID;

                        guestPreferenceID = Guid.NewGuid();
                        tblGuestPreferences tblguestpreferences = new tblGuestPreferences();
                        tblguestpreferences.guestPreferenceID = guestPreferenceID;
                        tblguestpreferences.preferenceID = 61;
                        tblguestpreferences.guestHubID = new Guid(prm.GuestHubID);
                        db.tblGuestPreferences.AddObject(tblguestpreferences);
                        tblImportantDates tblimportantdates = new tblImportantDates();
                        tblimportantdates.date = date;
                        tblimportantdates.name = prm.SpecialDateName;
                        tblimportantdates.guestPreferenceID = guestPreferenceID;
                        db.tblImportantDates.AddObject(tblimportantdates);
                        db.SaveChanges();
                        response.Type = Attempt_ResponseTypes.Ok;
                        response.Message = "Special Date Saved";
                        response.ObjectID = 0;
                        return response;
                    }
                    catch (Exception ex)
                    {//special date not saved
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "Special Date NOT Saved";
                        response.ObjectID = -1;
                        response.Exception = ex;
                        return response;
                    }
                }
                else
                {//specialdate already exist
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "Special Date is already Saved";
                    response.ObjectID = 0;
                    return response;
                }

            }
            else
            {
                Int32 pref = Int32.Parse(prm.PreferenceValue);

                var repetido = db.tblGuestPreferences.FirstOrDefault(m => m.guestHubID == new Guid(prm.GuestHubID) && m.preferenceID == pref);
                if (repetido != null)
                {
                    //preference already exist
                    response.Type = Attempt_ResponseTypes.Warning;
                    response.Message = "Preference is already Saved";
                    response.ObjectID = 0;
                    return response;
                }
                try
                {
                    //preference saved
                    Guid GuestPreferenceID;
                    GuestPreferenceID = Guid.NewGuid();
                    tblGuestPreferences tblguestpreferences = new tblGuestPreferences();
                    tblguestpreferences.guestPreferenceID = GuestPreferenceID;
                    tblguestpreferences.preferenceID = Int32.Parse(prm.PreferenceValue);
                    tblguestpreferences.guestHubID = new Guid(prm.GuestHubID);
                    db.tblGuestPreferences.AddObject(tblguestpreferences);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Preference Saved";
                    response.ObjectID = 0;
                    return response;
                }
                catch (Exception ex)
                {
                    //preference not saved
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Preference NOT Saved";
                    response.ObjectID = -1;
                    response.Exception = ex;
                    return response;
                }
            }

        }

        public List<GuestPreferences> GetPreferencesByGuest(string GuestHubID)
        {
            ePlatEntities db = new ePlatEntities();
            List<GuestPreferences> gp = new List<GuestPreferences>();
            var query = (from gid in db.tblGuestPreferences
                         join p in db.tblPreferences on gid.preferenceID equals p.preferenceID
                         where gid.guestHubID == new Guid(GuestHubID)
                         select new
                         {
                             PreferenceID = p.preferenceID,
                             Preference = p.name,
                             PreferenceTypeID = p.preferenceTypeID,
                             GuestHubID = gid.guestHubID,
                             GuestPreferenceID = gid.guestPreferenceID,
                         }).Distinct();
            if (query.Count() > 0)
            {
                foreach (var u in query)
                {
                    if (u.PreferenceID == 61)
                    {
                        var SpecialDate = from z in db.tblImportantDates where z.guestPreferenceID == u.GuestPreferenceID select z;
                        foreach (var t in SpecialDate)
                        {
                            gp.Add(new GuestPreferences()
                            {
                                DateName = t.name,
                                Date = t.date.ToString(),
                                ImportantDatesID = t.importantDatesID,
                                GuestPreferencesID = t.guestPreferenceID,
                                PreferenceID = u.PreferenceID,
                                GuestHubID = u.GuestHubID.ToString()
                            });
                        }

                    }
                    else
                    {
                        gp.Add(new GuestPreferences()
                        {
                            PreferenceID = u.PreferenceID,
                            PreferenceName = u.Preference,
                            PreferenceType = u.PreferenceTypeID,
                            GuestHubID = u.GuestHubID.ToString(),
                            GuestPreferencesID = u.GuestPreferenceID,
                        });
                    }

                }
            }
            return gp;
        }

        public AttemptResponse DeletePreference(string preference, string preferenceType)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            Guid preferenceID = new Guid(preference);

            if (preferenceType != "61")
            {
                try
                {
                    using (db)
                    {
                        var x = db.tblGuestPreferences.FirstOrDefault(m => m.guestPreferenceID == preferenceID);
                        db.tblGuestPreferences.DeleteObject(x);
                        db.SaveChanges();
                    }

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Preference Deleted";
                    response.ObjectID = 0;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Preference Not Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    using (db)
                    {
                        var x = db.tblGuestPreferences.FirstOrDefault(m => m.guestPreferenceID == preferenceID);
                        db.tblGuestPreferences.DeleteObject(x);
                        var y = db.tblImportantDates.FirstOrDefault(m => m.guestPreferenceID == preferenceID);
                        db.tblImportantDates.DeleteObject(y);
                        db.SaveChanges();
                    }

                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Preference Deleted";
                    response.ObjectID = 0;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Preference Not Deleted";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            response.Type = Attempt_ResponseTypes.Error;
            response.Message = "Preference Not Deleted";
            response.ObjectID = 0;

            return response;
        }
        /////////////////////////////////////////////////////////////////
        public GuestInfoModel GetGuestInfoByReservationID(string reservationID)
        {
            ePlatEntities db = new ePlatEntities();
            GuestInfoModel guest = new GuestInfoModel();
            int idreservacion = 0;
            if (reservationID != null)
                idreservacion = Int32.Parse(reservationID);
            //var query= db.tblGuestsHub.FirstOrDefault();
            var query = db.tblArrivals.FirstOrDefault(g => g.frontOfficeReservationID == idreservacion);
            if (query != null)
            {
                var guestquery = db.tblGuestsHub.FirstOrDefault(g => g.guestHubID == query.guestHubID);
                guest.GuestHubID = guestquery.guestHubID;
                guest.FirstName = guestquery.firstName;
                guest.LastName = guestquery.lastName;
                guest.Email = guestquery.email1;
            }
            return guest;

        }

        public AttemptResponse LoadFileHE(string adress)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            int filas, columnas;
            string str;
            Excel.Application xlapp = new Excel.Application();
            Excel.Workbook xlworkbook;
            Excel.Worksheet excelSheet;
            Excel.Range range;
            xlapp = new Excel.Application();
            List<tblTasks> data = new List<tblTasks>();
            HotelExpertFile row = new HotelExpertFile();

            try
            {

                xlworkbook = xlapp.Workbooks.Open(
                    @adress,
                    0,
                    true,
                    5,
                    "",
                    "",
                    true,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
                    "/t",
                    false,
                    false,
                    0,
                    true,
                    1,
                    0);
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "There was an error opening the Excel File";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;

            }

            excelSheet = (Excel.Worksheet)xlworkbook.Worksheets.get_Item(1);
            //excelSheet = xlworkbook.ActiveSheet;

            //rango
            range = (Excel.Range)excelSheet.get_Range("A1", "F1");
            //celdas ocupadas
            range = (Excel.Range)excelSheet.UsedRange;
            filas = range.Rows.Count;
            columnas = range.Columns.Count;


            try
            {
                for (int y = 14650; y < filas; y++)
                {
                    row = new HotelExpertFile();
                    for (int x = 1; x < columnas; x++)
                    {
                        if (y != 1)
                        {

                            if (x == 1)
                                row.callNumber = (int)(Int32.Parse((range.Cells[y, x] as Excel.Range).Value));
                            if (x == 2)
                                row.callIndate = (DateTime)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 3)
                                row.statCode = (int)(Int32.Parse((range.Cells[y, x] as Excel.Range).Value));
                            if (x == 4)
                                row.room = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 5)
                                row.guest = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 6)
                                row.problemDefinition = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 7)
                                row.receiver = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 8)
                                row.shift = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 9)
                                row.resolution = (int)(Int32.Parse((range.Cells[y, x] as Excel.Range).Value));
                            if (x == 10)
                                row.repairman = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 11)
                                row.callOutdate = (DateTime)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 12)
                                row.closedBy = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 13)
                                row.closedByDepartment = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 14)
                                row.timerStart = (DateTime)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 15)
                                row.callerCategory = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 16)
                                row.problemNumber = (int)(Int32.Parse((range.Cells[y, x] as Excel.Range).Value));
                            if (x == 17)
                                row.subProblemNumber = (int)(Int32.Parse((range.Cells[y, x] as Excel.Range).Value));
                            if (x == 18)
                                row.compute01 = (float)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 19)
                                row.problemDescription = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 20)
                                row.subProblemSubDesc = (string)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 21)
                                row.compute02 = (double)(range.Cells[y, x] as Excel.Range).Value;
                            if (x == 22)
                                row.compute024 = (double)(range.Cells[y, x] as Excel.Range).Value;

                        }


                    }
                    if (y != 1)
                        db.tblTasks.AddObject(SaveTask(row));
                    db.SaveChanges();

                }
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Upload Finished";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "There was an error in the upload of the file";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }




        }

        public tblTasks SaveTask(HotelExpertFile task)
        {
            ePlatEntities db = new ePlatEntities();
            tblTasks tbltask = new tblTasks();
            tbltask.room = task.room;
            tbltask.caller = task.guest;
            tbltask.task = task.problemDefinition;
            tbltask.subTask = task.subProblemSubDesc;
            tbltask.handledBy = task.repairman;
            tbltask.opened = task.callIndate;
            tbltask.closed = task.callOutdate;
            tbltask.status = task.statCode;
            tbltask.closedBy = task.closedBy;
            tbltask.scheduled = task.receiver;
            if ((((task.callOutdate - task.callIndate).Days == 0)) && (task.callOutdate > task.callIndate))
                tbltask.duration = task.callOutdate - task.callIndate;
            tbltask.callNumber = task.callNumber;
            tbltask.closedByDepartmet = task.closedByDepartment;
            tbltask.problemDesc = task.problemDescription;
            tbltask.callerCategory = task.callerCategory;
            //db.tblTasks.AddObject(tbltask);
            return tbltask;



        }

        public void BindGuestToTask()
        {
            List<int> lista = new List<int>();
            char[] delimiter1 = new char[] { 'A', 'B', 'a', 'b' };
            ePlatEntities db = new ePlatEntities();
            var query = from a in db.tblArrivals
                        where a.guestHubID != null
                        select new
                        {
                            Room = a.roomNumber,
                            DateIn = a.arrivalDate,
                            DateOut = a.checkOut,
                            ID = a.arrivalID
                        };
            int u = 0;
            foreach (var m in query)
            {
                string[] room = m.Room.Split(delimiter1, StringSplitOptions.RemoveEmptyEntries);
                string rm = string.Join("", room);
                var task = db.tblTasks.FirstOrDefault(z => z.room == rm);
                if (task != null)
                {
                    lista.Add(task.taskId);
                }

            }


        }

        public List<GuestTasksModel> GetTasks(string GuestHubID)
        {
            List<GuestTasksModel> tasks = new List<GuestTasksModel>();
            ePlatEntities db = new ePlatEntities();
            Guid ID = new Guid(GuestHubID);
            var query = db.tblTasks.FirstOrDefault(m => m.guestHubID == ID);
            GuestTasksModel task = new GuestTasksModel();
            if (ID == new Guid("8FD45F12-D669-4F18-9CEC-4BCD5C1F6B5D"))
                query = db.tblTasks.FirstOrDefault();
            if (query != null)
            {
                if (query.guestHubID != null)
                    task.GuestHubID = (Guid)query.guestHubID;
                task.taskID = query.taskId;
                task.task = query.task;
                task.subTask = query.subTask;
                task.date = (DateTime)query.opened;
                task.room = query.room;

                tasks.Add(task);
            }
            return tasks;


        }

        public string StringAdapter(string cadena)
        {
            int z = 0;
            int flag = 0;
            string date = "";

            char temp = ' ';
            char temp1 = ' ';

            foreach (char c in cadena)
            {
                z++;
                if (flag < 2)
                {
                    if (c == '/')
                    {
                        flag++;
                        if (z <= 3)
                        {
                            if (z == 2)
                            {
                                date = date + "0" + temp + "/";
                            }
                            else
                            {
                                date = date + temp1 + temp + "/";
                            }

                        }

                        z = 0;
                    }
                }
                else
                {
                    date = date + c;
                }
                temp1 = temp;
                temp = c;
            }
            string month = "";
            string day = "";

            bool isDay = false;
            bool isMonth = false;


            foreach (char c in date)
            {
                if (c == '/' && isMonth == true && isDay == false)
                {
                    date = day + "/" + month;
                    isDay = true;
                }
                if (c == '/' && isMonth == false)
                    isMonth = true;



                if (isMonth == true && isDay == true)
                    date = date + c;

                if (c != '/' && isMonth == false)
                    month = month + c;



                if (c != '/' && isMonth == true && isDay == false)
                    day = day + c;

            }
            return date;
        }

        public string TakeOffComillas(string cadena)
        {
            string result = "";
            foreach (char c in cadena)
            {
                if (c != '"')
                    result = result + c;
            }
            return result;
        }
        public Tuple<string, string> PullApartTopic(string cadena)
        {
            string topic = "";
            string question = "";
            int x = 0;
            foreach (char c in cadena)
            {
                if (x == 0)
                {

                    if (c == ':')
                        x = 1;
                    else
                        topic = topic + c;
                }
                else
                {
                    question = question + c;
                }
            }
            Tuple<string, string> t = new Tuple<string, string>(topic, question);
            return t;
        }
        public AttemptResponse LoadFileSurveys(string adress)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            int filas, columnas;
            string str;
            Excel.Application xlapp = new Excel.Application();
            Excel.Workbook xlworkbook;
            Excel.Worksheet excelSheet;
            Excel.Range range;
            xlapp = new Excel.Application();
            List<tblSurveys> data = new List<tblSurveys>();
            SurveysFile row = new SurveysFile();
            tblTopicQuestions topic = new tblTopicQuestions();
            tblQuestions question = new tblQuestions();

            try
            {

                xlworkbook = xlapp.Workbooks.Open(
                    @adress,
                    0,
                    true,
                    5,
                    "",
                    "",
                    true,
                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
                    "/t",
                    false,
                    false,
                    0,
                    true,
                    1,
                    0);
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "There was an error opening the Excel File";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;

            }

            excelSheet = (Excel.Worksheet)xlworkbook.Worksheets.get_Item(1);
            //excelSheet = xlworkbook.ActiveSheet;


            //celdas ocupadas
            range = (Excel.Range)excelSheet.UsedRange;
            filas = range.Rows.Count;
            columnas = range.Columns.Count;


            try
            {
                for (int y = 1; y < filas; y++)
                {
                    row = new SurveysFile();

                    for (int x = 1; x < columnas; x++)
                    {

                        if (y == 2)
                        {
                            if (x >= 23)
                            {
                                Tuple<string, string> t = PullApartTopic((string)(range.Cells[y, x] as Excel.Range).Value);
                                topic = db.tblTopicQuestions.FirstOrDefault(m => m.name == t.Item1);
                                if (topic == null)
                                {
                                    //*********Topic************
                                    question = new tblQuestions();
                                    if (t.Item2 == "")
                                    {

                                        question.topicQuestionID = 45;
                                        question.name = t.Item1;
                                        //test
                                        question.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        question.dateSaved = DateTime.Now;
                                        if ((db.tblQuestions.FirstOrDefault(m => m.topicQuestionID == question.topicQuestionID && m.name == question.name)) == null)
                                        {
                                            db.tblQuestions.AddObject(question);
                                            db.SaveChanges();
                                        }

                                    }
                                    else
                                    {
                                        topic = new tblTopicQuestions();

                                        topic.name = t.Item1;
                                        //test
                                        topic.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        topic.dateSaved = DateTime.Now;
                                        topic.surveyID = 1;
                                        if ((db.tblTopicQuestions.FirstOrDefault(m => m.surveyID == topic.surveyID && m.name == topic.name)) == null)
                                        {
                                            db.tblTopicQuestions.AddObject(topic);
                                            db.SaveChanges();
                                        }
                                        //**********Question*********
                                        question = new tblQuestions();
                                        question.name = t.Item2;
                                        //test
                                        question.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                        question.dateSaved = DateTime.Now;
                                        if (t.Item2 != "")
                                        {
                                            topic = db.tblTopicQuestions.FirstOrDefault(m => m.name == t.Item1);
                                            question.topicQuestionID = topic.topicQuestionID;
                                        }

                                        if ((db.tblQuestions.FirstOrDefault(m => m.topicQuestionID == question.topicQuestionID && m.name == question.name)) == null)
                                        {
                                            db.tblQuestions.AddObject(question);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    //**********Question*******+**

                                    question = new tblQuestions();



                                    question.topicQuestionID = topic.topicQuestionID;

                                    question.name = t.Item2;
                                    //test
                                    question.userID = new Guid("F37ABFB9-035B-4E48-8BAF-1F65EB7F42A4");
                                    question.dateSaved = DateTime.Now;
                                    if ((db.tblQuestions.FirstOrDefault(m => m.topicQuestionID == question.topicQuestionID && m.name == question.name)) == null)
                                    {
                                        db.tblQuestions.AddObject(question);
                                        db.SaveChanges();
                                    }
                                }

                            }
                        }
                        if (y > 2)
                        {

                            if (x == 1)
                                row.key = TakeOffComillas((string)(range.Cells[y, x] as Excel.Range).Value);
                            if (x == 2)
                                row.dateIn = DateTime.Parse(StringAdapter((string)(range.Cells[y, x] as Excel.Range).Value));

                            if (x == 3)
                                row.dateOut = DateTime.Parse(StringAdapter((string)(range.Cells[y, x] as Excel.Range).Value));
                            if (x == 18)
                                row.room = TakeOffComillas((string)(range.Cells[y, x] as Excel.Range).Value);
                            if (x == 4)
                                row.email = TakeOffComillas((string)(range.Cells[y, x] as Excel.Range).Value);
                            if (x == 7)
                                row.organziationID = TakeOffComillas((string)(range.Cells[y, x] as Excel.Range).Value);
                            if (x == 8)
                            {
                                if (((string)(range.Cells[y, x] as Excel.Range).Value) == "true")
                                    row.hadComment = true;
                                else
                                    row.hadComment = false;

                            }

                            if (x == 9)
                                if (((string)(range.Cells[y, x] as Excel.Range).Value) == "true")
                                    row.hadProblem = true;
                                else
                                    row.hadProblem = false;
                            if (x == 10)
                                row.propertyCode = (int)Int32.Parse(TakeOffComillas((string)(range.Cells[y, x] as Excel.Range).Value));
                            if (x == 15)
                                row.marketCodeID = TakeOffComillas((string)(range.Cells[y, x] as Excel.Range).Value);
                            if (x == 22)
                                row.rate = (float)Convert.ToDouble((range.Cells[y, x] as Excel.Range).Value);
                            if (x == 19)
                                row.sentDate = DateTime.Parse(StringAdapter((string)(range.Cells[y, x] as Excel.Range).Value));
                            if (x == 20)
                                row.submittedDate = DateTime.Parse(StringAdapter((string)(range.Cells[y, x] as Excel.Range).Value));
                            if (x == 21)
                                row.surveyLenguage = TakeOffComillas((string)(range.Cells[y, x] as Excel.Range).Value);


                        }


                    }
                    if (y != 1)
                        db.tblAppliedSurveys.AddObject(SaveSurvey(row));
                    db.SaveChanges();

                }
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Upload Finished";
                response.ObjectID = 0;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "There was an error in the upload of the file";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }




        }

        public tblAppliedSurveys SaveSurvey(SurveysFile row)
        {
            ePlatEntities db = new ePlatEntities();
            tblAppliedSurveys survey = new tblAppliedSurveys();
            survey.key_key = row.key;
            survey.dateIn = row.dateIn;
            survey.dateOut = row.dateOut;
            survey.email = row.email;
            survey.organizationID = row.organziationID;
            survey.hadComment = row.hadComment;
            survey.hadProblem = row.hadProblem;
            survey.propertyCode = row.propertyCode;
            survey.marketCodeID = row.marketCodeID;
            survey.rate = row.rate;
            survey.Room = row.room;
            survey.sentDate = row.sentDate;
            survey.submittedDate = row.submittedDate;
            survey.surveyLenguage = row.surveyLenguage;

            return survey;
        }

        public string GetCountStays(string GuestHubID)
        {
            ePlatEntities db = new ePlatEntities();
            Guid ID = new Guid(GuestHubID);
            string cuenta = db.tblArrivals.Count(m => m.guestHubID == ID).ToString();
            return cuenta;
        }

        public static PictureDataModel.FineUploaderResult SaveGuestPicture(PictureDataModel.FineUpload upload)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            string path = HttpContext.Current.Server.MapPath("~/");
            path = path.Substring(0, path.LastIndexOf("ePlatBack"));
            path = path + "ePlatBack\\Images\\GuestsPictures";
            var fileName = upload.Filename;

            var fileNameDecoded = HttpContext.Current.Server.UrlDecode(fileName);
            fileName = HttpUtility.UrlEncode(fileNameDecoded, Encoding.GetEncoding("iso-8859-8"));
            fileName = fileName.Replace("+", "");
            for (var i = 0; i < Regex.Matches(fileName, "%").Count; i++)
            {
                string encoded = fileName.Substring(fileName.IndexOf("%"), ((fileName.IndexOf("%") + 3) - fileName.IndexOf("%")));
                string newFileName = fileName.Replace(encoded, "_");
                fileName = newFileName;
            }

            string filePath = path + "\\" + fileName;
            //try
            //{
            upload.SaveAs(filePath);

            //PROCESO PARA EXTRAER LA INFORMACIÓN
            //ImportFromCSV(filePath);

            response.Type = Attempt_ResponseTypes.Ok;
            response.Message = "File Imported";
            //response.ObjectID = ;
            return new PictureDataModel.FineUploaderResult(true, new { response = response });
            //}
            //catch (Exception ex)
            //{
            //    response.Type = Attempt_ResponseTypes.Error;
            //    response.Message = "File NOT Imported";
            //    response.ObjectID = 0;
            //    response.Exception = ex;
            //    return new PictureDataModel.FineUploaderResult(false, new { response = response });
            //}
        }

        public tblAppliedSurveys GetClaraBridgeInfo(string GuestHubID)
        {
            ePlatEntities db = new ePlatEntities();
            Guid ID = new Guid(GuestHubID);
            tblAppliedSurveys appliedSurveyTBL = new tblAppliedSurveys();
            var query = db.tblArrivals.Where(m => m.guestHubID == ID);
            var email = from a in db.tblArrivals
                        join ap in db.tblAppliedSurveys on a.roomNumber equals ap.Room
                        where a.guestHubID == ID
                        && a.roomNumber != null
                        && a.roomNumber != "-"
                        && a.arrivalDate == ap.dateOut
                        select ap;
            if (email.Count() > 0)
            {
                var guest = db.tblGuestsHub.FirstOrDefault(m => m.guestHubID == ID);
                appliedSurveyTBL = (tblAppliedSurveys)email.First();
                guest.email1 = appliedSurveyTBL.email;
                appliedSurveyTBL.guestHubID = guest.guestHubID;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return appliedSurveyTBL;
                };

            }
            return appliedSurveyTBL;
            /*foreach(var arrival in query)
            {
                if (arrival.roomNumber != null && arrival.roomNumber != "-")
                {
                    
                    if ((db.tblAppliedSurveys.FirstOrDefault(m => m.Room == arrival.roomNumber) != null))
                    {
                        
                        appliedSurveyTBL = db.tblAppliedSurveys.FirstOrDefault(m => m.Room == arrival.roomNumber && m.dateOut== arrival.arrivalDate);
                        if(appliedSurveyTBL!=null)
                        {
                            if (db.tblGuestsHub.FirstOrDefault(m => m.guestHubID == ID) != null)
                            {
                                var guest = db.tblGuestsHub.FirstOrDefault(m => m.guestHubID == ID);
                                if (appliedSurveyTBL.email!=null)
                                guest.email1 = appliedSurveyTBL.email;
                                
                                appliedSurveyTBL.guestHubID = guest.guestHubID;
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    break;
                                };
                                break;
                            }
                        }

                    }
                }*/

        }


        public List<PreferencesAPI> GetPreferencesByGuestAPI(GuestPreferencesApi gpa)
        {
            ePlatEntities db = new ePlatEntities();
            int IDhuesped, idresort;
            idresort = gpa.ResortID;
            IDhuesped = gpa.GuestID;
            List<PreferencesAPI> gp = new List<PreferencesAPI>();
            var query = (from arrival in db.tblArrivals
                         join guest in db.tblGuestsHub on arrival.guestHubID equals guest.guestHubID
                         join guestpref in db.tblGuestPreferences on guest.guestHubID equals guestpref.guestHubID
                         join pref in db.tblPreferences on guestpref.preferenceID equals pref.preferenceID
                         where arrival.frontOfficeResortID == idresort
                         && arrival.frontOfficeGuestID == IDhuesped
                         select new
                         {
                             PreferenceID = pref.preferenceID,
                             Preference = pref.name,
                             PreferenceTypeID = pref.preferenceTypeID,
                             GuestPreferenceID = guestpref.guestPreferenceID,
                             GuestHubID = guest.guestHubID,
                         }).Distinct();




            /*var query = (from gid in db.tblGuestPreferences
                         join p in db.tblPreferences on gid.preferenceID equals p.preferenceID
                         where gid.guestHubID == new Guid(GuestHubID)
                         select new
                         {
                             PreferenceID = p.preferenceID,
                             Preference = p.name,
                             PreferenceTypeID = p.preferenceTypeID,
                             GuestHubID = gid.guestHubID,
                             GuestPreferenceID = gid.guestPreferenceID,
                         }).Distinct();*/
            if (query.Count() > 0)
            {
                foreach (var u in query)
                {
                    if (u.PreferenceID == 61)
                    {
                        var SpecialDate = from z in db.tblImportantDates where z.guestPreferenceID == u.GuestPreferenceID select z;
                        foreach (var t in SpecialDate)
                        {
                            gp.Add(new PreferencesAPI
                            {
                                Preference = t.name,
                                PreferenceID = u.PreferenceID,
                                PreferenceTypeID = u.PreferenceTypeID
                            });
                        }

                    }
                    else
                    {
                        gp.Add(new PreferencesAPI
                        {
                            Preference = u.Preference,
                            PreferenceTypeID = u.PreferenceTypeID,
                            PreferenceID = u.PreferenceID
                        });
                    }

                }
            }
            return gp;
        }


        public GuestPreferencesApi GetPreferencesGuestByGuestAPI(GuestPreferencesApi gpaa)
        {
            ePlatEntities db = new ePlatEntities();
            int IDhuesped, idresort;
            idresort = gpaa.ResortID;
            IDhuesped = gpaa.GuestID;
            List<PreferencesAPI> gp = new List<PreferencesAPI>();
            var query = (from arrival in db.tblArrivals
                         join guest in db.tblGuestsHub on arrival.guestHubID equals guest.guestHubID
                         join guestpref in db.tblGuestPreferences on guest.guestHubID equals guestpref.guestHubID
                         join pref in db.tblPreferences on guestpref.preferenceID equals pref.preferenceID
                         where arrival.frontOfficeResortID == idresort
                         && arrival.frontOfficeGuestID == IDhuesped
                         select new
                         {
                             GuestName = guest.firstName + " " + guest.lastName,
                             Email = guest.email1,
                             PreferenceID = pref.preferenceID,
                             Preference = pref.name,
                             PreferenceTypeID = pref.preferenceTypeID,
                             GuestPreferenceID = guestpref.guestPreferenceID,
                             GuestHubID = guest.guestHubID,
                         }).Distinct();




            /*var query = (from gid in db.tblGuestPreferences
                         join p in db.tblPreferences on gid.preferenceID equals p.preferenceID
                         where gid.guestHubID == new Guid(GuestHubID)
                         select new
                         {
                             PreferenceID = p.preferenceID,
                             Preference = p.name,
                             PreferenceTypeID = p.preferenceTypeID,
                             GuestHubID = gid.guestHubID,
                             GuestPreferenceID = gid.guestPreferenceID,
                         }).Distinct();*/
            if (query.Count() > 0)
            {
                foreach (var u in query)
                {
                    if (u.PreferenceID == 61)
                    {
                        var SpecialDate = from z in db.tblImportantDates where z.guestPreferenceID == u.GuestPreferenceID select z;
                        foreach (var t in SpecialDate)
                        {
                            gp.Add(new PreferencesAPI
                            {
                                Preference = t.name,
                                PreferenceID = u.PreferenceID,
                                PreferenceTypeID = u.PreferenceTypeID
                            });
                        }

                    }
                    else
                    {
                        gp.Add(new PreferencesAPI
                        {
                            Preference = u.Preference,
                            PreferenceTypeID = u.PreferenceTypeID,
                            PreferenceID = u.PreferenceID
                        });
                    }

                }
            }
            var query2 = db.tblArrivals.FirstOrDefault(m => m.frontOfficeGuestID == IDhuesped && m.frontOfficeResortID == idresort);
            string name = query2.firstName + " " + query2.lastName;
            Guid id = new Guid(query2.guestHubID.ToString());
            var query3 = db.tblGuestsHub.FirstOrDefault(m => m.guestHubID == id);
            GuestPreferencesApi gpa = new GuestPreferencesApi();
            gpa.Email = query3.email1;
            gpa.Preferences = gp;
            gpa.GuestName = name;
            gpa.GuestHubID = id;
            gpa.GuestID = gpaa.GuestID;
            gpa.ResortID = gpaa.ResortID;

            return gpa;
        }

        public AttemptResponse SaveGuestPreferenceAPI(GuestPreferencesApi gpa)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            tblGuestPreferences guestpreferencestbl = new tblGuestPreferences();
            gpa.GuestHubID = new Guid(db.tblArrivals.FirstOrDefault(k => k.frontOfficeGuestID == gpa.GuestID && k.frontOfficeResortID == gpa.ResortID).guestHubID.ToString());
            foreach (var preference in gpa.Preferences)
            {


                if (db.tblGuestPreferences.FirstOrDefault(m => m.guestHubID == gpa.GuestHubID && m.preferenceID == preference.PreferenceID) == null)
                {
                    guestpreferencestbl = new tblGuestPreferences();
                    guestpreferencestbl.preferenceID = preference.PreferenceID;
                    guestpreferencestbl.guestHubID = gpa.GuestHubID;
                    guestpreferencestbl.guestPreferenceID = Guid.NewGuid();
                    db.tblGuestPreferences.AddObject(guestpreferencestbl);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        response.Type = Attempt_ResponseTypes.Error;
                        response.Message = "There was an error updating the preferences, try it again later";
                        response.ObjectID = -1;
                        response.Exception = ex;
                        return response;
                    }
                }
            }
            response.Type = Attempt_ResponseTypes.Ok;
            response.Message = "The preferences was updated correctly";
            response.ObjectID = 1;

            return response;


        }

        public List<SurveysAnswers> GetSurveyAnswers(string SAID)
        {
            ePlatEntities db = new ePlatEntities();
            List<string> numeros = new List<string>();
            string r;
            for (int i = 0; i <= 100; i++)
            {
                numeros.Add(Convert.ToString(i));
            }

            int appliedSurvey = Convert.ToInt32(SAID);
            List<SurveysAnswers> sa = new List<SurveysAnswers>();
            var query = (from question in db.tblQuestions
                         join answer in db.tblAnswers on question.questionID equals answer.questionID
                         join applied in db.tblAppliedSurveys on answer.surveyAppliedID equals applied.surveyAppliedID
                         join survey in db.tblSurveys on applied.surveyID equals survey.surveyID
                         join tema in db.tblTopicQuestions on survey.surveyID equals tema.surveyID
                         where tema.topicQuestionID == question.topicQuestionID
                         && applied.surveyAppliedID == appliedSurvey
                         && numeros.Contains(answer.answer)
                         select new
                         {
                             Topic = tema.name,
                             Answer = answer.answer
                         }
                        ).Distinct();

            //string topic="";
            //int answers=0;
            //int flag=0;
            foreach (var j in query)
            {
                sa.Add(new SurveysAnswers()
                    {
                        Topic = j.Topic,
                        Answer = j.Answer
                    });
            }
            /*foreach(var j in query)
            {
                if(topic!=j.Topic && topic!="")
                {
                    sa.Add(new SurveysAnswers()
                    {
                        Topic=topic,
                        Answer=(answers/flag).ToString()
                    }
                 );
                    topic = j.Topic;
                    flag = 1;
                    answers = Convert.ToInt32(j.Answer);
                }
                else
                {
                    if(topic=="")
                    {
                        topic = j.Topic;
                        flag = 1;
                        answers = Convert.ToInt32(j.Answer);
                    }
                    else
                    {
                        flag++;
                        answers = answers + Convert.ToInt32(j.Answer);
                    }
                    
                }
                


                
                
            }*/
            return sa;
        }

    }
}
