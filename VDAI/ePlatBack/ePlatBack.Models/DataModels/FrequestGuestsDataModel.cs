using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ePlatBack.Models.ViewModels;

namespace ePlatBack.Models.DataModels
{
    public class FrequestGuestsDataModel
    {
        public static FrequentGuestsViewModel.FrequentGuestsResult SearchFrequentGuests(FrequentGuestsViewModel.SearchGuests model)
        {
            FrequentGuestsViewModel.FrequentGuestsResult response = new FrequentGuestsViewModel.FrequentGuestsResult();
            response.Guests = new List<FrequentGuestsViewModel.FrequentGuest>();
            ePlatEntities db = new ePlatEntities();

            DateTime fromDate = DateTime.Parse(model.Search_I_FromDate);
            DateTime toDate = DateTime.Parse(model.Search_F_ToDate).AddDays(1);

            List<int?> AvailableResorts = new List<int?>();
            AvailableResorts.Add(9);
            AvailableResorts.Add(11);
            AvailableResorts.Add(13);

            var Resorts = from p in db.tblPlaces
                          where AvailableResorts.Contains(p.frontOfficeResortID)
                          select new
                          {
                              p.frontOfficeResortID,
                              p.place
                          };

            response.Resort = Resorts.FirstOrDefault(x => x.frontOfficeResortID == model.Search_ResortID).place;
            response.FromDate = model.Search_I_FromDate;
            response.ToDate = model.Search_F_ToDate;

            List<FrontOfficeViewModel.LlegadasResult> FOArrivals = FrontOfficeDataModel.GetArrivals(model.Search_ResortID, fromDate, toDate);

            foreach (var reservation in FOArrivals.Where(x => x.idhuesped != null).OrderBy(x => x.llegada))
            {
                FrequentGuestsViewModel.FrequentGuest guest = new FrequentGuestsViewModel.FrequentGuest();

                guest.Reservations = new List<FrequentGuestsViewModel.ReservationItem>();
                guest.Stays = 0;


                //buscar reservaciones anteriores
                foreach (var resortid in AvailableResorts)
                {
                    List<FrontOfficeViewModel.HistorialReservacionesResult> history1 = new List<FrontOfficeViewModel.HistorialReservacionesResult>();
                    List<FrontOfficeViewModel.HistorialReservacionesResult> history2 = new List<FrontOfficeViewModel.HistorialReservacionesResult>();
                    if (resortid == model.Search_ResortID)
                    {
                        //buscar historial en el mismo hotel
                        //por idcliente
                        history1 = FrontOfficeDataModel.GetReservationsHistory((int)resortid, (int?)reservation.idhuesped, null);
                    }

                    //buscar historial en otros hoteles
                    //obtener correo huesped
                    FrontOfficeViewModel.ContactInfo contactInfo = FrontOfficeDataModel.GetContactInfo(model.Search_ResortID, reservation.idhuesped);
                    string email = contactInfo.Email;
                    if (email != null && email.IndexOf("tiene") >= 0)
                    {
                        email = null;
                    }
                    if (email != null && email != "")
                    {
                        history2 = FrontOfficeDataModel.GetReservationsHistory((int)resortid, null, email);
                    }

                    history1 = history1.Concat(history2).ToList();

                    foreach (var pastReservation in history1.Where(x => x.llegada < reservation.llegada))
                    {
                        FrequentGuestsViewModel.ReservationItem nr = new FrequentGuestsViewModel.ReservationItem();

                        nr.Resort = Resorts.FirstOrDefault(x => x.frontOfficeResortID == resortid).place;
                        nr.Agency = pastReservation.nameagencia;
                        nr.Confirmation = pastReservation.numconfirmacion;
                        nr.Arrival = pastReservation.llegada.Value.ToString("yyyy-MM-dd");
                        nr.Nights = ((TimeSpan)(pastReservation.salida - pastReservation.llegada)).Days;
                        nr.RoomType = pastReservation.nametipodehabitacion;
                        nr.Currency = "DLLSUSA";
                        if(pastReservation.SPA != null)
                        {
                            nr.Spa = decimal.Round((decimal)pastReservation.SPA, 2);
                        }
                        else
                        {
                            nr.Spa = 0;
                        }
                        if(pastReservation.ConsumoPOS != null)
                        {
                            nr.PoS = decimal.Round((decimal)pastReservation.ConsumoPOS, 2);
                        } else
                        {
                            nr.PoS = 0;
                        }
                        if(pastReservation.Paquete != null)
                        {
                            nr.Stay = decimal.Round((decimal)pastReservation.Paquete, 2);
                        } else
                        {
                            nr.Stay = 0;
                        }
                        if(pastReservation.Renta != null)
                        {
                            nr.Stay += decimal.Round((decimal)pastReservation.Renta, 2);
                        }
                        if(pastReservation.Otros != null)
                        {
                            nr.Others = decimal.Round((decimal)pastReservation.Otros, 2);
                        }
                        nr.Total = nr.Spa + nr.PoS + nr.Stay + nr.Others;
                        guest.Reservations.Add(nr);
                        guest.Stays += 1;
                    }
                }


                if (guest.Reservations.Count() > 0 && response.Guests.Count(x => x.FrontOfficeGuestID == reservation.idhuesped) == 0)
                {
                    guest.FrontOfficeGuestID = reservation.idhuesped;
                    guest.FirstName = reservation.nombres;
                    guest.LastName = reservation.apellidopaterno + (reservation.apellidomaterno != null && reservation.apellidomaterno != "" ? reservation.apellidomaterno : "");

                    //reservación actual
                    FrequentGuestsViewModel.ReservationItem nr = new FrequentGuestsViewModel.ReservationItem();

                    nr.Resort = Resorts.FirstOrDefault(x => x.frontOfficeResortID == model.Search_ResortID).place;
                    nr.Agency = reservation.Procedencia;
                    nr.Confirmation = reservation.numconfirmacion;
                    nr.Arrival = reservation.llegada.Value.ToString("yyyy-MM-dd");
                    nr.Nights = reservation.CuartosNoche;
                    nr.RoomType = reservation.TipoHab;
                    nr.Currency = reservation.codetipodemoneda;
                    nr.Spa = 0;
                    nr.PoS = 0;
                    if(reservation.Tarifa != null)
                    {
                        nr.Stay = decimal.Round((decimal)reservation.Tarifa, 2);
                        nr.Stay = decimal.Round((decimal)(nr.Stay * nr.Nights), 2);
                    }
                    nr.Others = 0;
                    nr.Total = nr.Stay;
                    guest.Reservations.Add(nr);
                    guest.Stays += 1;

                    guest.Reservations = guest.Reservations.OrderByDescending(x => x.Arrival).ToList();

                    response.Guests.Add(guest);
                }
            }

            return response;
        }
    }
}
